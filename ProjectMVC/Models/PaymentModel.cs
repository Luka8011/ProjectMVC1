using System;
using System.Collections.Generic;

namespace ProjectMVC.Models
{
    public class PaymentModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal TotalPendingBalance { get; set; }

        public List<InvoiceItemViewModel> PendingInvoices { get; set; } = new();
        public List<InvoiceItemViewModel> PaidInvoices { get; set; } = new();
    }

    public class InvoiceItemViewModel
    {
        public int InvoiceId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal AmountToPay { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? InvoicePaymentDate { get; set; }
        public string InvoiceStatus { get; set; } = "Pending";
        public string PatientName { get; set; } = string.Empty;
    }
}