import { Injectable, inject, signal, computed } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ReportApiService } from '../../../core/services/report-api.service';
import { MultiColumnReportRequest } from '../../../core/models/report-request.model';
import { ReportBase64Response } from '../../../core/models/report-base64.model';

@Injectable()
export class ReportStateService {
  private readonly apiService = inject(ReportApiService);
  private readonly sanitizer = inject(DomSanitizer);

  // State Signals
  readonly request = signal<MultiColumnReportRequest>({
    totalColumns: 18,
    columnsPerPage: 6,
    rowCount: 100
  });

  readonly isParametersOpen = signal<boolean>(true);
  readonly isPreviewPopupVisible = signal<boolean>(false);

  readonly reportResponse = signal<ReportBase64Response | null>(null);
  readonly previewUrl = signal<string | null>(null);
  readonly safePreviewUrl = computed<SafeResourceUrl | null>(() => {
    const url = this.previewUrl();
    return url ? this.sanitizer.bypassSecurityTrustResourceUrl(url) : null;
  });

  // Table Data for DevExtreme DataGrid
  readonly gridColumns = signal<{ dataField: string; caption: string; width?: number }[]>([]);
  readonly gridRows = signal<any[]>([]);

  readonly isLoading = signal<boolean>(false);
  readonly errorMessage = signal<string | null>(null);
  readonly isDrawerOpen = signal<boolean>(false);
  readonly toastMessage = signal<string | null>(null);

  // Computed Properties
  readonly totalParts = computed(() => {
    const req = this.request();
    return Math.ceil(req.totalColumns / req.columnsPerPage);
  });

  readonly formattedFileSize = computed(() => {
    const resp = this.reportResponse();
    if (!resp) return '-- KB';
    return `${(resp.fileSize / 1024).toFixed(0)} KB`;
  });

  loadReport(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    const currentReq = this.request();
    const directPreviewUrl = this.apiService.getPreviewUrl(currentReq);

    // 1. Tải Base64 và URL preview
    this.apiService.getBase64(currentReq).subscribe({
      next: (data) => {
        this.reportResponse.set(data);
        this.previewUrl.set(directPreviewUrl);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Lỗi khi tải báo cáo:', err);
        const msg = err.message || 'Không thể kết nối đến Backend TestReport.Api.';
        this.errorMessage.set(msg);
        this.isLoading.set(false);
      }
    });

    // 2. Tải song song dữ liệu nguồn cho DevExtreme DataGrid
    this.apiService.getReportData(currentReq).subscribe({
      next: (reportData) => {
        if (reportData?.columnNames) {
          const cols = [
            { dataField: 'stt', caption: 'STT', width: 70 },
            ...reportData.columnNames.map(col => ({
              dataField: col,
              caption: col,
              width: 120
            }))
          ];
          this.gridColumns.set(cols);

          const rows = (reportData.rows || []).map(r => {
            const rowObj: Record<string, any> = { stt: r.stt };
            if (r.values) {
              Object.entries(r.values).forEach(([k, v]) => {
                rowObj[k] = v;
              });
            }
            return rowObj;
          });
          this.gridRows.set(rows);
        }
      },
      error: (err) => {
        console.warn('Không tải được dữ liệu DataGrid:', err);
      }
    });
  }

  openPreviewPopup(): void {
    this.isPreviewPopupVisible.set(true);
    if (!this.reportResponse()) {
      this.loadReport();
    }
  }

  closePreviewPopup(): void {
    this.isPreviewPopupVisible.set(false);
  }

  toggleParameters(): void {
    this.isParametersOpen.update(v => !v);
  }

  updateTotalColumns(total: number): void {
    this.request.update(r => ({ ...r, totalColumns: total }));
  }

  updateColumnsPerPage(cols: number): void {
    this.request.update(r => ({ ...r, columnsPerPage: cols }));
  }

  updateRowCount(rows: number): void {
    this.request.update(r => ({ ...r, rowCount: rows }));
  }

  applyParameters(): void {
    this.loadReport();
  }

  toggleDrawer(): void {
    this.isDrawerOpen.update(v => !v);
  }

  closeDrawer(): void {
    this.isDrawerOpen.set(false);
  }

  showToast(message: string): void {
    this.toastMessage.set(message);
  }

  clearToast(): void {
    this.toastMessage.set(null);
  }

  copyBase64(): void {
    const resp = this.reportResponse();
    if (!resp?.base64) return;

    navigator.clipboard.writeText(resp.base64).then(() => {
      this.showToast('Đã sao chép chuỗi Base64 vào bộ nhớ tạm!');
    });
  }

  triggerDownload(): void {
    const resp = this.reportResponse();
    const fileName = resp?.fileName || 'BaoCao_NhieuCot_TachTrang.pdf';

    if (resp?.base64) {
      const blob = this.base64ToBlob(resp.base64, 'application/pdf');
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = fileName;
      document.body.appendChild(a);
      a.click();
      setTimeout(() => {
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      }, 3000);
      this.showToast(`Đã lưu tệp: ${fileName}`);
    } else {
      const downloadUrl = this.apiService.getExportPdfUrl(this.request());
      window.location.href = downloadUrl;
    }
  }

  triggerPrint(): void {
    const url = this.previewUrl();
    if (!url) return;

    const iframe = document.querySelector('#pdfFrame') as HTMLIFrameElement;
    if (iframe && iframe.contentWindow) {
      try {
        iframe.contentWindow.focus();
        iframe.contentWindow.print();
        return;
      } catch {
        window.open(url, '_blank');
      }
    } else {
      window.open(url, '_blank');
    }
  }

  openInNewTab(): void {
    const url = this.previewUrl();
    if (url) {
      window.open(url, '_blank');
    }
  }

  private base64ToBlob(base64: string, contentType = 'application/pdf'): Blob {
    const byteCharacters = atob(base64);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    return new Blob([byteArray], { type: contentType });
  }
}
