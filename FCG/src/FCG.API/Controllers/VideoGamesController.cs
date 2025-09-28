using FCG.Infrastructure.Data;
using FCG.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using FCG.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace FCG.Controllers
{
    public class VideoGamesController : Controller
    {
        private readonly DataContext _context;
        public VideoGamesController(DataContext context)
        {
            _context = context;
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

        //[HttpDelete("DeleteVideoGames/{id}")]
        //[Authorize(Roles = "Admin")]
        //public async Task<ActionResult> DeleteVideoGame(int id)
        //{

        //    var videoGame = await _context.VideoGames.FindAsync(id);

        //    if (videoGame is null)
        //        return NotFound(new { mensagem = "Jogo não encontrado." });

        //    _context.VideoGames.Remove(videoGame);

        //    await _context.SaveChangesAsync();

        //    return Ok(new { mensagem = "Jogo deletado com sucesso.", VideoGames = await _context.VideoGames.ToListAsync() });
        //}
    }
}
