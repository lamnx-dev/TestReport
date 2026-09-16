import { Component } from '@angular/core';
import { MultiColumnReportComponent } from './features/multi-column-report/multi-column-report.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [MultiColumnReportComponent],
  templateUrl: './app.html'
})
export class App {}
