using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        void DesenharEstoqueTablet()
        {
            if(aba==10){DesenharSaldoCollab();return;} if(aba==7||(aba==6&&paginaInvestimentos>0)){DesenharNoticiasEstoque();return;} Matrix4x4 anterior=InterfaceCena.matriz;
            float escala=Mathf.Min(Screen.width/1672f,Screen.height/941f);
            InterfaceCena.matriz=Matrix4x4.TRS(new Vector3((Screen.width-1672*escala)/2,(Screen.height-941*escala)/2,0),Quaternion.identity,new Vector3(escala,escala,1));
            float x=aba==2?8:1438,w=aba==2?208:218;const float y=110;
            bool compacto=aba==10||aba==6;
            UrbanoPainel(new Rect(x-5,y-5,w+10,compacto?237:354),compacto?42:51);
            Texto("DIA "+Estado.dia+"  ·  "+relogioDia.Exibicao,x+9,y+14,w-18,30,18,Color.white,true,TextAnchor.MiddleCenter);
            Texto("DINHEIRO NA LOJA",x+13,y+(compacto?48:66),w-26,22,13,Creme,true);
            Texto(FormatarDinheiro(Estado.dinheiro),x+13,y+(compacto?72:92),w-26,38,compacto?25:29,Hex("FFB600"),true);
            Linha(x+13,y+(compacto?107:140),w-26,Hex("123DFF"));
            Texto("ESTOQUE DE TECIDOS",x+13,y+(compacto?118:152),w-26,25,14,Creme,true);
            for(int i=0;i<4;i++)
            {
                float linha=y+(compacto?143+i*19:190+i*33);
                Texto(EconomiaJogo.CatalogoTecidos[i],x+13,linha,140,compacto?22:27,compacto?14:15,Creme);
                Texto(Estado.metrosTecidos[i]+" m",x+151,linha,54,compacto?22:27,compacto?15:17,Hex("FFB600"),true,TextAnchor.UpperRight);
            }
            if(aba==3&&produtoPedido<0)DesenharPecasDisponiveisTablet();InterfaceCena.matriz=anterior;
        }
        void DesenharPecasDisponiveisTablet()
        {
            const float x=1438,y=480,w=218;
            UrbanoPainel(new Rect(x-5,y,w+10,438),40);
            Texto("PEÇAS DISPONÍVEIS",x+6,y+14,w-12,25,15,Creme,true,TextAnchor.MiddleCenter);
            for(int tipo=0;tipo<EconomiaJogo.Produtos.Length;tipo++)
            {
                float linha=y+57+tipo*40;
                string nome=EconomiaJogo.Produtos[tipo].Replace("Camisa ","").Replace("Calça ","");
                Texto(nome,x+10,linha,155,26,14,Creme);
                Texto(economia.PecasDisponiveis(tipo).ToString(),x+165,linha,42,26,18,UrbanoAmarelo,true,TextAnchor.UpperRight);
            }
            Texto("Estoque + unidades no site.\nNão inclui peças reservadas.",x+10,y+385,w-20,44,12,SemSom);
        }
        void DesenharSaldoCollab()
        {
            Matrix4x4 anterior=IniciarReferenciaTela(null,false);
            UrbanoArredondado(new Rect(252,895,1244,35),9,Painel);
            Texto("DIA "+Estado.dia+" · "+relogioDia.Exibicao+"     SALDO: "+FormatarDinheiro(Estado.dinheiro)+"     EDIÇÕES DE 10 UNIDADES · 3 DIAS NO SITE",267,900,1214,25,17,Creme,true,TextAnchor.MiddleCenter);
            InterfaceCena.matriz=anterior;
        }
    }
}
