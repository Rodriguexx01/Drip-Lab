using GUI = DripLab.InterfaceCena;
using System;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        Texture2D publicidadeArte,despesasArte;
        bool despesasPainel;
        GUIStyle gestaoPrecoEstilo;

        void InicializarGestaoPrecoEstilo()
        {
            if(gestaoPrecoEstilo!=null)return;
            Font face=CarregarFonteProjeto(true);
            gestaoPrecoEstilo=new GUIStyle(InterfaceCena.tema.label){font=face!=null?face:fonte,fontSize=30,alignment=TextAnchor.MiddleCenter,padding=new RectOffset(0,0,0,0)};
            gestaoPrecoEstilo.normal.textColor=Color.white;gestaoPrecoEstilo.hover.textColor=Color.white;
            gestaoPrecoEstilo.active.textColor=Color.white;gestaoPrecoEstilo.focused.textColor=Color.white;
        }
        bool GestaoPrecoBotao(Rect area,string rotulo,bool pago,bool clicavel,int tamanho=30)
        {
            InicializarGestaoPrecoEstilo();
            DesenharPrecoUrbano(area,pago);
            // Mesma moldura dourada, sombra e textura dos outros controles.
            gestaoPrecoEstilo.fontSize=tamanho;
            DesenharRotuloUrbano(area,rotulo,gestaoPrecoEstilo,Creme,true);
            bool habilitado=InterfaceCena.habilitado;InterfaceCena.habilitado=habilitado&&clicavel;
            bool pressionado=InterfaceCena.Botao(area,GUIContent.none,GUIStyle.none);InterfaceCena.habilitado=habilitado;
            if(pressionado)Toque();return pressionado;
        }
        void PublicidadePagina()
        {
            if(publicidadeArte==null)publicidadeArte=Resources.Load<Texture2D>("UI/AdvertisingCatalog");
            Matrix4x4 anterior=IniciarReferenciaTela(publicidadeArte);
            if(publicidadeArte==null)Texto("A tela de publicidade não carregou.",300,350,1000,70,30,Creme,true);
            else
            {
                float[] linhas={273,444,613,782};
                for(int i=0;i<EconomiaJogo.PublicidadeNomes.Length;i++)
                {
                    bool ativo=economia.PublicidadeAtivo(i);
                    if(PublicidadeArteBotao(new Rect(926,linhas[i],233,75),i,ativo))
                    {
                        int valorAntes=Estado.dinheiro;string mensagem=economia.ContratarPublicidade(i);
                        bool sucesso=Estado.dinheiro<valorAntes;
                        AdicionarTecidoAviso(mensagem,sucesso,sucesso?"CONTRATADO":"NÃO CONCLUÍDO");
                        if(sucesso)Salvar(false);
                    }
                    if(ativo)
                    {
                        var campanha=Estado.campanhas.Find(c=>c.nome==EconomiaJogo.PublicidadeNomes[i]&&c.fimDia>Estado.dia);
                        Texto("Até o dia "+campanha.fimDia,938,linhas[i]+78,220,24,15,Creme,true,TextAnchor.MiddleCenter);
                    }
                }
            }
            GestaoNavegacao();
            InterfaceCena.matriz=anterior;
        }
        void FinanceiroPagina()
        {
            if(despesasArte==null)despesasArte=Resources.Load<Texture2D>("UI/ExpensesCatalog");
            Matrix4x4 anterior=IniciarReferenciaTela(despesasArte);
            bool habilitado=InterfaceCena.habilitado;InterfaceCena.habilitado=habilitado&&!despesasPainel;
            if(despesasArte==null)Texto("A tela de despesas não carregou.",300,350,1000,70,30,Creme,true);
            else
            {
                Rect[] areas={new Rect(977,284,405,119),new Rect(492,574,304,72),new Rect(1090,574,303,72),new Rect(492,785,304,72),new Rect(1090,785,303,72)};
                for(int i=0;i<EconomiaJogo.DespesaNomes.Length;i++)
                {
                    int categoria=i;
                    var contas=Estado.contas.FindAll(b=>b.categoria==categoria&&!b.pago);
                    int total=0;foreach(var conta in contas)total+=conta.valor;
                    bool pago=contas.Count==0;
                    if(DespesaArteBotao(areas[i],i,pago,total))
                    {
                        if(Estado.dinheiro<total)AdicionarTecidoAviso("Saldo insuficiente para pagar "+EconomiaJogo.DespesaNomes[i]+".",false);
                        else
                        {
                            foreach(var conta in contas)economia.PagarConta(conta.id);
                            AdicionarTecidoAviso(EconomiaJogo.DespesaNomes[i]+": tá pago! "+FormatarDinheiro(total)+".",true,"TÁ PAGO");Salvar(false);
                        }
                    }
                }
            }
            if(!pagamentoInicialObrigatorio)DesenharVoltarDespesas();
            if(!pagamentoInicialObrigatorio&&Estado.contas.Exists(b=>b.categoria<0&&!b.pago))
                if(Botao("OUTROS PAGAMENTOS",22,352,210,44,Cartao,true,14))despesasPainel=true;
            InterfaceCena.habilitado=habilitado;
            if(despesasPainel)DesenharOutrasDespesas();
            InterfaceCena.matriz=anterior;
        }
        void GestaoNavegacao()
        {
            if(BotaoVoltarIlustrado(new Rect(11,850,304,84),new GUIContent("","Voltar para gestão"),GUIStyle.none)){Toque();aba=8;}
            /* Navegação pelo botão Voltar; X duplicado removido. */
        }
        void DesenharOutrasDespesas()
        {
            InterfaceCena.DefinirTela("Tablet/Despesas/Pagamentos");
            Retangulo(0,0,1672,941,new Color(0,0,0,.8f));UrbanoPainel(new Rect(390,210,892,520),80);
            Texto("OUTROS PAGAMENTOS",430,241,812,45,30,Creme,true);
            var antigo=Estado.contas.Find(b=>b.categoria<0&&!b.pago);
            if(antigo!=null)
            {
                Texto("Conta anterior do ateliê: "+FormatarDinheiro(antigo.valor),430,467,812,37,22,Creme);
                if(Botao("PAGAR CONTA ANTERIOR",430,516,812,43,Cartao,Estado.dinheiro>=antigo.valor,18))SalvarPagamentoDespesa(economia.PagarConta(antigo.id));
            }
            // A falência agora fica somente no menu de Gestão.
            if(BotaoVoltarIlustrado(new Rect(430,646,245,53),new GUIContent("","Voltar às despesas"),GUIStyle.none))despesasPainel=false;
        }
        void SalvarPagamentoDespesa(string mensagem){AdicionarTecidoAviso(mensagem,true,"PAGAMENTO");Salvar(false);}
        bool pagamentoInicialObrigatorio;
        void DesenharPagamentoInicialObrigatorio()
        {
            if(Event.current.type==EventType.KeyDown)Event.current.Use();
            menuAberto=false;opcoesAudio=false;ajuda=false;reiniciarConfirmacao=false;tabletAberto=true;aba=5;despesasPainel=false;
            InterfaceCena.habilitado=true;InterfaceCena.DefinirTela("Tablet/Despesas");
            pagamentoInicialObrigatorio=true;
            try { FinanceiroPagina(); }
            finally { pagamentoInicialObrigatorio=false; }
            Matrix4x4 anterior=IniciarReferenciaTela(null,false);
            int pendentes=0,total=0;foreach(var conta in Estado.contas)if(conta.categoria>=0&&conta.categoria<EconomiaJogo.DespesaNomes.Length&&!conta.pago){pendentes++;total+=conta.valor;}
            UrbanoPainel(new Rect(22,864,1628,61),0);
            Texto("PAGUE AS DESPESAS PARA COMEÇAR · "+pendentes+" restantes: "+FormatarDinheiro(total)+" · Saldo: "+FormatarDinheiro(Estado.dinheiro),42,875,1588,38,23,Creme,true,TextAnchor.MiddleCenter);
            InterfaceCena.matriz=anterior;
            if(!economia.DespesasIniciaisPendentes)
            {
                tabletAberto=false;aba=0;relogioDia.Reiniciar();tecidoAvisos.Clear();
                AdicionarTecidoAviso("Despesas quitadas! A loja está liberada. Esta cobrança não se repete.",true,"BORA COMEÇAR");
            }
        }
        readonly Texture2D[] despesasBotoesArte=new Texture2D[6];
        static readonly string[] DespesasBotoesRecursos={"DespesaWifi","DespesaAgua","DespesaMercado","DespesaAluguel","DespesaEnergia","DespesaPaga"};
        static readonly Rect[] DespesasBotoesUv={new Rect(0.107135638f,0.181928441f,0.785728724f,0.634930261f),new Rect(0.152325511f,0.253774786f,0.695591535f,0.478442787f),new Rect(0.096710051f,0.227946493f,0.806579899f,0.535610991f),new Rect(0.102570379f,0.212974296f,0.794859241f,0.571358629f),new Rect(0.131925314f,0.209550046f,0.736761555f,0.577226814f),new Rect(0.074074074f,0.139547130f,0.851383769f,0.670879410f)};
        static readonly float[] DespesasBotoesProporcoes={3.712511939f,4.361596958f,4.517718529f,4.173521851f,3.829144130f,3.807169021f};
        bool DespesaArteBotao(Rect area,int categoria,bool pago,int total)
        {
            int indice=pago?5:categoria;
            if(despesasBotoesArte[indice]==null)despesasBotoesArte[indice]=Resources.Load<Texture2D>("UI/"+DespesasBotoesRecursos[indice]);
            Texture2D imagem=despesasBotoesArte[indice];
            if(imagem==null)return GestaoPrecoBotao(area,pago?"TÁ PAGO":FormatarDinheiro(total),pago,!pago);
            float proporcao=DespesasBotoesProporcoes[indice];
            float largura=Mathf.Min(area.width,area.height*proporcao),altura=largura/proporcao;
            Rect desenho=new Rect(area.center.x-largura/2,area.center.y-altura/2,largura,altura);
            InterfaceCena.DesenharTexturaComCoordenadas(desenho,imagem,DespesasBotoesUv[indice]);
            bool anterior=InterfaceCena.habilitado;InterfaceCena.habilitado=anterior&&!pago;
            bool pressionado=InterfaceCena.Botao(desenho,new GUIContent("",pago?EconomiaJogo.DespesaNomes[categoria]+": tá pago":"Pagar "+EconomiaJogo.DespesaNomes[categoria]+" por "+FormatarDinheiro(total)),GUIStyle.none);
            InterfaceCena.habilitado=anterior;
            if(pressionado)Toque();return pressionado;
        }
        void DesenharVoltarDespesas()
        {
            if(BotaoVoltarIlustrado(new Rect(22,867,275,53),new GUIContent("","Voltar à gestão"),GUIStyle.none)){Toque();aba=8;}
        }
        readonly Texture2D[] publicidadeBotoesArte=new Texture2D[5];
        static readonly string[] PublicidadeBotoesRecursos={"PublicidadeInfluencer","PublicidadeDrop","PublicidadeOutdoors","PublicidadeShows","PublicidadeContratado"};
        static readonly Rect[] PublicidadeBotoesUv={new Rect(0.109731544f,0.185009862f,0.777852349f,0.583431953f),new Rect(0.192393404f,0.176470588f,0.615956024f,0.606506239f),new Rect(0.195525445f,0.196638655f,0.608949110f,0.557142857f),new Rect(0.178131036f,0.171568627f,0.642995097f,0.604723708f),new Rect(0.010609172f,0.100102669f,0.973648186f,0.773100616f)};
        static readonly float[] PublicidadeBotoesProporcoes={3.134550372f,3.046289493f,2.914781297f,3.189388357f,3.778220452f};
        bool PublicidadeArteBotao(Rect area,int campanha,bool ativo)
        {
            int indice=ativo?4:campanha;
            if(publicidadeBotoesArte[indice]==null)publicidadeBotoesArte[indice]=Resources.Load<Texture2D>("UI/"+PublicidadeBotoesRecursos[indice]);
            Texture2D arte=publicidadeBotoesArte[indice];
            if(arte==null)return GestaoPrecoBotao(area,ativo?"CONTRATADO":FormatarDinheiro(EconomiaJogo.PublicidadeCustos[campanha]),ativo,!ativo,ativo?27:32);
            float largura=Mathf.Min(area.width,area.height*PublicidadeBotoesProporcoes[indice]),altura=largura/PublicidadeBotoesProporcoes[indice];
            Rect desenho=new Rect(area.center.x-largura/2,area.center.y-altura/2,largura,altura);
            InterfaceCena.DesenharTexturaComCoordenadas(desenho,arte,PublicidadeBotoesUv[indice]);
            bool anterior=InterfaceCena.habilitado;InterfaceCena.habilitado=anterior&&!ativo;
            string dica=ativo?EconomiaJogo.PublicidadeNomes[campanha]+": contratado":"Contratar "+EconomiaJogo.PublicidadeNomes[campanha]+" por "+FormatarDinheiro(EconomiaJogo.PublicidadeCustos[campanha]);
            bool pressionado=InterfaceCena.Botao(desenho,new GUIContent("",dica),GUIStyle.none);
            InterfaceCena.habilitado=anterior;if(pressionado)Toque();return pressionado;
        }
    }
}
