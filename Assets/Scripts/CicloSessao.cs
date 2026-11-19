using System;
using System.IO;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        readonly RelogioDia relogioDia=new RelogioDia();
        [UnityEngine.Serialization.FormerlySerializedAs("realSecondsPerGameMinute")] [Header("Relógio do jogo")]
        [SerializeField,Min(.1f)] float segundosReaisPorMinutoJogo=1;
        bool sessaoTemFoco=true, sessaoPausada;

        void AtualizarRelogioDia()
        {
            bool emAndamento=!economia.DespesasIniciaisPendentes&&!tutorialAtivo&&!validacao&&sessaoTemFoco&&!sessaoPausada&&!menuAberto&&!opcoesAudio&&!ajuda&&!reiniciarConfirmacao&&!roletaGirando&&!tecidoRoletaGirando&&resumo==""&&Estado.introducaoVista&&!historia.Ativo&&!carregamento.Ativo&&Estado.final=="";
            if(emAndamento)economia.AtualizarCotacaoTempoReal(Time.unscaledDeltaTime,(Estado.dia-1)*1440+(float)relogioDia.Minutos);
            if(relogioDia.Avancar(Time.unscaledDeltaTime,emAndamento,segundosReaisPorMinutoJogo))AvancarDiaJogo();
        }
        void AvancarDiaJogo()
        {
            if(Estado.final!=""||economia.DespesasIniciaisPendentes)return;
            selecaoCaixasAberta=false;dobraPecas.Sair();relogioDia.Reiniciar();
            tabletAberto=false;produtoConfeccao=-1;tecidoSelecionado=-1;produtoPedido=-1;despesasPainel=false;colaboracaoSelecao=-1;
            resumo=economia.ProximoDia();
        }
        void OnApplicationFocus(bool comFoco){sessaoTemFoco=comFoco;}
        void OnApplicationPause(bool pausado){sessaoPausada=pausado;}
        void OnApplicationQuit()
        {
            if(validacao)return;
            // Persist only a fresh starting state; the current run never resumes on reopening.
            try
            {
                var inicial=new EconomiaJogo();
                File.WriteAllText(caminhoSalvamento,JsonUtility.ToJson(inicial.estado,true));
            }
            catch(Exception error){Debug.LogWarning("Não foi possível gravar a partida inicial: "+error.Message);}
        }
    }
}
