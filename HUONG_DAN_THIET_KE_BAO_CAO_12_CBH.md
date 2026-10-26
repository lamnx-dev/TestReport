# CẨM NANG HƯỚNG DẪN THIẾT KẾ BÁO CÁO MẪU SỐ 12-CBH (KHỔ DỌC)
## "DANH SÁCH BÁO GIẢM HƯỞNG, TẠM DỪNG HƯỞNG CÁC CHẾ ĐỘ BHXH HÀNG THÁNG, TRỢ CẤP THẤT NGHIỆP"
### 🖱️ Thiết Kế Trực Quan Trên DevExpress XtraReports (Visual Studio)

---

## 📑 CẤU TRÚC PHÂN NHÓM VÀ GROUPBAND (ĐÃ CẬP NHẬT)

Báo cáo đã chuyển từ mô hình mảng phẳng sang **mô hình phân cấp nhóm (Master - Detail với GroupHeaderBand)**:
1. **Model dữ liệu (`BaoCao12CbhGroupDto`)**:
   - Thay vì trả về 1 mảng dữ liệu phẳng nhồi nhét cả dòng nhóm lẫn dòng chi tiết, API/Service trả về danh sách các nhóm: `List<BaoCao12CbhGroupDto> Groups`.
   - Mỗi nhóm gồm:
     - `Stt`: Số La Mã của nhóm (`"I"`, `"II"`, `"VIII"`, `"XI"`...).
     - `TenNhom`: Tên chế độ (`"Chế độ hưu trí"`, `"Chế độ tử tuất"`...).
     - `Items`: Danh sách các dòng chi tiết `List<BaoCao12CbhItemDto>`.
     - `TongSoTienConPhaiTra`, `TongSoTienPhaiThuHoi`: Tổng tiền của nhóm.
2. **Dải băng phân nhóm (`GroupHeaderBand - GroupHeaderNhom`)**:
   - Dùng để hiển thị dòng tiêu đề nhóm (`I. Chế độ hưu trí`, `II. Chế độ tử tuất`...) với chữ in đậm.
   - Nhóm theo trường: `Stt`.
   - Thuộc tính `GroupUnion = WithFirstDetail` đảm bảo tiêu đề nhóm không bị đứng lẻ loi cuối trang.
3. **Dải băng chi tiết con (`DetailReportBand - DetailReportItems`)**:
   - Liên kết với thuộc tính `Items` (`DataMember = "Items"`).
   - Hiển thị từng dòng chi tiết của đối tượng hưởng chế độ (STT 1, 2..., Họ tên, Mã BHXH, Lý do, Số tiền...).
4. **Dòng Tổng cộng (`tblTotal` trong `ReportFooterBand`)**:
   - Cột 5 (Số tiền còn phải trả): `sumSum([TongSoTienConPhaiTra])` với `SummaryRunning.Report`.
   - Cột 6 (Số tiền phải thu hồi): `sumSum([TongSoTienPhaiThuHoi])` với `SummaryRunning.Report`.

---

## 📐 BẢNG THÔNG SỐ KÍCH THƯỚC CHUẨN 8 CỘT (750 PX)

| STT | Ký hiệu | Tên Cột | Chiều rộng (`Width`) | Căn lề | Định dạng dòng Tổng cộng |
|:---:|:---:|:---|:---:|:---:|:---|
| 1 | `A` | **TT** | **35 px** | Giữa | Gộp với cột B: Hiển thị chữ **Tổng cộng** |
| 2 | `B` | **Họ và tên** | **155 px** | Trái (Padding 5) | (Nằm trong ô gộp 190 px) |
| 3 | `1` | **Mã BHXH** | **90 px** | Giữa | Để trống (Đã bỏ XXX) |
| 4 | `2` | **Lý do giảm** | **130 px** | Trái (Padding 4) | Để trống (Đã bỏ XXX) |
| 5 | `3` | **Ngày, tháng, năm giảm** | **80 px** | Giữa | Để trống (Đã bỏ XXX) |
| 6 | `4` | **Đã chi trả hết ngày...** | **80 px** | Giữa | Để trống (Đã bỏ XXX) |
| 7 | `5` | **Số tiền còn phải trả** | **90 px** | Phải (Padding 4) | **`sumSum([TongSoTienConPhaiTra])`** (`{0:#,##0}`) |
| 8 | `6` | **Số tiền phải thu hồi** | **90 px** | Phải (Padding 4) | **`sumSum([TongSoTienPhaiThuHoi])`** (`{0:#,##0}`) |
| **TỔNG** | | **Toàn bộ chiều rộng bảng** | **750 px** | | *(Khớp 100% trang in A4 dọc)* |

---

## 🚀 CÁCH XEM TRƯỚC VÀ XUẤT FILE PDF

1. **Xem trực tiếp trên trình duyệt (Preview)**:
   ```
   GET https://localhost:7080/api/report/12-cbh-preview
   ```
2. **Tải file PDF về máy**:
   ```
   GET https://localhost:7080/api/report/12-cbh-export-pdf?thang=10&nam=2026
   ```
3. **Mở Visual Studio Designer**:
   - Mở file `TestReport.Api/Infrastructure/Reports/BaoCao12CbhReport.cs` $\rightarrow$ Chuột phải chọn **View Designer**.
   - Nhấn **Ctrl + Shift + B** để nạp cấu trúc Field List mới.
   - Click vào tab **Preview** ở mép dưới để xem báo cáo trực quan.
