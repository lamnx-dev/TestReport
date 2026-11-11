import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Subscription } from 'rxjs';
import { ReportApiService } from './core/services/report-api.service';
import {
  AppearanceRegion,
  ReportAppearance,
  ReportCatalog,
  ReportElementStyle,
} from './core/models/report.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  private readonly api = inject(ReportApiService);
  private readonly sanitizer = inject(DomSanitizer);
  private readonly destroyRef = inject(DestroyRef);
  private renderSubscription?: Subscription;
  private catalogSubscription?: Subscription;
  private errorVersion = 0;
  private destroyed = false;
  private blobUrl?: string;
  readonly catalog = signal<ReportCatalog | null>(null);
  readonly catalogLoading = signal(false);
  readonly loading = signal(false);
  readonly error = signal('');
  readonly dirty = signal(false);
  readonly pdfUrl = signal<SafeResourceUrl | null>(null);
  readonly regions: { key: AppearanceRegion; label: string; hint: string }[] = [
    { key: 'title', label: 'Tiêu đề báo cáo', hint: 'Tên chính của báo cáo' },
    { key: 'tableHeader', label: 'Đầu bảng', hint: 'Các hàng tên cột' },
    { key: 'tableBody', label: 'Nội dung bảng', hint: 'Giữ định dạng nhấn mạnh dòng tổng' },
  ];
  reportId = 's11h';
  appearance: ReportAppearance = {};
  fileName = 'S11H.pdf';

  constructor() {
    this.destroyRef.onDestroy(() => {
      this.destroyed = true;
      this.renderSubscription?.unsubscribe();
      this.catalogSubscription?.unsubscribe();
      this.releasePdf();
    });
  }
  ngOnInit() {
    this.loadCatalog();
  }
  loadCatalog() {
    this.catalogSubscription?.unsubscribe();
    this.catalogLoading.set(true);
    this.error.set('');
    this.catalogSubscription = this.api.catalog().subscribe({
      next: (catalog) => {
        this.catalog.set(catalog);
        this.catalogLoading.set(false);
      },
      error: () => {
        this.error.set('Không tải được danh sách báo cáo. Hãy thử lại.');
        this.catalogLoading.set(false);
      },
    });
  }
  selectReport(id: string) {
    this.cancelRender();
    this.reportId = id;
    this.appearance = {};
    this.releasePdf();
    this.dirty.set(false);
    this.error.set('');
  }
  update(region: AppearanceRegion, property: keyof ReportElementStyle, value: unknown) {
    this.cancelRender();
    const style = { ...this.appearance[region] } as Record<string, unknown>;
    if (value === '' || value === null || value === undefined) delete style[property];
    else style[property] = value;
    this.appearance = { ...this.appearance, [region]: style };
    this.dirty.set(true);
    this.error.set('');
  }
  reset() {
    this.cancelRender();
    this.appearance = {};
    this.dirty.set(true);
    this.error.set('');
  }
  validationError(): string {
    const catalog = this.catalog();
    if (!catalog) return '';
    for (const style of Object.values(this.appearance)) {
      if (
        style.fontSize !== undefined &&
        (!Number.isFinite(style.fontSize) ||
          style.fontSize < catalog.minFontSize ||
          style.fontSize > catalog.maxFontSize)
      )
        return `Cỡ chữ phải từ ${catalog.minFontSize} đến ${catalog.maxFontSize} pt.`;
      if (style.color !== undefined && !/^#[0-9a-fA-F]{6}$/.test(style.color))
        return 'Màu chữ phải có dạng #RRGGBB.';
    }
    return '';
  }
  generate() {
    if (!this.catalog() || this.validationError()) return;
    this.cancelRender();
    this.error.set('');
    this.loading.set(true);
    this.dirty.set(true);
    const version = this.errorVersion;
    this.renderSubscription = this.api.render(this.reportId, this.appearance).subscribe({
      next: (response) => {
        const blob = response.body;
        this.loading.set(false);
        if (!blob || !blob.type.toLowerCase().startsWith('application/pdf')) {
          this.error.set('Máy chủ không trả về PDF hợp lệ.');
          return;
        }
        this.releasePdf();
        this.blobUrl = URL.createObjectURL(blob);
        this.pdfUrl.set(this.sanitizer.bypassSecurityTrustResourceUrl(this.blobUrl));
        const disposition = response.headers.get('Content-Disposition') ?? '';
        const name = /filename="?([^";]+)"?/i.exec(disposition)?.[1];
        this.fileName = name?.replace(/[\\/]/g, '_') ?? `${this.reportId.toUpperCase()}.pdf`;
        this.dirty.set(false);
      },
      error: async (failure: HttpErrorResponse) => {
        let message = 'Không thể tạo báo cáo. Vui lòng thử lại.';
        if (failure.error instanceof Blob) {
          try {
            const body = JSON.parse(await failure.error.text());
            message = body.message ?? body.title ?? message;
          } catch {
            /* Keep a user-friendly fallback. */
          }
        }
        if (version === this.errorVersion && !this.destroyed) {
          this.loading.set(false);
          this.error.set(message);
        }
      },
    });
  }
  canUsePdf() {
    return !!this.blobUrl && !this.dirty() && !this.loading();
  }
  download() {
    if (!this.canUsePdf()) return;
    const anchor = document.createElement('a');
    anchor.href = this.blobUrl!;
    anchor.download = this.fileName;
    anchor.click();
  }
  openForPrint() {
    if (this.canUsePdf()) window.open(this.blobUrl!, '_blank', 'noopener,noreferrer');
  }
  private cancelRender() {
    this.errorVersion++;
    this.renderSubscription?.unsubscribe();
    this.loading.set(false);
  }
  private releasePdf() {
    this.pdfUrl.set(null);
    if (this.blobUrl) URL.revokeObjectURL(this.blobUrl);
    this.blobUrl = undefined;
  }
}
