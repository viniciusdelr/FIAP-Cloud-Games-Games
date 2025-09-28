using Microsoft.EntityFrameworkCore;

namespace FCG.Domain.Entities
{
    public class VideoGames
    {
        public int Id { get; set; }
        public required string Title { get; set; } = string.Empty;
        public required string Developer { get; set; } = string.Empty;
        public required string Publisher { get; set; } = string.Empty;
        public required string Genre { get; set; } = string.Empty;
        public required DateTime InitialRelease { get; set; }

        [Precision(18, 2)]
        public required decimal Price { get; set; }
        [Precision(18, 2)]
        public decimal DiscountPrice { get; set; }
        public int DiscountPerc { get; set; }

    }
}