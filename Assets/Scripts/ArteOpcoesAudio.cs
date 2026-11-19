using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        Texture2D audioOpcoesArte, capsulaMascara, discoMascara; Texture2D audioVoltarArte; float audioVoltarPeso; bool audioVoltarAlvo;
        GUIStyle capsulaEstilo, audioPercentualEstilo, invisivelMarcador;
        void ConfigurarControleArte()
        {
            if(capsulaMascara!=null)return;
            capsulaMascara=FormaMascara(false);discoMascara=FormaMascara(true);
            capsulaEstilo=new GUIStyle {border=new RectOffset(16,16,16,16)};
            capsulaEstilo.normal.background=capsulaMascara;
            capsulaEstilo.hover.background=capsulaMascara;
            capsulaEstilo.active.background=capsulaMascara;
            capsulaEstilo.focused.background=capsulaMascara;
            invisivelMarcador=new GUIStyle {fixedWidth=56,fixedHeight=56};
            var rotuloFonte=CarregarFonteProjeto(true);
            audioPercentualEstilo=new GUIStyle(InterfaceCena.tema.label){font=rotuloFonte!=null?rotuloFonte:fonte,fontSize=38,alignment=TextAnchor.MiddleRight,padding=new RectOffset(0,0,0,0)};
            audioPercentualEstilo.normal.textColor=Color.white;
            audioPercentualEstilo.hover.textColor=Color.white;
            audioPercentualEstilo.active.textColor=Color.white;
            audioPercentualEstilo.focused.textColor=Color.white;
        }
        Texture2D FormaMascara(bool disco)
        {
            int h=disco?64:32;
            var textura=new Texture2D(64,h,TextureFormat.RGBA32,false){name=disco?"Audio thumb mask":"Audio bar mask",filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp,hideFlags=HideFlags.HideAndDontSave};
            var pixels=new Color[64*h];
            float raio=h/2f;
            for(int y=0;y<h;y++)for(int x=0;x<64;x++)
            {
                float centro=disco?32:Mathf.Clamp(x+.5f,16,48);
                float distancia=Vector2.Distance(new Vector2(x+.5f,y+.5f),new Vector2(centro,raio));
                pixels[y*64+x]=new Color(1,1,1,Mathf.Clamp01(raio-distancia));
            }
            textura.SetPixels(pixels);textura.Apply();return textura;
        }
        void Capsula(Rect area,Color cor)
        {
            if(Event.current.type!=EventType.Repaint)return;
            ConfigurarControleArte();Color anterior=InterfaceCena.cor;InterfaceCena.cor=cor;
            InterfaceCena.Caixa(area,GUIContent.none,capsulaEstilo);InterfaceCena.cor=anterior;
        }
        void Disco(float x,float y,float diametro,Color cor)
        {
            if(Event.current.type!=EventType.Repaint)return;
            Color anterior=InterfaceCena.cor;InterfaceCena.cor=cor;
            InterfaceCena.DesenharTextura(new Rect(x-diametro/2,y-diametro/2,diametro,diametro),discoMascara);
            InterfaceCena.cor=anterior;
        }
        void DesenharOpcoesAudio()
        {
            if(audioOpcoesArte==null)audioOpcoesArte=Resources.Load<Texture2D>("UI/AudioOptions");
            ConfigurarControleArte();Matrix4x4 anterior=IniciarReferenciaTela(audioOpcoesArte);
            if(audioOpcoesArte==null)
            {
                Texto("OPÇÕES DE ÁUDIO",400,140,850,90,48,Creme,true);
                Texto("VOLUME GERAL",400,300,600,50,32,Creme,true);
                Texto("MÚSICA",400,430,600,50,32,Creme,true);
                Texto("EFEITOS SONOROS",400,560,600,50,32,Creme,true);
            }
            float geral=IlustradoDeslizante("Volume geral",volumeGeral,338,300);
            float musica=IlustradoDeslizante("Música",volumeMusica,466,429);
            float efeitos=IlustradoDeslizante("Efeitos sonoros",volumeEfeitos,595,557);
            if(geral!=volumeGeral || musica!=volumeMusica || efeitos!=volumeEfeitos)
            {volumeGeral=geral;volumeMusica=musica;volumeEfeitos=efeitos;AplicarAudio();SalvarAudio();}
            DesenharFundoControleAudio(new Rect(1008,698,120,72));Capsula(new Rect(1016,707,108,56),semSom?Hex("FFB600"):Hex("A7A7A7"));
            Capsula(new Rect(1022,713,96,44),Hex("111217"));
            Disco(semSom?1095:1045,735,40,Color.white);
            DesenharFundoControleAudio(new Rect(1136,714,155,44));int anteriorTamanho=audioPercentualEstilo.fontSize;audioPercentualEstilo.fontSize=27;
            DesenharRotuloUrbano(new Rect(1140,721,142,35),semSom?"ATIVADO":"DESATIVADO",audioPercentualEstilo,Creme,true);
            audioPercentualEstilo.fontSize=anteriorTamanho;
            if(InterfaceCena.Botao(new Rect(1005,698,287,76),new GUIContent("","Mutar tudo"),GUIStyle.none))
            {semSom=!semSom;AplicarAudio();SalvarAudio();Toque();}
            if(DesenharBotaoVoltarAudio())
            {Toque();SalvarAudio();opcoesAudio=false;}
            InterfaceCena.matriz=anterior;
        }
        float IlustradoDeslizante(string rotulo,float valor,float y,float numeroY)
        {
            DesenharFundoControleAudio(new Rect(393,y-7,890,72));InterfaceCena.DefinirProximoControleNome("Audio"+rotulo);
            float proximo=InterfaceCena.HorizontalDeslizante(new Rect(397,y,881,56),valor,0,1,GUIStyle.none,invisivelMarcador);
            float centro=425+proximo*825;
            Capsula(new Rect(397,y+7,881,44),Hex("0734F8"));
            Capsula(new Rect(402,y+12,871,34),Hex("202125"));
            if(proximo>0)
            {
                InterfaceCena.IniciarGrupo(new Rect(403,y+13,Mathf.Clamp(centro-403,0,869),32));
                Capsula(new Rect(0,0,869,32),Hex("FFB600"));InterfaceCena.FimGrupo();
            }
            Disco(centro,y+29,58,Hex("0C0C14"));Disco(centro,y+29,52,Hex("123DFF"));
            Disco(centro,y+29,43,Color.white);Disco(centro,y+29,36,Hex("1748FF"));
            DesenharFundoControleAudio(new Rect(1180,numeroY-3,102,46));DesenharRotuloUrbano(new Rect(1182,numeroY,94,42),Mathf.RoundToInt(proximo*100)+"%",audioPercentualEstilo,Creme,true);
            return proximo;
        }
        void AtualizarDestaqueVoltarAudio()
        {
            if(!opcoesAudio)audioVoltarAlvo=false;
            audioVoltarPeso=Mathf.MoveTowards(audioVoltarPeso,audioVoltarAlvo?1:0,Time.unscaledDeltaTime*10);
        }
        bool DesenharBotaoVoltarAudio()
        {
            if(audioVoltarArte==null)audioVoltarArte=Resources.Load<Texture2D>("UI/AudioVoltar");
            Rect area=new Rect(60,779,390,130);
            float escala=1;
            area=new Rect(area.center-area.size*escala/2,area.size*escala);
            Rect cliqueArea=new Rect(area.x+area.width*.02f,area.y+area.height*.24f,area.width*.96f,area.height*.74f);
            bool destaque=InterfaceCena.habilitado&&cliqueArea.Contains(Event.current.mousePosition);
            audioVoltarAlvo=destaque;
            Rect uv=destaque?new Rect(730f/5202,1-2600f/2601,3720f/5202,1222f/2601):new Rect(730f/5202,1-1325f/2601,3720f/5202,1240f/2601);
            if(audioVoltarArte!=null)InterfaceCena.DesenharTexturaComCoordenadas(area,audioVoltarArte,uv);
            else UrbanoTexto("VOLTAR",cliqueArea,28,Creme,TextAnchor.MiddleCenter);
            InterfaceCena.DefinirProximoControleNome("AudioVoltar");
            return InterfaceCena.Botao(cliqueArea,new GUIContent("","Voltar"),GUIStyle.none);
        }
        void DesenharFundoControleAudio(Rect area)
        {
            // Reaproveita uma faixa limpa do próprio painel, conservando sua textura.
            if(audioOpcoesArte!=null)InterfaceCena.DesenharTexturaComCoordenadas(area,audioOpcoesArte,new Rect(area.x/1672f,1-418f/941f,area.width/1672f,13f/941f));
            else Retangulo(area.x,area.y,area.width,area.height,Painel);
        }
    }
}
