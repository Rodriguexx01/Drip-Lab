using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        readonly DobraPecas dobraPecas=new DobraPecas();
        readonly Texture2D[][] artesDobra={new Texture2D[3],new Texture2D[3],new Texture2D[3],new Texture2D[3],new Texture2D[3],new Texture2D[3],new Texture2D[2],new Texture2D[1]}; readonly string[] nomesPecasDobra={"UrbanPulse","AsfaltoCrew","CargoEclipse","ConcreteFlex","Brasa90","ParisNoturna","OndaUrbana","Vermelhinho"},titulosPecasDobra={"URBAN PULSE","ASFALTO CREW","CARGO ECLIPSE","CONCRETE FLEX","BRASA 90","PARIS NOTURNA","ONDA URBANA","VERMELHINHO"};
        static readonly Rect[] UrbanPecaAreasClique={new Rect(325,132,1020,725),new Rect(677,134,457,693),new Rect(677,300,334,341)}; static readonly Rect[] AsfaltoPecaAreasClique={new Rect(330,135,1008,721),new Rect(690,127,388,732),new Rect(677,311,338,364)}; static readonly Rect[] CargoPecaAreasClique={new Rect(493,99,694,783),new Rect(707,98,280,772),new Rect(683,280,310,420)}; static readonly Rect[] ConcretePecaAreasClique={new Rect(493,103,687,779),new Rect(700,105,279,775),new Rect(676,292,340,376)}; static readonly Rect[] BrasaPecaAreasClique={new Rect(211,109,1243,716),new Rect(673,101,284,725),new Rect(628,219,391,489)}; static readonly Rect[] ParisPecaAreasClique={new Rect(349,108,970,748),new Rect(697,104,405,760),new Rect(649,272,391,407)}; static readonly Rect[] OndaPecaAreasClique={new Rect(612,279,444,398),new Rect(624,418,432,178)}; static readonly Rect[][] PecaAreasClique={UrbanPecaAreasClique,AsfaltoPecaAreasClique,CargoPecaAreasClique,ConcretePecaAreasClique,BrasaPecaAreasClique,ParisPecaAreasClique,OndaPecaAreasClique,new Rect[0]};

        bool PrecisaPrepararPeca(Pedido pedido){return (pedido.tipo>=0&&pedido.tipo<artesDobra.Length)&&!dobraPecas.EstaDobrada(pedido.id);}
        void DesenharDobraAceite(Pedido pedido)
        {
            Texto("PREPARE: "+EconomiaJogo.Produtos[pedido.tipo].ToUpper(),650,400,637,45,28,Creme,true);
            Texto(pedido.tipo==7?"Aceite o pedido para colocar o par na bancada. Ele já está pronto para embalar.":pedido.tipo==6?"Aceite o pedido para colocar o gorro na bancada. Clique nele uma vez para enrolar antes de embalar.":"Aceite o pedido para colocar a peça na bancada. Clique nela duas vezes para fazer as dobras antes de embalar.",650,465,610,105,22,SemSom);
            if(Botao(pedido.tipo==7?"ACEITAR E COLOCAR NA BANCADA":pedido.tipo==6?"ACEITAR E ENROLAR NA BANCADA":"ACEITAR E DOBRAR NA BANCADA",650,610,637,65,Amarelo,true,24))
            {
                var arte=artesDobra[pedido.tipo]; for(int i=0;i<arte.Length;i++)if(arte[i]==null)arte[i]=Resources.Load<Texture2D>("Bench/"+nomesPecasDobra[pedido.tipo]+"Fold"+i);
                if(System.Array.Exists(arte,imagem=>imagem==null))
                {AdicionarTecidoAviso("As imagens de "+EconomiaJogo.Produtos[pedido.tipo]+" não carregaram.",false,"IMAGENS INDISPONÍVEIS");return;}
                dobraPecas.Iniciar(pedido.id,arte.Length-1);tabletAberto=false;Toque();
            }
            if(Botao("VOLTAR À LISTA",375,749,238,43,Cartao,true,16))produtoPedido=-1;
        }
        void VoltarDaBancada()
        {
            var pedido=Estado.pedidos.Find(o=>o.id==dobraPecas.IdAtivo);
            selecaoCaixasAberta=false;dobraPecas.Sair();
            if(pedido==null)return;
            pedidoSelecionado=pedido.id;produtoPedido=pedido.tipo;aba=3;tabletAberto=true;
        }
        bool DesenharBancadaDobra()
        {
            if(dobraPecas.IdAtivo<=0||tabletAberto)return false;
            var pedido=Estado.pedidos.Find(o=>o.id==dobraPecas.IdAtivo);
            if(pedido==null){selecaoCaixasAberta=false;dobraPecas.Sair();return false;} if(selecaoCaixasAberta){DesenharSelecaoCaixas(pedido);return true;} if(caixasPedidos.TryGetValue(pedido.id,out int embaladoCor)){DesenharPedidoEmbalado(pedido,embaladoCor);return true;}
            InterfaceCena.DefinirTela("Atelie/Dobrar/"+nomesPecasDobra[pedido.tipo]);
            int etapa=dobraPecas.Etapa(pedido.id),ultimo=artesDobra[pedido.tipo].Length-1; string peca=titulosPecasDobra[pedido.tipo];
            InterfaceCena.DesenharTextura(new Rect(0,0,1672,941),artesDobra[pedido.tipo][etapa],ScaleMode.StretchToFill);
            DesenharDobraPainel();
            UrbanoPainel(new Rect(415,139,850,69),24);
            Texto(etapa<ultimo?(pedido.tipo==6?peca+" · CLIQUE NO GORRO PARA ENROLAR":peca+" · CLIQUE NA ROUPA PARA DOBRAR ("+etapa+"/"+ultimo+")"):(pedido.tipo==7?peca+" · PRONTO PARA EMBALAR":peca+(pedido.tipo==6?" ENROLADA":" DOBRADA")+" · PRONTA PARA EMBALAR"),433,153,814,43,24,Creme,true,TextAnchor.MiddleCenter);
            if(etapa<ultimo&&InterfaceCena.Botao(PecaAreasClique[pedido.tipo][etapa],new GUIContent("",(pedido.tipo==6?"Enrolar ":"Dobrar a ")+peca),GUIStyle.none))
            {dobraPecas.Clique();TocarDobra();}
            if(etapa==ultimo&&DesenharAtalhoCaixas())
            {AbrirSelecaoCaixas();}
            if(Botao("VOLTAR AO PEDIDO",30,833,354,57,Cartao,true,20))
            {VoltarDaBancada();Toque();}
            return true;
        }
        void DesenharDobraPainel()
        {
            DesenharHudBancada(false);
        }
    }
}
