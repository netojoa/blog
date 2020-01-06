using Custom.Foundation.Xdb.Events;
using Sitecore.XConnect;
using Sitecore.XConnect.Schema;

namespace Custom.Foundation.Xdb.Models
{
    public class FormsModel
    {
        public static XdbModel Model { get; } = BuildModel();

        private static XdbModel BuildModel()
        {
            XdbModelBuilder modelBuilder = new XdbModelBuilder("FormsModel", new XdbModelVersion(1, 0));
            modelBuilder.DefineEventType<FormSubmission>(false);
            modelBuilder.ReferenceModel(Sitecore.XConnect.Collection.Model.CollectionModel.Model);
            return modelBuilder.BuildModel();
        }
    }
}