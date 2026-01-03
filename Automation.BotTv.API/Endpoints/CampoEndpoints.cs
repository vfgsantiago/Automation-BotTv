using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using Automation.BotTv.Model;
using Automation.BotTv.Repository;

namespace Automation.BotTv.API.Endpoints
{
    public static class CampoEndpoints
    {
        public static RouteGroupBuilder MapCampos(this RouteGroupBuilder builder)
        {
            builder.MapGet("/{id:int}", BuscarPorCodigo)
            .WithOpenApi(x => new Microsoft.OpenApi.Models.OpenApiOperation(x)
            {
                Summary = "Busca o campo pelo código do campo",
                Description = "Retorna um objeto de campo baseado em seu código",
                Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Campo" } }
            });

            builder.MapGet("/Painel/{id:int}", BuscarPorPainel)
            .WithOpenApi(x => new Microsoft.OpenApi.Models.OpenApiOperation(x)
            {
                Summary = "Busca todos os campos pelo o código do painel",
                Description = "Retorna uma lista de objetos com os campos pelo código do painel",
                Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Campo" } }
            });

            return builder;
        }

        static async Task<Results<Ok<CampoMOD>, NotFound>> BuscarPorCodigo(int id, CampoREP repositorioCampo)
        {
            var campo = await repositorioCampo.BuscarCampoPorCodigo(id);
            if (campo is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(campo);
        }

        static async Task<Ok<List<PainelCampoMOD>>> BuscarPorPainel(int id, PainelCampoREP repositorioPainelCampo)
        {
            var maquinas = await repositorioPainelCampo.BuscarCamposPorPainel(id);
            return TypedResults.Ok(maquinas);
        }
    }
}
