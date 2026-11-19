using GUI = DripLab.InterfaceCena;
using System;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        Texture2D atelieArte, bancadaHudArte;
        GUIStyle bancadaPainelEstilo;
        float bancadaRespiracaoPeso;
        [UnityEngine.Serialization.FormerlySerializedAs("benchBreathingStrength")] [Header("Respiração no ateliê")]
        [SerializeField,Range(0,2)] float intensidadeRespiracaoBancada=1;
        void ConfigurarBancada()
        {
            if(atelieArte==null)atelieArte=Resources.Load<Texture2D>("Bench/Workshop");
            if(bancadaHudArte==null)bancadaHudArte=Resources.Load<Texture2D>("UI/WorkshopHud");
            if(bancadaPainelEstilo!=null)return;
            var painelFonte=CarregarFonteProjeto(true);
            bancadaPainelEstilo=new GUIStyle(InterfaceCena.tema.label){font=painelFonte!=null?painelFonte:fonte,fontSize=36,padding=new RectOffset(0,0,0,0),alignment=TextAnchor.MiddleLeft,wordWrap=false};
        }
        void AtualizarBancada()
        {
            bool visivel=!menuAberto&&!historia.Ativo&&!carregamento.Ativo&&Estado.introducaoVista;
            bool parado=visivel&&!tabletAberto&&!ajuda&&!opcoesAudio&&!reiniciarConfirmacao&&resumo==""&&Estado.final=="";
            bancadaRespiracaoPeso=Mathf.MoveTowards(bancadaRespiracaoPeso,parado?1:0,Time.unscaledDeltaTime*3);
        }
        void DesenharAtelieFundo(Rect area)
        {
            if(atelieArte!=null)InterfaceCena.DesenharTextura(area,atelieArte,ScaleMode.ScaleAndCrop);
        }
        void DesenharAtelie()
        {
            ConfigurarBancada();Matrix4x4 anterior=IniciarReferenciaTela(null), referencia=InterfaceCena.matriz; if(DesenharBancadaDobra()){InterfaceCena.matriz=anterior;return;}
            if(atelieArte==null)
            {
                Texto("Carregando o ateliê...",400,400,800,90,36,Creme);
                InterfaceCena.matriz=anterior;return;
            }
            float peso=bancadaRespiracaoPeso*intensidadeRespiracaoBancada;
            float t=Time.unscaledTime;
            Vector2 oscilacao=new Vector2(Mathf.Sin(t*.72f)*2.2f,Mathf.Sin(t*1.16f)*3f)*peso;
            float aproximacao=1.018f+.0014f*Mathf.Sin(t*1.16f)*peso;
            Vector3 centro=new Vector3(836,515,0);
            InterfaceCena.matriz=referencia*Matrix4x4.TRS(centro+(Vector3)oscilacao,Quaternion.Euler(0,0,Mathf.Sin(t*.63f)*.065f*peso),new Vector3(aproximacao,aproximacao,1))*Matrix4x4.Translate(-centro);
            InterfaceCena.DesenharTextura(new Rect(0,0,1672,941),atelieArte,ScaleMode.StretchToFill);

            if(InterfaceCena.Botao(new Rect(1270,410,380,260),new GUIContent("","Usar tablet"),GUIStyle.none))
            {Toque();aba=0;paginaPedidos=0;tabletAberto=true;}
            // The sticker box is decorative.

            // The HUD is drawn after the world transform so its text never sways.
            InterfaceCena.matriz=referencia;
            DesenharAtalhosBancada();
            DesenharHudBancada(true);
            InterfaceCena.matriz=anterior;
        }
        void DesenharHudBancada(bool permitirEncerrarDia)
        {
            if(bancadaHudArte==null)bancadaHudArte=Resources.Load<Texture2D>("UI/WorkshopHud");
            if(bancadaHudArte==null)return;
            // A imagem é uma folha transparente: usa apenas a faixa central da arte.
            InterfaceCena.DesenharTexturaComCoordenadas(new Rect(0,0,1672,124),bancadaHudArte,new Rect(.001f,.385f,.998f,.230f));
            BancadaPainelRotulo("R$ "+Estado.dinheiro.ToString("N2",System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),new Rect(103,29,302,66),Color.white,42);
            BancadaPainelRotulo("FAMA "+Estado.fama+"/100",new Rect(492,24,290,50),Color.white,35);
            // Redesenha a moldura inteira para esconder a barra fixa da imagem.
            UrbanoArredondado(new Rect(475,77,307,28),14,Hex("AEB8C5"));
            UrbanoArredondado(new Rect(478,80,301,22),11,Hex("05070D"));
            float larguraFama=295*Mathf.Clamp01(Estado.fama/100f);
            if(larguraFama>0)
            {
                UrbanoArredondado(new Rect(481,83,larguraFama,16),8,Hex("1249FF"));
                if(larguraFama>8)UrbanoArredondado(new Rect(484,85,larguraFama-6,4),2,Hex("47A4FF"));
            }
            for(int i=0;i<4;i++)if(Estado.fama>=(i+1)*25)BancadaEstrela(new Vector2(802+i*28,91),UrbanoAmarelo);
            BancadaPainelRotulo("DIA "+Estado.dia,new Rect(1008,29,235,66),Color.white,42);
            BancadaPainelRotulo(relogioDia.Exibicao,new Rect(1370,29,250,66),Color.white,44);
            Rect relogio=new Rect(1270,12,390,101);
            if(permitirEncerrarDia&&InterfaceCena.Botao(relogio,new GUIContent("","Encerrar dia"),GUIStyle.none))AvancarDiaJogo();
            if(permitirEncerrarDia&&relogio.Contains(Event.current.mousePosition)&&InterfaceCena.habilitado)
            {
                UrbanoArredondado(new Rect(1431,126,206,38),8,UrbanoAzul);
                BancadaPainelRotulo("ENCERRAR DIA",new Rect(1445,130,177,30),Creme,21);
            }
        }
        void BancadaPainelRotulo(string texto,Rect area,Color cor,int tamanho)
        {
            bancadaPainelEstilo.fontSize=tamanho;bancadaPainelEstilo.alignment=cor==Color.black?TextAnchor.MiddleLeft:TextAnchor.MiddleCenter;
            bancadaPainelEstilo.normal.textColor=cor;bancadaPainelEstilo.hover.textColor=cor;
            bancadaPainelEstilo.active.textColor=cor;bancadaPainelEstilo.focused.textColor=cor;
            DesenharRotuloUrbano(area,texto,bancadaPainelEstilo,cor,tamanho>=25);
        }
        void BancadaEstrela(Vector2 centro,Color cor)
        {
            for(int i=0;i<5;i++)
            {
                float a=(-90+i*144)*Mathf.Deg2Rad,b=(-90+(i+1)*144)*Mathf.Deg2Rad;
                DesenharLinha(centro+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*11,centro+new Vector2(Mathf.Cos(b),Mathf.Sin(b))*11,cor,2);
            }
        }
    }
}
