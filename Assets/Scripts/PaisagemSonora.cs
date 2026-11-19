using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        AudioSource musicaAuxiliar,musicaAtiva,fonteCliques,fonteTiques;
        AudioClip trilhaMenu,trilhaAtelie,somVitoria,somDerrota,somTablet,somTecido,somCompra,somEntrega,somErro,somNotificacao,somTique,somPositivo,somNegativo;
        float misturaMusica,ultimoClique=-1,ultimoAvisoSonoro=-1,proximoTique;
        bool tabletEstavaAberto,roletaEstavaGirando;

        AudioSource CriarFonteSonora(int prioridade)
        {
            var origem=gameObject.AddComponent<AudioSource>();origem.playOnAwake=false;origem.spatialBlend=0;origem.priority=prioridade;return origem;
        }
        void IniciarAudioUnderground()
        {
            volumeGeral=Mathf.Clamp01(PlayerPrefs.GetFloat("DripLab.Master",.8f));
            volumeMusica=Mathf.Clamp01(PlayerPrefs.GetFloat("DripLab.Music",.6f));
            volumeEfeitos=Mathf.Clamp01(PlayerPrefs.GetFloat("DripLab.Effects",.75f));
            semSom=PlayerPrefs.GetInt("DripLab.Muted",0)==1;
            fonteAudio.playOnAwake=false;fonteAudio.spatialBlend=0;
            fonteMusica=CriarFonteSonora(150);musicaAuxiliar=CriarFonteSonora(150);fonteCliques=CriarFonteSonora(70);fonteTiques=CriarFonteSonora(110);
            trilhaMenu=CarregarSom("TrilhaFundo");trilhaAtelie=trilhaMenu;
            clique=CarregarSom("Clique");somTablet=CarregarSom("AbrirTablet");somTecido=CarregarSom("DobrarTecido");
            somCompra=CarregarSom("Compra");somEntrega=CarregarSom("Entrega");somErro=CarregarSom("Erro");somNotificacao=CarregarSom("Notificacao");
            somTique=CarregarSom("TiqueRoleta");somPositivo=CarregarSom("ResultadoPositivo");somNegativo=CarregarSom("ResultadoNegativo");
            somVitoria=CarregarSom("Vitoria");somDerrota=CarregarSom("Derrota");
            musicaAtiva=fonteMusica;musicaAtiva.clip=trilhaMenu;musicaAtiva.loop=true;misturaMusica=0;
            AtualizarVolumesAudio();if(musicaAtiva.clip!=null)musicaAtiva.Play();
            InterfaceCena.AoClicar=ReproduzirClique;
        }
        AudioClip CarregarSom(string nome)
        {
            var som=Resources.Load<AudioClip>("Audio/"+nome);
            if(som==null)Debug.LogWarning("Áudio não encontrado: "+nome);
            return som;
        }
        void AtualizarVolumesAudio()
        {
            if(fonteMusica==null||musicaAuxiliar==null)return;
            bool silenciar=semSom||!sessaoTemFoco||sessaoPausada;
            float efeitos=volumeGeral*volumeEfeitos;
            fonteAudio.mute=silenciar;fonteAudio.volume=efeitos*.7f;
            fonteCliques.mute=silenciar;fonteCliques.volume=efeitos*.6f;
            fonteTiques.mute=silenciar;fonteTiques.volume=efeitos*.42f;
            float volume=volumeGeral*volumeMusica*.35f*(historia.Ativo?.48f:resumo!=""?.75f:1);
            fonteMusica.mute=silenciar;musicaAuxiliar.mute=silenciar;
            fonteMusica.volume=volume*(musicaAtiva==fonteMusica?misturaMusica:1-misturaMusica);
            musicaAuxiliar.volume=volume*(musicaAtiva==musicaAuxiliar?misturaMusica:1-misturaMusica);
        }
        void AtualizarPaisagemSonora()
        {
            if(musicaAtiva==null)return;
            AudioClip desejada=Estado.final=="win"?somVitoria:Estado.final=="bankrupt"?somDerrota:menuAberto||historia.Ativo||carregamento.Ativo?trilhaMenu:trilhaAtelie;
            if(desejada!=null&&musicaAtiva.clip!=desejada)
            {
                musicaAtiva=musicaAtiva==fonteMusica?musicaAuxiliar:fonteMusica;
                musicaAtiva.Stop();musicaAtiva.clip=desejada;musicaAtiva.loop=Estado.final=="";musicaAtiva.volume=0;musicaAtiva.Play();misturaMusica=0;
            }
            misturaMusica=Mathf.MoveTowards(misturaMusica,1,Time.unscaledDeltaTime*1.7f);
            if(misturaMusica>=1){var anterior=musicaAtiva==fonteMusica?musicaAuxiliar:fonteMusica;if(anterior.isPlaying)anterior.Stop();}
            AtualizarVolumesAudio();
            if(tabletAberto&&!tabletEstavaAberto)TocarEfeito(somTablet,.58f);
            tabletEstavaAberto=tabletAberto;
            bool girando=(roletaGirando||tecidoRoletaGirando)&&!menuAberto&&sessaoTemFoco&&!sessaoPausada;
            if(girando)
            {
                if(!roletaEstavaGirando)proximoTique=Time.unscaledTime;
                if(Time.unscaledTime>=proximoTique)
                {
                    if(somTique!=null)fonteTiques.PlayOneShot(somTique);
                    float progresso=roletaGirando?roletaTempoDecorrido/3f:tecidoRoletaTempoDecorrido/2.6f;
                    proximoTique=Time.unscaledTime+Mathf.Lerp(.045f,.24f,Mathf.Clamp01(progresso)*Mathf.Clamp01(progresso));
                }
            }
            roletaEstavaGirando=girando;
        }
        void ReproduzirClique()
        {
            if(fonteCliques==null||clique==null||Time.unscaledTime-ultimoClique<.045f)return;
            ultimoClique=Time.unscaledTime;fonteCliques.PlayOneShot(clique);
        }
        void TocarEfeito(AudioClip som,float intensidade=1)
        {if(fonteAudio!=null&&som!=null)fonteAudio.PlayOneShot(som,intensidade);}
        void TocarDobra(){TocarEfeito(somTecido,.72f);}
        void TocarResultado(bool sucesso){TocarEfeito(sucesso?somPositivo:somNegativo,.8f);}
        void TocarAvisoAcao(string mensagem,bool sucesso,string titulo)
        {
            if(Time.unscaledTime-ultimoAvisoSonoro<.09f)return;
            ultimoAvisoSonoro=Time.unscaledTime;
            string texto=(titulo+" "+mensagem).ToLowerInvariant();
            if(!sucesso||texto.Contains("insuficiente")||texto.Contains("inválid")||texto.Contains("você não tem")){TocarEfeito(somErro,.85f);return;}
            if(texto.Contains("resultado da roleta")){TocarResultado(texto.Contains("ganhou"));return;}
            if(texto.Contains("pedido entregue")||texto.Contains("pedido enviado")){TocarEfeito(somEntrega,.9f);return;}
            if(texto.Contains("comprad")||texto.Contains("vendid")||texto.Contains("contratad")||texto.Contains("contas da semana pagas")){TocarEfeito(somCompra,.8f);return;}
            TocarEfeito(somNotificacao,.55f);
        }
        void LiberarPaisagemSonora()
        {
            if(InterfaceCena.AoClicar==ReproduzirClique)InterfaceCena.AoClicar=null;
        }
    }
}
