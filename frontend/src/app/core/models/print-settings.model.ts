export interface PrintFontSettings {
  family: 'Arial' | 'Times New Roman' | 'Tahoma';
  size: number;
  bold: boolean;
  italic: boolean;
}

export interface MultiColumnPrintSettings {
  paperOrientation: 'landscape' | 'portrait';
  repeatTitleOnEveryPage: boolean;
  showSigner: boolean;
  signerTitle: string;
  signerName: string;
  titleFont: PrintFontSettings;
  subtitleFont: PrintFontSettings;
  headerFont: PrintFontSettings;
  detailFont: PrintFontSettings;
}
