using Microsoft.AspNetCore.Http.HttpResults;
using Automation.BotTv.Model;
using Automation.BotTv.Repository;

namespace Automation.BotTv.API.Endpoints
{
    public static class AcessoEndpoints
    {
        public static RouteGroupBuilder MapAcessos(this RouteGroupBuilder builder)
        {
            builder.MapGet("/Painel/{id:int}", BuscarPorPainel)
            .WithOpenApi(x => new Microsoft.OpenApi.Models.OpenApiOperation(x)
            {
                Summary = "Busca o acesso pelo código do painel",
                Description = "Retorna um objeto de acesso baseado no código do painel",
                Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Acesso" } }
            });

            return builder;
        }

        static async Task<Results<Ok<AcessoMOD>, NotFound>> BuscarPorPainel(int id, AcessoREP repositorioAcesso)
        {
            var acesso = await repositorioAcesso.BuscarPorPainel(id);
            if (acesso is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(acesso);
        }
    }
}
