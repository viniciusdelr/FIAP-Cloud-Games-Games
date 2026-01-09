using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FCG.Domain.Entities
{
    public class Library
    {
        [Key]
        public int Id { get; set; }
        public required string Username { get; set; } = string.Empty;
        public int IdUser { get; set; }
        public int IdGame { get; set; }
        public DateTime PurchasedDate { get; set; }

        [Precision(18, 2)]
        public decimal ValuePaid { get; set; }

        public Guid CorrelationId { get; set; }
        public string Status { get; set; } = "Pendente";
    }
}