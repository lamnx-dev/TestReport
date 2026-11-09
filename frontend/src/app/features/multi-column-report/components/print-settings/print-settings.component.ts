import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DxPopupModule, DxButtonModule } from 'devextreme-angular';
import { MultiColumnPrintSettings } from '../../../../core/models/print-settings.model';
import { ReportStateService } from '../../services/report-state.service';

@Component({
  selector: 'app-print-settings',
  standalone: true,
  imports: [FormsModule, DxPopupModule, DxButtonModule],
  template: `
    <dx-popup
      title="Tùy chỉnh in báo cáo nhiều cột"
      [visible]="state.isPrintSettingsOpen()"
      [showCloseButton]="true"
      [hideOnOutsideClick]="false"
      [width]="640"
      [height]="'auto'"
      [maxHeight]="'90vh'"
      (onShowing)="copySettings()"
      (onHiding)="state.closePrintSettings()">
      @if (draft; as settings) {
        <div class="p-4 max-h-[75vh] overflow-y-auto text-sm text-gray-800">
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <label class="flex flex-col gap-1 font-medium">
              Khổ giấy
              <select class="border border-gray-300 rounded p-2 bg-white" [(ngModel)]="settings.paperOrientation">
                <option value="landscape">A4 ngang</option>
                <option value="portrait">A4 dọc</option>
              </select>
            </label>
            <label class="flex items-center gap-2 pt-6">
              <input type="checkbox" [(ngModel)]="settings.repeatTitleOnEveryPage" />
              Lặp tiêu đề trên mỗi trang
            </label>
          </div>

          <h3 class="font-semibold mt-5 mb-2">Font trên bản in</h3>
          <div class="space-y-3">
            @for (section of fontSections; track section.key) {
              <div class="grid grid-cols-[100px_1fr_80px_60px_60px] gap-2 items-center">
                <span>{{ section.label }}</span>
                <select class="border border-gray-300 rounded p-2 bg-white min-w-0" [(ngModel)]="settings[section.key].family">
                  <option value="Arial">Arial</option>
                  <option value="Times New Roman">Times New Roman</option>
                  <option value="Tahoma">Tahoma</option>
                </select>
                <input class="border border-gray-300 rounded p-2 w-full" type="number" min="6" max="20" step="0.5" aria-label="Cỡ chữ" [(ngModel)]="settings[section.key].size" />
                <label class="flex items-center gap-1"><input type="checkbox" [(ngModel)]="settings[section.key].bold" /> Đậm</label>
                <label class="flex items-center gap-1"><input type="checkbox" [(ngModel)]="settings[section.key].italic" /> Nghiêng</label>
              </div>
            }
          </div>

          <div class="mt-5 border-t border-gray-200 pt-4">
            <label class="flex items-center gap-2 font-medium">
              <input type="checkbox" [(ngModel)]="settings.showSigner" /> Hiển thị người ký
            </label>
            @if (settings.showSigner) {
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-3 mt-3">
                <label class="flex flex-col gap-1">Chức danh
                  <input class="border border-gray-300 rounded p-2" maxlength="100" [(ngModel)]="settings.signerTitle" />
                </label>
                <label class="flex flex-col gap-1">Họ tên
                  <input class="border border-gray-300 rounded p-2" maxlength="100" [(ngModel)]="settings.signerName" />
                </label>
              </div>
            }
          </div>

          @if (state.printSettingsError()) {
            <p class="text-red-600 mt-3" role="alert">{{ state.printSettingsError() }}</p>
          }
          <div class="flex flex-wrap justify-between gap-2 mt-6 border-t border-gray-200 pt-4">
            <dx-button text="Khôi phục mặc định" stylingMode="outlined" [disabled]="state.isSavingPrintSettings()" (onClick)="state.resetPrintSettings()"></dx-button>
            <div class="flex gap-2">
              <dx-button text="Hủy" stylingMode="outlined" [disabled]="state.isSavingPrintSettings()" (onClick)="state.closePrintSettings()"></dx-button>
              <dx-button text="Lưu và xem trước" type="default" stylingMode="contained" [disabled]="state.isSavingPrintSettings() || !isValid(settings)" (onClick)="save(settings)"></dx-button>
            </div>
          </div>
        </div>
      }
    </dx-popup>
  `
})
export class PrintSettingsComponent {
  readonly state = inject(ReportStateService);
  draft: MultiColumnPrintSettings | null = null;
  readonly fontSections: { key: 'titleFont' | 'subtitleFont' | 'headerFont' | 'detailFont'; label: string }[] = [
    { key: 'titleFont', label: 'Tiêu đề' },
    { key: 'subtitleFont', label: 'Phụ đề' },
    { key: 'headerFont', label: 'Đầu bảng' },
    { key: 'detailFont', label: 'Dữ liệu' }
  ];

  copySettings(): void {
    const settings = this.state.printSettings();
    this.draft = settings ? structuredClone(settings) : null;
  }

  isValid(settings: MultiColumnPrintSettings): boolean {
    return this.fontSections.every(section => {
      const size = settings[section.key].size;
      return Number.isFinite(size) && size >= 6 && size <= 20;
    }) && settings.signerTitle.length <= 100 && settings.signerName.length <= 100;
  }

  save(settings: MultiColumnPrintSettings): void {
    if (this.isValid(settings)) this.state.savePrintSettings(settings);
  }
}
