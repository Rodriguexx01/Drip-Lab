using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        readonly TemporizadorCarregamento carregamento=new TemporizadorCarregamento();
        Texture2D carregamentoArte;
        void IniciarCarregamento()
        {
            menuAberto=false;tabletAberto=false;opcoesAudio=false;
            carregamentoArte=Resources.Load<Texture2D>("UI/LoadingCrown");
            carregamento.Start();
        }
        void AtualizarCarregamento()
        {
            if(!carregamento.Ativo)return;
            carregamento.Atualizar(Time.unscaledDeltaTime);
            if(!carregamento.Ativo)
            {
                if(!Estado.introducaoVista)economia.RegistrarNoticia("Tio Cleiton deu R$ 2.500 para começar. Agora é com você, DG!");
                Estado.introducaoVista=true;tabletAberto=false;Salvar(false);IniciarTutorial();
            }
        }
        Matrix4x4 IniciarReferenciaTela(Texture2D arte,bool limparFundo=true)
        {
            arteReferenciaAtual=arte;Matrix4x4 anterior=InterfaceCena.matriz;InterfaceCena.matriz=Matrix4x4.identity;
            if(limparFundo)Retangulo(0,0,Screen.width,Screen.height,Color.black);
            float escala=Mathf.Min(Screen.width/1672f,Screen.height/941f);
            InterfaceCena.matriz=Matrix4x4.TRS(new Vector3((Screen.width-1672*escala)/2,(Screen.height-941*escala)/2,0),Quaternion.identity,new Vector3(escala,escala,1));
            if(arte!=null)InterfaceCena.DesenharTextura(new Rect(0,0,1672,941),arte,ScaleMode.StretchToFill);
            return anterior;
        }
        void DesenharCarregamento()
        {
            Matrix4x4 anterior=IniciarReferenciaTela(carregamentoArte);
            if(carregamentoArte==null)Texto("CARREGANDO...",500,450,672,80,40,Creme,true,TextAnchor.MiddleCenter);
            else
            {
                // A barra da imagem é um exemplo estático; cobre seu preenchimento
                // e desenha o progresso real dentro da nova moldura azul.
                // Redesenha toda a barra para ocultar o progresso fixo da imagem.
                UrbanoArredondado(new Rect(560,618,552,39),19,Hex("074BFF"));
                UrbanoArredondado(new Rect(566,624,540,27),13,Hex("05070D"));
                if(carregamento.Progresso>0)
                {
                    float largura=532*Mathf.Clamp01(carregamento.Progresso);
                    UrbanoArredondado(new Rect(570,628,largura,19),9,Hex("FFAD00"));
                    if(largura>12)UrbanoArredondado(new Rect(573,630,largura-6,4),2,Hex("FFD34D"));
                }
            }
            InterfaceCena.matriz=anterior;
        }
        Texture2D arteReferenciaAtual;
        Texture2D botoesVoltarArte,botoesAjudaArte,botoesFecharArte;
        static readonly Rect VoltarAmareloUv=new Rect(.070f,.077f,.860f,.366f);
        static readonly Rect VoltarAzulUv=new Rect(.070f,.532f,.860f,.366f);
        static readonly Rect AjudaAmareloUv=new Rect(.571f,.115f,.300f,.770f);
        static readonly Rect AjudaAzulUv=new Rect(.130f,.115f,.300f,.770f);
        static readonly Rect FecharAmareloUv=new Rect(.620f,.108f,.264f,.784f);
        static readonly Rect FecharAzulUv=new Rect(.120f,.108f,.260f,.784f);
        static readonly Rect AtalhoAmareloUv=new Rect(.108f,.040f,.784f,.420f);
        static readonly Rect AtalhoAzulUv=new Rect(.108f,.540f,.784f,.420f);
        static readonly Rect ProximoDiaAmareloUv=new Rect(.040f,.065f,.920f,.400f);
        static readonly Rect ProximoDiaAzulUv=new Rect(.040f,.535f,.920f,.400f);
        void DesenharBotaoDoisEstados(Rect area,Texture2D arte,Rect normal,Rect hover)
        {
            if(arte==null)return;
            bool sobre=InterfaceCena.habilitado&&area.Contains(Event.current.mousePosition);
            InterfaceCena.DesenharTexturaComCoordenadas(area,arte,sobre?hover:normal);
        }
        bool BotaoVoltarIlustrado(Rect area,GUIContent conteudo,GUIStyle estilo,Texture2D arte=null,[System.Runtime.CompilerServices.CallerFilePath]string arquivo="",[System.Runtime.CompilerServices.CallerLineNumber]int linha=0)
        {
            if(botoesVoltarArte==null)botoesVoltarArte=Resources.Load<Texture2D>("UI/BackButtons");
            if(botoesVoltarArte!=null)
            {
                bool hover=InterfaceCena.habilitado&&area.Contains(Event.current.mousePosition);
                UrbanoArredondado(area,16,Hex("05070D"));
                InterfaceCena.DesenharTexturaComCoordenadas(area,botoesVoltarArte,hover?VoltarAzulUv:VoltarAmareloUv);
            }
            else {UrbanoArredondado(area,12,UrbanoAmarelo);Texto("VOLTAR",area.x,area.y,area.width,area.height,24,Color.black,true,TextAnchor.MiddleCenter);}
            return InterfaceCena.Botao(area,conteudo,estilo,arquivo,linha);
        }
        bool BotaoAjudaIlustrado(Rect area)
        {
            if(botoesAjudaArte==null)botoesAjudaArte=Resources.Load<Texture2D>("UI/HelpButtons");
            if(botoesAjudaArte!=null)
            {
                bool hover=InterfaceCena.habilitado&&area.Contains(Event.current.mousePosition);
                InterfaceCena.DesenharTexturaComCoordenadas(area,botoesAjudaArte,hover?AjudaAzulUv:AjudaAmareloUv);
            }
            else {UrbanoArredondado(area,12,UrbanoAmarelo);Texto("?",area.x,area.y,area.width,area.height,42,Color.black,true,TextAnchor.MiddleCenter);}
            return InterfaceCena.Botao(area,new GUIContent("","Como jogar"),GUIStyle.none);
        }
        bool BotaoFecharIlustrado(Rect area,string descricao)
        {
            if(botoesFecharArte==null)botoesFecharArte=Resources.Load<Texture2D>("UI/CloseButtons");
            if(botoesFecharArte!=null)
            {
                bool hover=InterfaceCena.habilitado&&area.Contains(Event.current.mousePosition);
                InterfaceCena.DesenharTexturaComCoordenadas(area,botoesFecharArte,hover?FecharAzulUv:FecharAmareloUv);
            }
            else {UrbanoArredondado(area,12,UrbanoAmarelo);Texto("X",area.x,area.y,area.width,area.height,32,Color.black,true,TextAnchor.MiddleCenter);}
            return InterfaceCena.Botao(area,new GUIContent("",descricao),GUIStyle.none);
        }
    }
}
