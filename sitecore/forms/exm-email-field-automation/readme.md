Files and packages are located in the [deploy folder](https://github.com/JANeto87/blog/tree/master/sitecore/forms/exm-email-field-automation/deploy).

Assemblies are located in the packages.

## XP configuration

Install the Sitecore package "Forms and Marketing Automation items.zip".

Optionally, you can install the Sitecore package "Sample site and submit action.zip" as well.

## XConnect configuration

Copy the file named **FormsModel, 1.0.json** to the following locations:

- *XConnect*\App_data\Models
- *XConnect*\App_data\jobs\continuous\IndexWorker\App_data\Models

Also, copy the assembly file `Custom.Foundation.Xdb.dll` to the following location:

- *XConnect*\App_data\jobs\continuous\AutomationEngine\
- *XConnect*\App_data\jobs\continuous\IndexWorker\
- *XConnect*\bin

Restart the **MarketingAutomationService** and **IndexWorker** Windows services

## Marketing Automation configuration

Copy the `Custom.Foundation.Xdb.dll` assembly to the following location:

- *XConnect*\App_data\jobs\continuous\AutomationEngine.

Copy the xml file `sc.Custom.Forms.Predicates.xml` to the following location:

- *XConnect*\App_data\jobs\continuous\AutomationEngine\App_Data\Config\sitecore\Segmentation\sc.Custom.Forms.Predicates.xml

Restart the **Sitecore Marketing Automation Engine** Windows service.