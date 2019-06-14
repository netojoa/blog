using HtmlAgilityPack;

namespace Bootstrap.Foundation.Forms.Extensions
{
    public static class HtmlAttributeCollectionExtensions
    {
        public static void AppendOrCreate(this HtmlAttributeCollection helper, string name, string value)
        {
            if (!helper.Contains(name))
                helper.Append(name, value);
            else
                helper[name].Value += value;
        }
    }
}