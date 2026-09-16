import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MultiColumnReportRequest } from '../models/report-request.model';
import { ReportBase64Response } from '../models/report-base64.model';
import { MultiColumnReportData } from '../models/report-data.model';

@Injectable({
  providedIn: 'root'
})
export class ReportApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5168/api/report';

  private buildParams(request: MultiColumnReportRequest): HttpParams {
    return new HttpParams()
      .set('totalColumns', request.totalColumns.toString())
      .set('columnsPerPage', request.columnsPerPage.toString())
      .set('rowCount', request.rowCount.toString());
  }

  getPreviewUrl(request: MultiColumnReportRequest): string {
    const params = this.buildParams(request).toString();
    return `${this.baseUrl}/multi-column-preview?${params}`;
  }

  getExportPdfUrl(request: MultiColumnReportRequest): string {
    const params = this.buildParams(request).toString();
    return `${this.baseUrl}/multi-column-export-pdf?${params}`;
  }

  getBase64(request: MultiColumnReportRequest): Observable<ReportBase64Response> {
    const params = this.buildParams(request);
    return this.http.get<ReportBase64Response>(`${this.baseUrl}/multi-column-base64`, { params });
  }

  getPreviewBlob(request: MultiColumnReportRequest): Observable<Blob> {
    const params = this.buildParams(request);
    return this.http.get(`${this.baseUrl}/multi-column-preview`, {
      params,
      responseType: 'blob'
    });
  }

  getReportData(request: MultiColumnReportRequest): Observable<MultiColumnReportData> {
    const params = this.buildParams(request);
    return this.http.get<MultiColumnReportData>(`${this.baseUrl}/multi-column-data`, { params });
  }
}
