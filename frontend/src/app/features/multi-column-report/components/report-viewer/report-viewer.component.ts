import { Component, inject } from '@angular/core';
import { DxLoadPanelModule, DxButtonModule, DxDataGridModule, DxPopupModule, DxToolbarModule } from 'devextreme-angular';
import { ReportStateService } from '../../services/report-state.service';

@Component({
  selector: 'app-report-viewer',
  standalone: true,
  imports: [DxLoadPanelModule, DxButtonModule, DxDataGridModule, DxPopupModule, DxToolbarModule],
  host: {
    class: 'flex flex-1 w-full h-full overflow-hidden'
  },
  template: `
    <div class="flex flex-1 w-full h-full relative overflow-hidden flex-col">
      <!-- 1. Mặc định luôn hiển thị DataGrid nguồn của DevExtreme -->
      <div class="flex-1 w-full h-full p-2.5 overflow-hidden">
        <dx-data-grid
          [dataSource]="state.gridRows()"
          [columns]="state.gridColumns()"
          [showBorders]="true"
          [showRowLines]="true"
          [rowAlternationEnabled]="true"
          [allowColumnReordering]="true"
          [allowColumnResizing]="true"
          [columnAutoWidth]="true"
          [height]="'100%'">
          <dxo-search-panel [visible]="true" placeholder="Tìm kiếm nhanh..."></dxo-search-panel>
          <dxo-paging [pageSize]="25"></dxo-paging>
          <dxo-pager
            [showPageSizeSelector]="true"
            [allowedPageSizes]="[15, 25, 50, 100]"
            [showInfo]="true">
          </dxo-pager>
          <dxo-filter-row [visible]="true"></dxo-filter-row>
          <dxo-header-filter [visible]="true"></dxo-header-filter>
        </dx-data-grid>
      </div>

      <!-- 2. Popup Fullscreen Preview với Iframe khi nhấn nút -->
      <dx-popup
        [fullScreen]="true"
        [visible]="state.isPreviewPopupVisible()"
        (onHiding)="state.closePreviewPopup()"
        [showTitle]="true"
        title="Bản In Báo Cáo Nhiều Cột (DevExpress Preview)"
        [showCloseButton]="true"
        [hideOnOutsideClick]="false">
        
        <!-- Toolbar nhanh bên trong Popup -->
        <dxi-toolbar-item
          widget="dxButton"
          toolbar="top"
          location="after"
          [options]="{
            icon: 'preferences',
            text: 'Tùy chỉnh in',
            stylingMode: 'outlined',
            onClick: onSettingsClick
          }">
        </dxi-toolbar-item>

        <dxi-toolbar-item
          widget="dxButton"
          toolbar="top"
          location="after"
          [options]="{
            icon: 'print',
            text: 'In báo cáo',
            type: 'default',
            stylingMode: 'contained',
            onClick: onPrintClick
          }">
        </dxi-toolbar-item>

        <dxi-toolbar-item
          widget="dxButton"
          toolbar="top"
          location="after"
          [options]="{
            icon: 'download',
            text: 'Tải PDF',
            stylingMode: 'outlined',
            onClick: onDownloadClick
          }">
        </dxi-toolbar-item>

        <dxi-toolbar-item
          widget="dxButton"
          toolbar="top"
          location="after"
          [options]="{
            icon: 'export',
            text: 'Mở tab riêng',
            stylingMode: 'text',
            onClick: onOpenTabClick
          }">
        </dxi-toolbar-item>

        <!-- Iframe hiển thị tài liệu PDF -->
        <div class="w-full h-full flex bg-gray-600 overflow-hidden">
          @if (state.safePreviewUrl()) {
            <iframe
              id="pdfFrame"
              class="flex-1 w-full h-full border-0 block"
              [src]="state.safePreviewUrl()"
              title="DevExpress Report Preview">
            </iframe>
          } @else {
            <div class="flex items-center justify-center w-full h-full text-white text-sm">
              <p>Đang chuẩn bị dữ liệu báo cáo...</p>
            </div>
          }
        </div>
      </dx-popup>

      <!-- DevExtreme Default Load Panel -->
      <dx-load-panel
        [visible]="state.isLoading()"
        [showIndicator]="true"
        [showPane]="true"
        [shading]="true"
        message="DevExpress Reporting đang kết xuất báo cáo...">
      </dx-load-panel>

      <!-- Error State using dx-card và Tailwind CSS -->
      @if (state.errorMessage() && !state.isLoading()) {
        <div class="absolute inset-0 flex items-center justify-center bg-white/90 z-10">
          <div class="dx-card p-8 text-center max-w-md shadow-lg border border-red-200">
            <i class="dx-icon-warning text-4xl text-red-500 mb-3 block"></i>
            <h3 class="m-0 mb-2 text-base font-bold text-gray-800">Chưa kết nối được Backend</h3>
            <p class="text-xs text-gray-600 mb-4">{{ state.errorMessage() }}</p>
            <dx-button
              type="default"
              text="Thử lại"
              icon="refresh"
              (onClick)="state.loadReport()">
            </dx-button>
          </div>
        </div>
      }
    </div>
  `
})
export class ReportViewerComponent {
  readonly state = inject(ReportStateService);

  readonly onPrintClick = () => this.state.triggerPrint();
  readonly onSettingsClick = () => this.state.openPrintSettings();
  readonly onDownloadClick = () => this.state.triggerDownload();
  readonly onOpenTabClick = () => this.state.openInNewTab();
}
