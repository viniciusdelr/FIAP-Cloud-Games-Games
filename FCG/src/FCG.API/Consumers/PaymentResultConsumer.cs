using FCG.API.Events;
using FCG.Domain.Entities;
using FCG.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FCG.Games.API.Consumers
{
    public class PaymentResultConsumer : IConsumer<PaymentResult>
    {
        private readonly DataContext _context;

        public PaymentResultConsumer(DataContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<PaymentResult> context)
        {
            var result = context.Message;

            // 1. Procuramos no banco a compra que gerou esse CorrelationId
            var compra = await _context.Library
                .FirstOrDefaultAsync(x => x.CorrelationId == result.CorrelationId);

            if (compra != null)
            {
                // 2. Atualizamos o status com base na resposta do Payments
                if (result.Success)
                {
                    compra.Status = "Aprovado";
                    Console.WriteLine($"[GAMES] Sucesso! Compra {compra.CorrelationId} aprovada.");
                }
                else
                {
                    compra.Status = "Rejeitado";
                    // Opcional: compra.StatusMessage = result.Message;
                    Console.WriteLine($"[GAMES] Falha! Compra {compra.CorrelationId} negada: {result.Message}");
                }

                // 3. Salvamos a alteração definitiva no banco de produção
                await _context.SaveChangesAsync();
            }
        }
    }
}