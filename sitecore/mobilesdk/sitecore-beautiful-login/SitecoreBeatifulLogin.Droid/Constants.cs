using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SitecoreBeautifulLogin.Droid
{
    public class Constants
    {

        public struct Sitecore
        {

            public const string RestBaseUrl = "http://sitecoresandbox.local.nlc";
            public const string SitecoreUserName = "admin";
            public const string SitecorePassword = "b";
            public const string SitecoreShellSite = "/sitecore/shell";
            public const string SitecoreDefaultDatabase = "master";
            public const string SitecoreDefaultLanguage = "en";
            public const string SitecoreMediaLibraryRoot = "/sitecore/media library";
            public const string SitecoreMediaPrefix = "~/media/";
            public const string SitecoreDefaultMediaResourceExtension = "ashx";
        }
    }
}