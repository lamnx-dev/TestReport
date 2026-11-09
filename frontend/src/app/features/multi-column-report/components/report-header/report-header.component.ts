import { Component, inject } from '@angular/core';
import { DxToolbarModule, DxButtonModule } from 'devextreme-angular';
import { ReportStateService } from '../../services/report-state.service';

@Component({
  selector: 'app-report-header',
  standalone: true,
  imports: [DxToolbarModule, DxButtonModule],
  template: `
    <div class="px-2 py-3 border-b border-gray-200">
      <dx-toolbar>
        <!-- Vị trí Before: Tiêu đề & Thông tin tài liệu -->
        <dxi-item location="before" locateInMenu="never">
          <div class="flex items-center gap-2.5">
            <span class="text-gray-900 font-bold text-sm">DevExpress Report Viewer</span>
            <span class="text-xs font-semibold text-gray-800">{{
              state.reportResponse()?.fileName || 'BaoCao_NhieuCot_TachTrang.pdf'
            }}</span>
            @if (state.reportResponse()) {
              <span class="dx-badge text-[11px] px-1.5 py-0.5 rounded">
                {{ state.reportResponse()?.totalPages }} trang &bull;
                {{ state.reportResponse()?.totalRecords }} dòng &bull;
                {{ state.formattedFileSize() }}
              </span>
            }
          </div>
        </dxi-item>

        <!-- Vị trí Center: Nút mở Bản In Báo Cáo Fullscreen Popup -->
        <dxi-item location="center" locateInMenu="never">
          <dx-button
            icon="print"
            text="In / Xem Báo Cáo (Popup)"
            type="default"
            stylingMode="contained"
            hint="Mở bản in báo cáo nhiều cột dạng Popup toàn màn hình"
            [disabled]="state.isLoading()"
            (onClick)="state.openPreviewPopup()"
          >
          </dx-button>
        </dxi-item>

        <!-- Vị trí After: Cụm nút tác vụ mặc định của DevExtreme -->
        <dxi-item location="after" locateInMenu="auto">
          <dx-button
            icon="preferences"
            [text]="state.isParametersOpen() ? 'Đóng tham số' : 'Tham số'"
            stylingMode="outlined"
            (onClick)="state.toggleParameters()"
          >
          </dx-button>
        </dxi-item>

        <dxi-item location="after" locateInMenu="auto">
          <dx-button
            icon="preferences"
            text="Tùy chỉnh in"
            stylingMode="outlined"
            [disabled]="!state.printSettings() || state.isSavingPrintSettings()"
            (onClick)="state.openPrintSettings()"
          ></dx-button>
        </dxi-item>

        <dxi-item location="after" locateInMenu="auto">
          <dx-button
            icon="refresh"
            text="Tải lại"
            stylingMode="outlined"
            [disabled]="state.isLoading()"
            (onClick)="state.loadReport()"
          >
          </dx-button>
        </dxi-item>

        <dxi-item location="after" locateInMenu="auto">
          <dx-button
            icon="download"
            text="Tải PDF"
            stylingMode="outlined"
            [disabled]="state.isLoading() || !state.reportResponse()"
            (onClick)="state.triggerDownload()"
          >
          </dx-button>
        </dxi-item>

        <dxi-item location="after" locateInMenu="auto">
          <dx-button
            icon="export"
            stylingMode="text"
            hint="Mở trên tab riêng"
            [disabled]="state.isLoading() || !state.reportResponse()"
            (onClick)="state.openInNewTab()"
          >
          </dx-button>
        </dxi-item>

        <dxi-item location="after" locateInMenu="auto">
          <dx-button
            icon="codeblock"
            stylingMode="text"
            hint="Xem Base64 & API"
            (onClick)="state.toggleDrawer()"
          >
          </dx-button>
        </dxi-item>
      </dx-toolbar>
    </div>
  `,
})
export class ReportHeaderComponent {
  readonly state = inject(ReportStateService);
}
