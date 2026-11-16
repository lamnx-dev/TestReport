# FE báo cáo

Angular: chọn báo cáo, tùy chỉnh style ba vùng và xem PDF. Không có bảng dữ liệu.

1. Chạy BE tại http://localhost:5168: `dotnet run --project TestReport.Api --launch-profile http` từ thư mục gốc.
2. Tại `frontend`, chạy `npm install` nếu chưa có dependency, rồi `npm start`.
3. Mở http://localhost:4200. `proxy.conf.json` chuyển `/api/**` về BE.

Build: `npm run build`. Khi triển khai, reverse proxy `/api` về BE cùng origin với FE.
