using System;
using System.IO;

namespace TestReport.Api.Domain.Responses
{
    public class ReportFileResponse : IDisposable
    {
        public Stream Stream { get; }
        public string ContentType { get; }
        public string FileName { get; }

        public ReportFileResponse(Stream stream, string contentType, string fileName)
        {
            Stream = stream;
            ContentType = contentType;
            FileName = fileName;
        }

        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}

