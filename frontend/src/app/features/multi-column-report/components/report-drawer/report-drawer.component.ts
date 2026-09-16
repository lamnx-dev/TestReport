import { Component, inject } from '@angular/core';
import { DxButtonModule, DxToastModule } from 'devextreme-angular';
import { ReportStateService } from '../../services/report-state.service';

@Component({
  selector: 'app-report-drawer',
  standalone: true,
  imports: [DxButtonModule, DxToastModule],
  template: `
    <!-- Drawer Container với Tailwind transition và positioning -->
    <div
      class="fixed bottom-0 left-0 right-0 h-80 bg-white border-t border-gray-200 shadow-2xl flex flex-col z-50 transition-transform duration-300"
      [class.translate-y-0]="state.isDrawerOpen()"
      [class.translate-y-full]="!state.isDrawerOpen()">
      
      <!-- Drawer Header -->
      <div class="px-5 py-2.5 bg-gray-50 border-b border-gray-200 flex items-center justify-between gap-4">
        <div class="flex items-center gap-2.5">
          <span class="text-sm font-bold text-gray-800">🔍 Chi Tiết Báo Cáo & Chuỗi Base64 PDF</span>
          @if (state.reportResponse()) {
            <span class="text-xs text-gray-500 font-medium">
              ({{ state.reportResponse()?.fileName }} - {{ state.formattedFileSize() }})
            </span>
          }
        </div>
        <div class="flex items-center gap-2">
          <dx-button
            icon="copy"
            text="Sao chép Base64"
            stylingMode="contained"
            type="default"
            [disabled]="!state.reportResponse()?.base64"
            (onClick)="state.copyBase64()">
          </dx-button>
          <dx-button
            icon="close"
            stylingMode="outlined"
            (onClick)="state.closeDrawer()">
          </dx-button>
        </div>
      </div>

      <!-- Drawer Body -->
      <div class="flex-1 p-4 overflow-auto bg-slate-900">
        <pre class="m-0 text-slate-100 font-mono text-xs leading-relaxed break-all whitespace-pre-wrap">{{ state.reportResponse()?.base64 || 'Chưa có dữ liệu Base64 từ API...' }}</pre>
      </div>
    </div>

    <!-- DevExtreme Toast Notification -->
    <dx-toast
      [visible]="!!state.toastMessage()"
      [message]="state.toastMessage() || ''"
      type="success"
      [displayTime]="2500"
      (onHidden)="state.clearToast()">
    </dx-toast>
  `
})
export class ReportDrawerComponent {
  readonly state = inject(ReportStateService);
}
