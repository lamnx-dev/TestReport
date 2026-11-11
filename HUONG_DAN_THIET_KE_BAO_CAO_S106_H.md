# Thiết kế sổ S106-H — A4 ngang

Mẫu tham chiếu: sheet `S106-H` trong `Phân tích_Sổ kế toán_TT24.xlsx`, vùng mẫu B12:O89. Những ghi chú hướng dẫn lấy dữ liệu trong Excel không thuộc bản in.

## Mở và xem mẫu

1. Build `dotnet build TestReport.Api --no-restore`.
2. Trong Visual Studio, mở `TestReport.Api/Infrastructure/Reports/S106HReport.cs`, chọn **View Designer**.
3. Chọn **Preview** để xem dữ liệu minh họa. `ObjectDataSource` khai báo kiểu `S106HReportDocument` và phương thức tĩnh `GetSampleData` để Designer nhận Field List và dữ liệu mẫu.

Constructor không tham số chỉ gọi `InitializeComponent()` như S11-H: giữ text mặc định khai báo trực tiếp trong Designer; dữ liệu bảng minh họa do ObjectDataSource cung cấp khi Preview. Constructor nhận dữ liệu mới thay text label bằng `ApplyLabels`. Khi sử dụng dữ liệu của ứng dụng, truyền đối tượng riêng:

```csharp
var data = new S106HReportData(); // Danh sách rỗng vẫn in cấu trúc sổ, không nạp mẫu.
using var report = new S106HReport(data);
report.ExportToPdf("S106-H.pdf");
```

Không thêm API, truy vấn cơ sở dữ liệu hoặc giao diện Angular cho mẫu này.

## Bố cục trong Designer

- Đơn vị `HundredthsOfAnInch`, A4 ngang 1169 × 827, lề 40; bảng rộng 1089. Times New Roman 9, tiêu đề 14 đậm, bảng 8.5.
- `ReportHeader`: đơn vị, mã mẫu, căn cứ nguyên văn Excel, tên sổ và kỳ báo cáo.
- `DetailReport`: `DetailReportBand` bind `PartIRows`; header ba tầng, 14 cột rộng 65 / 70 / 65 / 189 / 10 × 70.
- `DetailReport1`: bind `PartIIRows`; header ba tầng, 9 cột rộng 65 / 70 / 65 / 189 / 5 × 140. GroupHeader theo `NhomTieuMuc` hiển thị mã tiểu mục và lặp khi sang trang.
- Hai phần trên in theo thứ tự tháng đầu vào, không tự sắp xếp theo ngày. Dòng cộng tháng đã nằm cuối dữ liệu của tháng, không cần GroupHeader riêng cho tháng.
- `DetailReport2`: bind `UnusedRows`, 6 cột rộng 160 / 180 / 180 / 180 / 180 / 209.
- Header bảng lặp mỗi trang trong từng phần, giữ cùng dòng đầu. Dòng chi tiết và khối khóa sổ/chữ ký giữ liền trang; ba phần chạy nối tiếp.
- `ReportFooter`: số trang thực tế (`XRPageInfo`), ngày mở sổ, ngày ký và ba vị trí ký. `BottomMargin`: trang hiện tại / tổng số trang.

Các control, hàng và ô bảng được khai báo trong `.Designer.cs` để có thể chọn và chỉnh trực tiếp. `.cs` chỉ khởi tạo và gán nguồn dữ liệu runtime; `.resx` dành cho tài nguyên Designer.

## Cấp dữ liệu và binding

`S106HReportData` nhận thông tin đơn vị, `KyBaoCao` (chuỗi hiển thị tháng/quý/năm), ngày mở sổ/ngày ký nullable và tên người ký.

- `NhanVaNopTra`: đầu kỳ, điều chỉnh, danh sách tháng, lũy kế từ đầu năm và cuối kỳ. Mỗi tháng có chi tiết và `CongThang`; mỗi khoản có `Nhan`, `NopTra`.
- `TieuMuc`: mã tiểu mục, các tháng chi tiết, cộng tháng và lũy kế của riêng tiểu mục.
- `TongDaSuDungTrongNam`: tổng năm đã được bên gọi tính sẵn, kể cả khi kỳ đang in chỉ là một tháng/quý.
- `ChuaSuDung`, `GhiChuChuaSuDung`: giá trị phần III đã tính sẵn.
- `S106HAmounts`: tổng số, tổ chức/hoạt động, CNTT, xây dựng cơ bản và khác; tất cả là `decimal`. Cùng trường `ToChucHoatDong` được hiển thị bằng tên “Kinh phí chi hoạt động bộ máy” ở phần II theo Excel.

`PartIRows` và `PartIIRows` chỉ tạo dòng trình bày: thêm nhãn, dòng cộng và khóa nhóm. Không cộng số tiền, tính số dư, lọc theo kỳ hoặc thay đổi đối tượng đầu vào. Bên gọi phải cung cấp các nhóm tháng theo đúng thứ tự cần in và số lũy kế thực tế; không dùng tổng những tháng đang hiển thị thay cho lũy kế đầu năm.

`S106HReportDocument` chụp các danh sách dòng một lần khi khởi tạo báo cáo, để ObjectDataSource duyệt cùng một collection trong quá trình in. Các bảng bind qua `PartIRows`, `PartIIRows`, `UnusedRows`. Label đầu/cuối sổ được gán `Text` trực tiếp trong `ApplyLabels`; label mã tiểu mục được gán theo nhóm trong `BeforePrint`, không dùng ExpressionBinding. Hãy tạo lại report khi thay đổi dữ liệu nguồn.

Ngày định dạng `dd/MM/yyyy` với dấu `/` cố định. Khoản tiền `null` để trống, số 0 hiển thị `0`, số âm đặt trong ngoặc; số tiền căn phải, không xuống dòng, tự thu nhỏ chữ khi vượt bề rộng. Diễn giải và ghi chú được xuống dòng và tăng chiều cao. Với số tiền rất lớn, chữ thu nhỏ có thể khó đọc trên A4; cần kiểm tra PDF ở tỷ lệ 100% theo nhu cầu in.

Danh sách rỗng vẫn giữ header, các dòng chỉ tiêu cố định và vùng ký; không tự chèn giao dịch mẫu. Muốn in tháng không phát sinh, bên gọi vẫn thêm nhóm tháng đó và truyền số cộng bằng 0.

## Nghiệm thu

- Build sạch; mở Designer, kiểm tra Field List, Preview rồi lưu/mở lại mẫu.
- Đối chiếu đủ ba phần, tên các nhóm kinh phí, dòng tháng/lũy kế và chữ ký với Excel.
- Xem PDF ở 100%: A4 ngang, không tràn lề, header chứng từ gộp đúng hai cột, phần I có đủ 10 cột tiền.
- Kiểm tra một tháng, nhiều tháng, nhiều tiểu mục, tháng rỗng, đối tượng rỗng; dữ liệu mẫu chỉ xuất hiện khi chủ động dùng mẫu.
- Kiểm tra nhiều trang, diễn giải dài, số tiền lớn, số âm, 0 và ngày trống; dòng cộng vẫn ở cuối tháng, không cộng lẫn tiểu mục, tên tiểu mục/header lặp đúng, chữ ký không bị cắt.

Việc hiển thị và lưu lại bằng Visual Studio Designer cần kiểm tra trực tiếp trên máy có DevExpress Visual Studio integration.
