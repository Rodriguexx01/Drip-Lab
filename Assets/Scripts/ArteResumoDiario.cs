using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        Texture2D diarioResumoArte,proximoDiaBotaoArte;
        void DesenharArteResumoDiario()
        {
            InterfaceCena.DefinirTela("Resumo");
            if(diarioResumoArte==null)diarioResumoArte=Resources.Load<Texture2D>("UI/DailySummary");
            Matrix4x4 anterior=IniciarReferenciaTela(diarioResumoArte);
            if(diarioResumoArte==null)Texto("RESUMO DO DIA",350,220,972,100,50,Creme,true,TextAnchor.MiddleCenter);
            Matrix4x4 referencia=InterfaceCena.matriz;
            InterfaceCena.matriz=referencia*Matrix4x4.TRS(new Vector3(1388,298,0),Quaternion.Euler(0,0,-12.5f),Vector3.one)*Matrix4x4.Translate(new Vector3(-1388,-298,0));
            // A placa já pertence à arte limpa; somente o dia é desenhado aqui.
            Texto("DIA "+Estado.diaResumo,1312,268,152,60,40,Creme,true,TextAnchor.MiddleCenter);
            InterfaceCena.matriz=referencia;
            DesenharResumoValor(new Rect(174,668,392,89),Estado.lucroResumo,Hex("00BF2F"),true);
            DesenharResumoValor(new Rect(646,668,387,89),Estado.custosResumo,Hex("FF9400"),true);
            DesenharResumoValor(new Rect(1114,668,385,89),Estado.entregasResumo,UrbanoAzul,false);
            if(proximoDiaBotaoArte==null)proximoDiaBotaoArte=Resources.Load<Texture2D>("UI/NextDayButtons");
            Rect proximoDiaArea=new Rect(557,799,558,122);
            if(proximoDiaBotaoArte!=null)
            {
                UrbanoArredondado(proximoDiaArea,24,Hex("05070D"));
                DesenharBotaoDoisEstados(proximoDiaArea,proximoDiaBotaoArte,ProximoDiaAmareloUv,ProximoDiaAzulUv);
            }
            if(InterfaceCena.Botao(proximoDiaArea,new GUIContent("","Começar o próximo dia"),GUIStyle.none))
            {resumo="";Toque();}
            if(BotaoVoltarIlustrado(new Rect(11,850,306,84),new GUIContent("","Voltar ao menu"),GUIStyle.none))
            {resumo="";menuAberto=true;Toque();}
            InterfaceCena.matriz=anterior;
        }
        void DesenharResumoValor(Rect area,int valor,Color cor,bool formatarDinheiro)
        {
            // Mantém a moldura e o fundo originais, sem outro painel sobreposto.
            string rotulo=formatarDinheiro?"R$ "+valor.ToString("N2",System.Globalization.CultureInfo.GetCultureInfo("pt-BR")):valor.ToString();
            Texto(rotulo,area.x+4,area.y+4,area.width-8,area.height-8,formatarDinheiro?(rotulo.Length>12?38:48):66,Creme,true,TextAnchor.MiddleCenter);
        }
    }
}
