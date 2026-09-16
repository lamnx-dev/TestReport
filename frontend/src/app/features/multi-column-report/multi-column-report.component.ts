import { Component, OnInit, inject } from '@angular/core';
import { ReportStateService } from './services/report-state.service';
import { ReportHeaderComponent } from './components/report-header/report-header.component';
import { ReportToolbarComponent } from './components/report-toolbar/report-toolbar.component';
import { ReportViewerComponent } from './components/report-viewer/report-viewer.component';
import { ReportDrawerComponent } from './components/report-drawer/report-drawer.component';

@Component({
  selector: 'app-multi-column-report',
  standalone: true,
  imports: [
    ReportHeaderComponent,
    ReportToolbarComponent,
    ReportViewerComponent,
    ReportDrawerComponent
  ],
  providers: [ReportStateService],
  template: `
    <div class="flex flex-col h-screen w-screen overflow-hidden bg-slate-50">
      <!-- DevExpress Application Header Toolbar -->
      <app-report-header></app-report-header>

      <!-- Main Workspace: Parameters Panel + Viewer Canvas -->
      <div class="flex flex-row flex-1 overflow-hidden relative">
        <app-report-toolbar></app-report-toolbar>
        <main class="flex flex-1 overflow-hidden relative">
          <app-report-viewer></app-report-viewer>
        </main>
      </div>

      <!-- Base64 & Technical Inspector Drawer -->
      <app-report-drawer></app-report-drawer>
    </div>
  `
})
export class MultiColumnReportComponent implements OnInit {
  private readonly state = inject(ReportStateService);

  ngOnInit(): void {
    // Tự động tải dữ liệu lần đầu
    this.state.loadReport();
  }
}
