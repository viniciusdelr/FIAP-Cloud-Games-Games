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
    public class LibraryController : Controller
    {
        private readonly DataContext _context;
        public LibraryController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("GetUserLibrary/{id}")]
        [Authorize]
        public async Task<ActionResult> GetUserLibrary(int id)
        {
            var videoGame = await _context.Library.FindAsync(id);

            if (videoGame is null)
                return NotFound(new { mensagem = "Jogo não encontrado." });

            return Ok(videoGame);
        }


    }
}
