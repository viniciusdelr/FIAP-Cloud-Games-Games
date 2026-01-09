using Microsoft.EntityFrameworkCore;

namespace FCG.Application.DTOs
{
    public class LibraryDto
    {
        public required string Username { get; set; } = string.Empty;
        public int IdUser { get; set; }

        public int IdGame { get; set; }
        public DateTime PurchasedDate { get; set; }

        [Precision(18, 2)]
        public decimal ValuePaid { get; set; }
    }
}
