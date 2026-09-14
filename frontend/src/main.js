import "./style.css";

const API_BASE = "http://localhost:5168/api/report";

let currentReportMode = "multicol"; // 'multicol' hoặc 'weather'
let currentReportData = null;
let currentBlobUrl = null;

// Khung HTML chính
document.querySelector("#app").innerHTML = `
  <header class="app-header">
    <!-- Left: Brand & File Info -->
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
        <span class="report-name" id="metaFileName">BaoCao_NhieuCot_TachTrang.pdf</span>
        <div class="report-meta">
          <span id="connectionDot" class="status-dot" title="Trạng thái kết nối API"></span>
          <span id="metaPagesBadge" class="badge badge-pages">-- trang</span>
          <span id="metaRecordsBadge" class="badge badge-records">-- dòng</span>
          <span id="metaFileSize" class="meta-size">-- KB</span>
        </div>
      </div>
    </div>

    <!-- Center: Mode switcher & Controls -->
    <div class="header-center">
      <div class="report-type-switch">
        <button id="tabMultiCol" class="tab-btn active" title="Báo cáo nhiều cột chia đoạn in">
          📊 Báo Cáo Nhiều Cột (Tách Trang)
        </button>
        <button id="tabWeather" class="tab-btn" title="Báo cáo thời tiết mẫu">
          ⛅ Báo Cáo Thời Tiết
        </button>
      </div>
    </div>

    <!-- Right: Actions Group -->
    <div class="header-right">
      <button id="btnReload" class="btn btn-icon-text" title="Làm mới dữ liệu từ API">
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2"><path d="M21.5 2v6h-6M21.34 15.57a10 10 0 1 1-.57-8.38l5.67-5.19"/></svg>
        <span>Tải lại</span>
      </button>

      <div class="btn-divider"></div>

      <button id="btnPrintDirect" class="btn btn-outline" title="In trực tiếp báo cáo (Ctrl+P)" disabled>
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2">
          <polyline points="6 9 6 2 18 2 18 9"></polyline>
          <path d="M6 18H4a2 2 0 0 1-2-2v-5a2 2 0 0 1 2-2h16a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2h-2"></path>
          <rect x="6" y="14" width="12" height="8"></rect>
        </svg>
        <span>In</span>
      </button>

      <button id="btnDownloadPdf" class="btn btn-primary" title="Tải xuống tệp PDF vào thư mục Downloads" disabled>
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><polyline points="7 10 12 15 17 10"/><line x1="12" y1="15" x2="12" y2="3"/></svg>
        <span>Tải PDF</span>
      </button>

      <button id="btnOpenTab" class="btn btn-icon" title="Mở trên tab trình duyệt riêng" disabled>
        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6"/><polyline points="15 3 21 3 21 9"/><line x1="10" y1="14" x2="21" y2="3"/></svg>
      </button>

      <button id="btnToggleDrawer" class="btn btn-icon" title="Xem chuỗi Base64 và thông tin chi tiết">
        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="16 18 22 12 16 6"/><polyline points="8 6 2 12 8 18"/></svg>
      </button>
    </div>

  </header>

  <!-- Sub-header Bar: Cấu hình phân chia cột hoặc quy mô dòng -->
  <div class="sub-header-bar">
    <div id="subHeaderInfo" class="info-tag">
      <span>💡 Cơ chế: In hết Cột 1 đến Cột x, sau đó sang trang in Cột x+1 đến Cột n. Cột STT (tiêu đề trống) luôn giữ ở đầu.</span>
    </div>

    <!-- Controls cho chế độ nhiều cột -->
    <div id="multicolControls" class="multicol-controls">
      <div class="scale-selector-wrap">
        <span class="selector-label">Tổng số cột:</span>
        <select id="selectTotalCols" class="custom-select">
          <option value="12">12 cột</option>
          <option value="18" selected>18 cột (3 phần)</option>
          <option value="24">24 cột (4 phần)</option>
          <option value="30">30 cột (5 phần)</option>
        </select>
      </div>

      <div class="scale-selector-wrap">
        <span class="selector-label">Số cột / trang in:</span>
        <select id="selectColsPerPage" class="custom-select">
          <option value="4">4 cột / trang</option>
          <option value="6" selected>6 cột / trang</option>
          <option value="8">8 cột / trang</option>
        </select>
      </div>

      <div class="scale-selector-wrap">
        <span class="selector-label">Số dòng:</span>
        <select id="selectMultiRowCount" class="custom-select">
          <option value="30">30 dòng</option>
          <option value="50">50 dòng</option>
          <option value="100" selected>100 dòng (Mặc định)</option>
          <option value="200">200 dòng</option>
        </select>
      </div>

    </div>

    <!-- Controls cho chế độ thời tiết cũ -->
    <div id="weatherControls" class="multicol-controls" style="display: none;">
      <div class="scale-selector-wrap">
        <span class="selector-label">Quy mô:</span>
        <select id="selectWeatherRowCount" class="custom-select">
          <option value="30">1 trang (30 dòng)</option>
          <option value="90">3 trang (90 dòng)</option>
          <option value="150" selected>5 trang (150 dòng)</option>
          <option value="300">10 trang (300 dòng)</option>
        </select>
      </div>
    </div>
  </div>

  <main class="app-main">
    <div class="viewer-container">
      <iframe id="pdfFrame" title="DevExpress Report Preview"></iframe>

      <!-- Loading Overlay -->
      <div id="loadingOverlay" class="overlay" style="display: none;">
        <div class="spinner"></div>
        <p class="overlay-title">Đang kết xuất báo cáo...</p>
        <p class="overlay-desc">DevExpress Reporting đang xử lý phân chia nhóm cột và xuất file PDF</p>
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
      <span class="drawer-title">🔍 Thông tin chi tiết & Base64</span>
      <div style="display: flex; gap: 8px;">
        <button id="btnCopyBase64" class="btn btn-icon-text">Sao chép Base64</button>
        <button id="btnCloseDrawer" class="btn btn-icon-text">Đóng</button>
      </div>
    </div>
    <div id="base64Content" class="drawer-body">Chưa có dữ liệu...</div>
  </div>

  <div id="toast" class="toast">Đã sao chép!</div>
`;

// DOM References
const pdfFrame = document.querySelector("#pdfFrame");
const loadingOverlay = document.querySelector("#loadingOverlay");
const errorOverlay = document.querySelector("#errorOverlay");
const errorDetails = document.querySelector("#errorDetails");
const btnReload = document.querySelector("#btnReload");
const btnRetry = document.querySelector("#btnRetry");
const btnPrintDirect = document.querySelector("#btnPrintDirect");
const btnDownloadPdf = document.querySelector("#btnDownloadPdf");
const btnOpenTab = document.querySelector("#btnOpenTab");
const btnToggleDrawer = document.querySelector("#btnToggleDrawer");
const btnCloseDrawer = document.querySelector("#btnCloseDrawer");
const btnCopyBase64 = document.querySelector("#btnCopyBase64");
const base64Drawer = document.querySelector("#base64Drawer");
const base64Content = document.querySelector("#base64Content");
const metaFileName = document.querySelector("#metaFileName");
const metaPagesBadge = document.querySelector("#metaPagesBadge");
const metaRecordsBadge = document.querySelector("#metaRecordsBadge");
const metaFileSize = document.querySelector("#metaFileSize");
const connectionDot = document.querySelector("#connectionDot");
const toast = document.querySelector("#toast");

const tabMultiCol = document.querySelector("#tabMultiCol");
const tabWeather = document.querySelector("#tabWeather");
const subHeaderInfo = document.querySelector("#subHeaderInfo");
const multicolControls = document.querySelector("#multicolControls");
const weatherControls = document.querySelector("#weatherControls");
const selectTotalCols = document.querySelector("#selectTotalCols");
const selectColsPerPage = document.querySelector("#selectColsPerPage");
const selectMultiRowCount = document.querySelector("#selectMultiRowCount");
const selectWeatherRowCount = document.querySelector("#selectWeatherRowCount");

function showToast(msg) {
  toast.textContent = msg;
  toast.classList.add("show");
  setTimeout(() => toast.classList.remove("show"), 2500);
}

function base64ToBlob(base64, contentType = "application/pdf") {
  const byteCharacters = atob(base64);
  const byteNumbers = new Array(byteCharacters.length);
  for (let i = 0; i < byteCharacters.length; i++) {
    byteNumbers[i] = byteCharacters.charCodeAt(i);
  }
  const byteArray = new Uint8Array(byteNumbers);
  return new Blob([byteArray], { type: contentType });
}

// Tải file PDF trực tiếp về máy vào thư mục Downloads của hệ điều hành
async function triggerDownload() {
  const fileName =
    currentReportMode === "multicol"
      ? "BaoCao_NhieuCot_TachTrang.pdf"
      : "BaoCao_Report1.pdf";

  showToast(`Đang tải file ${fileName}...`);

  let downloadUrl = "";
  if (currentReportMode === "multicol") {
    const totalCols = selectTotalCols.value;
    const colsPerPage = selectColsPerPage.value;
    const rowCount = selectMultiRowCount.value;
    downloadUrl = `${API_BASE}/multi-column-export-pdf?totalColumns=${totalCols}&columnsPerPage=${colsPerPage}&rowCount=${rowCount}`;
  } else {
    const count = selectWeatherRowCount.value || 150;
    downloadUrl = `${API_BASE}/export-pdf?count=${count}`;
  }

  try {
    // 1. Nếu đã có dữ liệu base64 trong RAM, tạo file tải về ngay lập tức (không cần chờ tải lại mạng)
    if (currentReportData?.base64) {
      const blob = base64ToBlob(currentReportData.base64, "application/pdf");
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.style.display = "none";
      a.href = url;
      a.download = fileName;
      document.body.appendChild(a);
      a.click();
      setTimeout(() => {
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      }, 3000);
      showToast(`Đã lưu tệp: ${fileName}`);
      return;
    }

    // 2. Fetch trực tiếp từ API endpoint
    const response = await fetch(downloadUrl);
    if (!response.ok) throw new Error("HTTP " + response.status);
    const blob = await response.blob();
    const url = window.URL.createObjectURL(new Blob([blob], { type: "application/pdf" }));
    const a = document.createElement("a");
    a.style.display = "none";
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    setTimeout(() => {
      document.body.removeChild(a);
      window.URL.revokeObjectURL(url);
    }, 3000);

    showToast(`Đã lưu tệp: ${fileName}`);
  } catch (err) {
    console.error("Lỗi khi tải file:", err);
    // Fallback: Mở tab tải trực tiếp
    window.location.href = downloadUrl;
  }
}



// In báo cáo trực tiếp
function triggerPrint() {
  if (!currentBlobUrl) return;
  try {
    if (pdfFrame && pdfFrame.contentWindow) {
      pdfFrame.contentWindow.focus();
      pdfFrame.contentWindow.print();
    } else {
      window.open(currentBlobUrl, "_blank");
    }
  } catch {
    window.open(currentBlobUrl, "_blank");
  }
}

// Hàm tải báo cáo theo chế độ đang chọn
async function loadReport() {
  loadingOverlay.style.display = "flex";
  errorOverlay.style.display = "none";
  btnReload.disabled = true;
  btnDownloadPdf.disabled = true;
  btnPrintDirect.disabled = true;
  btnOpenTab.disabled = true;
  connectionDot.className = "status-dot loading";

  try {
    let url = "";

    let previewUrl = "";
    let downloadFileName = "BaoCao_NhieuCot_TachTrang.pdf";

    if (currentReportMode === "multicol") {
      const totalCols = selectTotalCols.value;
      const colsPerPage = selectColsPerPage.value;
      const rowCount = selectMultiRowCount.value;
      const query = `totalColumns=${totalCols}&columnsPerPage=${colsPerPage}&rowCount=${rowCount}`;

      url = `${API_BASE}/multi-column-base64?${query}`;
      previewUrl = `${API_BASE}/multi-column-preview?${query}`;
      downloadFileName = "BaoCao_NhieuCot_TachTrang.pdf";

      const totalParts = Math.ceil(totalCols / colsPerPage);
      subHeaderInfo.innerHTML = `💡 Báo cáo <strong>${totalCols} cột</strong> chia làm <strong>${totalParts} phần in</strong> (${colsPerPage} cột/trang). Mỗi trang đều có <strong>cột STT (tiêu đề trống)</strong> ở đầu đối chiếu dòng.`;
    } else {
      const count = selectWeatherRowCount.value || 150;
      url = `${API_BASE}/base64?count=${count}`;
      previewUrl = `${API_BASE}/preview?count=${count}`;
      downloadFileName = "BaoCao_Report1.pdf";
      subHeaderInfo.innerHTML = `💡 Báo cáo thời tiết mẫu DevExpress chuẩn khổ dọc Portrait A4 (${count} dòng).`;
    }

    // Đặt document.title để thanh công cụ của trình duyệt tự nhận tên file có đuôi .pdf
    document.title = downloadFileName;

    const res = await fetch(url);
    if (!res.ok) {
      throw new Error(`HTTP ${res.status}: ${res.statusText}`);
    }

    const data = await res.json();
    currentReportData = data;
    currentReportData.fileName = downloadFileName;

    // Sử dụng trực tiếp previewUrl từ API (có header Content-Disposition chứa tên file chuẩn)
    // Điều này giúp nút Download bên trong toolbar của iframe tự nhận diện đúng tên file kèm đuôi .pdf!
    pdfFrame.src = previewUrl;
    currentBlobUrl = previewUrl;

    metaFileName.textContent = downloadFileName;
    metaPagesBadge.textContent = `${data.totalPages} trang`;
    metaRecordsBadge.textContent = `${data.totalRecords} dòng`;
    metaFileSize.textContent = `${(data.fileSize / 1024).toFixed(0)} KB`;
    base64Content.textContent = data.base64;


    btnDownloadPdf.disabled = false;
    btnPrintDirect.disabled = false;
    btnOpenTab.disabled = false;
    connectionDot.className = "status-dot";
  } catch (err) {
    console.error("Error fetching report:", err);
    errorDetails.innerHTML = `Lỗi: <strong>${err.message}</strong><br><br>Đảm bảo backend đang chạy:<br><code>dotnet run --project TestReport</code>`;
    errorOverlay.style.display = "flex";
    connectionDot.className = "status-dot error";
  } finally {
    loadingOverlay.style.display = "none";
    btnReload.disabled = false;
  }
}

// Chuyển tab
tabMultiCol.addEventListener("click", () => {
  if (currentReportMode === "multicol") return;
  currentReportMode = "multicol";
  tabMultiCol.classList.add("active");
  tabWeather.classList.remove("active");
  multicolControls.style.display = "flex";
  weatherControls.style.display = "none";
  loadReport();
});

tabWeather.addEventListener("click", () => {
  if (currentReportMode === "weather") return;
  currentReportMode = "weather";
  tabWeather.classList.add("active");
  tabMultiCol.classList.remove("active");
  weatherControls.style.display = "flex";
  multicolControls.style.display = "none";
  loadReport();
});

// Thay đổi cấu hình
selectTotalCols.addEventListener("change", loadReport);
selectColsPerPage.addEventListener("change", loadReport);
selectMultiRowCount.addEventListener("change", loadReport);
selectWeatherRowCount.addEventListener("change", loadReport);

btnReload.addEventListener("click", loadReport);
btnRetry.addEventListener("click", loadReport);

btnDownloadPdf.addEventListener("click", triggerDownload);
btnPrintDirect.addEventListener("click", triggerPrint);

btnOpenTab.addEventListener("click", () => {
  if (!currentBlobUrl) return;
  window.open(currentBlobUrl, "_blank");
});

btnToggleDrawer.addEventListener("click", () => {
  base64Drawer.classList.toggle("open");
});

btnCloseDrawer.addEventListener("click", () => {
  base64Drawer.classList.remove("open");
});

btnCopyBase64.addEventListener("click", () => {
  if (!currentReportData?.base64) return;
  navigator.clipboard.writeText(currentReportData.base64);
  showToast("Đã sao chép chuỗi Base64!");
});

// Tải báo cáo lần đầu khi khởi động
loadReport();

