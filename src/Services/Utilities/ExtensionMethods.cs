using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Services.Utilities
{
    public static class ExtensionMethods
    {
        public static IEnumerable<TSource> RemoveDuplicates<TSource>(this IEnumerable<TSource> source)
        {
            var results = new List<TSource>();
            var stringList = new List<string>();

            foreach (var item in source.ToList())
            {
                var itemStr = item.SerializeToXmlString();
                if (stringList.Contains(itemStr)) continue;
                stringList.Add(itemStr);
                results.Add(item);
            }
            return results;
        }

        public static string SerializeToXmlString<T>(this T toSerialize)
        {
            var xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
}
