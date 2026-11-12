# Thiết kế và API S90-H

Mẫu tham chiếu: sheet `S90-H`, vùng A12:E41 trong `Phân tích_Sổ kế toán_TT24.xlsx`. Khổ A4 dọc, Times New Roman, năm cột STT / Chỉ tiêu / Mã số / Số tiền / Ghi chú. Giữ nguyên căn cứ trong Excel, không coi đây là xác nhận hiệu lực pháp lý của văn bản.

## Designer

Build backend, mở `TestReport.Api/Infrastructure/Reports/S90HReport.cs` trong Visual Studio, chọn **View Designer**. Các band, label, bảng, hàng và ô đều được khai báo trong `.Designer.cs`; `.resx` và cấu hình `DependentUpon` đi kèm. Field List và Preview dùng ObjectDataSource gọi `S90HReportData.GetSampleRows()`.

Constructor không tham số dành cho Designer. Khi dùng dữ liệu thật, gọi `new S90HReport(data)`. Danh sách đầu vào rỗng vẫn in đủ 16 dòng và vùng ký, không thay bằng mẫu. Báo cáo chụp danh sách trình bày khi khởi tạo; tạo report mới khi thay dữ liệu.

Header bảng lặp mỗi trang; ba vị trí ký giữ trong cùng một khối. Tag `ReportTitle`, `TableHeader`, `TableBody` hỗ trợ đổi kiểu chữ qua chức năng hiện có. Các dòng nhóm/tổng giữ in đậm khi đặt nội dung bảng thành không đậm.

## API

| Phương thức và đường dẫn | Dữ liệu | Kết quả |
|---|---|---|
| `GET /api/report/s90-h-preview` | Không | PDF mẫu inline, tên `S90-H.pdf` |
| `POST /api/report/s90-h-export-pdf` | JSON `S90HReportData` | PDF attachment, tên `S90-H.pdf` |
| `GET /api/reports` | Không | Danh mục có `s90h` |
| `POST /api/reports/s90h/render` | `{ "appearance": {} }` | PDF mẫu tùy chỉnh, tên `S90H.pdf` |

Ví dụ body xuất PDF:

```json
{
  "donViCapTren": "BẢO HIỂM XÃ HỘI VIỆT NAM",
  "donViBaoCao": "BẢO HIỂM XÃ HỘI THÀNH PHỐ HÀ NỘI",
  "kyBaoCao": "Quý IV năm 2026",
  "donViTinh": "đồng",
  "ngayMoSo": "2026-01-01",
  "ngayKy": "2026-12-31",
  "chucDanhKeToan": "KẾ TOÁN",
  "chucDanhKeToanTruong": "KẾ TOÁN TRƯỞNG",
  "chucDanhThuTruong": "THỦ TRƯỞNG ĐƠN VỊ",
  "keToan": "Nguyễn Văn An",
  "keToanTruong": "Trần Thị Bình",
  "thuTruongDonVi": "Lê Văn Minh",
  "chiTieu": [
    { "maChiTieu": "01", "soTien": 1000000000, "ghiChu": "" },
    { "maChiTieu": "02", "soTien": 950000000, "ghiChu": "Số liệu đã tổng hợp" },
    { "maChiTieu": "03", "soTien": 900000000 },
    { "maChiTieu": "04", "soTien": 50000000 },
    { "maChiTieu": "05", "soTien": 700000000 },
    { "maChiTieu": "06", "soTien": 0 },
    { "maChiTieu": "khau-hao-bo-sung", "soTien": 0 },
    { "maChiTieu": "07", "soTien": 50000000 },
    { "maChiTieu": "08", "soTien": 10000000 },
    { "maChiTieu": "09", "soTien": 90000000 },
    { "maChiTieu": "10", "soTien": 100000000 },
    { "maChiTieu": "10.1", "soTien": 40000000 },
    { "maChiTieu": "10.2", "soTien": 35000000 },
    { "maChiTieu": "10.3", "soTien": 25000000 }
  ]
}
```

`canCu` có thể truyền để thay nguyên văn căn cứ mặc định. Ngày nullable, hiển thị `dd/MM/yyyy`; ngày ký hiển thị dạng ngày/tháng/năm bằng chữ. Tên người ký mặc định trống.

Các mã được hỗ trợ là `01` đến `10` (có số 0 đầu), `10.1`, `10.2`, `10.3`, `khau-hao-bo-sung`. Mã nội bộ cuối cùng dành cho dòng khấu hao bổ sung quỹ, không in vào cột mã. Dòng I và “Trích lập các quỹ” là nhãn cố định không nhận số liệu. Ba STT cuối là `7.1`, `7.2`, `7.3` theo định dạng hiển thị của ô Excel.

Chỉ tiêu thiếu hoặc tiền null để trống; 0 in `0`; số âm trong ngoặc. Báo cáo giữ thứ tự template, không sửa đầu vào và không tính lại tổng 02, 09, 10. Ghi chú nguồn/công thức trong Excel không thuộc nội dung bản in. Bên gọi chịu trách nhiệm tổng hợp số liệu.

Body null/rỗng, JSON sai, số tiền không hợp lệ, danh sách null, phần tử null, mã lạ hoặc trùng trả HTTP 400. Body `{}` hợp lệ và in biểu mẫu trống. Không có truy vấn cơ sở dữ liệu, thao tác tổng hợp hay khóa sổ.

## Angular và kiểm tra

Màn hình hiện có lấy danh mục từ backend; chọn **S90-H — Bảng xác định chênh lệch thu, chi**, tạo báo cáo, tùy chỉnh chữ, tải PDF hoặc mở để in. Không có màn hình nhập số liệu.

```powershell
dotnet build TestReport.Api --no-restore
dotnet run --project tests/ReportChecks --no-restore
cd frontend
npm run build
```

Chạy backend cổng 5168 và frontend cổng 4200, sau đó `python tests/report-ui-checks.py` để kiểm tra trình duyệt. Yêu cầu Python Playwright và Edge; dùng `REPORT_UI_URL` nếu giao diện chạy ở địa chỉ khác. ReportChecks xuất PDF/PNG kiểm tra vào thư mục chạy của chương trình.

Nghiệm thu thêm trên Visual Studio có DevExpress integration: mở Designer, Field List, Preview, lưu và mở lại. Kiểm tra bản in PDF ở 100%, nhất là số tiền lớn phải thu nhỏ chữ và ghi chú nhiều trang.
