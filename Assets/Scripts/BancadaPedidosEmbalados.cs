using GUI = DripLab.InterfaceCena;
using System.Collections.Generic;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        readonly Dictionary<int,int> caixasPedidos=new Dictionary<int,int>();
        readonly Texture2D[] artesCaixasEmbaladas=new Texture2D[4];
        Texture2D entregarPedidoArte;

        bool PrepararCaixaPedido(Pedido pedido,int cor)
        {
            if(artesCaixasEmbaladas[cor]==null)artesCaixasEmbaladas[cor]=Resources.Load<Texture2D>("Bench/PackedBox"+cor);
            if(artesCaixasEmbaladas[cor]==null){AdicionarTecidoAviso("A imagem da caixa não carregou.",false,"IMAGEM INDISPONÍVEL");return false;}
            caixasPedidos[pedido.id]=cor;caixa=cor;pedidoSelecionado=pedido.id;selecaoCaixasAberta=false;Toque();return true;
        }
        void DesenharPedidoPreparado(Pedido pedido)
        {
            Texto("A peça está pronta. Escolha a caixa e entregue o pedido pela bancada.",650,403,620,105,25,Creme,true);
            if(Botao(caixasPedidos.ContainsKey(pedido.id)?"VOLTAR AO PEDIDO EMBALADO":"ESCOLHER CAIXA",650,610,637,65,Amarelo,true,24))
            {
                dobraPecas.Iniciar(pedido.id,artesDobra[pedido.tipo].Length-1);tabletAberto=false;
                if(!caixasPedidos.ContainsKey(pedido.id))AbrirSelecaoCaixas();else Toque();
            }
            if(Botao("VOLTAR À LISTA",375,749,238,43,Cartao,true,16))produtoPedido=-1;
        }
        void DesenharPedidoEmbalado(Pedido pedido,int cor)
        {
            InterfaceCena.DefinirTela("Embalagem/Entrega");
            InterfaceCena.DesenharTextura(new Rect(0,0,1672,941),artesCaixasEmbaladas[cor],ScaleMode.StretchToFill);
            DesenharDobraPainel();
            if(entregarPedidoArte==null)entregarPedidoArte=Resources.Load<Texture2D>("UI/DeliverOrder");
            bool entregar;
            if(entregarPedidoArte!=null)
            {
                InterfaceCena.DesenharTextura(new Rect(486,711,700,233.333f),entregarPedidoArte,ScaleMode.ScaleToFit);
                entregar=InterfaceCena.Botao(new Rect(496,747,680,148),new GUIContent("","Entregar pedido #"+pedido.id),GUIStyle.none);
            }
            else entregar=Botao("ENTREGAR PEDIDO",526,790,620,100,Amarelo,true,28);
            if(entregar)
            {
                int valorAntes=Estado.pedidos.Count;
                string mensagem=economia.Entregar(pedido.id,cor);
                bool sucesso=Estado.pedidos.Count==valorAntes-1;
                if(sucesso)
                {
                    caixasPedidos.Remove(pedido.id);dobraPecas.Sair();selecaoCaixasAberta=false;
                    pedidoSelecionado=0;produtoPedido=-1;tabletAberto=false;Salvar(false);Toque();
                }
                AdicionarTecidoAviso(mensagem,sucesso,sucesso?"PEDIDO ENTREGUE":"NÃO CONCLUÍDO");
                return;
            }
            if(Botao("TROCAR CAIXA",30,833,354,57,Cartao,true,20))AbrirSelecaoCaixas();
        }
    }
}
