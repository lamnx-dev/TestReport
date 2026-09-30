using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TestReport.Api.Domain.Models
{
    /// <summary>
    /// DTO đại diện cho một dòng dữ liệu trên báo cáo (dùng cho DevExpress Object Binding)
    /// </summary>
    public class ReportItemDto
    {
        [DisplayName("STT")]
        public int Stt { get; set; }

        [DisplayName("Mã đơn / Mã chứng từ")]
        public string Code { get; set; } = string.Empty;

        [DisplayName("Tên khách hàng / Đối tác")]
        public string CustomerName { get; set; } = string.Empty;

        [DisplayName("Tên sản phẩm / Dịch vụ")]
        public string ProductName { get; set; } = string.Empty;

        [DisplayName("Phân loại / Danh mục")]
        public string Category { get; set; } = string.Empty;

        [DisplayName("Ngày lập")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DisplayName("Số lượng")]
        public int Quantity { get; set; }

        [DisplayName("Đơn giá")]
        public decimal UnitPrice { get; set; }

        [DisplayName("Thành tiền")]
        public decimal TotalAmount { get; set; }

        [DisplayName("Trạng thái")]
        public string Status { get; set; } = "Hoàn thành";

        [DisplayName("Ghi chú")]
        public string Note { get; set; } = string.Empty;

        public ReportItemDto()
        {
        }

        public ReportItemDto(
            int stt,
            string code,
            string customerName,
            string productName,
            string category,
            DateTime createdDate,
            int quantity,
            decimal unitPrice,
            string status = "Hoàn thành",
            string note = "")
        {
            Stt = stt;
            Code = code;
            CustomerName = customerName;
            ProductName = productName;
            Category = category;
            CreatedDate = createdDate;
            Quantity = quantity;
            UnitPrice = unitPrice;
            TotalAmount = quantity * unitPrice;
            Status = status;
            Note = note;
        }

        /// <summary>
        /// Phương thức sinh dữ liệu mẫu để DevExpress Designer có thể xem trước (Preview) ngay trong Visual Studio
        /// </summary>
        public static List<ReportItemDto> GetSampleData()
        {
            var now = DateTime.Now;
            return new List<ReportItemDto>
            {
                new ReportItemDto(1, "ORD-2026-001", "Nguyễn Văn An", "Nâng cấp gói Doanh nghiệp", "Dịch vụ", now.AddDays(-5), 1, 15000000, "Hoàn thành", "Thanh toán qua chuyển khoản"),
                new ReportItemDto(2, "ORD-2026-002", "Trần Thị Bích", "Bản quyền phần mềm Standard", "Bản quyền", now.AddDays(-4), 3, 2500000, "Hoàn thành", "Gia hạn 1 năm"),
                new ReportItemDto(3, "ORD-2026-003", "Công ty TNHH Minh Long", "Gói tư vấn chuyển đổi số", "Tư vấn", now.AddDays(-3), 1, 45000000, "Đang xử lý", "Tạm ứng 50%"),
                new ReportItemDto(4, "ORD-2026-004", "Lê Hoàng Nam", "License DevExpress Universal", "Bản quyền", now.AddDays(-3), 2, 32000000, "Hoàn thành", "Đã xuất hóa đơn"),
                new ReportItemDto(5, "ORD-2026-005", "Phạm Thu Hương", "Hosting Cloud Pro 6 tháng", "Hạ tầng", now.AddDays(-2), 6, 800000, "Hoàn thành", ""),
                new ReportItemDto(6, "ORD-2026-006", "Đặng Quốc Hưng", "Khóa đào tạo Quản trị dữ liệu", "Đào tạo", now.AddDays(-1), 4, 3500000, "Chờ duyệt", ""),
                new ReportItemDto(7, "ORD-2026-007", "Ngô Quỳnh Mai", "Gói hỗ trợ kỹ thuật 24/7", "Dịch vụ", now, 1, 12000000, "Hoàn thành", "Hợp đồng năm 2026"),
                new ReportItemDto(8, "ORD-2026-008", "Vũ Minh Quân", "Tên miền quốc tế .com", "Hạ tầng", now, 2, 350000, "Hoàn thành", ""),
            };
        }
    }
}
