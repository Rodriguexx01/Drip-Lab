using GUI = DripLab.InterfaceCena;
using System;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        Texture2D pedidosCatalogoArte, pedidosFundoArte;
        int produtoPedido=-1,pedidoProdutosPagina;

        void PedidosPagina()
        {
            if(pedidosCatalogoArte==null)pedidosCatalogoArte=Resources.Load<Texture2D>("UI/OrdersCatalog");
            if(pedidosFundoArte==null)pedidosFundoArte=Resources.Load<Texture2D>("UI/OrdersBackground");
            Matrix4x4 anterior=IniciarReferenciaTela(null);
            if(pedidosFundoArte!=null)
            {
                Matrix4x4 conteudo=InterfaceCena.matriz;
                InterfaceCena.matriz=Matrix4x4.identity;
                // Fill the screen uniformly, cropping only excess edges on other aspect ratios.
                InterfaceCena.DesenharTextura(new Rect(0,0,Screen.width,Screen.height),pedidosFundoArte,ScaleMode.ScaleAndCrop);
                InterfaceCena.matriz=conteudo;
            }
            bool habilitado=InterfaceCena.habilitado;InterfaceCena.habilitado=habilitado&&produtoPedido<0;
            if(pedidosCatalogoArte==null)Texto("A tela de pedidos não carregou.",350,350,972,70,32,Creme,true);
            else
            {
                // The complete background preserves proportions; original product cards remain conditional.
                InterfaceCena.DesenharTexturaComCoordenadas(new Rect(485,46,704,68),pedidosCatalogoArte,new Rect(485f/1672f,1-114f/941f,704f/1672f,68f/941f));
                float[] linhas={152,349,547,740};
                var tipos=new System.Collections.Generic.List<int>();
                for(int tipo=0;tipo<EconomiaJogo.Produtos.Length;tipo++)
                {int categoria=tipo;if(Estado.pedidos.Exists(o=>o.tipo==categoria))tipos.Add(tipo);}
                int paginas=Mathf.Max(1,(tipos.Count+3)/4);Texto("PEDIDOS PENDENTES: "+Estado.pedidos.Count+" / "+EconomiaJogo.LimitePedidos,335,118,1004,27,17,Creme,true,TextAnchor.MiddleCenter);
                pedidoProdutosPagina=Mathf.Clamp(pedidoProdutosPagina,0,paginas-1);
                for(int indiceLinha=0;indiceLinha<4;indiceLinha++)
                {
                    int posicao=pedidoProdutosPagina*4+indiceLinha;if(posicao>=tipos.Count)break;
                    int tipo=tipos[posicao];float linha=linhas[indiceLinha];
                    int categoria=tipo;
                    int quantidade=Estado.pedidos.FindAll(o=>o.tipo==categoria).Count;
                    if(tipo<EconomiaJogo.QuantidadeProdutosComuns)
                        InterfaceCena.DesenharTexturaComCoordenadas(new Rect(335,linha,1004,175),pedidosCatalogoArte,new Rect(335f/1672f,1-(linhas[tipo]+175)/941f,1004f/1672f,175f/941f));
                    else
                    {
                        UrbanoPainel(new Rect(335,linha,1004,175),44);
                        UrbanoTexto(EconomiaJogo.Produtos[tipo].ToUpper(),new Rect(375,linha+17,650,42),32,Creme);
                        Texto("EDIÇÃO ESPECIAL / 1 unidade por pedido",375,linha+90,650,40,22,UrbanoAmarelo,true);
                        DesenharColaboracaoPedidoArte(tipo,new Rect(1090,linha+15,220,145));
                    }
                    bool anteriorHabilitado=InterfaceCena.habilitado;InterfaceCena.habilitado=anteriorHabilitado&&quantidade>0;
                    if(InterfaceCena.Botao(new Rect(335,linha,1004,175),new GUIContent("","Ver pedidos de "+EconomiaJogo.Produtos[tipo]),GUIStyle.none))
                    {
                        Toque();produtoPedido=tipo;paginaPedidos=0;pedidoSelecionado=0;caixa=0;
                    }
                    InterfaceCena.habilitado=anteriorHabilitado;
                    UrbanoPainel(new Rect(30,linha+40,270,92),30);
                    Texto(quantidade+" PENDENTE"+(quantidade==1?"":"S"),44,linha+53,242,28,20,Amarelo,true,TextAnchor.MiddleCenter);
                    Texto("DISPONÍVEIS: "+economia.PecasDisponiveis(tipo),44,linha+89,242,32,16,Creme,false,TextAnchor.MiddleCenter);
                }
                if(paginas>1)
                {
                    if(Botao("‹",30,892,55,34,Cartao,pedidoProdutosPagina>0,23))pedidoProdutosPagina--;
                    Texto((pedidoProdutosPagina+1)+" / "+paginas,90,898,150,25,17,Creme,true,TextAnchor.MiddleCenter);
                    if(Botao("›",245,892,55,34,Cartao,pedidoProdutosPagina<paginas-1,23))pedidoProdutosPagina++;
                }
                if(Estado.pedidos.Count==0){UrbanoPainel(new Rect(350,370,972,150),60);Texto("NENHUM PEDIDO AINDA",380,395,912,40,30,Creme,true,TextAnchor.MiddleCenter);Texto("As roupas aparecem aqui quando alguém comprar no site.",380,454,912,40,22,SemSom,false,TextAnchor.MiddleCenter);}
                if(InterfaceCena.Botao(new Rect(485,46,704,68),new GUIContent("","Voltar ao menu do tablet"),GUIStyle.none)){Toque();aba=0;}
            }
            /* Navegação pelo botão Voltar; X duplicado removido. */
            InterfaceCena.habilitado=habilitado;
            if(produtoPedido>=0)DesenharPreparacaoPedido();
            InterfaceCena.matriz=anterior;
        }

        void DesenharPreparacaoPedido()
        {
            InterfaceCena.DefinirTela("Tablet/Pedidos/Embalagem");
            var pendente=Estado.pedidos.FindAll(o=>o.tipo==produtoPedido);
            if(pendente.Count==0){produtoPedido=-1;return;}
            paginaPedidos=Mathf.Clamp(paginaPedidos,0,(pendente.Count-1)/4);
            var pedido=pendente.Find(o=>o.id==pedidoSelecionado);
            if(pedido==null){pedido=pendente[paginaPedidos*4];pedidoSelecionado=pedido.id;}
            Retangulo(0,0,1672,941,new Color(0,0,0,.85f));UrbanoPainel(new Rect(345,139,982,678),64);
            Texto(EconomiaJogo.Produtos[produtoPedido].ToUpper(),375,165,922,45,29,Creme,true);
            Texto(PrecisaPrepararPeca(pedido)&&pedido.tipo==7?"Aceite o pedido para colocar o par na bancada, pronto para embalar.":PrecisaPrepararPeca(pedido)?"Aceite o pedido e dobre a peça na bancada antes de embalar.":"Escolha a caixa e entregue até o dia indicado para evitar cancelamento.",375,221,922,45,18,SemSom);
            for(int i=0;i<4;i++)
            {
                int indice=paginaPedidos*4+i;if(indice>=pendente.Count)break;
                var item=pendente[indice];
                if(Botao("#"+item.id.ToString("000")+" · "+item.cliente+"\n"+FormatarDinheiro(item.preco)+" · prazo D"+(item.dia+3),375,288+i*86,238,74,item.id==pedidoSelecionado?Azul:Cartao,true,17))
                {pedidoSelecionado=item.id;caixa=0;}
            }
            // Resolve a new selection before drawing its packing controls.
            pedido=pendente.Find(o=>o.id==pedidoSelecionado);
            if(Botao("‹",375,648,50,36,Cartao,paginaPedidos>0,23)){paginaPedidos--;pedidoSelecionado=0;}
            Texto((paginaPedidos+1)+" / "+((pendente.Count-1)/4+1),431,654,124,28,16,Creme,false,TextAnchor.MiddleCenter);
            if(Botao("›",563,648,50,36,Cartao,(paginaPedidos+1)*4<pendente.Count,23)){paginaPedidos++;pedidoSelecionado=0;}
            Texto(pedido.cliente+" · 1 unidade · "+FormatarDinheiro(pedido.preco),650,281,637,33,24,Color.white,true);
            Texto("Prefere caixa "+EconomiaJogo.Caixas[pedido.caixa]+"",650,327,637,50,18,SemSom);
            if(PrecisaPrepararPeca(pedido))DesenharDobraAceite(pedido);else DesenharPedidoPreparado(pedido);

















        }
    }
}
