using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace GatewayService.Helpers
{
#nullable disable
    public class StringWriterUtf8 : StringWriter
    {
        public StringWriterUtf8(StringBuilder sb) : base(sb)
        {
        }

        public override Encoding Encoding => Encoding.UTF8;
    }

    public static class XmlHelper
    {
        /// <summary>
        /// De-serializes an XML string into a plain old C# object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T XmlToPoco<T>(string xml) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            using var textReader = new StringReader(xml);
            return (T)serializer.Deserialize(textReader);
        }
        
        public static string GetPrettyXml(string xml)
        {
            using var mStream = new MemoryStream();
            using var writer = new XmlTextWriter(mStream, Encoding.Unicode);
            var document = new XmlDocument();
            document.LoadXml(xml);
            document.LoadXml(xml);
            document.PreserveWhitespace = false;
            writer.Formatting = Formatting.Indented;

            // Write the XML into a formatting XmlTextWriter
            document.WriteContentTo(writer);
            writer.Flush();
            mStream.Flush();

            // Have to rewind the MemoryStream in order to read
            // its contents.
            mStream.Position = 0;

            // Read MemoryStream contents into a StreamReader.
            var sReader = new StreamReader(mStream);

            // Extract the text from the StreamReader.
            var formattedXml = sReader.ReadToEnd();

            return formattedXml;
        }
    }

    internal class Utf8StringWriter : StringWriter
    {
        public Utf8StringWriter() : base(new StringBuilder())
        {
            Encoding = Encoding.UTF8;
        }

        public override Encoding Encoding { get; }
    }
}
