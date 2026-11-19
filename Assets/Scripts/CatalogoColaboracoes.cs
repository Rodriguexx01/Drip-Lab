using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        Texture2D colaboracaoCatalogoArte;
        int colaboracaoSelecao=-1;
        static readonly Rect[] ColaboracaoMiniaturas={new Rect(190,215,410,315),new Rect(885,215,350,315),new Rect(215,565,340,290),new Rect(865,570,440,275)};
        void ColaboracaoPagina()
        {
            if(colaboracaoCatalogoArte==null)colaboracaoCatalogoArte=Resources.Load<Texture2D>("UI/CollabCatalog");
            Matrix4x4 anterior=IniciarReferenciaTela(colaboracaoCatalogoArte);
            bool habilitado=InterfaceCena.habilitado;InterfaceCena.habilitado=habilitado&&colaboracaoSelecao<0;
            if(colaboracaoCatalogoArte==null)Texto("A tela de collabs não carregou.",300,350,1000,75,32,Creme,true);
            else
            {
                Rect[] areas={new Rect(600,451,210,65),new Rect(1280,451,214,65),new Rect(600,790,210,65),new Rect(1280,790,214,65)};
                for(int i=0;i<areas.Length;i++)
                {
                    Rect area=areas[i];bool lancado=economia.ProdutoEstaLancado(EconomiaJogo.QuantidadeProdutosComuns+i);
                    // Custos e preços já constam na nova arte, nos mesmos valores do modelo.
                    if(lancado)DesenharBotaoCollab(area,i,true);
                    else if(DesenharBotaoCollab(area,i,false)){Toque();colaboracaoSelecao=i;}
                    // O valor do lote permanece visível na placa de fabricação da arte.
                    // A confirmação mantém o saldo, a taxa total e o prazo de três dias.
                }
                if(BotaoVoltarIlustrado(new Rect(12,852,230,82),new GUIContent("","Voltar ao site"),GUIStyle.none)){Toque();aba=2;}
            }
            /* Navegação pelo botão Voltar; X duplicado removido. */
            InterfaceCena.habilitado=habilitado;
            if(colaboracaoSelecao>=0)DesenharColaboracaoConfirmacao();
            InterfaceCena.matriz=anterior;
        }
        void DesenharColaboracaoConfirmacao()
        {
            InterfaceCena.DefinirTela("Tablet/Collabs/Confirmacao");
            int indice=colaboracaoSelecao,tipo=EconomiaJogo.QuantidadeProdutosComuns+indice;
            Retangulo(0,0,1672,941,new Color(0,0,0,.8f));UrbanoPainel(new Rect(390,220,892,510),75);
            UrbanoTexto("LANÇAR / "+EconomiaJogo.Produtos[tipo].ToUpper(),new Rect(430,248,812,45),32,Color.white);
            Texto("Taxa total: "+FormatarDinheiro(EconomiaJogo.TaxasColaboracoes[indice]),430,330,812,44,30,UrbanoAmarelo,true);
            Texto("10 unidades especiais · 3 dias no site",430,392,812,36,24,Creme);
            Texto("Preço de venda: "+FormatarDinheiro(EconomiaJogo.PrecosBase[tipo])+" por unidade",430,442,812,36,23,Creme);
            Texto("Seu saldo: "+FormatarDinheiro(Estado.dinheiro),430,492,812,36,23,Creme);
            bool pode=Estado.dinheiro>=EconomiaJogo.TaxasColaboracoes[indice]&&!economia.ProdutoEstaLancado(tipo)&&Estado.drops.Count<EconomiaJogo.LimiteAnuncios;
            Texto(pode?"A taxa inclui o lote especial. Não é necessário fabricar antes.":"Saldo insuficiente ou lançamento indisponível.",430,548,812,53,19,pode?Creme:UrbanoAmarelo);
            if(Botao("CANCELAR",430,641,285,51,Cartao,true,21)){colaboracaoSelecao=-1;return;}
            if(Botao("CONFIRMAR LANÇAMENTO",735,641,507,51,Amarelo,pode,23))
            {
                int valorAntes=Estado.drops.Count;string mensagem=economia.LancarColaboracao(indice);bool sucesso=Estado.drops.Count==valorAntes+1;
                AdicionarTecidoAviso(mensagem,sucesso,sucesso?"COLLAB LANÇADA":"NÃO CONCLUÍDO");
                if(sucesso){Salvar(false);colaboracaoSelecao=-1;}
            }
        }
        void DesenharColaboracaoPedidoArte(int tipo,Rect area)
        {
            if(colaboracaoCatalogoArte==null)colaboracaoCatalogoArte=Resources.Load<Texture2D>("UI/CollabCatalog");
            if(colaboracaoCatalogoArte==null)return;
            Rect origem=ColaboracaoMiniaturas[tipo-EconomiaJogo.QuantidadeProdutosComuns];
            float escala=Mathf.Min(area.width/origem.width,area.height/origem.height);
            var ajustado=new Rect(area.center.x-origem.width*escala/2,area.center.y-origem.height*escala/2,origem.width*escala,origem.height*escala);
            InterfaceCena.DesenharTexturaComCoordenadas(ajustado,colaboracaoCatalogoArte,new Rect(origem.x/1672f,1-origem.yMax/941f,origem.width/1672f,origem.height/941f));
        }
        readonly Texture2D[] collabBotoesArte=new Texture2D[2];
        static readonly string[] CollabBotoesRecursos={"CollabLancar","CollabLancado"};
        static readonly Rect[] CollabBotoesUv={new Rect(0.021934325f,0.123556582f,0.955233453f,0.719399538f),new Rect(0.055447681f,0.187055016f,0.888241640f,0.595792880f)};
        static readonly float[] CollabBotoesProporcoes={3.984483681f,4.472569256f};
        bool DesenharBotaoCollab(Rect area,int indice,bool lancado)
        {
            int estado=lancado?1:0;
            if(collabBotoesArte[estado]==null)collabBotoesArte[estado]=Resources.Load<Texture2D>("UI/"+CollabBotoesRecursos[estado]);
            Texture2D imagem=collabBotoesArte[estado];
            if(imagem==null)return UrbanoBotao(lancado?"LANÇADO":"LANÇAR",area,true,!lancado,24);
            float proporcao=CollabBotoesProporcoes[estado],largura=Mathf.Min(area.width,area.height*proporcao),altura=largura/proporcao;
            Rect desenho=new Rect(area.center.x-largura/2,area.center.y-altura/2,largura,altura);
            InterfaceCena.DesenharTexturaComCoordenadas(desenho,imagem,CollabBotoesUv[estado]);
            bool anterior=InterfaceCena.habilitado;InterfaceCena.habilitado=anterior&&!lancado;
            string dica=lancado?"Já lançado: "+EconomiaJogo.Produtos[EconomiaJogo.QuantidadeProdutosComuns+indice]:"Lançar lote de 10 por "+FormatarDinheiro(EconomiaJogo.TaxasColaboracoes[indice]);
            bool pressionado=InterfaceCena.Botao(desenho,new GUIContent("",dica),GUIStyle.none);
            InterfaceCena.habilitado=anterior;return pressionado;
        }
    }
}
