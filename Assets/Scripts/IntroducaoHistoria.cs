using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        readonly ReproducaoHistoria historia=new ReproducaoHistoria();
        readonly Texture2D[] historiaArte=new Texture2D[2];
        GUIStyle historiaTextoEstilo;
        Font historiaFonte;
        AudioClip digitandoClipe;
        int ultimoDigitadosQuantidade;
        float proximoDigitandoSom; Texture2D historiaPularArte; float historiaPularPeso; bool historiaPularAlvo;

        void IniciarHistoria()
        {
            menuAberto=false;tabletAberto=false;ajuda=false;reiniciarConfirmacao=false;opcoesAudio=false;
            carregamento.Cancelar();
            bancadaRespiracaoPeso=0;
            resumo="";aviso="";
            if(historiaArte[0]==null)historiaArte[0]=Resources.Load<Texture2D>("Story/02-Cleiton");
            if(historiaArte[1]==null)historiaArte[1]=Resources.Load<Texture2D>("Story/01-DG");
            if(historiaFonte==null)historiaFonte=CarregarFonteProjeto(true);
            if(digitandoClipe==null)
            {
                const int taxa=22050,comprimento=661;
                float[] som=new float[comprimento];uint semente=37591;
                for(int i=0;i<comprimento;i++)
                {
                    semente=unchecked(semente*1664525+1013904223);
                    float ruido=(semente>>8)/8388608f-1;
                    float t=i/(float)taxa;
                    som[i]=(ruido*.14f+Mathf.Sin(t*2*Mathf.PI*1450)*.08f)*Mathf.Exp(-t*150);
                }
                digitandoClipe=AudioClip.Create("Digitação do diálogo",comprimento,1,taxa,false);digitandoClipe.SetData(som,0);
            }
            historia.Start();ultimoDigitadosQuantidade=0;proximoDigitandoSom=0;historiaPularPeso=0;historiaPularAlvo=false;
        }
        void AtualizarHistoria()
        {
            if(menuAberto||!historia.Ativo)return;historiaPularPeso=Mathf.MoveTowards(historiaPularPeso,historiaPularAlvo?1:0,Time.unscaledDeltaTime*10);
            historia.Atualizar(Time.unscaledDeltaTime);
            int visivel=historia.VisivelQuantidade;
            if(visivel>ultimoDigitadosQuantidade && historia.EstaDigitando && Time.unscaledTime>=proximoDigitandoSom)
            {
                char c=ReproducaoHistoria.Linhas[historia.Pagina][visivel-1];
                if(!char.IsWhiteSpace(c)){fonteAudio.PlayOneShot(digitandoClipe,.48f);proximoDigitandoSom=Time.unscaledTime+.045f;}
            }
            ultimoDigitadosQuantidade=visivel;
            if(!historia.Ativo)ConcluirHistoria();
        }
        void PularHistoria()
        {
            Toque();historia.Pular();ultimoDigitadosQuantidade=0;proximoDigitandoSom=0;
            if(!historia.Ativo)ConcluirHistoria();
        }
        void ConcluirHistoria()
        {
            IniciarCarregamento();
        }
        void DesenharHistoria()
        {
            Matrix4x4 anterior=InterfaceCena.matriz;InterfaceCena.matriz=Matrix4x4.identity;
            Retangulo(0,0,Screen.width,Screen.height,Color.black);
            var arte=historiaArte[historia.Pagina];
            if(arte==null)
            {
                InterfaceCena.matriz=anterior;
                Texto("A imagem desta cena não carregou. Volte ao menu com Esc e tente novamente.",100,100,1240,130,26,Creme);
                return;
            }
            // Use original reference coordinates for both images, regardless of import dimensions.
            float escala=Mathf.Min(Screen.width/1672f,Screen.height/941f);
            float x=(Screen.width-1672*escala)/2,y=(Screen.height-941*escala)/2;
            InterfaceCena.matriz=Matrix4x4.TRS(new Vector3(x,y,0),Quaternion.identity,new Vector3(escala,escala,1));
            InterfaceCena.DesenharTextura(new Rect(0,0,1672,941),arte,ScaleMode.StretchToFill);
            DesenharBaseFalasHistoria();if(historiaTextoEstilo==null)
            {
                historiaTextoEstilo=new GUIStyle(InterfaceCena.tema.label){font=historiaFonte!=null?historiaFonte:fonte,fontSize=40,wordWrap=true,richText=true,alignment=TextAnchor.UpperLeft,padding=new RectOffset(0,0,0,0)};
                historiaTextoEstilo.normal.textColor=Color.black;
                historiaTextoEstilo.hover.textColor=Color.black;
                historiaTextoEstilo.active.textColor=Color.black;
                historiaTextoEstilo.focused.textColor=Color.black;
                historiaTextoEstilo.onNormal.textColor=Color.black;
                historiaTextoEstilo.onHover.textColor=Color.black;
                historiaTextoEstilo.onActive.textColor=Color.black;
                historiaTextoEstilo.onFocused.textColor=Color.black;
            }
            string linha=ReproducaoHistoria.Linhas[historia.Pagina];int quantidade=historia.VisivelQuantidade;
            // Invisible remaining characters keep line wrapping steady while the text types.
            string texto=linha.Substring(0,quantidade)+"<color=#00000000>"+linha.Substring(quantidade)+"</color>";
            var dialogo=new Rect(74,818,1235,99);
            DesenharRotuloUrbano(dialogo,texto,historiaTextoEstilo,Color.black,false);
            if(historia.EstaDigitando)
            {
                float oscilacao=Mathf.Sin(Time.unscaledTime*8)*2;
                DesenharLinha(new Vector2(1317,902+oscilacao),new Vector2(1332,886+oscilacao),Tinta,5);
                DesenharLinha(new Vector2(1315,905+oscilacao),new Vector2(1319,899+oscilacao),Hex("F8B528"),4);
                Linha(1309,912,32,Tinta);
            }
            if(DesenharBotaoPularHistoria())PularHistoria();
            // The dialogue is plain text, not a hover-sensitive button.
            else if(Event.current.type==EventType.MouseUp && Event.current.button==0 && dialogo.Contains(Event.current.mousePosition))
            {historia.Revelar();ultimoDigitadosQuantidade=historia.VisivelQuantidade;Event.current.Use();}
            if(Event.current.type==EventType.KeyDown && (Event.current.keyCode==KeyCode.Return || Event.current.keyCode==KeyCode.Space))
            {if(historia.Pagina==0)PularHistoria();else {historia.Revelar();ultimoDigitadosQuantidade=historia.VisivelQuantidade;}Event.current.Use();}
            InterfaceCena.matriz=anterior;
        }
        void DesenharBaseFalasHistoria()
        {
            // Substitui os textos impressos pelos nomes e falas da página atual.
            Retangulo(60,805,1552,117,Color.white);float larguraNome=historia.Pagina==0?350:247;
            Capsula(new Rect(65,746,larguraNome,50),Hex("0750AC"));
            Texto(historia.Pagina==0?"TIO CLEITON":"DG",77,752,larguraNome-24,38,29,Color.white,true,TextAnchor.MiddleCenter);
        }
        bool DesenharBotaoPularHistoria()
        {
            if(historiaPularArte==null)historiaPularArte=Resources.Load<Texture2D>("UI/HistoriaPular");
            Rect area=new Rect(1360,839,270,89.5f);
            float escala=1+historiaPularPeso*.045f;
            area=new Rect(area.center-area.size*escala/2,area.size*escala);
            bool destaque=InterfaceCena.habilitado&&area.Contains(Event.current.mousePosition);
            historiaPularAlvo=destaque;
            Rect uv=new Rect((destaque?5495f:220f)/10573f,1-2565f/3524f,4860f/10573f,1610f/3524f);
            if(historiaPularArte!=null)InterfaceCena.DesenharTexturaComCoordenadas(area,historiaPularArte,uv);
            else UrbanoTexto("PULAR",area,27,Creme,TextAnchor.MiddleCenter);
            bool cliqueMouse=Event.current.type==EventType.MouseUp&&Event.current.button==0;
            return InterfaceCena.Botao(area,new GUIContent("","Pular para a próxima cena"),GUIStyle.none)&&cliqueMouse;
        }
    }
}
