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
            var UserLibrary = await _context.Library.Where(l => l.IdUser == id).ToListAsync();

            if (UserLibrary is null)
                return NotFound(new { mensagem = "Carteira de Usuario não encontrado." });

            return Ok(UserLibrary);
        }


    }
}
