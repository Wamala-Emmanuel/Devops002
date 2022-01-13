using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayService.Helpers
{
#nullable disable

    public static class LoggerMessageExtensions
    {
        public static void ProcessingUploadedCaseFile(
            this ILogger logger,
            string id,
            Exception exception = null)
        {
            Action<ILogger, string, Exception> _processingUploadedCaseFile =
                LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(1, nameof(ProcessingUploadedCaseFile)),
                "Processing uploaded case file (Id = '{Id}')");

            _processingUploadedCaseFile(logger, id, exception);
        }

        public static void DuplicateCaseFileUploaded(
            this ILogger logger,
            string caseId,
            Exception exception = null)
        {
            Action<ILogger, string, Exception> _duplicateCaseFileUploaded =
                LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(2, nameof(DuplicateCaseFileUploaded)),
                "Duplicate case file uploaded (CaseId = '{CaseId}')");

            _duplicateCaseFileUploaded(logger, caseId, exception);
        }

        public static void ReadingCaseJsonData(
            this ILogger logger,
            string id,
            Exception exception = null)
        {
            Action<ILogger, string, Exception> _readingCaseJsonData =
                LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(3, nameof(ReadingCaseJsonData)),
                "Reading Case JSON data (Id = '{Id}')");

            _readingCaseJsonData(logger, id, exception);
        }

        public static void ErrorReadingCaseJsonData(
            this ILogger logger,
            string id,
            string data,
            Exception exception = null)
        {
            Action<ILogger, string, string, Exception> _errorReadingCaseJsonData =
                LoggerMessage.Define<string, string>(
                LogLevel.Error,
                new EventId(4, nameof(ErrorReadingCaseJsonData)),
                "Invalid JSON data (Id = '{Id}' Data = '{Data}')");

            _errorReadingCaseJsonData(logger, id, data, exception);
        }

        public static void RetrievedCaseData(
            this ILogger logger,
            string caseId,
            string productCode,
            Exception exception = null)
        {
            Action<ILogger, string, string, Exception> _retrievedCaseData =
                LoggerMessage.Define<string, string>(
                LogLevel.Information,
                new EventId(5, nameof(RetrievedCaseData)),
                "Retrieved case data (CaseId = '{CaseId}' ProductCode = '{ProductCode}')");

            _retrievedCaseData(logger, caseId, productCode, exception);
        }

        public static void FileUploadedToDigitalArchive(
            this ILogger logger,
            Guid? caseId,
            string jobId,
            Exception exception = null)
        {
            Action<ILogger, Guid?, string, Exception> _fileUploadedToDigitalArchive =
                LoggerMessage.Define<Guid?, string>(
                LogLevel.Information,
                new EventId(6, nameof(FileUploadedToDigitalArchive)),
                "File upload to digital archive successful (CaseId = '{CaseId}' JobId = '{JobId}')");

            _fileUploadedToDigitalArchive(logger, caseId, jobId, exception);
        }

        public static void GatewayCompleted(
            this ILogger logger,
            string caseId,
            string productCode,
            Exception exception = null)
        {
            Action<ILogger, string, string, Exception> _gatewayCompleted =
                LoggerMessage.Define<string, string>(
                LogLevel.Information,
                new EventId(7, nameof(GatewayCompleted)),
                "Gateway completed (CaseId = '{CaseId}' ProductCode = '{ProductCode}')");

            _gatewayCompleted(logger, caseId, productCode, exception);
        }

        public static void ErrorProcessingUpload(
            this ILogger logger,
            string id,
            Exception exception)
        {
            Action<ILogger, string, Exception> _errorProcessingUpload =
                LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(8, nameof(ErrorProcessingUpload)),
                "Error processing upload (Id = '{Id}' Exception = '{Exception}')");

            _errorProcessingUpload(logger, id, exception);
        }

        public static void ExceptionMessage(
            this ILogger logger,
            string description,
            Exception exception)
        {
            Action<ILogger, string, Exception> _exceptionMessage =
                LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(9, nameof(ExceptionMessage)),
                "Exception (Description = '{Description}' Exception = '{Exception}')");

            _exceptionMessage(logger, description, exception);
        }
    }
}
