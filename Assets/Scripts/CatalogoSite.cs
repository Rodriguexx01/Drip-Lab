using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        Texture2D siteCatalogoArte;
        void SitePagina()
        {
            if(siteCatalogoArte==null)siteCatalogoArte=Resources.Load<Texture2D>("UI/SiteCatalog");
            Matrix4x4 anterior=IniciarReferenciaTela(siteCatalogoArte);
            if(siteCatalogoArte==null)Texto("A tela do site não carregou.",300,350,1000,75,32,Creme,true);
            else
            {
                Rect[] botoes={new Rect(318,451,279,65),new Rect(789,451,279,65),new Rect(318,800,279,65),new Rect(789,800,279,65)};
                for(int tipo=0;tipo<botoes.Length;tipo++)
                {
                    // Cartões exibem somente a arte e o botão de lançamento.
                    DesenharSiteLancarBotao(tipo,botoes[tipo]);
                }
                if(BotaoVoltarIlustrado(new Rect(22,875,275,55),new GUIContent("","Voltar ao menu"),GUIStyle.none)){Toque();aba=0;}
                if(InterfaceCena.Botao(new Rect(1180,200,300,657),new GUIContent("","Preparar um drop limitado"),GUIStyle.none)){Toque();aba=10;colaboracaoSelecao=-1;}
            }
            /* Navegação pelo botão Voltar; X duplicado removido. */
            InterfaceCena.matriz=anterior;
        }










        void DesenharSiteLancarBotao(int tipo,Rect area)
        {
            bool lancado=economia.ProdutoEstaLancado(tipo);
            int pronto=Estado.estoque[tipo];
            if(lancado)
            {
                DesenharEstadoSiteBotao(area,1,tipo,pronto);
                return;
            }
            if(pronto<=0)
            {
                DesenharEstadoSiteBotao(area,2,tipo,pronto);
                return;
            }
            if(DesenharEstadoSiteBotao(area,0,tipo,pronto))
            {
                Toque();int valorAntes=Estado.drops.Count;
                string mensagem=economia.LancarProdutoPronto(tipo);
                bool sucesso=Estado.drops.Count==valorAntes+1;
                AdicionarTecidoAviso(sucesso?pronto+" peças de "+EconomiaJogo.Produtos[tipo]+" lançadas no site.":mensagem,sucesso,sucesso?"LANÇADO":"NÃO CONCLUÍDO");
                if(sucesso)Salvar(false);
            }
        }
        readonly Texture2D[] siteBotoesArte=new Texture2D[3];
        static readonly string[] SiteBotoesRecursos={"SiteLancar","SiteLancado","SiteSemEstoque"};
        static readonly Rect[] SiteBotoesUv={new Rect(0.021934325f,0.123556582f,0.955233453f,0.719399538f),new Rect(0.055447681f,0.187055016f,0.888241640f,0.595792880f),new Rect(0.040616773f,0.178638586f,0.915005641f,0.608499436f)};
        static readonly float[] SiteBotoesProporcoes={3.984483681f,4.472569256f,4.511124845f};
        bool DesenharEstadoSiteBotao(Rect area,int estado,int tipo,int quantidade)
        {
            if(siteBotoesArte[estado]==null)siteBotoesArte[estado]=Resources.Load<Texture2D>("UI/"+SiteBotoesRecursos[estado]);
            Texture2D imagem=siteBotoesArte[estado];
            string rotulo=estado==0?"LANÇAR":estado==1?"LANÇADO":"SEM ESTOQUE";
            if(imagem==null)return UrbanoBotao(rotulo,area,true,estado==0,24);
            float proporcao=SiteBotoesProporcoes[estado],largura=Mathf.Min(area.width,area.height*proporcao),altura=largura/proporcao;
            Rect desenho=new Rect(area.center.x-largura/2,area.center.y-altura/2,largura,altura);
            InterfaceCena.DesenharTexturaComCoordenadas(desenho,imagem,SiteBotoesUv[estado]);
            bool anterior=InterfaceCena.habilitado;InterfaceCena.habilitado=anterior&&estado==0;
            string dica=estado==0?"Lançar "+quantidade+" peças de "+EconomiaJogo.Produtos[tipo]+" por "+FormatarDinheiro(economia.PrecoVendaEstoque(tipo))+" cada":rotulo+" · "+EconomiaJogo.Produtos[tipo];
            bool pressionado=InterfaceCena.Botao(desenho,new GUIContent("",dica),GUIStyle.none);
            InterfaceCena.habilitado=anterior;return pressionado;
        }
    }
}
