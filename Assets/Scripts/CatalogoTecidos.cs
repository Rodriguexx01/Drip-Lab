using GUI = DripLab.InterfaceCena;
using System.Collections.Generic;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        Texture2D tecidoCatalogoArte, tecidoRoletaArte;
        GUIStyle tecidoPrecoEstilo;
        int tecidoSelecionado=-1;
        bool tecidoRoletaGirando, tecidoRoletaGanhou;
        float tecidoRoletaTempoDecorrido, tecidoRoletaAngulo, tecidoRoletaAlvo;
        string tecidoRoletaMensagem="";
        class TecidoAviso {public string texto,titulo;public bool sucesso;public float ate;}
        readonly List<TecidoAviso> tecidoAvisos=new List<TecidoAviso>();

        void TecidoPagina()
        {
            if(tecidoCatalogoArte==null)tecidoCatalogoArte=Resources.Load<Texture2D>("UI/FabricCatalog");
            Matrix4x4 anterior=IniciarReferenciaTela(tecidoCatalogoArte);
            bool habilitado=InterfaceCena.habilitado;InterfaceCena.habilitado=habilitado&&tecidoSelecionado<0;
            if(tecidoCatalogoArte==null)
                Texto("O catálogo não carregou. Feche com Esc e tente novamente.",250,350,1172,100,32,Creme,true);
            else
            {
                float[] linhas={263,433,604,775};
                for(int i=0;i<4;i++)
                {
                    TecidoComprarBotao(i,new Rect(1064,linhas[i],163,74));
                    if(TecidoBotaoArte(new Rect(1252,linhas[i]+7,134,58),4,"Negociar "+EconomiaJogo.CatalogoTecidos[i]))AbrirRoletaNegociacao(i);
                }
                if(BotaoVoltarIlustrado(new Rect(22,880,275,55),new GUIContent("","Voltar à gestão"),GUIStyle.none)){Toque();aba=8;}
            }
            /* Navegação pelo botão Voltar; X duplicado removido. */
            InterfaceCena.habilitado=habilitado;
            if(tecidoSelecionado>=0)DesenharRoletaNegociacao();
            InterfaceCena.matriz=anterior;
        }
        void TecidoComprarBotao(int tipo,Rect area)
        {
            if(tecidoPrecoEstilo==null)
            {
                var precoFonte=CarregarFonteProjeto(true);
                tecidoPrecoEstilo=new GUIStyle(InterfaceCena.tema.label){font=precoFonte!=null?precoFonte:fonte,fontSize=34,alignment=TextAnchor.MiddleCenter,padding=new RectOffset(0,0,0,0)};
                tecidoPrecoEstilo.normal.textColor=Color.white;tecidoPrecoEstilo.hover.textColor=Color.white;
                tecidoPrecoEstilo.active.textColor=Color.white;tecidoPrecoEstilo.focused.textColor=Color.white;
            }
            // Preço ilustrado por tecido, com valor negociado indicado quando ativo.
            DesenharTecidoPreco(area,tipo);

            if(InterfaceCena.Botao(area,new GUIContent("","Comprar 1 metro de "+EconomiaJogo.CatalogoTecidos[tipo]),GUIStyle.none))
            {
                Toque();int valorAntes=Estado.tecido;
                string mensagem=economia.ComprarTecidoCatalogo(tipo,1);
                bool sucesso=Estado.tecido==valorAntes+1;
                AdicionarTecidoAviso(sucesso?"Comprado 1 metro de "+EconomiaJogo.CatalogoTecidos[tipo]+".":mensagem,sucesso);
                if(sucesso)Salvar(false);
            }
        }
        void AbrirRoletaNegociacao(int tipo)
        {
            Toque();tecidoSelecionado=tipo;tecidoRoletaGirando=false;
            bool utilizada=economia.NegociacaoTecidoUtilizada(tipo), comDesconto=economia.ObterPrecoTecido(tipo)<EconomiaJogo.PrecosTecidos[tipo];
            tecidoRoletaAngulo=utilizada&&!comDesconto?45:0;
            tecidoRoletaMensagem=utilizada?(comDesconto?"SIM! Seu desconto de 10% está ativo hoje.":"NÃO. Hoje o fornecedor manteve o preço."):"Gire a roleta para tentar 10% de desconto.";
        }
        void GirarTecidoRoleta()
        {
            if(!economia.TentarNegociarTecido(tecidoSelecionado,out tecidoRoletaGanhou))return;
            tecidoRoletaAngulo=0;
            tecidoRoletaAlvo=GeometriaRoleta.AnguloParada(0,tecidoRoletaGanhou?0:1,8)+economia.Sortear(-12,13);
            tecidoRoletaTempoDecorrido=0;tecidoRoletaGirando=true;tecidoRoletaMensagem="Girando...";
            Salvar(false);
        }
        void AtualizarTecidoNegociacao()
        {
            tecidoAvisos.RemoveAll(n=>Time.unscaledTime>=n.ate);
            if(!tecidoRoletaGirando)return;
            if(tecidoSelecionado<0||!tabletAberto||aba!=9){tecidoRoletaGirando=false;return;}
            if(menuAberto||!sessaoTemFoco||sessaoPausada)return;
            tecidoRoletaTempoDecorrido+=Time.unscaledDeltaTime;
            float t=Mathf.Clamp01(tecidoRoletaTempoDecorrido/2.6f);
            tecidoRoletaAngulo=tecidoRoletaAlvo*(1-Mathf.Pow(1-t,3));
            if(t>=1)
            {
                tecidoRoletaGirando=false;
                tecidoRoletaMensagem=tecidoRoletaGanhou?"SIM! 10% de desconto: "+FormatarDinheiro(economia.ObterPrecoTecido(tecidoSelecionado))+" por metro hoje.":"NÃO. O preço continua "+FormatarDinheiro(economia.ObterPrecoTecido(tecidoSelecionado))+" por metro.";
                TocarResultado(tecidoRoletaGanhou);
            }
        }
        void CriarTecidoRoletaArte()
        {
            if(tecidoRoletaArte!=null)return;
            const int tamanho=512;
            var pixels=new Color[tamanho*tamanho];
            Color sim=Hex("FFB600"),no=Hex("123DFF");
            for(int y=0;y<tamanho;y++)for(int x=0;x<tamanho;x++)
            {
                float d=Vector2.Distance(new Vector2(x+.5f,y+.5f),new Vector2(256,256));
                Color c=d>247?Creme:(y>=256?sim:no);
                c.a=Mathf.Clamp01(256-d);pixels[y*tamanho+x]=c;
            }
            tecidoRoletaArte=new Texture2D(tamanho,tamanho,TextureFormat.RGBA32,false){name="Roleta Sim ou Não",filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp,hideFlags=HideFlags.HideAndDontSave};
            tecidoRoletaArte.SetPixels(pixels);tecidoRoletaArte.Apply();
        }
        void DesenharRoletaNegociacao()
        {
            DesenharArteNegociacao();
















        }
        void AdicionarTecidoAviso(string mensagem,bool sucesso,string titulo=null)
        {
            TocarAvisoAcao(mensagem,sucesso,titulo); tecidoAvisos.Insert(0,new TecidoAviso{texto=mensagem,titulo=titulo??(sucesso?"COMPRA REALIZADA":"NÃO CONCLUÍDA"),sucesso=sucesso,ate=Time.unscaledTime+4});
            if(tecidoAvisos.Count>3)tecidoAvisos.RemoveAt(3);
        }
        void DesenharAvisosCompra()
        {
            if(tecidoAvisos.Count==0)return;
            Matrix4x4 anterior=InterfaceCena.matriz;InterfaceCena.matriz=Matrix4x4.identity;
            float escala=Mathf.Min(Screen.width/1672f,Screen.height/941f);
            InterfaceCena.matriz=Matrix4x4.TRS(new Vector3((Screen.width-1672*escala)/2,(Screen.height-941*escala)/2,0),Quaternion.identity,new Vector3(escala,escala,1));
            for(int i=0;i<tecidoAvisos.Count;i++)
            {
                var n=tecidoAvisos[i];if(Time.unscaledTime>=n.ate)continue;
                float y=485+i*145;
                UrbanoPainel(new Rect(1438,y,218,135),31);
                UrbanoTexto(n.titulo,new Rect(1453,y+14,188,26),17,Color.white);
                Texto(n.texto,1453,y+52,187,77,17,Color.white);
            }
            InterfaceCena.matriz=anterior;
        }
        readonly Texture2D[] tecidoBotoesArte=new Texture2D[5];
        static readonly string[] TecidoBotoesRecursos={"TecidoAlgodaoPreco","TecidoLaPreco","TecidoTricolinePreco","TecidoLinhoPreco","TecidoNegociar"};
        static readonly Rect[] TecidoBotoesUv={new Rect(0.227052608f,0.259960379f,0.544794189f,0.459608188f),new Rect(0.191283597f,0.217533283f,0.615046471f,0.510424516f),new Rect(0.106113956f,0.160691421f,0.787932138f,0.653008963f),new Rect(0.147589585f,0.225573601f,0.704949729f,0.529775715f),new Rect(0.146341463f,0.204572237f,0.707199246f,0.573415037f)};
        static readonly float[] TecidoBotoesProporcoes={2.370689655f,2.409940945f,2.413235294f,2.661313869f,2.466913276f};
        bool TecidoBotaoArte(Rect area,int indice,string dica,bool clicavel=true)
        {
            if(tecidoBotoesArte[indice]==null)tecidoBotoesArte[indice]=Resources.Load<Texture2D>("UI/"+TecidoBotoesRecursos[indice]);
            Texture2D arte=tecidoBotoesArte[indice];
            if(arte!=null)
            {
                float largura=area.width,altura=area.height; // Uniform visual size for every fabric price.
                InterfaceCena.DesenharTexturaComCoordenadas(new Rect(area.center.x-largura/2,area.center.y-altura/2,largura,altura),arte,TecidoBotoesUv[indice]);
            }
            else if(indice==4)DesenharRotuloUrbano(area,"NEGOCIAR",tecidoPrecoEstilo,Creme,true);
            else DesenharRotuloUrbano(area,FormatarDinheiro(EconomiaJogo.PrecosTecidos[indice]),tecidoPrecoEstilo,Creme,true);
            return clicavel&&InterfaceCena.Botao(area,new GUIContent("",dica),GUIStyle.none);
        }
        void DesenharTecidoPreco(Rect area,int tipo)
        {
            TecidoBotaoArte(area,tipo,"",false);
            int preco=economia.ObterPrecoTecido(tipo);
            if(preco>=EconomiaJogo.PrecosTecidos[tipo])return;
            // Artwork retains the original price; strike it through and show the payable amount.
            Retangulo(area.x+area.width*.12f,area.center.y-1,area.width*.76f,3,Hex("A60808"));
            Texto("-10%: "+FormatarDinheiro(preco),area.x-12,area.yMax+1,area.width+24,25,19,Creme,true,TextAnchor.MiddleCenter);
        }
    }
}

