namespace TestReport.Api.Domain.Responses
{
    public class ReportBase64Response
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/pdf";
        public int FileSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalColumns { get; set; }
        public int ColumnsPerPage { get; set; }
        public int TotalPages { get; set; }
        public string Base64 { get; set; } = string.Empty;
        public string DataUrl => $"data:{ContentType};base64,{Base64}";
    }
}

