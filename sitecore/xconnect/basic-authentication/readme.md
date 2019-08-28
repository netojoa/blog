# Installation

Copy the assembly `Custom.XConnect.Security.Web.dll` to `C:\inetpub\wwwroot\site-prefix.xconnect\bin`.


Generate a new encrypted username and password by using the following:

```
using System;
using System.Text;
					
public class Program
{
	public static void Main()
	{
		string user = "admin";
		string password = "b";
		var byteArray = Encoding.ASCII.GetBytes(user + ":" + password);
        var encryptedUsernameAndPassword = Convert.ToBase64String(byteArray);
		Console.WriteLine(encryptedUsernameAndPassword);
	}
}

```

You can use https://dotnetfiddle.net/.

Add the following entry to the `C:\inetpub\wwwroot\site-prefix.xconnect\App_Config\AppSettings.config` file:

```
<add key="usernameAndPassword" value="YWRtaW46Yg==" />
```

**Note:** The `value` attribute should receive the outcome of the code above.

Update the `C:\inetpub\wwwroot\site-prefix.xconnect\App_data\config\sitecore\CoreServices\sc.XConnect.Security.EnforceSSLWithCertificateValidation.xml` file with the following:

```
<Settings>
  <Sitecore>
    <XConnect>
      <Services>
        <CertificateValidationHttpConfiguration>
          <Type>Custom.XConnect.Security.Web.CertificateValidationHttpConfiguration, Custom.XConnect.Security.Web</Type>
		  <!-- <Type>Sitecore.XConnect.Security.Web.CertificateValidationHttpConfiguration, Sitecore.XConnect.Security.Web</Type> -->
          <As>Sitecore.XConnect.DependencyInjection.Web.Abstractions.IHttpConfiguration, Sitecore.XConnect.DependencyInjection.Web</As>
          <LifeTime>Transient</LifeTime>
        </CertificateValidationHttpConfiguration>
      </Services>
    </XConnect>
  </Sitecore>
</Settings>
```
Restart xConnect Application Pool.