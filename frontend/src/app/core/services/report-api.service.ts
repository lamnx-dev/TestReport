import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ReportAppearance, ReportCatalog } from '../models/report.model';

@Injectable({ providedIn: 'root' })
export class ReportApiService {
  private readonly http = inject(HttpClient);
  catalog() {
    return this.http.get<ReportCatalog>('/api/reports');
  }
  render(reportId: string, appearance: ReportAppearance) {
    return this.http.post(
      `/api/reports/${encodeURIComponent(reportId)}/render`,
      { appearance },
      { responseType: 'blob', observe: 'response' },
    );
  }
}
