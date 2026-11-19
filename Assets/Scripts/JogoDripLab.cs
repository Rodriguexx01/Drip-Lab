using GUI = DripLab.InterfaceCena;
using System;
using System.IO;
using System.Collections;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab : MonoBehaviour
    {
        EconomiaJogo economia;
        EstadoJogo Estado { get { return economia.estado; } }
        int aba, pedidoSelecionado, caixa, paginaPedidos;
        bool ajuda, reiniciarConfirmacao, riscoRoleta, apostaVermelho=true;
        bool menuAberto=true, opcoesAudio, tabletAberto;
        int aposta=50, roletaNumero=-1;
        float avisoAte;
        string aviso="", resumo="", caminhoSalvamento;
        GUIStyle textoEstilo, botaoEstilo;
        Font fonte;
        AudioSource fonteAudio;
        AudioClip clique;
        Texture2D branco;
        bool validacao;
        static readonly Color Tinta=Hex("20232D"), Creme=Hex("F3F0E8"), Amarelo=Hex("FFB600"), Azul=Hex("123DFF"), SemSom=Hex("B9C4D8"), Painel=Hex("090B12"), Cartao=Hex("142B6B");
        static readonly Color[] CoresCaixas={Hex("D9232B"),Hex("26BE35"),Hex("123DFF"),Hex("FF9400")};
        static Color Hex(string valor) { Color c;ColorUtility.TryParseHtmlString("#"+valor,out c);return c; }
        void Awake()
        {
            Application.targetFrameRate=60;
            caminhoSalvamento=Path.Combine(Application.persistentDataPath,"driplab-save-v1.json");
            validacao=Array.IndexOf(Environment.GetCommandLineArgs(),"-driplab-qa")>=0;
            economia=new EconomiaJogo();
            // Every launch starts a new run, even after a crash or an old saved game.
            relogioDia.Reiniciar();
            fonte=CarregarFonteProjeto();
            if(fonte==null)fonte=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            branco=Texture2D.whiteTexture;
            fonteAudio=gameObject.AddComponent<AudioSource>();
            fonteAudio.volume=.10f;
            fonteAudio.mute=true;




            ConfigurarAudio();
            atelieArte=Resources.Load<Texture2D>("Bench/Workshop");
        }
        void Start() { if(validacao)StartCoroutine(CapturarValidacao()); }
        void Salvar(bool notificar=true) {if(notificar)Aviso("A partida recomeça no dia 1 ao fechar o jogo.");}
        void Executar(string mensagem) {Aviso(mensagem);Salvar(false);}
        void Aviso(string mensagem) {aviso=mensagem;avisoAte=Time.realtimeSinceStartup+5;AdicionarTecidoAviso(mensagem,true,"DRIP LAB");}
        void Toque() {ReproduzirClique();}
        void InicializarEstilo()
        {
            if(textoEstilo!=null)return;
            textoEstilo=new GUIStyle(InterfaceCena.tema.label){font=fonte,richText=true,wordWrap=true,padding=new RectOffset(0,0,0,0)};
            botaoEstilo=new GUIStyle(InterfaceCena.tema.label){font=fonte,fontSize=16,alignment=TextAnchor.MiddleCenter,wordWrap=true,richText=true,padding=new RectOffset(0,0,0,0)};
        }
        void Retangulo(float x,float y,float w,float h,Color c,[System.Runtime.CompilerServices.CallerFilePath]string arquivo="",[System.Runtime.CompilerServices.CallerLineNumber]int linha=0) {if(Event.current.type!=EventType.Repaint)return;using var elemento=InterfaceCena.Elemento("Forma",new Rect(x,y,w,h),"",null,arquivo,linha);if(!elemento.Visivel)return;InterfaceCena.cor=c*(elemento.No!=null?elemento.No.tonalidade:Color.white);InterfaceCena.DesenharTextura(elemento.Area,branco);InterfaceCena.cor=Color.white;}
        void Texto(string valor,float x,float y,float w,float h,int tamanho,Color cor,bool negrito=false,TextAnchor alinhamento=TextAnchor.UpperLeft,[System.Runtime.CompilerServices.CallerFilePath]string arquivo="",[System.Runtime.CompilerServices.CallerLineNumber]int linha=0)
        {
            if(Event.current.type!=EventType.Repaint)return;
            using var elemento=InterfaceCena.Elemento("Texto",new Rect(x,y,w,h),valor,null,arquivo,linha);if(!elemento.Visivel)return;
            ConfigurarUrbanoFonte();textoEstilo.font=negrito&&urbanoFonte!=null?urbanoFonte:fonte;
            textoEstilo.fontSize=tamanho;textoEstilo.normal.textColor=cor;textoEstilo.fontStyle=negrito?FontStyle.Bold:FontStyle.Normal;textoEstilo.alignment=alinhamento;
            textoEstilo.hover.textColor=cor;textoEstilo.active.textColor=cor;textoEstilo.focused.textColor=cor;
            DesenharRotuloUrbano(elemento.Area,elemento.Texto,textoEstilo,cor,negrito&&tamanho>=18);
        }
        bool Botao(string valor,float x,float y,float w,float h,Color corFundo,bool habilitado=true,int tamanho=16,[System.Runtime.CompilerServices.CallerFilePath]string arquivo="",[System.Runtime.CompilerServices.CallerLineNumber]int linha=0)
        {
            using var elemento=InterfaceCena.Elemento("Botao",new Rect(x,y,w,h),valor,null,arquivo,linha);if(!elemento.Visivel)return false;
            Color? destaque=corFundo==Hex("DC6577")||corFundo==Hex("A74552")?(Color?)Hex("D9232B"):null;
            return UrbanoBotao(elemento.Texto,elemento.Area,corFundo==Amarelo||corFundo==Creme||corFundo==Azul,habilitado,tamanho,destaque);
        }
        void Linha(float x,float y,float w,Color c) {Retangulo(x,y,w,1,c);}
        void Pequeno(string v,float x,float y,float w=640) {Texto(v,x,y,w,35,14,SemSom);}
        void Cabecalho(string n,string titulo,string subtitulo)
        {
            Texto(n,698,304,680,22,12,Amarelo,true);
            Texto(titulo,698,330,680,36,28,Color.white,true);
            Pequeno(subtitulo,698,373);
        }
        void DesenharInterfaceJogo()
        {
            InicializarEstilo();if(tutorialAtivo){DesenharTutorial();return;}InterfaceCena.matriz=Matrix4x4.TRS(Vector3.zero,Quaternion.identity,new Vector3(Screen.width/1440f,Screen.height/900f,1));
            if(Estado.introducaoVista&&!historia.Ativo&&!carregamento.Ativo&&economia.DespesasIniciaisPendentes){if(avisoDespesasAberto)DesenharAvisoDespesasObrigatorias();else DesenharPagamentoInicialObrigatorio();return;} if(Event.current.type==EventType.KeyDown && Event.current.keyCode==KeyCode.Escape)
            {
                if(carregamento.Ativo||roletaGirando||tecidoRoletaGirando) { }
                else if(selecaoCaixasAberta){selecaoCaixasAberta=false;} else if(dobraPecas.IdAtivo>0&&!tabletAberto)VoltarDaBancada(); else if(opcoesAudio)opcoesAudio=false; else if(tabletAberto&&aba==6&&paginaInvestimentos>0){paginaInvestimentos=0;riscoRoleta=false;}
                else if(tabletAberto && aba==9 && tecidoSelecionado>=0)tecidoSelecionado=-1;
                else if(tabletAberto && aba==10 && colaboracaoSelecao>=0)colaboracaoSelecao=-1;
                else if(tabletAberto && aba==1 && produtoConfeccao>=0)produtoConfeccao=-1;
                else if(tabletAberto && aba==5 && despesasPainel)despesasPainel=false;
                else if(tabletAberto && aba==3 && produtoPedido>=0)produtoPedido=-1;
                else if(ajuda)ajuda=false;
                else if(reiniciarConfirmacao)reiniciarConfirmacao=false;
                else if(tabletAberto)tabletAberto=false;
                else menuAberto=!menuAberto;
                Event.current.Use();
            }
            if(menuAberto)
            {
                InterfaceCena.habilitado=!opcoesAudio;
                InterfaceCena.DefinirTela("Menu");DesenharMenuPrincipal();InterfaceCena.habilitado=true;
                if(opcoesAudio){InterfaceCena.DefinirTela("Audio");DesenharOpcoesAudio();}
                return;
            }
            if(carregamento.Ativo){InterfaceCena.habilitado=true;InterfaceCena.DefinirTela("Carregamento");DesenharCarregamento();return;}
            if(!Estado.introducaoVista && !historia.Ativo)IniciarHistoria();
            if(historia.Ativo){InterfaceCena.habilitado=true;InterfaceCena.DefinirTela("Historia/"+historia.Pagina);DesenharHistoria();return;}
            bool podeInteragir=Estado.introducaoVista && Estado.final=="" && !ajuda && !reiniciarConfirmacao && resumo=="" && !opcoesAudio;
            InterfaceCena.habilitado=podeInteragir && !tabletAberto;
            InterfaceCena.DefinirTela("Atelie");DesenharAtelie();
            if(tabletAberto)
            {
                Retangulo(0,0,1440,900,new Color(.02f,.02f,.03f,.7f));
                InterfaceCena.habilitado=podeInteragir;
                InterfaceCena.DefinirTela(TabletHierarquiaTela());DesenharTablet();
                Matrix4x4 matrizAjuda=IniciarReferenciaTela(null,false);
                if(aba==0&&BotaoAjudaIlustrado(new Rect(34,27,72,72)))ajuda=true;
                InterfaceCena.matriz=matrizAjuda;
            }
            InterfaceCena.habilitado=podeInteragir;
            InterfaceCena.habilitado=true;
            if(!Estado.introducaoVista)DesenharIntroducao();
            else if(Estado.final!=""){InterfaceCena.DefinirTela(Estado.final=="win"?"Final/Vitoria":"Final/Falencia");DesenharFinal();}
            else if(opcoesAudio){InterfaceCena.DefinirTela("Audio");DesenharOpcoesAudio();}
            else if(ajuda){InterfaceCena.DefinirTela("Ajuda");DesenharAjuda();}
            else if(reiniciarConfirmacao){InterfaceCena.DefinirTela("Reinicio");DesenharReiniciar();}
            else if(resumo!=""){InterfaceCena.DefinirTela("Resumo");DesenharResumo();}
            if(tabletAberto&&Estado.final==""){InterfaceCena.DefinirTela("HUD/Tablet");DesenharEstoqueTablet();}
            InterfaceCena.DefinirTela("HUD/Notificacoes");if(Estado.final=="")DesenharAvisosCompra();
        }
        string FormatarDinheiro(int n) {return "R$ "+n.ToString("N0",System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));}
        void Indicador(string rotulo,string valor,int x,int y,int largura)
        {Texto(rotulo,x,y,largura,20,11,Hex("65676E"),true);Texto(valor,x,y+22,largura,33,26,Tinta,true);}
        void DesenharTablet()
        {
            if(aba==0){DesenharInicioTablet();return;}
            if(aba==1){ProducaoPagina();return;}
            if(aba==2){SitePagina();return;}
            if(aba==3){PedidosPagina();return;}
            if(aba==4){PublicidadePagina();return;}
            if(aba==5){FinanceiroPagina();return;} if(aba==6){DesenharArteInvestimentos();return;} if(aba==7){DesenharArteNoticias();return;}
            if(aba==8){DesenharInicioGestao();return;}
            if(aba==9){TecidoPagina();return;}
            if(aba==10){ColaboracaoPagina();return;}
            DesenharUrbanoTabletPaginas();
        }
        void PainelGeral()
        {
            Cabecalho("VISÃO GERAL", "O próximo grande drop começa aqui.","Transforme tecido em peças, pedidos em fãs e alcance 100 de fama.");
            Mini("TECIDO",Estado.tecido+" un.",698,422,205);
            Mini("PEÇAS PRONTAS",TotalEstoque()+" un.",924,422,214);
            Mini("A EMBALAR",Estado.pedidos.Count+" pedidos",1159,422,210);
            Texto("SEU CORRE DE HOJE",698,520,650,25,12,Amarelo,true);
            string tarefa=Estado.tecido==0 && Estado.producao.Count==0 && TotalEstoque()==0 && Estado.drops.Count==0 && Estado.pedidos.Count==0?"01  Compre tecido e envie seu primeiro lote à gráfica.":Estado.pedidos.Count>0?"03  Monte as caixas e envie "+Estado.pedidos.Count+" pedidos para receber.":Estado.producao.Count>0?"02  Seu lote está na gráfica. Avance o dia para receber.":TotalEstoque()>0?"02  Publique as peças no site e escolha o preço do drop.":"04  Divulgue a marca e avance o dia para movimentar as vendas.";
            Texto(tarefa,698,549,668,50,19,Color.white);
            Linha(698,614,670,Hex("383C48"));
            Texto("NA RUA / NOTÍCIAS",698,630,650,22,12,Azul,true);
            Texto(Estado.noticias.Count>0?Estado.noticias[0]:"Tio Cleiton: ‘São R$ 2.500, DG. Acredito no seu sonho!’",698,660,666,59,17,Color.white);
            Pequeno(Estado.tituloEvento,698,727);
        }
        int TotalEstoque(){int total=0;foreach(int quantidade in Estado.estoque)total+=quantidade;return total;}
        void Mini(string nome,string valor,float x,float y,float largura)
        {UrbanoPainel(new Rect(x,y,largura,78),21);Texto(nome,x+14,y+12,largura-25,19,11,Creme,true);Texto(valor,x+14,y+39,largura-25,32,25,Color.white,true);}
        int SeletorQuantidade(int valor,float x,float y,int minimo,int maximo,int passo=1)
        {
            if(Botao("−",x,y,40,38,Cartao,valor>minimo,23))valor=Math.Max(minimo,valor-passo);
            Texto(valor.ToString(),x+42,y,70,38,21,Color.white,true,TextAnchor.MiddleCenter);
            if(Botao("+",x+114,y,40,38,Cartao,valor<maximo,23))valor=Math.Min(maximo,valor+passo);
            return valor;
        }
        void RiscoPagina()
        {
            Cabecalho("RISCO / DINHEIRO FICTÍCIO","Nem todo atalho leva ao topo.","Cripto e roleta são opcionais. Só usam o dinheiro fictício desta partida.");
            if(Botao("DGCoin / cripto",698,417,325,38,!riscoRoleta?Azul:Cartao))riscoRoleta=false;
            if(Botao("Roleta / vermelho ou preto",1043,417,325,38,riscoRoleta?Azul:Cartao))riscoRoleta=true;
            if(!riscoRoleta)
            {
                Texto("DGC",698,476,120,35,27,Amarelo,true);
                Texto("R$ "+Estado.precoMoeda.ToString("F2"),823,476,290,35,27,Color.white,true);
                Pequeno("Cotação simulada: −30% a +35% por dia. Pode perder valor.",698,519);
                DesenharGrafico();
                Pequeno("CARTEIRA  "+Estado.moedas.ToString("F3")+" DGC  /  "+FormatarDinheiro(Mathf.FloorToInt(Estado.moedas*Estado.precoMoeda)),698,654);
                if(Botao("COMPRAR R$ 100",698,706,215,42,Amarelo,Estado.dinheiro>=100,14))Executar(economia.ComprarMoeda(100));
                if(Botao("COMPRAR R$ 500",924,706,215,42,Azul,Estado.dinheiro>=500,14))Executar(economia.ComprarMoeda(500));
                if(Botao("VENDER TUDO",1150,706,218,42,Cartao,Estado.moedas>0,14))Executar(economia.VenderMoeda());
            }
        }
        void DesenharGrafico()
        {
            float x=707,y=570,w=645,h=65;Linha(x,y+h,w,UrbanoAzul);
            float maximo=110,minimo=90;foreach(float p in Estado.historicoCotacoes){maximo=Mathf.Max(maximo,p);minimo=Mathf.Min(minimo,p);}
            if(maximo-minimo<1)maximo=minimo+1;
            for(int i=1;i<Estado.historicoCotacoes.Count;i++)
            {float x1=x+(i-1)*w/15,x2=x+i*w/15;float y1=y+h-(Estado.historicoCotacoes[i-1]-minimo)/(maximo-minimo)*h,y2=y+h-(Estado.historicoCotacoes[i]-minimo)/(maximo-minimo)*h;DesenharLinha(new Vector2(x1,y1),new Vector2(x2,y2),Amarelo,3);}
            if(Estado.historicoCotacoes.Count==1)Retangulo(x,y+30,12,4,Amarelo);
        }
        void DesenharLinha(Vector2 a,Vector2 b,Color cor,float largura)
        {
            if(Event.current.type!=EventType.Repaint)return;
            Matrix4x4 anterior=InterfaceCena.matriz;GUIUtility.RotateAroundPivot(Mathf.Atan2(b.y-a.y,b.x-a.x)*Mathf.Rad2Deg,a);
            Retangulo(a.x,a.y,Vector2.Distance(a,b),largura,cor);InterfaceCena.matriz=anterior;
        }
        void Escurecer() {Retangulo(0,0,1440,900,new Color(.04f,.05f,.09f,.86f));}
        void DesenharIntroducao()
        {
            Escurecer();UrbanoPainel(new Rect(270,139,900,622),64);
            Texto("DRIPLAB  /  DO QUARTO PRA RUA",312,181,810,30,15,Creme,true);
            Texto("R$ 2.500.\nUm sonho maior que o quarto.",311,242,805,135,44,Creme,true);
            Texto("Você é DG, tem vinte e poucos anos e quer abrir sua própria marca de streetwear. A grana está curta. Seu tio Cleiton dá R$ 2.500 para começar e acredita no seu corre.",314,411,785,93,23,Creme);
            Texto("Compre tecido, crie suas peças, lance drops e embale cada pedido. Faça sua marca chegar a 100 de fama para vencer.",314,530,783,72,21,Creme);
            if(Botao("COMEÇAR MEU CORRE  →",314,655,492,60,Creme,true,22)){Estado.introducaoVista=true;economia.RegistrarNoticia("Tio Cleiton deu R$ 2.500 para começar. Agora é com você, DG!");Salvar(false);}
            Texto("PROTÓTIPO 01\nUNITY / 2D",906,665,205,45,13,Creme,true,TextAnchor.MiddleRight);
        }
        void DesenharResumo()
        {
            DesenharArteResumoDiario();





        }
        void DesenharAjuda()
        {
            Escurecer();UrbanoPainel(new Rect(280,99,880,706),74);
            Texto("MANUAL DO FUNDADOR",320,137,790,50,34,Creme,true);
            string[] passos={"01  TABLET: clique no aparelho sobre a mesa para abrir os aplicativos. Feche no X ou com Esc.","02  PRODUÇÃO: compre tecido e envie o lote à gráfica. Encerre o dia para receber as peças prontas.","03  SITE: publique as peças. O drop limitado dura 3 dias. Avance o dia para gerar pedidos; mídia aumenta a procura.","04  PEDIDOS: aceite, prepare a peça, escolha a caixa e entregue para receber. A embalagem custa R$ 5 ou R$ 7, descontados da venda.","05  CONTAS: pague as despesas da loja. Chegue a 100 de fama para vencer.","Pedidos atrasados cancelam após 3 dias. Esc abre o menu; as opções de áudio ajustam música e efeitos. Cripto e roleta são opcionais."};
            for(int i=0;i<passos.Length;i++)Texto(passos[i],323,207+i*78,793,71,18,Creme);
            if(Botao("ENTENDI",323,727,380,44,Tinta))ajuda=false;if(Botao("REVER TUTORIAL",723,727,390,44,Azul))IniciarTutorial();
        }
        void DesenharFinal()
        {
            DesenharArteFinal();






        }
        int CapitalTotal()
        {
            float valor=Estado.dinheiro+Estado.tecido*18+Mathf.FloorToInt(Estado.moedas*Estado.precoMoeda);
            for(int i=0;i<EconomiaJogo.Produtos.Length;i++)valor+=Estado.estoque[i]*(EconomiaJogo.ValorContabilProduto(i));
            foreach(var p in Estado.producao)valor+=p.quantidade*(EconomiaJogo.ValorContabilProduto(p.tipo));
            foreach(var d in Estado.drops)valor+=d.restante*(EconomiaJogo.ValorContabilProduto(d.tipo));
            foreach(var o in Estado.pedidos)valor+=EconomiaJogo.ValorContabilProduto(o.tipo);
            return Mathf.RoundToInt(valor);
        }
        void DesenharReiniciar()
        {
            Escurecer();UrbanoPainel(new Rect(400,275,640,340),84);
            Texto("Começar de novo?",440,314,560,52,32,Creme,true);
            Texto("Seu progresso atual será substituído por uma nova jornada do DG.",440,385,550,90,22,Creme);
            if(Botao("CONTINUAR JOGANDO",440,521,275,48,Tinta))reiniciarConfirmacao=false;
            if(Botao("RECOMEÇAR",730,521,266,48,Azul))NovaPartida();
        }
        void NovaPartida(){avisoDespesasAberto=false;tutorialAtivo=false;etapaTutorial=0;paginaInvestimentos=0;fichaRoleta=50;quantidadeFichas=1;quantidadeCripto=1;periodoCripto=1;apostaZero=false;riscoRoleta=false;caixasPedidos.Clear();selecaoCaixasAberta=false;dobraPecas.Reiniciar();economia=new EconomiaJogo();relogioDia.Reiniciar();roletaGirando=false;anguloRoleta=0;mensagemRoleta="Escolha vermelho ou preto e gire a roleta.";tecidoRoletaGirando=false;aba=0;resumo="";aviso="";reiniciarConfirmacao=false;ajuda=false;tabletAberto=false;pedidoSelecionado=0;paginaPedidos=0;produtoConfeccao=-1;tecidoSelecionado=-1;produtoPedido=-1;despesasPainel=false;colaboracaoSelecao=-1;pedidoProdutosPagina=0;tecidoAvisos.Clear();}
        void DesenharCaixa(float x,float y,float escala,Color c,int estampa)
        {
            float u=escala/100;
            Retangulo(x,y+15*u,100*u,65*u,Color.Lerp(c,Color.black,.18f));
            Retangulo(x,y,100*u,20*u,Color.Lerp(c,Color.white,.1f));
            Retangulo(x+43*u,y,14*u,80*u,Hex("E8D9B4"));
            Retangulo(x+9*u,y+36*u,33*u,26*u,Creme);
            if(estampa==0||estampa==3)Texto(estampa==0?"DG":"DRIP",x+10*u,y+39*u,31*u,20*u,Mathf.RoundToInt((estampa==0?11:8)*u),Tinta,true,TextAnchor.MiddleCenter);
            else if(estampa==1)
            {
                Vector2 centro=new Vector2(x+25*u,y+49*u);
                for(int i=0;i<5;i++)
                {
                    float a=(-90+i*144)*Mathf.Deg2Rad,b=(-90+(i+1)*144)*Mathf.Deg2Rad;
                    DesenharLinha(centro+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*10*u,centro+new Vector2(Mathf.Cos(b),Mathf.Sin(b))*10*u,Tinta,2*u);
                }
            }
            else
            {
                Retangulo(x+23*u,y+39*u,6*u,19*u,Hex("D56B48"));Retangulo(x+18*u,y+46*u,16*u,12*u,Hex("D56B48"));Retangulo(x+24*u,y+49*u,5*u,9*u,Amarelo);
            }
            Retangulo(x+66*u,y+54*u,25*u,15*u,Color.white);
            for(int i=0;i<7;i++)Retangulo(x+(68+i*3)*u,y+57*u,1*u,9*u,Tinta);
        }
        IEnumerator CapturarValidacao()
        {
            string direcao=Path.Combine(Application.dataPath,"../../Screenshots");Directory.CreateDirectory(direcao);
            yield return new WaitForSeconds(1);yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(direcao,"01-menu.png"));
            yield return new WaitForSeconds(.5f);menuAberto=false;Estado.introducaoVista=true;economia.ComprarTecido(20);economia.Fabricar(0,10);economia.ProximoDia();economia.Lancar(0,10,89,true);economia.ContratarPublicidade(0);economia.ProximoDia();aba=0;tabletAberto=true;
            yield return new WaitForSeconds(.5f);yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(direcao,"02-atelie-tablet.png"));
            yield return new WaitForSeconds(.5f);aba=3;caixa=1;
            yield return new WaitForSeconds(.5f);yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(direcao,"03-embalagem.png"));
            yield return new WaitForSeconds(.5f);aba=1;
            yield return new WaitForSeconds(.5f);yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(direcao,"04-producao.png"));
            yield return new WaitForSeconds(1);Application.Quit();
        }
    }
}
