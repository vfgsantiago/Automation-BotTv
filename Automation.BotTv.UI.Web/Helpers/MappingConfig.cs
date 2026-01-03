using Mapster;
using System.Reflection;
using Automation.BotTv.Model;
using Automation.BotTv.UI.Web.Models;

namespace Automation.BotTv.UI.Web.Helpers
{
    public class MappingConfig
    {
        public static void RegisterMaps(IServiceCollection services)
        {
            #region Login
            TypeAdapterConfig<UsuarioMOD, UsuarioViewMOD>
             .NewConfig()
             .Map(member => member.Id, source => source.CdUsuario)
             .Map(member => member.Nome, source => source.NmUsuario)
             .Map(member => member.Cpf, source => source.NrCpf)
             .Map(member => member.Avatar, source => source.Avatar)
             .TwoWays()
             .IgnoreNonMapped(true);

            TypeAdapterConfig<AvatarMOD, AvatarViewMOD>
             .NewConfig()
             .Map(member => member.Id, source => source.CdAvatar)
             .Map(member => member.Foto, source => source.TxLocalFoto)
             .Map(member => member.Tipo, source => source.TpAvatar)
             .TwoWays()
             .IgnoreNonMapped(true);
            #endregion

            #region Painel
            TypeAdapterConfig<PainelMOD, PainelViewMOD>
            .NewConfig()
            .Map(member => member.CdPainel, source => source.CdPainel)
            .Map(member => member.NoPainel, source => source.NoPainel)
            .Map(member => member.TxUrlPainel, source => source.TxUrlPainel)
            .Map(member => member.CdPainelTipo, source => source.CdPainelTipo)
            .Map(member => member.SnAtivo, source => source.SnAtivo)
            .Map(member => member.CdUsuarioCadastrou, source => source.CdUsuarioCadastrou)
            .Map(member => member.DtCadastro, source => source.DtCadastro)
            .Map(member => member.CdUsuarioAlterou, source => source.CdUsuarioAlterou)
            .Map(member => member.DtAlteracao, source => source.DtAlteracao)
            .Map(member => member.PainelTipo, source => source.PainelTipo)
            .TwoWays()
            .IgnoreNonMapped(true);

            TypeAdapterConfig<PainelTipoMOD, PainelTipoViewMOD>
          .NewConfig()
          .Map(member => member.CdPainelTipo, source => source.CdPainelTipo)
          .Map(member => member.NoPainelTipo, source => source.NoPainelTipo)
          .Map(member => member.TxObservacao, source => source.TxObservacao)
          .Map(member => member.SnAtivo, source => source.SnAtivo)
          .Map(member => member.CdUsuarioCadastrou, source => source.CdUsuarioCadastrou)
          .Map(member => member.DtCadastro, source => source.DtCadastro)
          .Map(member => member.CdUsuarioAlterou, source => source.CdUsuarioAlterou)
          .Map(member => member.DtAlteracao, source => source.DtAlteracao)
          .TwoWays()
          .IgnoreNonMapped(true);
            #endregion

            #region Campo
            TypeAdapterConfig<CampoTipoMOD, CampoTipoViewMOD>
            .NewConfig()
            .Map(member => member.CdCampoTipo, source => source.CdCampoTipo)
            .Map(member => member.NoCampoTipo, source => source.NoCampoTipo)
            .Map(member => member.TxObservacao, source => source.TxObservacao)
            .Map(member => member.SnAtivo, source => source.SnAtivo)
            .Map(member => member.CdUsuarioCadastrou, source => source.CdUsuarioCadastrou)
            .Map(member => member.DtCadastro, source => source.DtCadastro)
            .Map(member => member.CdUsuarioAlterou, source => source.CdUsuarioAlterou)
            .Map(member => member.DtAlteracao, source => source.DtAlteracao)
            .TwoWays()
            .IgnoreNonMapped(true);

            TypeAdapterConfig<CampoAcaoMOD, CampoAcaoViewMOD>
            .NewConfig()
            .Map(member => member.CdAcao, source => source.CdAcao)
            .Map(member => member.NoAcao, source => source.NoAcao)
            .Map(member => member.SnAtivo, source => source.SnAtivo)
            .Map(member => member.CdUsuarioCadastrou, source => source.CdUsuarioCadastrou)
            .Map(member => member.DtCadastro, source => source.DtCadastro)
            .Map(member => member.CdUsuarioAlterou, source => source.CdUsuarioAlterou)
            .Map(member => member.DtAlteracao, source => source.DtAlteracao)
            .TwoWays()
            .IgnoreNonMapped(true);
            #endregion

            #region Maquina
            TypeAdapterConfig<MaquinaMOD, MaquinaViewMOD>
           .NewConfig()
           .Map(member => member.CdMaquina, source => source.CdMaquina)
           .Map(member => member.TxIdMaquina, source => source.TxIdMaquina)
           .Map(member => member.NoMaquina, source => source.NoMaquina)
           .Map(member => member.SnAtivo, source => source.SnAtivo)
           .Map(member => member.CdUsuarioCadastrou, source => source.CdUsuarioCadastrou)
           .Map(member => member.DtCadastro, source => source.DtCadastro)
           .Map(member => member.CdUsuarioAlterou, source => source.CdUsuarioAlterou)
           .Map(member => member.DtAlteracao, source => source.DtAlteracao)
           .TwoWays()
           .IgnoreNonMapped(true);
            #endregion

            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        }
    }
}

