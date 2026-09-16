export interface MultiColumnRow {
  stt: number;
  values: Record<string, string>;
}

export interface MultiColumnReportData {
  totalColumns: number;
  columnsPerPage: number;
  columnNames: string[];
  rows: MultiColumnRow[];
}
