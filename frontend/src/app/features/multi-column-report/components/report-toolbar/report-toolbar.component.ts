import { Component, inject } from '@angular/core';
import { DxSelectBoxModule, DxButtonModule } from 'devextreme-angular';
import { ReportStateService } from '../../services/report-state.service';

@Component({
  selector: 'app-report-toolbar',
  standalone: true,
  imports: [DxSelectBoxModule, DxButtonModule],
  host: {
    class: 'flex shrink-0 h-full',
  },
  template: `
    @if (state.isParametersOpen()) {
      <div
        class="dx-card w-72 h-full rounded-none border-t-0 border-b-0 border-l-0 border-r border-gray-200 flex flex-col shrink-0 z-10 bg-white"
      >
        <!-- Panel Header -->
        <div class="px-3.5 py-2.5 flex items-center justify-between border-b border-gray-200">
          <h4 class="m-0 text-xs font-semibold flex items-center gap-1.5 text-gray-800">
            <i class="dx-icon-preferences text-gray-700"></i>
            <span>Tham Số Báo Cáo</span>
          </h4>
          <dx-button icon="close" stylingMode="text" (onClick)="state.toggleParameters()">
          </dx-button>
        </div>

        <!-- Panel Content -->
        <div class="p-3.5 flex flex-col gap-4 overflow-y-auto flex-1">
          <!-- DxSelectBox with standard DevExtreme label -->
          <dx-select-box
            label="Tổng số cột dữ liệu"
            labelMode="floating"
            [items]="totalColumnOptions"
            displayExpr="text"
            valueExpr="id"
            [value]="state.request().totalColumns"
            (onValueChanged)="state.updateTotalColumns($event.value)"
          >
          </dx-select-box>

          <dx-select-box
            label="Số cột in / trang"
            labelMode="floating"
            [items]="colsPerPageOptions"
            displayExpr="text"
            valueExpr="id"
            [value]="state.request().columnsPerPage"
            (onValueChanged)="state.updateColumnsPerPage($event.value)"
          >
          </dx-select-box>

          <dx-select-box
            label="Quy mô dòng dữ liệu"
            labelMode="floating"
            [items]="rowCountOptions"
            displayExpr="text"
            valueExpr="id"
            [value]="state.request().rowCount"
            (onValueChanged)="state.updateRowCount($event.value)"
          >
          </dx-select-box>

          <!-- Default DevExtreme Button -->
          <dx-button
            icon="check"
            text="Áp dụng tham số"
            type="default"
            stylingMode="contained"
            [width]="'100%'"
            [disabled]="state.isLoading()"
            (onClick)="state.applyParameters()"
          >
          </dx-button>

          <div
            class="p-2.5 rounded border border-gray-200 bg-gray-50 text-xs leading-relaxed text-gray-700"
          >
            <p class="m-0 flex items-start gap-1.5">
              <i class="dx-icon-info text-gray-700 mt-0.5"></i>
              <span>
                Tự động chia thành <strong>{{ state.totalParts() }} phần in</strong>. Cột STT luôn
                hiển thị ở đầu mỗi trang.
              </span>
            </p>
          </div>
        </div>
      </div>
    }
  `,
})
export class ReportToolbarComponent {
  readonly state = inject(ReportStateService);

  readonly totalColumnOptions = [
    { id: 12, text: '12 cột (2 phần in)' },
    { id: 18, text: '18 cột (3 phần in)' },
    { id: 24, text: '24 cột (4 phần in)' },
    { id: 30, text: '30 cột (5 phần in)' },
  ];

  readonly colsPerPageOptions = [
    { id: 4, text: '4 cột / trang' },
    { id: 6, text: '6 cột / trang' },
    { id: 8, text: '8 cột / trang' },
  ];

  readonly rowCountOptions = [
    { id: 30, text: '30 dòng' },
    { id: 50, text: '50 dòng' },
    { id: 100, text: '100 dòng (Mặc định)' },
    { id: 200, text: '200 dòng' },
  ];
}
