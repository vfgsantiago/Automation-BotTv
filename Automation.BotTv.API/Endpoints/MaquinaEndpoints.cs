using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using Automation.BotTv.Model;
using Automation.BotTv.Repository;

namespace Automation.BotTv.API.Endpoints
{
    public static class MaquinaEndpoints
    {
        public static RouteGroupBuilder MapMaquinas(this RouteGroupBuilder builder)
        {
            builder.MapGet("{tx}/Painel/{id:int}", BuscarPorPainelEMaquina)
            .WithOpenApi(x => new Microsoft.OpenApi.Models.OpenApiOperation(x)
            {
                Summary = "Busca a máquina pelo código do painel e pela identificação da máquina",
                Description = "Retorna um objeto de máquina baseado em sua identificação e o código do painel",
                Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Maquina" } }
            });

            return builder;
        }

        static async Task<Results<Ok<PainelMaquinaMOD>, NotFound>> BuscarPorPainelEMaquina(int id, string tx, PainelMaquinaREP repositorioPainelMaquina)
        {
            var maquina = await repositorioPainelMaquina.BuscarPorPainelEMaquina(id, tx);
            if (maquina is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(maquina);
        }
    }
}
