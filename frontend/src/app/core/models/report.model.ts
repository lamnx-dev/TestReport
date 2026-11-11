export interface ReportElementStyle {
  fontFamily?: string;
  fontSize?: number;
  bold?: boolean;
  italic?: boolean;
  color?: string;
}
export type AppearanceRegion = 'title' | 'tableHeader' | 'tableBody';
export type ReportAppearance = Partial<Record<AppearanceRegion, ReportElementStyle>>;
export interface ReportCatalog {
  reports: { id: string; name: string }[];
  fonts: string[];
  minFontSize: number;
  maxFontSize: number;
}
