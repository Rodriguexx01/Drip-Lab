using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        Texture2D producaoCatalogoArte;
        int produtoConfeccao=-1, quantidadeLote=10, tecidoConfeccao;

        void ProducaoPagina()
        {
            if(producaoCatalogoArte==null)producaoCatalogoArte=Resources.Load<Texture2D>("UI/ProductionCatalog");
            Matrix4x4 anterior=IniciarReferenciaTela(producaoCatalogoArte);
            bool habilitado=InterfaceCena.habilitado;InterfaceCena.habilitado=habilitado&&produtoConfeccao<0;
            if(producaoCatalogoArte==null)
                Texto("A tela de confecção não carregou. Volte com Esc e tente novamente.",250,350,1172,100,32,Creme,true);
            else
            {
                float[] linhas={305,460,615,773};
                for(int i=0;i<linhas.Length;i++)
                    if(InterfaceCena.Botao(new Rect(1136,linhas[i],250,98),new GUIContent("","Fabricar "+EconomiaJogo.Produtos[i]),GUIStyle.none))
                    {Toque();produtoConfeccao=i;quantidadeLote=10;tecidoConfeccao=0;for(int f=0;f<4;f++)if(Estado.metrosTecidos[f]>0){tecidoConfeccao=f;break;}}
                if(BotaoVoltarIlustrado(new Rect(11,852,301,82),new GUIContent("","Voltar para gestão"),GUIStyle.none)){Toque();aba=8;}
            }
            /* Navegação pelo botão Voltar; X duplicado removido. */
            InterfaceCena.habilitado=habilitado;
            if(produtoConfeccao>=0)DesenharQuantidadeConfeccao();
            InterfaceCena.matriz=anterior;
        }

        void DesenharQuantidadeConfeccao()
        {
            InterfaceCena.DefinirTela("Tablet/Confeccao/Quantidade");
            int tipo=produtoConfeccao;
            Retangulo(0,0,1672,941,new Color(0,0,0,.8f));
            UrbanoPainel(new Rect(390,115,892,710),77);
            UrbanoTexto(EconomiaJogo.Produtos[tipo].ToUpper(),new Rect(430,143,812,52),34,Color.white);
            UrbanoTexto("ESCOLHA A QUANTIDADE E O TECIDO",new Rect(430,220,812,30),23,UrbanoAmarelo);
            if(UrbanoBotao("−",new Rect(430,264,45,43),false,quantidadeLote>10,28))quantidadeLote-=10;
            UrbanoArredondado(new Rect(483,264,75,43),9,UrbanoAzul);
            UrbanoTexto(quantidadeLote.ToString(),new Rect(483,264,75,43),25,Color.white,TextAnchor.MiddleCenter);
            if(UrbanoBotao("+",new Rect(566,264,45,43),true,quantidadeLote<EconomiaJogo.LimiteLoteConfeccao,28))quantidadeLote+=10;
            Texto("peças  ·  lotes de 10 em 10",630,270,600,38,23,Color.white);
            for(int i=0;i<4;i++)
            {
                string rotulo=EconomiaJogo.CatalogoTecidos[i]+" · "+Estado.metrosTecidos[i]+" m";
                if(UrbanoBotao(rotulo,new Rect(430+(i%2)*412,328+(i/2)*58,400,48),tecidoConfeccao==i,true,20))tecidoConfeccao=i;
            }
            int metros=quantidadeLote/EconomiaJogo.PecasPorMetro;
            int custo=quantidadeLote*EconomiaJogo.CustosConfeccao[tipo];
            int precoVenda=EconomiaJogo.PrecoVendaPorTecido(tipo,tecidoConfeccao);
            Texto("O preço final depende do tecido: tecido mais barato, peça mais barata.",430,452,812,33,19,UrbanoAmarelo);
            Texto("VENDA AUTOMÁTICA: "+FormatarDinheiro(precoVenda)+" por peça",430,490,812,39,26,Creme,true);
            Texto("Consumo: "+metros+" m · Confecção: "+FormatarDinheiro(custo),430,538,812,34,22,Creme);
            Texto("Seu saldo: "+FormatarDinheiro(Estado.dinheiro)+" · Entrega no dia "+(Estado.dia+1),430,577,812,33,20,Creme);
            string problema=Estado.final!=""?"Esta jornada terminou.":Estado.metrosTecidos[tecidoConfeccao]<metros?"Falta o tecido selecionado para esse lote.":Estado.dinheiro<custo?"Saldo insuficiente para pagar a confecção.":"1 metro rende 10 peças. O preço de venda será aplicado no SITE.";
            bool pode=Estado.final==""&&Estado.metrosTecidos[tecidoConfeccao]>=metros&&Estado.dinheiro>=custo;
            Texto(problema,430,625,812,52,19,UrbanoAmarelo);
            if(UrbanoBotao("CANCELAR",new Rect(430,713,285,51),false,true,22)){produtoConfeccao=-1;return;}
            if(UrbanoBotao("FABRICAR "+quantidadeLote+" PEÇAS",new Rect(735,713,507,51),true,pode,25))
            {
                int valorAntes=Estado.producao.Count;
                string resultado=economia.Fabricar(tipo,quantidadeLote,tecidoConfeccao);
                bool sucesso=Estado.producao.Count==valorAntes+1;
                AdicionarTecidoAviso(sucesso?quantidadeLote+" peças de "+EconomiaJogo.Produtos[tipo]+". Entrega: dia "+(Estado.dia+1)+".":resultado,sucesso,sucesso?"LOTE EM PRODUÇÃO":"NÃO CONCLUÍDA");
                if(sucesso){Salvar(false);produtoConfeccao=-1;}
            }
        }
    }
}
