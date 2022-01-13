using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.TypeConversion;
using GatewayService.Helpers;
using GatewayService.Models;
using Microsoft.Extensions.Configuration;

namespace GatewayService.Services
{
    public class CoreCsvService : ICoreCsvService
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _culture;

        public CoreCsvService(IConfiguration configuration) : this(new FileSystem(), configuration)
        {
            var niraConfig = configuration.GetNiraSettings();

            _culture = niraConfig.NiraDateTimeConfig.Culture;
        }

        public CoreCsvService(IFileSystem fileSystem, IConfiguration configuration)
        {
            var niraConfig = configuration.GetNiraSettings();

            _culture = niraConfig.NiraDateTimeConfig.Culture;
            _fileSystem = fileSystem;
        }

        public async Task WriteRecordsToCsvFileAsync<T>(string fullPath, int page, List<T> records)
        {

            if (page == 0 || page == 1)
            {
                using var writer = new StreamWriter(fullPath);
                
                await WriteRecordsAsync(records, writer);
                return;
            }

            using var stream = _fileSystem.File.Open(fullPath, FileMode.Append);
            {
                using var writer = new StreamWriter(stream);
                await WriteRecordsAsync(records, writer, false);
            }
        }

        private async Task WriteRecordsAsync<T>(List<T> records, StreamWriter writer, bool hasHeader = true)
        {
            var options = new TypeConverterOptions
            {
                Formats = new[] { "dd.MM.yyyy hh:mm:ss" }
            };

            using var csv = new CsvWriter(writer, new CultureInfo(_culture));
            csv.Configuration.HasHeaderRecord = hasHeader;
            csv.Configuration.TypeConverterOptionsCache.AddOptions<DateTime>(options);
            await csv.WriteRecordsAsync(records);
        }
    }
}
