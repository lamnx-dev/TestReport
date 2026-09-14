import './style.css';

const API_BASE = 'http://localhost:5168/api/report';

let currentReportData = null;
let currentBlobUrl = null;

// Initialize clean single-header layout
document.querySelector('#app').innerHTML = `
  <header class="app-header">
    <!-- Left: File & Status -->
    <div class="header-left">
      <div class="brand-logo" title="DevExpress Reporting">
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round">
          <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path>
          <polyline points="14 2 14 8 20 8"></polyline>
          <line x1="16" y1="13" x2="8" y2="13"></line>
          <line x1="16" y1="17" x2="8" y2="17"></line>
        </svg>
      </div>
      <div class="header-title-block">
        <span class="report-name" id="metaFileName">BaoCao_Report1.pdf</span>
        <div class="report-meta">
          <span id="connectionDot" class="status-dot" title="Trạng thái kết nối API"></span>
          <span id="metaPagesBadge" class="badge badge-pages">5 trang</span>
          <span id="metaRecordsBadge" class="badge badge-records">150 dòng</span>
          <span id="metaFileSize" class="meta-size">-- KB</span>
        </div>
      </div>
    </div>

    <!-- Center: Page scale control -->
    <div class="header-center">
      <div class="scale-selector-wrap">
        <span class="selector-label">Quy mô:</span>
        <select id="selectRowCount" class="custom-select">
          <option value="150" selected>5 trang (150 dòng)</option>
          <option value="90">3 trang (90 dòng)</option>
          <option value="30">1 trang (30 dòng)</option>
          <option value="300">10 trang (300 dòng)</option>
        </select>
      </div>
    </div>

    <!-- Right: Actions Group -->
    <div class="header-right">
      <button id="btnReload" class="btn btn-icon-text" title="Làm mới dữ liệu từ API">
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2"><path d="M21.5 2v6h-6M21.34 15.57a10 10 0 1 1-.57-8.38l5.67-5.19"/></svg>
        <span>Tải lại</span>
      </button>

      <div class="btn-divider"></div>

      <a id="btnDownloadPdf" href="${API_BASE}/export-pdf?count=150" class="btn btn-primary" download="BaoCao_Report1.pdf" title="Tải xuống tệp PDF">
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><polyline points="7 10 12 15 17 10"/><line x1="12" y1="15" x2="12" y2="3"/></svg>
        <span>Tải PDF</span>
      </a>

      <a id="btnDownloadExcel" href="${API_BASE}/export-excel?count=150" class="btn btn-outline" download title="Xuất ra tệp Excel (.xlsx)">
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/><path d="M8 13h8"/><path d="M8 17h8"/><path d="M10 9h4"/></svg>
        <span>Excel</span>
      </a>

      <button id="btnOpenTab" class="btn btn-icon" title="Mở trên tab trình duyệt riêng" disabled>
        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6"/><polyline points="15 3 21 3 21 9"/><line x1="10" y1="14" x2="21" y2="3"/></svg>
      </button>

      <button id="btnToggleDrawer" class="btn btn-icon" title="Xem chuỗi Base64 nguyên bản">
        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="16 18 22 12 16 6"/><polyline points="8 6 2 12 8 18"/></svg>
      </button>
    </div>
  </header>

  <main class="app-main">
    <div class="viewer-container">
      <iframe id="pdfFrame" title="DevExpress Report Preview"></iframe>

      <!-- Loading Overlay -->
      <div id="loadingOverlay" class="overlay" style="display: none;">
        <div class="spinner"></div>
        <p class="overlay-title">Đang tải báo cáo...</p>
        <p class="overlay-desc">DevExpress Reporting đang kết xuất tài liệu PDF nhiều trang</p>
      </div>

      <!-- Error Overlay -->
      <div id="errorOverlay" class="overlay" style="display: none;">
        <div class="error-card">
          <div class="error-icon">
            <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>
          </div>
          <h2 class="overlay-title">Chưa kết nối được Backend</h2>
          <p id="errorDetails" class="overlay-desc" style="margin: 8px 0 14px;">
            Vui lòng kiểm tra backend tại terminal:<br><code>dotnet run --project TestReport</code>
          </p>
          <button id="btnRetry" class="btn btn-primary">Thử lại</button>
        </div>
      </div>
    </div>
  </main>

  <!-- Drawer for Base64 Inspector -->
  <div id="base64Drawer" class="drawer">
    <div class="drawer-header">
      <span class="drawer-title">🔍 Chuỗi Base64 PDF từ API</span>
      <div style="display: flex; gap: 8px;">
        <button id="btnCopyBase64" class="btn btn-icon-text">Sao chép Base64</button>
        <button id="btnCloseDrawer" class="btn btn-icon-text">Đóng</button>
      </div>
    </div>
    <div id="base64Content" class="drawer-body">Chưa có dữ liệu...</div>
  </div>

  <div id="toast" class="toast">Đã sao chép!</div>
`;

// DOM Elements
const pdfFrame = document.querySelector('#pdfFrame');
const loadingOverlay = document.querySelector('#loadingOverlay');
const errorOverlay = document.querySelector('#errorOverlay');
const errorDetails = document.querySelector('#errorDetails');
const btnReload = document.querySelector('#btnReload');
const btnRetry = document.querySelector('#btnRetry');
const btnDownloadPdf = document.querySelector('#btnDownloadPdf');
const btnDownloadExcel = document.querySelector('#btnDownloadExcel');
const btnOpenTab = document.querySelector('#btnOpenTab');
const btnToggleDrawer = document.querySelector('#btnToggleDrawer');
const btnCloseDrawer = document.querySelector('#btnCloseDrawer');
const btnCopyBase64 = document.querySelector('#btnCopyBase64');
const base64Drawer = document.querySelector('#base64Drawer');
const base64Content = document.querySelector('#base64Content');
const metaFileName = document.querySelector('#metaFileName');
const metaPagesBadge = document.querySelector('#metaPagesBadge');
const metaRecordsBadge = document.querySelector('#metaRecordsBadge');
const metaFileSize = document.querySelector('#metaFileSize');
const connectionDot = document.querySelector('#connectionDot');
const selectRowCount = document.querySelector('#selectRowCount');
const toast = document.querySelector('#toast');

function showToast(msg) {
  toast.textContent = msg;
  toast.classList.add('show');
  setTimeout(() => toast.classList.remove('show'), 2200);
}

// Convert Base64 string to Blob
function base64ToBlob(base64, contentType = 'application/pdf') {
  const byteCharacters = atob(base64);
  const byteNumbers = new Array(byteCharacters.length);
  for (let i = 0; i < byteCharacters.length; i++) {
    byteNumbers[i] = byteCharacters.charCodeAt(i);
  }
  const byteArray = new Uint8Array(byteNumbers);
  return new Blob([byteArray], { type: contentType });
}

// Fetch report from API
async function fetchReportBase64() {
  loadingOverlay.style.display = 'flex';
  errorOverlay.style.display = 'none';
  btnReload.disabled = true;

  const count = selectRowCount.value || 150;
  btnDownloadPdf.href = `${API_BASE}/export-pdf?count=${count}`;
  btnDownloadExcel.href = `${API_BASE}/export-excel?count=${count}`;

  connectionDot.className = 'status-dot loading';

  try {
    const res = await fetch(`${API_BASE}/base64?count=${count}`);
    if (!res.ok) {
      throw new Error(`HTTP ${res.status}: ${res.statusText}`);
    }

    const data = await res.json();
    currentReportData = data;

    // Convert base64 to Blob URL
    const blob = base64ToBlob(data.base64, data.contentType || 'application/pdf');

    if (currentBlobUrl) {
      URL.revokeObjectURL(currentBlobUrl);
    }
    currentBlobUrl = URL.createObjectURL(blob);

    // Render in iframe
    pdfFrame.src = currentBlobUrl;

    // Update metadata
    metaFileName.textContent = data.fileName || 'BaoCao_Report1.pdf';
    metaPagesBadge.textContent = `${data.totalPages || 5} trang`;
    metaRecordsBadge.textContent = `${data.totalRecords || count} dòng`;
    metaFileSize.textContent = (data.fileSize / 1024).toFixed(0) + ' KB';

    // Update base64 viewer
    base64Content.textContent = data.base64;

    // Enable buttons
    btnOpenTab.disabled = false;

    connectionDot.className = 'status-dot';
  } catch (err) {
    console.error('Error fetching report:', err);
    errorDetails.innerHTML = `Lỗi: <strong>${err.message}</strong><br><br>Đảm bảo backend đang chạy:<br><code>dotnet run --project TestReport</code>`;
    errorOverlay.style.display = 'flex';
    connectionDot.className = 'status-dot error';
  } finally {
    loadingOverlay.style.display = 'none';
    btnReload.disabled = false;
  }
}

// Event Listeners
btnReload.addEventListener('click', fetchReportBase64);
btnRetry.addEventListener('click', fetchReportBase64);
selectRowCount.addEventListener('change', fetchReportBase64);

btnDownloadPdf.addEventListener('click', () => {
  showToast('Đang tải file PDF...');
});

btnOpenTab.addEventListener('click', () => {
  if (!currentBlobUrl) return;
  window.open(currentBlobUrl, '_blank');
});

btnToggleDrawer.addEventListener('click', () => {
  base64Drawer.classList.toggle('open');
});

btnCloseDrawer.addEventListener('click', () => {
  base64Drawer.classList.remove('open');
});

btnCopyBase64.addEventListener('click', () => {
  if (!currentReportData?.base64) return;
  navigator.clipboard.writeText(currentReportData.base64);
  showToast('Đã sao chép chuỗi Base64!');
});

// Auto load on start
fetchReportBase64();
