export interface ReportBase64Response {
  fileName: string;
  base64: string;
  fileSize: number;
  totalPages: number;
  totalRecords: number;
  contentType: string;
}
