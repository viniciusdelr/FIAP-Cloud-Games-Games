using FCG.API.Events;
using FCG.Application.DTOs;
using FCG.Domain.Entities;
using FCG.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FCG.Controllers
{
    public class VideoGamesController : Controller
    {
        private readonly DataContext _context;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        public VideoGamesController(DataContext context, ISendEndpointProvider sendEndpointProvider)
        {
            _context = context;
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost("PostVideoGames")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PostVideoGames([FromBody] VideoGamesDto dtoVideoGame)
        {

            var videoGame = new VideoGames
            {
                Title = dtoVideoGame.Title,
                Developer = dtoVideoGame.Developer,
                Publisher = dtoVideoGame.Publisher,
                Genre = dtoVideoGame.Genre,
                InitialRelease = dtoVideoGame.InitialRelease,
                Price = dtoVideoGame.Price,
                DiscountPerc = dtoVideoGame.DiscountPerc,
                DiscountPrice = dtoVideoGame.Price - (dtoVideoGame.Price * dtoVideoGame.DiscountPerc / 100)
            };

            _context.VideoGames.Add(videoGame);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Jogo Cadastrado!" });
        }

        [HttpPost("PostListVideoGames")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PostListVideoGames([FromBody] List<VideoGamesDto> dtoListVideoGames)
        {
            if (dtoListVideoGames == null || !dtoListVideoGames.Any())
                return BadRequest(new { mensagem = "Lista de jogos vazia." });

            var videoGamesList = dtoListVideoGames.Select(dto => new VideoGames
            {
                Title = dto.Title,
                Developer = dto.Developer,
                Publisher = dto.Publisher,
                Genre = dto.Genre,
                InitialRelease = dto.InitialRelease,
                Price = dto.Price,
                DiscountPerc = dto.DiscountPerc,
                DiscountPrice = dto.Price - (dto.Price * dto.DiscountPerc / 100)
            }).ToList();

            _context.VideoGames.AddRange(videoGamesList);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Jogos cadastrados com sucesso!" });
        }

        [HttpGet("GetVideoGames")]
        [Authorize]
        public async Task<ActionResult> GetVideoGames()
        {
            var videoGames = await _context.VideoGames.ToListAsync();

            return Ok(videoGames);
        }

        [HttpGet("GetByIdVideoGames/{id}")]
        [Authorize]
        public async Task<ActionResult> GetVideoGames(int id)
        {
            var videoGame = await _context.VideoGames.FindAsync(id);

            if (videoGame is null)
                return NotFound(new { mensagem = "Jogo não encontrado." });

            return Ok(videoGame);
        }

        [HttpPut("PutVideoGames/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PutVideoGames(int id, [FromBody] VideoGamesDto dto)
        {
            var UpdatedVideoGame = await _context.VideoGames.FindAsync(id);

            if (UpdatedVideoGame is null)
                return NotFound(new { mensagem = "Jogo não encontrado." });

            UpdatedVideoGame.Title = dto.Title;
            UpdatedVideoGame.Developer = dto.Developer;
            UpdatedVideoGame.Publisher = dto.Publisher;
            UpdatedVideoGame.Genre = dto.Genre;
            UpdatedVideoGame.InitialRelease = dto.InitialRelease;
            UpdatedVideoGame.Price = dto.Price;
            UpdatedVideoGame.DiscountPerc = dto.DiscountPerc;
            UpdatedVideoGame.DiscountPrice = dto.Price - (dto.Price * dto.DiscountPerc / 100);

            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Jogo atualizado com sucesso.", UpdatedVideoGame });
        }


        [HttpPost("buy")]
        public async Task<ActionResult> BuyGame([FromBody] LibraryDto dtoLibrary)
        {
            // 1. Criamos um ID único para rastrear essa transação
            var correlationId = Guid.NewGuid();

            // 2. Criamos o registro no banco vinculando ao ID de transação
            var library = new Library
            {
                Username = dtoLibrary.Username,
                IdUser = dtoLibrary.IdUser,
                IdGame = dtoLibrary.IdGame,
                PurchasedDate = DateTime.UtcNow,
                ValuePaid = dtoLibrary.ValuePaid,
                CorrelationId = correlationId, // Agora o campo existe na entidade!
                Status = "Pendente"            // Definimos como inicial
            };

            _context.Library.Add(library);
            await _context.SaveChangesAsync();

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:buygame-queue"));

            // 3. DISPARAMOS PARA O SERVICE BUS
            await endpoint.Send(new GamePurchased
            {
                CorrelationId = correlationId,
                UserId = dtoLibrary.IdUser,
                Price = dtoLibrary.ValuePaid
            });

            return Accepted(new
            {
                Message = "Compra em processamento. Aguardando validação de pagamento.",
                TransactionId = correlationId
            });
        }

        public record BuyGameRequest(int UserId, int GameId, decimal Price);

    }
}
