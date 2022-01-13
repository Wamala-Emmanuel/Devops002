using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace GatewayService.Helpers
{
#nullable disable
    public class ZipUtils
    {

        public static string GetJsonData(string zipPath)
        {
            using (var archive = ZipFile.OpenRead(zipPath))
            {
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    {
                        return StreamToString(entry.Open());
                    }
                }
            }
            return null;
        }

        public static List<string> ExtractDocuments(string zipPath)
        {
            var documentFiles = new List<string>();
            using (var archive = ZipFile.OpenRead(zipPath))
            {
                // Skip the Json file
                var documentEntries = archive.Entries.Where(x=>x.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase) == false);
                foreach (var documentEntry in documentEntries)
                {
                    using var stream = documentEntry.Open();
                    var filePath = $"{zipPath}-{documentEntry.Name}";
                    var fileStream = File.OpenWrite(filePath);
                    stream.CopyTo(fileStream);
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.Flush();
                    }
                    documentFiles.Add(filePath);
                }       

            }
            return documentFiles;
        }

        public static string StreamToString(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}
