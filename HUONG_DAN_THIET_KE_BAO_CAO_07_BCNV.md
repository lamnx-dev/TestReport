# CẨM NANG HƯỚNG DẪN THIẾT KẾ BÁO CÁO MẪU SỐ 07/BCNV (KHỔ DỌC)
## "BÁO CÁO TỔNG HỢP LÃI THU HỒI CHI SAI" TRÊN DEVEXPRESS XTRAREPORTS
### 🖱️ Phương pháp: KÉO THẢ 100% TRÊN VISUAL STUDIO DESIGNER (KHÔNG CẦN VIẾT CODE)

---

## 📑 MỤC LỤC CHI TIẾT
1. [Chuẩn Bị & Khái Niệm Cần Biết](#1-chuẩn-bị--khái-niệm-cần-biết)
2. [Bước 1: Tạo File Báo Cáo Mới Trong Visual Studio](#bước-1-tạo-file-báo-cáo-mới-trong-visual-studio)
3. [Bước 2: Cấu Hình Khổ Giấy A4 Khổ Dọc (A4 Portrait) Bằng Properties](#bước-2-cấu-hình-khổ-giấy-a4-khổ-dọc-a4-portrait-bằng-properties)
4. [Bước 3: Thêm Các Dải (Bands) Cần Thiết Cho Báo Cáo](#bước-3-thêm-các-dải-bands-cần-thiết-cho-báo-cáo)
5. [Bước 4: Kết Nối Nguồn Dữ Liệu DTO Qua Cửa Sổ Field List](#bước-4-kết-nối-nguồn-dữ-liệu-dto-qua-cửa-sổ-field-list)
6. [Bước 5: Kéo Thả Report Header (Cơ Quan, Tiêu Đề Báo Cáo)](#bước-5-kéo-thả-report-header-cơ-quan-tiêu-đề-báo-cáo)
7. [Bước 6: Kỹ Thuật Kéo Thả & Gộp Ô Bảng Tiêu Đề Đa Cấp (Page Header 15 Cột)](#bước-6-kỹ-thuật-kéo-thả--gộp-ô-bảng-tiêu-đề-đa-cấp-page-header-15-cột)
8. [Bước 7: Kéo Thả Dòng Dữ Liệu (Detail Band) & Gán Dữ Liệu Bằng Chuột](#bước-7-kéo-thả-dòng-dữ-liệu-detail-band--gán-dữ-liệu-bằng-chuột)
9. [Bước 8: Cấu Hình In Đậm Tự Động Cho Dòng Cấp Cha Bằng Expression GUI](#bước-8-cấu-hình-in-đậm-tự-động-cho-dòng-cấp-cha-bằng-expression-gui)
10. [Bước 9: Kéo Thả Dòng CỘNG & Cấu Hình Tính Tổng Bằng Smart Tag](#bước-9-kéo-thả-dòng-cộng--cấu-hình-tính-tổng-bằng-smart-tag)
11. [Bước 10: Kéo Thả Khối Chữ Ký 3 Cột (Report Footer)](#bước-10-kéo-thả-khối-chữ-ký-3-cột-report-footer)
12. [Bước 11: Xem Trước (Preview) & Xuất File PDF/Excel Ngay Trên VS](#bước-11-xem-trước-preview--xuất-file-pdfexcel-ngay-trên-vs)
13. [Bảng Kê Chi Tiết Thông Số Kích Thước 15 Cột Khổ Dọc (750 px)](#bảng-kê-chi-tiết-thông-số-kích-thước-15-cột-khổ-dọc-750-px)

---

## 1. CHUẨN BỊ & KHÁI NIỆM CẦN BIẾT

### 3 Cửa Sổ Làm Việc Chính Trên Visual Studio:
1. **Report Designer (Khu vực giữa)**: Bàn vẽ trực quan, nơi bạn kéo thả nhãn, bảng và căn chỉnh.
2. **Toolbox (`Ctrl + Alt + X`) (Bên trái)**: Chứa các linh kiện:
   - `XRLabel`: Nhãn hiển thị văn bản (tiêu đề, tên đơn vị, mẫu số).
   - `XRTable`: Bảng biểu (dùng cho cả Header, Detail và Tổng cộng).
   - `XRTableRow` & `XRTableCell`: Dòng và ô trong bảng.
3. **Properties (`F4`) & Field List (Bên phải)**:
   - **Properties (`F4`)**: Dùng để chỉnh Font, kích thước (Width, Height), màu sắc, đường viền (Borders), căn lề (TextAlignment).
   - **Field List**: Hiển thị danh sách các cột dữ liệu lấy từ DTO, cho phép kéo thả thẳng vào bảng.
   - **Smart Tag (Biểu tượng mũi tên nhỏ `>` ở góc mỗi control)**: Menu cài đặt nhanh công thức tính tổng, format tiền tệ mà không cần vào Properties.

---

## BƯỚC 1: TẠO FILE BÁO CÁO MỚI TRONG VISUAL STUDIO

1. Mở Solution trên Visual Studio.
2. Mở cửa sổ **Solution Explorer** (`Ctrl + Alt + L`).
3. Tìm đến thư mục: **`TestReport.Api` -> `Infrastructure` -> `Reports`**.
4. **Chuột phải** vào thư mục `Reports` -> Chọn **Add** -> **New Item...** (hoặc phím tắt `Ctrl + Shift + A`).
5. Trong hộp thoại tìm kiếm góc trên bên phải, gõ: `Report`.
6. Chọn mục: **DevExpress v25.2 Report** (hoặc `XtraReport Class`).
7. Đặt tên file ở ô *Name*: **`BaoCao07BcnvReport.cs`**.
8. Nhấn nút **Add**.
9. Hộp thoại **Report Wizard** của DevExpress hiện ra:
   - Chọn mục: **Blank Report** (Báo cáo trắng).
   - Nhấn **Finish**.
10. Giao diện thiết kế trực quan **DevExpress Report Designer** màu trắng xám sẽ xuất hiện trên màn hình.

---

## BƯỚC 2: CẤU HÌNH KHỔ GIẤY A4 KHỔ DỌC (A4 PORTRAIT) BẰNG PROPERTIES

1. **Click chuột trái vào vùng nền màu xám** nằm bên ngoài trang in màu trắng (để chọn đối tượng chính là Report).
2. Nhấn phím **`F4`** (hoặc nhìn sang bảng **Properties** ở góc dưới bên phải).
3. Thiết lập chính xác các thông số sau:
   - **`Paper Kind`**: Chọn **`A4`** *(210 x 297 mm)*.
   - **`Landscape`**: Chọn **`False`** *(Báo cáo KHỔ DỌC)*.
   - **`Margins`** (Click vào dấu `+` để mở rộng):
     - `Left`: Điền **`38`** *(khoảng 9.6 mm)*
     - `Right`: Điền **`39`** *(khoảng 9.9 mm)*
     - `Top`: Điền **`40`** *(khoảng 10 mm)*
     - `Bottom`: Điền **`40`** *(khoảng 10 mm)*

> [!IMPORTANT]
> **Quy tắc vàng 750 px**:
> Chiều ngang trang A4 dọc là 827 px. Trừ lề trái (38) và lề phải (39), vùng in màu trắng khả dụng rộng **đúng 750 px**.
> Toàn bộ các bảng (`tblHeader`, `tblDetail`, `tblTotal`) bạn sẽ thiết lập chiều rộng **Width đúng 750 px** để vừa khít, không bị tràn viền đỏ báo lỗi!

---

## BƯỚC 3: THÊM CÁC DẢI (BANDS) CẦN THIẾT CHO BÁO CÁO

Mặc định khi tạo mới, báo cáo chỉ có 3 dải: `TopMargin`, `Detail`, `BottomMargin`.
Bạn cần bổ sung thêm các dải phục vụ tiêu đề và tiêu đề bảng:

1. **Chuột phải** vào bất kỳ vị trí trống nào trên bản vẽ -> Di chuột vào **Insert Band**:
   - Click chọn **`ReportHeader`** *(Dùng chứa Đơn vị cấp trên, Tên báo cáo, Mẫu số)*.
   - Click tiếp **Insert Band** -> Chọn **`PageHeader`** *(Dùng chứa Bảng tiêu đề 15 cột - dải này sẽ tự động lặp lại khi sang trang mới)*.
   - Click tiếp **Insert Band** -> Chọn **`ReportFooter`** *(Dùng chứa Dòng CỘNG tổng số và Khối 3 chữ ký)*.
2. Dùng chuột kéo các đường viền ngăn cách giữa các Band để điều chỉnh độ cao (hoặc chọn Band và điền `HeightF` trong Properties `F4`):
   - `ReportHeader`: Đặt chiều cao `120 px`
   - `PageHeader`: Đặt chiều cao `160 px`
   - `Detail`: Đặt chiều cao `24 px`
   - `ReportFooter`: Đặt chiều cao `180 px`

---

## BƯỚC 4: KẾT NỐI NGUỒN DỮ LIỆU DTO QUA CỬA SỔ FIELD LIST

DTO dữ liệu mẫu đã được tạo sẵn tại file [BaoCao07BcnvDto.cs](file:///d:/Workspace/TECA/TestReport/TestReport.Api/Domain/Models/BaoCao07BcnvDto.cs). Ta sẽ gắn DTO này vào Report bằng giao diện:

1. Nhìn sang bên phải màn hình, chọn tab **Field List** (Nếu không thấy, vào menu Visual Studio: **View** -> **Other Windows** -> **Field List**).
2. Chuột phải vào chữ `Data Source` trống trên Field List -> Chọn **Add Data Source...**
3. Hộp thoại **Data Source Wizard** mở ra:
   - Chọn loại nguồn: **`Object Binding`** -> Nhấn **Next**.
   - Trong danh sách các Type hiện ra, gõ tìm: `BaoCao07BcnvItemDto` (nằm trong namespace `TestReport.Api.Domain.Models`).
   - Chọn `BaoCao07BcnvItemDto` -> Nhấn **Next**.
   - Tại màn hình Data Member: Chọn constructor mặc định -> Nhấn **Finish**.
4. **Kết quả**: Cửa sổ **Field List** sẽ hiển thị đầy đủ các cột dữ liệu:
   - `Stt`, `ChiTieu`, `MaSo`, `IsBold`
   - `Col1_SoPhaiThuKyTruoc` ... `Col12_TrongDoConPhaiThuHoi`

---

## BƯỚC 5: KÉO THẢ REPORT HEADER (CƠ QUAN, TIÊU ĐỀ BÁO CÁO)

Mở **Toolbox** (`Ctrl + Alt + X`) -> Tìm nhóm **DX.25.2: Report Controls**:

```
+-----------------------------------------------------------------------------+
| ĐƠN VỊ QUẢN LÝ CẤP TRÊN: ......                           Mẫu số 07/BCNV    |
| ĐƠN VỊ BÁO CÁO: ......                                                      |
|                                                                             |
|                    BÁO CÁO TỔNG HỢP LÃI THU HỒI CHI SAI                     |
|                               Quý ... Năm......                             |
+-----------------------------------------------------------------------------+
```

1. **Đơn vị quản lý cấp trên (Góc trên bên trái)**:
   - Kéo 1 **`XRLabel`** thả vào `ReportHeaderBand` tại góc trên trái.
   - Nhấn `F4`, chỉnh:
     - `(Name)`: **`lblDonViCapTren`**
     - `Location`: `(0, 0)`
     - `Size`: `(450, 20)`
     - `Font`: `Times New Roman, 9pt, Regular`
     - **`Expressions -> Text`**: `'ĐƠN VỊ QUẢN LÝ CẤP TRÊN: ' + ?pDonViCapTren`
2. **Đơn vị báo cáo (Bên dưới đơn vị cấp trên)**:
   - Kéo tiếp 1 **`XRLabel`** đặt tại `Location`: `(0, 20)`.
   - Nhấn `F4`, chỉnh:
     - `(Name)`: **`lblDonViBaoCao`**
     - `Size`: `(450, 23)`
     - `Font`: `Times New Roman, 9pt, Bold`
     - **`Expressions -> Text`**: `'ĐƠN VỊ BÁO CÁO: ' + ?pDonViBaoCao`
3. **Mẫu số 07/BCNV (Góc trên bên phải)**:
   - Kéo 1 **`XRLabel`** đặt tại `Location`: `(500, 0)`.
   - `Size`: `(250, 23)`
   - `Text`: `Mẫu số 07/BCNV`
   - `Font`: `Times New Roman, 9pt, Bold`
   - `TextAlignment`: `TopRight` (Căn lề phải)
4. **Tiêu đề báo cáo (Chính giữa)**:
   - Kéo 1 **`XRLabel`** đặt tại `Location`: `(0, 55)`.
   - `Size`: `(750, 20)` (Rộng bằng toàn bộ trang in).
   - `Text`: `BÁO CÁO TỔNG HỢP LÃI THU HỒI CHI SAI`
   - `Font`: `Times New Roman, 13pt, Bold`
   - `TextAlignment`: `MiddleCenter` (Căn giữa)
5. **Kỳ báo cáo (Dưới tiêu đề)**:
   - Kéo 1 **`XRLabel`** đặt tại `Location`: `(0, 78)`.
   - Nhấn `F4`, chỉnh:
     - `(Name)`: **`lblQuyNam`**
     - `Size`: `(750, 20)`.
     - `Font`: `Times New Roman, 9.5pt, Italic`
     - `TextAlignment`: `MiddleCenter`
     - **`Expressions -> Text`**: `'Quý ' + ?pQuy + ' năm ' + ?pNam`

---

## BƯỚC 6: KỸ THUẬT KÉO THẢ & GỘP Ô BẢNG TIÊU ĐỀ ĐA CẤP (PAGE HEADER 15 CỘT)

Đây là thao tác quan trọng nhất. Hãy thực hiện tuần tự bằng chuột:

### Thao tác 1: Tạo bảng khung ban đầu
1. Mở **Toolbox**, kéo 1 **`XRTable`** thả vào **`PageHeaderBand`**.
2. Nhấn `F4`, cấu hình thuộc tính cho cả bảng `tblHeader`:
   - `Location`: `(0, 0)`
   - `Size`: `(750, 160)`
   - `Borders`: `All` (Đóng khung lưới xung quanh và bên trong)
   - `BorderColor`: `Black`
   - `Font`: `Times New Roman, 7.5pt, Bold`
   - `TextAlignment`: `MiddleCenter`
   - `WordWrap`: `True` *(Bắt buộc bật để chữ tự động xuống dòng)*

### Thao tác 2: Tạo đủ 4 hàng và 15 cột bằng chuột phải
- Mặc định bảng khi kéo vào chỉ có 1 hàng 3 ô.
- **Thêm hàng**: Click chuột phải vào hàng -> Chọn **Insert** -> **Row Below** (Làm 3 lần để có đủ **4 hàng**).
- **Thêm cột**: Click chuột phải vào ô -> Chọn **Insert** -> **Column to the Right** (Làm cho đến khi một hàng có đủ **15 ô**).

### Thao tác 3: Gộp ô (Merge Cells) bằng chuột (Không viết code)

```
Sơ đồ gộp ô:
+-----+---------+----+-----------------------------+-------------------------------------------------+
|     |         |    | THỰC HIỆN THU HỒI CỦA ĐV    | NỘP SỐ THU HỒI CHI SAI VỀ ĐV QUẢN LÝ CẤP TRÊN   |
|     |         |    |         (235 px)            |                    (325 px)                     |
| STT | CHỈ TIÊU| Mã +----+----+----+----+----+----+----+----+----+----+----+----+----+-------------------+
|     |         | số |Cột |Cột |Cột |Cột |Cột |Cột |Cột |Cột |Cột |Cột |Cột |Cột |Cột |     Trong đó      |
|     |         |    | 1  | 2  | 3  | 4  | 5  | 6  | 7  | 8  | 9  | 10 | 11 | 12 | 13 +---------+---------+
|     |         |    |    |    |    |    |    |    |    |    |    |    |    |    |    | Cột 11  | Cột 12  |
+-----+---------+----+----+----+----+----+----+----+----+----+----+----+----+----+----+---------+---------+
|  A  |    B    | B  | 1  | 2  | 3  | 4  | 5  | 6  | 7  | 8  | 9  | 10 | 11 | 12 | 13 |   11    |   12    |
+-----+---------+----+----+----+----+----+----+----+----+----+----+----+----+----+----+---------+---------+
```

1. **Gộp cột dọc STT (3 hàng đầu)**:
   - Giữ phím `Ctrl`, click chuột chọn 3 ô thuộc cột 1 từ Hàng 1, Hàng 2, Hàng 3.
   - **Chuột phải** -> Chọn **Merge Cells** (hoặc trong Properties đặt `RowSpan = 3`).
   - Gõ chữ: `STT`. Đặt `Width = 25`.
2. **Gộp cột dọc CHỈ TIÊU (3 hàng đầu)**:
   - Giữ `Ctrl`, chọn 3 ô thuộc cột 2 từ Hàng 1, Hàng 2, Hàng 3 -> Chuột phải -> **Merge Cells**.
   - Gõ chữ: `CHỈ TIÊU`. Đặt `Width = 137`.
3. **Gộp cột dọc MÃ SỐ (3 hàng đầu)**:
   - Giữ `Ctrl`, chọn 3 ô thuộc cột 3 từ Hàng 1, Hàng 2, Hàng 3 -> Chuột phải -> **Merge Cells**.
   - Gõ chữ: `Mã số`. Đặt `Width = 28`.
4. **Gộp ngang Hàng 1 - Nhóm 1 (5 cột con)**:
   - Trên Hàng 1, bôi đen 5 ô (từ ô thứ 4 đến ô thứ 8) -> Chuột phải -> **Merge Cells** (hoặc đặt `ColSpan = 5`).
   - Gõ chữ: `THỰC HIỆN THU HỒI CỦA ĐƠN VỊ`. Đặt `Width = 235`.
5. **Gộp ngang Hàng 1 - Nhóm 2 (7 cột con)**:
   - Trên Hàng 1, bôi đen 7 ô còn lại -> Chuột phải -> **Merge Cells** (hoặc đặt `ColSpan = 7`).
   - Gõ chữ: `NỘP SỐ THU HỒI CHI SAI VỀ ĐƠN VỊ QUẢN LÝ CẤP TRÊN`. Đặt `Width = 325`.
6. **Gộp dọc Hàng 2 & Hàng 3 cho từng cột 1 đến 10**:
   - Chọn ô Hàng 2 và ô Hàng 3 của từng cột số liệu -> Chuột phải -> **Merge Cells** (đặt `RowSpan = 2`):
     - Cột 1 (47px): `Số còn phải thu lãi kỳ trước chuyển sang`
     - Cột 2 (47px): `Điều chỉnh số còn phải thu lãi kỳ trước chuyển sang`
     - Cột 3 (47px): `Số phải thu lãi trong kỳ`
     - Cột 4 (47px): `Số đã thu lãi trong kỳ`
     - Cột 5 (47px): `Số còn phải thu lãi chuyển kỳ sau`
     - Cột 6 (47px): `Số còn phải nộp về BHXH cấp trên kỳ trước chuyển sang`
     - Cột 7 (47px): `Điều chỉnh số còn phải nộp về BHXH cấp trên...`
     - Cột 8 (46px): `Số phải nộp trong kỳ`
     - Cột 9 (46px): `Số đã nộp trong kỳ`
     - Cột 10 (47px): `Số còn phải nộp chuyển kỳ sau`
7. **Gộp ô "Trong đó" (Hàng 2) & 2 ô con (Hàng 3)**:
   - Trên Hàng 2, bôi đen 2 ô cuối cùng -> Chuột phải -> **Merge Cells** (đặt `ColSpan = 2`).
   - Gõ chữ: `Trong đó`. Đặt `Width = 92`.
   - Xuống Hàng 3 ngay bên dưới ô "Trong đó" có 2 ô:
     - Ô trái (46px): Gõ `Số tiền đã thu chưa nộp`
     - Ô phải (46px): Gõ `Số còn phải thu hồi`
8. **Điền hàng chỉ số tham chiếu (Hàng 4)**:
   - Lần lượt click vào 15 ô của Hàng 4 và gõ nội dung:
     `A` | `B` | `B` | `1` | `2` | `3` | `4` | `5` | `6` | `7` | `8` | `9` | `10` | `11` | `12`

---

## BƯỚC 7: KÉO THẢ DÒNG DỮ LIỆU (DETAIL BAND) & GÁN DỮ LIỆU BẰNG CHUỘT

Tại dải **`DetailBand`**:

### Cách tạo bảng dữ liệu siêu nhanh:
1. Mở **Toolbox**, kéo 1 **`XRTable`** vào `DetailBand`.
2. Đặt tên: `tblDetail`.
3. Nhấn `F4`, đặt:
   - `Location`: `(0, 0)`
   - `Size`: `(750, 24)`
   - `Borders`: `Left, Right, Bottom` *(Không chọn Top để tránh viền đôi với Header)*
   - `Font`: `Times New Roman, 7.5pt, Regular`
4. Chuột phải thêm cột để bảng có **đúng 15 ô**.
5. Nhập độ rộng (`Width`) của 15 ô khớp **100%** với các cột Header:
   - Ô 1: `25` | Ô 2: `137` | Ô 3: `28`
   - Ô 4 -> 10: `47, 47, 47, 47, 47, 47, 47`
   - Ô 11, 12: `46, 46`
   - Ô 13: `47`
   - Ô 14, 15: `46, 46`

### Gán dữ liệu bằng kéo thả chuột từ Field List (0 dòng code):
1. Mở cửa sổ **Field List** ở bên phải.
2. Click giữ chuột vào trường **`Stt`** -> Kéo thả trực tiếp vào **Ô 1** của `tblDetail`.
3. Kéo trường **`ChiTieu`** -> Thả vào **Ô 2** (Trong Properties chỉnh `TextAlignment = MiddleLeft`, `Padding.Left = 3`).
4. Kéo trường **`MaSo`** -> Thả vào **Ô 3** (`TextAlignment = MiddleCenter`).
5. Lần lượt kéo các trường **`Col1_SoPhaiThuKyTruoc`** đến **`Col12_TrongDoConPhaiThuHoi`** thả vào **Ô 4 đến Ô 15**.

### Định dạng hiển thị số tiền có dấu phẩy (`#,##0`):
1. Click chọn các ô từ Ô 4 đến Ô 15 (có thể bôi đen cùng lúc).
2. Trong bảng Properties:
   - `TextAlignment`: Chọn **`MiddleRight`** (Căn lề phải).
   - Click vào biểu tượng mũi tên Smart Tag `>` ở góc trên ô -> Tìm dòng **Format String**:
     - Chọn định dạng Number: **`{0:#,##0}`** (hoặc gõ `#,##0;-#,##0;"-"` để hiển thị dấu gạch ngang khi bằng 0).

---

## BƯỚC 8: CẤU HÌNH IN ĐẬM TỰ ĐỘNG CHO DÒNG CẤP CHA BẰNG EXPRESSION GUI

Các dòng chỉ tiêu chính (`I`, `II`, `III`, `IV`) cần in đậm tự động theo giá trị trường `IsBold`:

1. Click chọn cả hàng **`xrTableRowDetail`** trong `DetailBand`.
2. Nhìn sang bảng **Properties (`F4`)** -> Cuộn tìm mục **Expressions** (có biểu tượng chữ `f` nhỏ).
3. Tìm dòng **`Font.Bold`**:
   - Click vào nút ba chấm `...` ở bên phải.
   - Hộp thoại **Expression Editor** hiện ra.
   - Gõ hoặc click chọn trường: **`[IsBold]`** (hoặc `Iif([IsBold] == True, True, False)`).
   - Nhấn **OK**.
4. **Thụt lề cho dòng con**:
   - Click chọn riêng ô `cellChiTieu`.
   - Trong mục **Expressions**, tìm dòng **`Padding.Left`** -> Click `...`
   - Nhập: **`Iif([IsBold] == False, 12, 3)`** -> Nhấn **OK**.
   *(Dòng con sẽ tự động lùi vào 12px, dòng cha giữ nguyên 3px rất chuyên nghiệp!)*

---

## BƯỚC 9: KÉO THẢ DÒNG CỘNG & CẤU HÌNH TÍNH TỔNG BẰNG SMART TAG

Tại dải **`ReportFooterBand`**:

1. Kéo 1 **`XRTable`** vào đầu `ReportFooterBand`:
   - `Location`: `(0, 0)`
   - `Size`: `(750, 24)`
   - `Borders`: `All`
   - `Font`: `Times New Roman, 7.5pt, Bold`
2. Tạo 15 ô tương ứng.
3. **Gộp 3 ô đầu**: Bôi đen Ô 1, 2, 3 -> Chuột phải -> **Merge Cells** (Tổng rộng `190 px` = 25+137+28).
   - Gõ chữ: **`CỘNG`**
   - `TextAlignment`: `MiddleCenter`
4. **Cấu hình tự động tính tổng (Sum) qua giao diện Smart Tag**:
   - Click vào ô Cột 1 (dưới `Col1`).
   - Click vào biểu tượng mũi tên nhỏ **`>`** (Smart Tag) ở góc trên ô.
   - Click vào mục **Summary**:
     - **Summary Running**: Chọn **`Report`**
     - **Summary Function**: Chọn **`Sum`**
     - **Argument Expression**: Chọn trường **`[Col1_SoPhaiThuKyTruoc]`**
     - **Format String**: Điền **`{0:#,##0}`**
   - Lặp lại thao tác này cho các ô từ Cột 2 đến Cột 12 (chỉ cần chọn đúng Argument tương ứng `Col2`, `Col3`... `Col12`).
   *(Hệ thống sẽ tự động cộng dồn tất cả các dòng dữ liệu khi preview mà bạn không cần viết 1 dòng code SQL hay C#!)*

---

## BƯỚC 10: KÉO THẢ KHỐI CHỮ KÝ 3 CỘT (REPORT FOOTER)

Vẫn tại dải **`ReportFooterBand`**, bên dưới dòng CỘNG:

```
                                                 Ngày .... tháng .... năm ......
     NGƯỜI LẬP BIỂU       KẾ TOÁN TRƯỞNG/PHỤ TRÁCH             THỦ TRƯỞNG ĐƠN VỊ
      (Ký, họ tên)               (Ký, họ tên)                (Ký, họ tên, đóng dấu)
```

1. **Dòng ngày tháng**:
   - Kéo 1 `XRLabel` đặt tại `Location = (450, 35)`, `Width = 300`, `Height = 18`.
   - `Text`: `Ngày .... tháng .... năm ......`
   - `Font`: `Times New Roman, 8.5pt, Italic`, `TextAlignment = MiddleCenter`.
2. **Bảng chữ ký 3 chức danh**:
   - Kéo 1 `XRTable` đặt tại `Location = (0, 60)`, `Width = 750`, `Height = 90`.
   - `Borders`: Chọn **`None`** (Không kẻ viền).
   - Chuột phải chia thành **3 cột bằng nhau** (Mỗi cột rộng đúng **`250 px`**).
   - **Cột 1 (Người lập biểu)**:
     - Dòng 1: Gõ `NGƯỜI LẬP BIỂU` (Font Bold, 8.5pt, MiddleCenter)
     - Dòng 2: Gõ `(Ký, họ tên)` (Font Italic, 8pt, MiddleCenter)
   - **Cột 2 (Kế toán trưởng)**:
     - Dòng 1: Gõ `KẾ TOÁN TRƯỞNG/PHỤ TRÁCH KẾ TOÁN` (Font Bold, 8.5pt, MiddleCenter)
     - Dòng 2: Gõ `(Ký, họ tên)` (Font Italic, 8pt, MiddleCenter)
   - **Cột 3 (Thủ trưởng đơn vị)**:
     - Dòng 1: Gõ `THỦ TRƯỞNG ĐƠN VỊ` (Font Bold, 8.5pt, MiddleCenter)
     - Dòng 2: Gõ `(Ký, họ tên, đóng dấu)` (Font Italic, 8pt, MiddleCenter)

---

## BƯỚC 11: XEM TRƯỚC (PREVIEW) & XUẤT FILE PDF/EXCEL NGAY TRÊN VS

Sau khi kéo thả hoàn tất:

1. **Cách 1: Xem trước bằng tab Preview**:
   - Ở mép dưới cùng của màn hình Designer, bạn sẽ thấy 3 tab: `Designer`, `Preview`, `Scripts`.
   - Nhấp chuột vào tab **`Preview`**.
   - Báo cáo sẽ tự động kết xuất dữ liệu mẫu từ DTO lên màn hình trực quan dạng trang giấy in A4 khổ dọc.
2. **Cách 2: Xuất thử file PDF**:
   - Trong cửa sổ Preview, trên thanh công cụ trên cùng có nút **Export To** (biểu tượng đĩa mềm/mũi tên).
   - Chọn **PDF** hoặc **Excel (XLSX)** để lưu ra máy kiểm tra độ sắc nét của bảng 15 cột.

---

## BẢNG KÊ CHI TIẾT THÔNG SỐ KÍCH THƯỚC 15 CỘT KHỔ DỌC (750 PX)

Khi chỉnh thuộc tính `Width` cho từng ô trong cửa sổ Properties, bạn chỉ cần nhập theo đúng bảng sau:

| STT Cột | Ký hiệu | Tên Cột | Chiều rộng (`Width`) | Căn lề (`TextAlignment`) |
|:---:|:---:|:---|:---:|:---:|
| 1 | `A` | STT | **25 px** | `MiddleCenter` |
| 2 | `B` | CHỈ TIÊU | **137 px** | `MiddleLeft` (Padding.Left = 3) |
| 3 | `B` | Mã số | **28 px** | `MiddleCenter` |
| 4 | `1` | Số còn phải thu lãi kỳ trước... | **47 px** | `MiddleRight` |
| 5 | `2` | Điều chỉnh số còn phải thu lãi... | **47 px** | `MiddleRight` |
| 6 | `3` | Số phải thu lãi trong kỳ | **47 px** | `MiddleRight` |
| 7 | `4` | Số đã thu lãi trong kỳ | **47 px** | `MiddleRight` |
| 8 | `5` | Số còn phải thu lãi chuyển kỳ sau | **47 px** | `MiddleRight` |
| 9 | `6` | Số còn phải nộp về cấp trên... | **47 px** | `MiddleRight` |
| 10 | `7` | Điều chỉnh số còn phải nộp... | **47 px** | `MiddleRight` |
| 11 | `8` | Số phải nộp trong kỳ | **46 px** | `MiddleRight` |
| 12 | `9` | Số đã nộp trong kỳ | **46 px** | `MiddleRight` |
| 13 | `10` | Số còn phải nộp chuyển kỳ sau | **47 px** | `MiddleRight` |
| 14 | `11` | Số tiền đã thu chưa nộp (Trong đó) | **46 px** | `MiddleRight` |
| 15 | `12` | Số còn phải thu hồi (Trong đó) | **46 px** | `MiddleRight` |
| **TỔNG** | | **Toàn bộ bề ngang bảng** | **750 px** | *(Vừa khít trang in A4 dọc)* |
