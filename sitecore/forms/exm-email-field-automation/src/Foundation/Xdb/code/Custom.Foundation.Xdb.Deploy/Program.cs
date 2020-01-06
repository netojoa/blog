/*
    Run this console application. It will generate JSON file. Deploy this JSON file at the following places:
    - <Instance_Name>.xconnect\App_data\Models
    - <Instance_Name>.xconnect\App_data\jobs\continuous\IndexWorker\App_data\Models
    
    Deploy your Custom facet class project dll at the following places:
    - <Instance_Name>.xconnect\App_data\jobs\continuous\AutomationEngine\
    - <Instance_Name>.xconnect\App_data\jobs\continuous\IndexWorker\
    - <Instance_Name>.xconnect\bin
    - <Instance_Name>.sc\bin
 */

using Custom.Foundation.Xdb.Models;
using System;
using System.IO;

namespace Custom.Foundation.Xdb.Deploy
{
    class Program
    {
        private static void Main(string[] args)
        {
            RegisterXdbModel();
        }

        private static void RegisterXdbModel()
        {
            var model = Sitecore.XConnect.Serialization.XdbModelWriter.Serialize(FormsModel.Model);
            string filePath = "C:\\Temp\\" + FormsModel.Model.FullName + ".json";
            File.WriteAllText(filePath, model);
            Console.WriteLine("{0}", filePath);
            Console.Read();
        }
    }
}
