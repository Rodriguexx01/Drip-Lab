using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        Texture2D compartilhadoTabletFundo;
        void DesenharUrbanoTabletPaginas()
        {
            if(compartilhadoTabletFundo==null)compartilhadoTabletFundo=Resources.Load<Texture2D>("UI/OrdersBackground");
            Matrix4x4 anterior=IniciarReferenciaTela(compartilhadoTabletFundo);
            if(aba==6&&riscoRoleta){DesenharRoletaApostas();InterfaceCena.matriz=anterior;return;}
            UrbanoPainel(new Rect(315,125,1030,675),60);
            UrbanoTexto("DRIP LAB / "+(aba==6?"INVESTIMENTOS":"NOTÍCIAS"),new Rect(350,147,890,40),32,Color.white);
            /* Navegação pelo botão Voltar; X duplicado removido. */
            string[] abas={"INÍCIO","CONFECÇÃO","SITE","PEDIDOS","PUBLICIDADE","DESPESAS","EXTRAS"};
            for(int i=0;i<abas.Length;i++)
                if(Botao(abas[i],350+i*137,216,128,40,aba==i?Amarelo:Cartao,true,15))
                {aba=i;paginaPedidos=0;if(i==3)produtoPedido=-1;}
            // Existing page coordinates are mapped into the same full-screen artwork frame.
            Matrix4x4 conteudo=InterfaceCena.matriz;
            InterfaceCena.matriz=conteudo*Matrix4x4.TRS(new Vector3(-587.3f,-39.2f,0),Quaternion.identity,new Vector3(1.35f,1.05f,1));
            if(aba==6)RiscoPagina();else if(aba==7)TabletNoticiasPagina();
            InterfaceCena.matriz=conteudo;
            if(Botao("RECOMEÇAR",515,831,185,48,Cartao,true,18))reiniciarConfirmacao=true;
            if(Botao("ÁUDIO",715,831,150,48,Cartao,true,18))opcoesAudio=true;
            if(Botao("MENU",880,831,150,48,Cartao,true,18))menuAberto=true;
            if(Botao("ENCERRAR DIA",1045,831,300,48,Amarelo,Estado.final==""&&Estado.introducaoVista,21))AvancarDiaJogo();
            Texto("A partida recomeça no dia 1 ao fechar o jogo.",315,900,1030,25,16,Creme,false,TextAnchor.MiddleCenter);
            InterfaceCena.matriz=anterior;
        }
    }
}
