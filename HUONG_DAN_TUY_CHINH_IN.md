# Chọn báo cáo và tùy chỉnh in

Chạy API: `dotnet run --project TestReport.Api --launch-profile http`.
Chạy FE: `cd frontend` rồi `npm start`. Mở http://localhost:4200.
FE dùng `/api/reports`; development proxy chuyển `/api/**` tới http://localhost:5168.
Khi triển khai, cấu hình reverse proxy `/api` về API cùng origin với FE.

## Luồng sử dụng

Chọn S11-H hoặc S106-H, tùy chỉnh ba vùng và bấm **Tạo báo cáo**.
PDF xuất hiện bên phải; **Tải PDF** và **Mở PDF để in** dùng kết quả đã tạo.
Sau khi sửa tùy chỉnh, cần tạo lại PDF để dùng hai nút này. Đổi báo cáo xóa cấu hình và PDF cũ.
**Khôi phục theo mẫu** xóa toàn bộ tùy chỉnh; bấm tạo để nhận bản mặc định.
Cấu hình chỉ nằm trong phiên màn hình. Dữ liệu là dữ liệu mẫu tại BE.

## Gán Tag trong Visual Studio DevExpress Designer

Chọn control → F4 → thuộc tính **Tag** (nhóm Data):

| Vùng | Tag | Cấu hình JSON |
|---|---|---|
| Tên chính của báo cáo | `ReportTitle` | `appearance.title` |
| Bảng hoặc hàng tên cột | `TableHeader` | `appearance.tableHeader` |
| Bảng hoặc hàng dữ liệu | `TableBody` | `appearance.tableBody` |

Giữ nguyên tên control mặc định. Style bảng/hàng truyền xuống các ô; Tag con ưu tiên từng thuộc tính, các thuộc tính còn lại kế thừa từ Tag cha.
Không đặt Tag lên band. Bộ xử lý hỗ trợ bảng/hàng/ô, label và control nằm trong DetailReport; chưa đi vào tài liệu riêng của XRSubreport.
Chỉ tên báo cáo chính nhận `ReportTitle`; tên đơn vị, mã mẫu, ngày ký và chữ ký giữ nguyên.

## API

- `GET /api/reports`: danh sách mẫu, fonts, minFontSize và maxFontSize.
- `POST /api/reports/{reportId}/render`: mã `s11h` hoặc `s106h`; body `{}` hoặc `{ "appearance": { ... } }`.
- Trả `application/pdf` và tên file qua `Content-Disposition`. Không dùng Base64.
- Style hỗ trợ `fontFamily`, `fontSize`, `bold`, `italic`, `color`; thiếu hoặc null giữ mặc định.
- Font: Arial, Times New Roman, Tahoma. Cỡ 6–24 pt. Màu `#RRGGBB`.
- Sai style trả 400; sai mã trả 404; lỗi render trả 500 với thông báo chung, chi tiết chỉ ghi log BE.

```json
{
  "appearance": {
    "title": { "fontSize": 16, "bold": true },
    "tableHeader": { "fontFamily": "Times New Roman", "color": "#123456" },
    "tableBody": { "fontSize": 9 }
  }
}
```

API tạo instance report riêng mỗi request, áp dụng appearance trước khi xuất PDF.
ExpressionBinding vẫn được giữ và có ưu tiên khi in, nên định dạng có điều kiện của dòng tổng vẫn tồn tại.
Khổ giấy, căn lề ô, định dạng tiền/ngày, viền và kích thước không bị đổi.
Cỡ chữ lớn có thể không vừa khung cố định; kiểm tra PDF trước khi in. Các font cần có trên máy chạy BE.

## Thêm mẫu

Gán Tag trong Designer, thêm mẫu vào catalog và `SampleReportFactory`. Luồng render và bộ áp dụng appearance giữ dùng chung.
API demo cũ vẫn được giữ nhưng FE mới không sử dụng.

## Kiểm tra

- BE: `dotnet build TestReport.Api --no-restore`.
- FE: `cd frontend` rồi `npm run build`.
- Regression: `dotnet run --project tests/ReportChecks`.
- Browser: khi BE/FE đang chạy, dùng `python tests/report-ui-checks.py` (cần Python Playwright và Microsoft Edge; có thể đặt `PLAYWRIGHT_BROWSER_PATH` tới Chromium). Ảnh kiểm tra nằm trong `frontend/dist/checks`.
- Mở hai mẫu bằng Visual Studio có DevExpress integration, kiểm tra Tag, Preview, lưu và mở lại.
- Kiểm tra PDF ở 100%, đầu bảng nhiều trang, chữ lớn, dòng tổng và tải/in trên trình duyệt.
