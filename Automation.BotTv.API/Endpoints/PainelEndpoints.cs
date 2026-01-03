using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using System.Runtime.CompilerServices;
using Automation.BotTv.Model;
using Automation.BotTv.Repository;

namespace Automation.BotTv.API.Endpoints
{
    public static class PainelEndpoints
    {
        public static RouteGroupBuilder MapPaineis(this RouteGroupBuilder builder)
        {
            builder.MapGet("/", BuscarAtivos)
            .WithOpenApi(x => new Microsoft.OpenApi.Models.OpenApiOperation(x)
            {
                Summary = "Busca todos os painéis ativos",
                Description = "Retorna uma lista de objetos com todos os registros de painéis ativos",
                Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Paineis" } }
            });

            return builder;
        }

        static async Task<Ok<List<PainelMOD>>> BuscarAtivos(PainelREP repositorioPainel)
        {
            var paineis = await repositorioPainel.BuscarAtivos();
            return TypedResults.Ok(paineis);
        }
    }
}
