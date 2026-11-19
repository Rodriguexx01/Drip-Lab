using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        Texture2D siteAtalhoArte;
        void DesenharAtalhosBancada()
        {
            if(siteAtalhoArte==null)siteAtalhoArte=Resources.Load<Texture2D>("UI/SiteButtons");
            BancadaAtalho(siteAtalhoArte,new Rect(24,135,340,113.333f),"Abrir o site da loja"); DesenharInvestimentoAtalho();
        }
        void BancadaAtalho(Texture2D arte,Rect area,string dica)
        {
            if(arte==null)return;
            Rect visual=new Rect(area.x,area.y+8.5f,area.width,96);
            DesenharBotaoDoisEstados(visual,arte,AtalhoAmareloUv,AtalhoAzulUv);
            if(InterfaceCena.Botao(area,new GUIContent("",dica),GUIStyle.none))
            {Toque();aba=2;tabletAberto=true;paginaPedidos=0;}
        }
    }
}
