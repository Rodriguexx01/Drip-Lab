using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        bool tutorialAtivo;
        int etapaTutorial;
        Texture2D tutorialPecaPronta;

        sealed class PassoTutorial
        {
            public readonly int pagina, investimento;
            public readonly string titulo, explicacao;
            public readonly Rect alvo, balao;
            public PassoTutorial(int pagina, string titulo, string explicacao, Rect alvo, Rect balao,int investimento=0)
            { this.pagina=pagina; this.titulo=titulo; this.explicacao=explicacao; this.alvo=alvo; this.balao=balao; this.investimento=investimento; }
        }

        static readonly PassoTutorial[] PassosTutorial = {
            new PassoTutorial(-1,"SEU PRIMEIRO CORRE",
                "Faça a DRIP LAB chegar a 100 de fama para vencer. O capital fica no canto superior esquerdo e a fama nesta barra.\n\nEste guia mostra onde ir e em que ordem agir. Nada será comprado e o relógio ficará pausado até você terminar.",
                new Rect(480,12,420,101),new Rect(485,235,700,390)),
            new PassoTutorial(-1,"ABRA O TABLET",
                "Clique no tablet sobre a bancada. Em GESTÃO, você paga as despesas, compra tecidos e fabrica peças. PEDIDOS reúne as encomendas; NOTÍCIAS traz novidades.\n\nUse AVANÇAR para conhecer as telas. Ao terminar, quite as despesas para liberar as ações da loja.",
                new Rect(1270,410,380,260),new Rect(480,330,650,340)),
            new PassoTutorial(8,"PAGUE AS DESPESAS",
                "Em GESTÃO > DESPESAS, clique no valor de cada uma das cinco contas. O campo muda para TÁ PAGO. Depois do guia, essa tela abre automaticamente; quite tudo para liberar a loja.\n\nAs contas somam R$ 1.500 e são cobradas uma vez por partida. Dos R$ 2.500 iniciais, sobram R$ 1.000 para comprar tecido e fabricar.",
                new Rect(463,782,744,117),new Rect(35,300,405,420)),
            new PassoTutorial(8,"COMPRE TECIDOS",
                "Depois das contas, abra TECIDOS. Cada clique no preço compra 1 metro, suficiente para 10 peças. Escolha o material e confira o saldo antes de repetir a compra.\n\nO tecido altera o custo e o preço final das peças. Reserve capital para fabricar o lote.",
                new Rect(463,326,744,120),new Rect(35,310,405,420)),
            new PassoTutorial(-3,"NEGOCIE ANTES DE COMPRAR",
                "No catálogo de TECIDOS, NEGOCIAR abre esta roleta. Clique em GIRAR: há 50% de chance de SIM e 50% de NÃO.\n\nSIM dá 10% de desconto naquele tecido, válido apenas no dia atual; NÃO mantém o preço. Há uma tentativa por tipo de tecido por dia. Volte ao catálogo para comprar: negociar não compra material.",
                new Rect(618,820,433,100),new Rect(1145,300,485,450)),
            new PassoTutorial(8,"ENVIE PARA CONFECÇÃO",
                "Com tecido disponível, entre em CONFECÇÃO. Escolha a peça e o tecido; ajuste o lote com − e +, em múltiplos de 10. Confira consumo, custo e preço de venda antes de FABRICAR.\n\nO lote fica pronto no próximo dia. Volte à bancada e encerre o dia para recebê-lo.",
                new Rect(463,482,744,116),new Rect(35,310,405,420)),
            new PassoTutorial(-1,"QUANDO AVANÇAR O DIA",
                "A loja abre às 12h e fecha às 22h. Clique no relógio para encerrar antes, quando concluir suas tarefas.\n\nO resumo mostra lucro, custos e peças vendidas. PRÓXIMO DIA entrega a produção e movimenta as vendas dos drops publicados.",
                new Rect(1270,12,390,101),new Rect(650,200,740,370)),
            new PassoTutorial(2,"LANCE SUAS PEÇAS NO SITE",
                "Quando o lote estiver pronto, abra SITE pela bancada. LANÇAR publica as peças e mostra o preço calculado; SEM ESTOQUE pede nova produção e LANÇADO indica que o anúncio já está ativo.\n\nDepois, avance o dia para receber pedidos. Publicidade pode aumentar a procura. Cabem até 8 lançamentos ativos.",
                new Rect(318,455,279,51),new Rect(600,245,680,390)),
            new PassoTutorial(2,"ONDE ENTRAR NAS COLLABS",
                "No SITE, DROP COLLAB abre as peças especiais. Escolha uma, confira a taxa e confirme para publicar 10 unidades por 3 dias, sem comprar tecido nem fabricar.\n\nA taxa sai do capital na hora; as vendas acontecem depois, conforme a procura. As sobras voltam ao estoque.",
                new Rect(1180,200,240,657),new Rect(335,300,770,360)),
            new PassoTutorial(4,"CONTRATE PUBLICIDADE",
                "Abra GESTÃO > PUBLICIDADE. Compare o preço e a duração de cada campanha e clique no valor para contratar.\n\nA contratação desconta o capital imediatamente, dá fama e ajuda a aumentar a procura enquanto a campanha estiver ativa. CONTRATADO impede contratar a mesma campanha novamente antes de ela terminar.",
                new Rect(926,273,233,584),new Rect(50,300,800,390)),
            new PassoTutorial(0,"ATENDA OS PEDIDOS",
                "Quando houver vendas, abra PEDIDOS no tablet. Escolha o modelo, confira cliente e prazo, selecione a encomenda e clique em ACEITAR para levar a peça à bancada.\n\nEntregue a tempo: após mais de três dias de atraso, o pedido é cancelado e você perde 3 de fama.",
                new Rect(430,353,826,150),new Rect(430,535,826,360)),
            new PassoTutorial(-2,"DOBRE, EMBALE E ENTREGUE",
                "Após aceitar, clique na peça: duas vezes para dobrar roupas, uma vez para enrolar o gorro. Calçados não precisam de dobras. No estágio final aparece CAIXAS, embaixo e ao centro.\n\nEscolha a cor da caixa e clique em ENTREGAR PEDIDO. Só então você recebe o pagamento e o pedido sai da lista. Calçados já estão prontos para embalar.",
                new Rect(569,761,534,142),new Rect(32,290,480,430)),
            new PassoTutorial(-6,"ESCOLHA A COR DA CAIXA",
                "CAIXAS abre a escolha da embalagem. Combine a cor com a preferência do cliente para ganhar mais fama. A vermelha custa R$ 5; as outras, R$ 7.\n\nDepois clique em ENTREGAR PEDIDO. O pagamento entra com o custo da caixa descontado, e o pedido sai da lista. Você pode trocar a caixa antes de entregar.",
                new Rect(598,460,201,60),new Rect(375,35,920,320)),
            new PassoTutorial(6,"ESCOLHA O INVESTIMENTO",
                "O botão EXTRAS fica na bancada, abaixo de SITE. APOSTA abre a roleta; CRIPTOMOEDA abre a negociação de DRIP COIN.\n\nAmbos usam o capital da loja e são opcionais. Mantenha uma reserva para tecido e produção: você pode vencer só com as vendas.",
                new Rect(299,743,1076,117),new Rect(430,300,810,340)),
            new PassoTutorial(6,"COMO APOSTAR NA ROLETA",
                "Escolha a ficha de R$ 10, R$ 50 ou R$ 100 e ajuste a quantidade com − e +. O total pode chegar a R$ 500. Escolha VERMELHO, PRETO ou ZERO e clique em GIRAR.\n\nO capital muda somente ao terminar o giro. Se acertar, vermelho e preto retornam 2× o total apostado; zero retorna 3×. Se errar, perde a aposta. Os retornos incluem o valor apostado.",
                new Rect(1170,320,425,500),new Rect(80,300,970,430),1),
            new PassoTutorial(6,"COMPRAR E VENDER DRIP COIN",
                "Use − e + para escolher a quantidade e confira TOTAL. COMPRAR troca capital por moedas; VENDER troca as moedas da carteira por capital, sempre na cotação atual. O preço pode cair.\n\nA cotação muda durante o jogo; o gráfico mostra o histórico disponível. 1H, 1D e 1S exibem a última hora, 24 horas e 7 dias. Trocar o período não negocia moedas.",
                new Rect(1190,530,405,310),new Rect(100,280,950,410),2),
            new PassoTutorial(-1,"PRONTO PARA COMEÇAR",
                "Seu ciclo é: pagar as despesas, comprar tecido, fabricar, avançar o dia, lançar no site e entregar pedidos. Chegue a 100 de fama para vencer.\n\nX fecha o tablet; Esc volta ou abre o menu. Em OPÇÕES DE ÁUDIO você ajusta o som. Consulte NOTÍCIAS e reveja este guia pelo botão ? no canto superior esquerdo da tela inicial do tablet.",
                new Rect(24,241,340,114),new Rect(480,280,740,410))
        };

        void IniciarTutorial()
        {
            etapaTutorial=0;tutorialAtivo=true;ajuda=false;tabletAberto=false;
        }

        void ConcluirTutorial()
        {
            tutorialAtivo=false;tabletAberto=false;aba=0;
            ExibirAvisoDespesasIniciais();
        }

        void ExibirAvisoDespesasIniciais()
        {
            if(!economia.ConsumirAvisoDespesas())return;
            avisoDespesasAberto=true;quadroAberturaAviso=Time.frameCount;TocarEfeito(somNotificacao,.85f);
            tecidoAvisos.Clear();
        }

        void DesenharTutorial()
        {
            var passo=PassosTutorial[etapaTutorial];
            // Renderiza as telas reais sem executar compras, navegação ou avanço do dia.
            bool habilitado=InterfaceCena.habilitado;int abaAnterior=aba;bool tabletAnterior=tabletAberto;
            int investimentoAnterior=paginaInvestimentos,apostaAnterior=aposta;
            int tecidoAnterior=tecidoSelecionado,produtoAnterior=produtoConfeccao,colaboracaoAnterior=colaboracaoSelecao,pedidoAnterior=produtoPedido;
            int loteAnterior=quantidadeLote,materialAnterior=tecidoConfeccao,paginaProdutosAnterior=pedidoProdutosPagina;
            bool despesasAnterior=despesasPainel;
            string negociacaoMensagemAnterior=tecidoRoletaMensagem;
            InterfaceCena.habilitado=false;
            try
            {
                aba=passo.pagina;tabletAberto=true; // Oculta uma dobra em andamento durante a revisão do guia.
                tecidoSelecionado=-1;produtoConfeccao=-1;colaboracaoSelecao=-1;produtoPedido=-1;despesasPainel=false;
                if(passo.pagina==6)paginaInvestimentos=passo.investimento;
                InterfaceCena.DefinirTela(passo.pagina<0?"Atelie":TabletHierarquiaTela());
                switch(passo.pagina)
                {
                    case -2: DesenharExemploEmbalagemTutorial();break;
                    case -3: InterfaceCena.DefinirTela("Tablet/Tecidos");aba=9;tecidoSelecionado=0;tecidoRoletaMensagem="Exemplo: tente o desconto antes de comprar.";TecidoPagina();break;
                    case -4: InterfaceCena.DefinirTela("Tablet/Confeccao");aba=1;produtoConfeccao=0;quantidadeLote=10;tecidoConfeccao=0;ProducaoPagina();break;
                    case -5: InterfaceCena.DefinirTela("Tablet/Collabs");aba=10;colaboracaoSelecao=0;ColaboracaoPagina();break;
                    case -6: DesenharArteEstaticaTutorial("UI/BoxSelection");break;
                    case -7: DesenharEntregaTutorial();break;
                    case -8: DesenharArteEstaticaTutorial("UI/DailySummary");break;
                    case -9: InterfaceCena.DefinirTela("Audio");DesenharOpcoesAudio();break;
                    default: if(passo.pagina<0)DesenharAtelie();else DesenharTablet();break;
                }
                if(passo.pagina>=0)DesenharEstoqueTablet();
            }
            finally
            {
                aba=abaAnterior;tabletAberto=tabletAnterior;paginaInvestimentos=investimentoAnterior;aposta=apostaAnterior;
                tecidoSelecionado=tecidoAnterior;produtoConfeccao=produtoAnterior;colaboracaoSelecao=colaboracaoAnterior;produtoPedido=pedidoAnterior;
                quantidadeLote=loteAnterior;tecidoConfeccao=materialAnterior;tecidoRoletaMensagem=negociacaoMensagemAnterior;
                despesasPainel=despesasAnterior;pedidoProdutosPagina=paginaProdutosAnterior;
                InterfaceCena.habilitado=habilitado;
            }

            InterfaceCena.DefinirTela("Tutorial");
            Matrix4x4 anterior=IniciarReferenciaTela(null,limparFundo:false);
            Rect alvo=passo.alvo,balao=passo.balao;
            Color sombra=new Color(0,0,0,.62f);
            Retangulo(0,0,1672,alvo.y,sombra);
            Retangulo(0,alvo.yMax,1672,941-alvo.yMax,sombra);
            Retangulo(0,alvo.y,alvo.x,alvo.height,sombra);
            Retangulo(alvo.xMax,alvo.y,1672-alvo.xMax,alvo.height,sombra);
            MolduraTutorial(alvo,Amarelo,4);
            DesenharLigacaoTutorial(balao,alvo);
            Retangulo(balao.x+7,balao.y+7,balao.width,balao.height,Color.black);
            Retangulo(balao.x,balao.y,balao.width,balao.height,Painel);
            MolduraTutorial(balao,Amarelo,3);
            Retangulo(balao.x+3,balao.y+3,balao.width-6,63,Azul);
            Texto((etapaTutorial+1)+" / "+PassosTutorial.Length+"  •  "+passo.titulo,balao.x+18,balao.y+12,balao.width-36,42,23,Creme,true,TextAnchor.MiddleLeft);
            Texto(passo.explicacao,balao.x+22,balao.y+85,balao.width-44,balao.height-170,23,Creme);
            float largura=(balao.width-56)/2;
            bool voltar=UrbanoBotao("VOLTAR",new Rect(balao.x+20,balao.yMax-65,largura,46),false,etapaTutorial>0,20);
            bool avancar=UrbanoBotao(etapaTutorial==PassosTutorial.Length-1?"COMEÇAR":"AVANÇAR",new Rect(balao.x+36+largura,balao.yMax-65,largura,46),true,true,20);
            bool pular=UrbanoBotao("PULAR TUTORIAL",new Rect(1360,866,282,48),false,true,20);
            if(Event.current.type==EventType.KeyDown)
            {
                if(Event.current.keyCode==KeyCode.RightArrow||Event.current.keyCode==KeyCode.Return){avancar=true;Toque();Event.current.Use();}
                else if(Event.current.keyCode==KeyCode.LeftArrow){voltar=true;Toque();Event.current.Use();}
                else if(Event.current.keyCode==KeyCode.Escape)Event.current.Use();
            }
            if(pular)ConcluirTutorial();
            else if(avancar){if(etapaTutorial<PassosTutorial.Length-1)etapaTutorial++;else ConcluirTutorial();}
            else if(voltar&&etapaTutorial>0)etapaTutorial--;
            InterfaceCena.matriz=anterior;
        }

        void DesenharExemploEmbalagemTutorial()
        {
            ConfigurarBancada();
            if(tutorialPecaPronta==null)tutorialPecaPronta=Resources.Load<Texture2D>("Bench/UrbanPulseFold2");
            Matrix4x4 anterior=IniciarReferenciaTela(tutorialPecaPronta!=null?tutorialPecaPronta:atelieArte);
            DesenharDobraPainel();
            // Apenas uma demonstração visual: não cria nem entrega um pedido.
            DesenharAtalhoCaixas();
            InterfaceCena.matriz=anterior;
        }

        readonly System.Collections.Generic.Dictionary<string,Texture2D> artesExemploTutorial=new System.Collections.Generic.Dictionary<string,Texture2D>();
        Texture2D ArteExemploTutorial(string caminho)
        {
            if(!artesExemploTutorial.TryGetValue(caminho,out var arte))
            {arte=Resources.Load<Texture2D>(caminho);artesExemploTutorial[caminho]=arte;}
            return arte;
        }
        void DesenharArteEstaticaTutorial(string caminho)
        {
            Matrix4x4 anterior=IniciarReferenciaTela(ArteExemploTutorial(caminho));
            InterfaceCena.matriz=anterior;
        }
        void DesenharEntregaTutorial()
        {
            ConfigurarBancada();
            Matrix4x4 anterior=IniciarReferenciaTela(ArteExemploTutorial("Bench/PackedBox0"));
            DesenharDobraPainel();
            var botao=ArteExemploTutorial("UI/DeliverOrder");
            if(botao!=null)InterfaceCena.DesenharTextura(new Rect(486,711,700,233.333f),botao,ScaleMode.ScaleToFit);
            UrbanoBotao("TROCAR CAIXA",new Rect(30,833,354,57),false,false,20);
            InterfaceCena.matriz=anterior;
        }

        static Vector2[] CalcularLigacaoTutorial(Rect balao,Rect alvo)
        {
            // Conecta as bordas por segmentos retos, sempre fora dos dois retângulos.
            if(balao.xMax<=alvo.x||alvo.xMax<=balao.x)
            {
                bool direita=balao.xMax<=alvo.x;
                float y= Mathf.Clamp(alvo.center.y,balao.y+20,balao.yMax-20);
                Vector2 origem=new Vector2(direita?balao.xMax:balao.x,y);
                Vector2 ponta=new Vector2(direita?alvo.x:alvo.xMax,Mathf.Clamp(y,alvo.y+8,alvo.yMax-8));
                float meio=(origem.x+ponta.x)/2;
                return new[]{origem,new Vector2(meio,origem.y),new Vector2(meio,ponta.y),ponta};
            }
            bool abaixo=balao.yMax<=alvo.y;
            float x=Mathf.Clamp(alvo.center.x,balao.x+20,balao.xMax-20);
            Vector2 inicio=new Vector2(x,abaixo?balao.yMax:balao.y);
            Vector2 fim=new Vector2(Mathf.Clamp(x,alvo.x+8,alvo.xMax-8),abaixo?alvo.y:alvo.yMax);
            float metade=(inicio.y+fim.y)/2;
            return new[]{inicio,new Vector2(inicio.x,metade),new Vector2(fim.x,metade),fim};
        }
        void DesenharLigacaoTutorial(Rect balao,Rect alvo)
        {
            Vector2[] pontos=CalcularLigacaoTutorial(balao,alvo);
            for(int i=1;i<pontos.Length;i++)
            {
                Vector2 a=pontos[i-1],b=pontos[i];
                if(Mathf.Abs(a.x-b.x)<.1f)Retangulo(a.x-2,Mathf.Min(a.y,b.y),4,Mathf.Abs(b.y-a.y),Amarelo);
                else Retangulo(Mathf.Min(a.x,b.x),a.y-2,Mathf.Abs(b.x-a.x),4,Amarelo);
            }
            Vector2 ponta=pontos[pontos.Length-1];
            Retangulo(ponta.x-5,ponta.y-5,10,10,Amarelo);
        }

        void MolduraTutorial(Rect area,Color cor,float espessura)
        {
            Retangulo(area.x,area.y,area.width,espessura,cor);
            Retangulo(area.x,area.yMax-espessura,area.width,espessura,cor);
            Retangulo(area.x,area.y,espessura,area.height,cor);
            Retangulo(area.xMax-espessura,area.y,espessura,area.height,cor);
        }
        bool avisoDespesasAberto;
        int quadroAberturaAviso;
        Texture2D avisoDespesasArte;
        void DesenharAvisoDespesasObrigatorias()
        {
            menuAberto=false;tabletAberto=false;
            InterfaceCena.habilitado=false;InterfaceCena.DefinirTela("Atelie");DesenharAtelie();InterfaceCena.habilitado=true;
            InterfaceCena.DefinirTela("Aviso/DespesasIniciais");
            Matrix4x4 anterior=IniciarReferenciaTela(null,false);
            Retangulo(0,0,1672,941,new Color(0,0,0,.72f));
            if(avisoDespesasArte==null)avisoDespesasArte=Resources.Load<Texture2D>("UI/AvisoDespesasIniciais");
            Rect area=new Rect(186,235,1300,433.3333f);
            if(avisoDespesasArte!=null)InterfaceCena.DesenharTextura(area,avisoDespesasArte,ScaleMode.ScaleToFit);
            else { UrbanoPainel(area,0);Texto("Você deve pagar as despesas antes de jogar.",area.x+70,area.y+100,1160,220,48,Creme,true,TextAnchor.MiddleCenter); }
            Texto("CLIQUE NO AVISO PARA IR ÀS DESPESAS",336,696,1000,50,24,Creme,true,TextAnchor.MiddleCenter);
            bool cliqueMouse=Event.current.type==EventType.MouseUp&&Event.current.button==0;
            bool pressionado=InterfaceCena.Botao(area,new GUIContent("","Ir pagar as despesas"),GUIStyle.none);
            if(pressionado&&cliqueMouse&&Time.frameCount>quadroAberturaAviso){avisoDespesasAberto=false;Toque();}
            if(Event.current.type==EventType.KeyDown)Event.current.Use();
            InterfaceCena.matriz=anterior;
        }
    }
}
