using System;
using FCG.Domain.Entities;
using Xunit;

namespace FCG.Domain.Tests
{
    public class VideoGamesTests
    {
        [Fact]
        public void Should_Create_VideoGame_With_Required_Properties()
        {
            var videoGame = new VideoGames
            {
                Id = 1,
                Title = "Title",
                Developer = "Developer",
                Publisher = "Publisher",
                Genre = "RPG",
                InitialRelease = new DateTime(2020, 1, 1),
                Price = 59.99m,
                DiscountPrice = 49.99m,
                DiscountPerc = 17
            };

            Assert.Equal(1, videoGame.Id);
            Assert.Equal("Title", videoGame.Title);
            Assert.Equal("Developer", videoGame.Developer);
            Assert.Equal("Publisher", videoGame.Publisher);
            Assert.Equal("RPG", videoGame.Genre);
            Assert.Equal(new DateTime(2020, 1, 1), videoGame.InitialRelease);
            Assert.Equal(59.99m, videoGame.Price);
            Assert.Equal(49.99m, videoGame.DiscountPrice);
            Assert.Equal(17, videoGame.DiscountPerc);
        }

        [Theory]
        [InlineData(100, 20, 80)]
        [InlineData(50, 0, 50)]
        [InlineData(200, 50, 100)] 
        public void DiscountPrice_Should_Be_Correct(decimal price, int discountPerc, decimal expectedDiscountPrice)
        {
            var videoGame = new VideoGames
            {
                Price = price,
                DiscountPerc = discountPerc,
                Title = "Title",
                Developer = "Developer",
                Publisher = "Publisher",
                Genre = "RPG",
                InitialRelease = new DateTime(2020, 1, 1),
            };

            videoGame.DiscountPrice = videoGame.Price - (videoGame.Price * videoGame.DiscountPerc / 100);

            Assert.Equal(expectedDiscountPrice, videoGame.DiscountPrice);
        }

    }
}
