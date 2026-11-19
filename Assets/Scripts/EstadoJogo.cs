using System;
using System.Collections.Generic;


namespace DripLab
{
    public static class MatematicaJogo
    {
        public static float Limitar(float valor,float minimo,float maximo) {return Math.Max(minimo,Math.Min(maximo,valor));}
        public static int Limitar(int valor,int minimo,int maximo) {return Math.Max(minimo,Math.Min(maximo,valor));}
        public static int ArredondarParaInteiro(float valor) {return (int)Math.Round(valor,MidpointRounding.ToEven);}
    }
    [Serializable] public class Producao { public int tipo, quantidade, diaEntrega, tipoTecido=-1, precoVenda; }
    [Serializable] public class Colecao
    {
        public int id, tipo, restante, preco, fimDia, vendidas;
        public string nome;
        public bool limitado;
    }
    [Serializable] public class Pedido
    {
        public int id, tipo, preco, dia, caixa, adesivo;
        public string cliente;
    }
    [Serializable] public class Conta { public int id, valor, vencimentoDia, categoria=-1, periodo; public bool pago; }
    [Serializable] public class PontoCotacaoCripto { public float minuto,preco; }
    [Serializable] public class Campanha { public string nome; public int forca, fimDia; }
    [Serializable] public class EstadoJogo
    {
        public int receitaDia,custosDia,entregasDia,diaResumo=1,lucroResumo,custosResumo,entregasResumo; public bool noticiaDropPublicada; public int versao = 1, dia = 1, dinheiro = 2500, divida, tecido, fama, entregues, proximoId = 1;
        public int[] estoque = new int[8];
        public int[] valorVendaEstoque = new int[8];
        public int[] metrosTecidos = new int[4];
        public int despesaEstruturaVersao;
        public bool despesasIniciaisCriadas,avisoDespesasExibido;
        public int reiniciarRevisao;
        public int[] tecidoNegociacaoDia=new int[4], tecidoDescontoDia=new int[4];
        public List<Producao> producao = new List<Producao>();
        public List<Colecao> drops = new List<Colecao>();
        public List<Pedido> pedidos = new List<Pedido>();
        public List<Conta> contas = new List<Conta>();
        public List<Campanha> campanhas = new List<Campanha>();
        public List<string> noticias = new List<string>();
        public float precoMoeda = 100, moedas;
        public List<float> historicoCotacoes = new List<float> {100}; public List<PontoCotacaoCripto> cotacoesTempoReal=new List<PontoCotacaoCripto>(); public int sementeCotacao=42719;
        public int sementeAleatoria = 91373, demandaEvento = 0;
        public string tituloEvento = "Sua marca começa aqui", final = "";
        public string ultimoResumo = "Compre tecido, produza seu primeiro lote e publique um drop. Depois avance o dia para atrair pedidos.";
        public bool introducaoVista;
    }

    // Rules and save data are separate from the visual presentation.
    public class EconomiaJogo
    {
        public EstadoJogo estado;
        public static readonly string[] Produtos = {"Camisa Urban Pulse", "Camisa Asfalto Crew", "Calça Cargo Eclipse", "Calça Concreto Flex", "Brasa 90", "Paris Noturna", "Onda Urbana", "Vermelhinho"};
        public const int QuantidadeProdutosComuns=4, UnidadesColaboracao=10, LimiteAnuncios=8;
        public const int LimitePedidos=10,MultiplicadorZero=3;
        public static readonly int[] TaxasColaboracoes={900,700,350,1200};
        public const int PecasPorMetro=10, LimiteLoteConfeccao=1000;
        public static readonly int[] CustosConfeccao = {22, 36, 28, 36};
        public static readonly int[] PrecosBase = {89,79,149,139,159,129,79,179};
        public static readonly string[] CatalogoTecidos = {"100% algodão", "Lã peruana", "Tricoline", "Linho"};
        public static readonly int[] PrecosTecidos = {60, 80, 10, 20};
        public static readonly string[] PublicidadeNomes={"Influencer","Drop Collab","Outdoors","Brinde em shows"};
        public static readonly int[] PublicidadeCustos={800,500,200,2000}, PublicidadeDias={1,1,5,3}, PublicidadeFama={10,5,2,25};
        public static readonly string[] DespesaNomes={"Wi-Fi","Água","Mercado","Aluguel","Energia"};
        public static readonly int[] DespesaCustos={150,120,330,700,200};
        public static readonly string[] Caixas = {"Vermelha", "Verde", "Azul", "Laranja"};
        public static readonly string[] Nomes = {"Bia", "Luan", "Rafa", "Yasmin", "Caio", "Manu", "Davi", "Lara", "Gui", "Júlia", "Pedro", "Nina"};
        public EconomiaJogo(EstadoJogo dados = null)
        {
            estado = dados ?? new EstadoJogo();
            estado.divida=0; // Kept only for compatibility with older saves; never payable.
            if(estado.estoque==null)estado.estoque=new int[Produtos.Length];
            else if(estado.estoque.Length<Produtos.Length)Array.Resize(ref estado.estoque,Produtos.Length);
            if(estado.valorVendaEstoque==null)estado.valorVendaEstoque=new int[Produtos.Length];
            else if(estado.valorVendaEstoque.Length<Produtos.Length)Array.Resize(ref estado.valorVendaEstoque,Produtos.Length);
            for(int i=0;i<Produtos.Length;i++)if(estado.estoque[i]>0&&estado.valorVendaEstoque[i]<=0)estado.valorVendaEstoque[i]=estado.estoque[i]*PrecosBase[i];
            if(estado.despesaEstruturaVersao<1){foreach(var conta in estado.contas)conta.categoria=-1;estado.despesaEstruturaVersao=1;}
            if(estado.metrosTecidos==null)estado.metrosTecidos=new int[4];
            else if(estado.metrosTecidos.Length!=4)Array.Resize(ref estado.metrosTecidos,4);
            GarantirContasIniciais();
            VerificarVitoria();
        }
        void GarantirContasIniciais()
        {
            if(estado.despesasIniciaisCriadas)return;
            for(int categoria=0;categoria<DespesaNomes.Length;categoria++)
            {
                int indice=categoria;
                Conta inicial=estado.contas.Find(b=>b.categoria==indice&&b.pago)??estado.contas.Find(b=>b.categoria==indice);
                estado.contas.RemoveAll(b=>b.categoria==indice&&b!=inicial);
                if(inicial==null)estado.contas.Add(new Conta{id=estado.proximoId++,categoria=categoria,periodo=0,valor=DespesaCustos[categoria],vencimentoDia=8});
                else {inicial.periodo=0;inicial.valor=DespesaCustos[categoria];}
            }
            estado.despesasIniciaisCriadas=true;
        }
        public bool DespesasIniciaisPendentes { get { return estado.contas.Exists(b=>b.categoria>=0&&b.categoria<DespesaNomes.Length&&!b.pago); } }
        public bool ConsumirAvisoDespesas()
        {
            if(estado.avisoDespesasExibido)return false;
            estado.avisoDespesasExibido=true;
            return estado.contas.Exists(b=>!b.pago);
        }
        public int PecasDisponiveis(int tipo)
        {
            if(tipo<0||tipo>=Produtos.Length)return 0;
            int quantidade=estado.estoque[tipo];
            foreach(var colecao in estado.drops)if(colecao.tipo==tipo)quantidade+=colecao.restante;
            return quantidade;
        }
        public bool PublicidadeAtivo(int indice){return indice>=0&&indice<PublicidadeNomes.Length&&estado.campanhas.Exists(c=>c.nome==PublicidadeNomes[indice]&&c.fimDia>estado.dia);}
        public int Sortear(int minimo, int maximo)
        {
            estado.sementeAleatoria = (int)(((long)estado.sementeAleatoria * 48271) % 2147483647);
            if (estado.sementeAleatoria <= 0) estado.sementeAleatoria = 91373;
            return minimo + estado.sementeAleatoria % (maximo - minimo);
        }
        public void RegistrarNoticia(string texto)
        {
            estado.noticias.Insert(0, "D" + estado.dia.ToString("00") + " · " + texto);
            if (estado.noticias.Count > 40) estado.noticias.RemoveAt(estado.noticias.Count - 1);
        }
        bool Ativo() { return estado.final == ""; }
        public string ComprarTecido(int quantidade)
        {
            if (!Ativo()) return "Esta jornada terminou.";
            if (quantidade < 1 || quantidade > 100 || estado.dinheiro < quantidade * 18) return "Saldo insuficiente para esse tecido.";
            estado.dinheiro -= quantidade * 18;estado.custosDia+=quantidade*18; estado.tecido += quantidade;estado.metrosTecidos[0]+=quantidade;
            RegistrarNoticia("Chegaram " + quantidade + " unidades de tecido.");
            return "Tecido recebido! Agora envie um lote à gráfica.";
        }
        public static int PrecoVendaPorTecido(int tipo,int tipoTecido)
        {
            if(tipo<0||tipo>=QuantidadeProdutosComuns||tipoTecido<0||tipoTecido>=PrecosTecidos.Length)return 0;
            return MatematicaJogo.ArredondarParaInteiro(PrecosBase[tipo]*(.5f+.5f*PrecosTecidos[tipoTecido]/60f));
        }
        public int PrecoVendaEstoque(int tipo)
        {
            if(tipo<0||tipo>=Produtos.Length)return 0;
            return estado.estoque[tipo]>0&&estado.valorVendaEstoque[tipo]>0?MatematicaJogo.ArredondarParaInteiro((float)estado.valorVendaEstoque[tipo]/estado.estoque[tipo]):PrecosBase[tipo];
        }
        void AdicionarEstoquePronto(int tipo,int quantidade,int precoVenda)
        {
            if(estado.estoque[tipo]>0&&estado.valorVendaEstoque[tipo]<=0)estado.valorVendaEstoque[tipo]=estado.estoque[tipo]*PrecosBase[tipo];
            estado.estoque[tipo]+=quantidade;estado.valorVendaEstoque[tipo]+=quantidade*precoVenda;
        }
        void RemoverEstoquePronto(int tipo,int quantidade)
        {
            int valorAntes=estado.estoque[tipo];
            int total=estado.valorVendaEstoque[tipo]>0?estado.valorVendaEstoque[tipo]:valorAntes*PrecosBase[tipo];
            estado.estoque[tipo]-=quantidade;
            estado.valorVendaEstoque[tipo]=valorAntes<=0||estado.estoque[tipo]==0?0:MatematicaJogo.ArredondarParaInteiro((float)total*estado.estoque[tipo]/valorAntes);
        }
        public string Fabricar(int tipo, int quantidade,int escolhidoTecido=-1)
        {
            if (!Ativo()) return "Esta jornada terminou.";
            if (tipo < 0 || tipo >= QuantidadeProdutosComuns || quantidade < PecasPorMetro || quantidade > LimiteLoteConfeccao || quantidade % PecasPorMetro != 0) return "Escolha de 10 a 1.000 peças, em múltiplos de 10.";
            int tecido = quantidade / PecasPorMetro, custo = CustosConfeccao[tipo] * quantidade;
            if (estado.tecido < tecido) return "Falta tecido para produzir esse lote.";
            if (estado.dinheiro < custo) return "Saldo insuficiente para pagar a gráfica.";
            if(escolhidoTecido < -1 || escolhidoTecido>=PrecosTecidos.Length)return "Tecido inválido.";
            if(escolhidoTecido>=0&&estado.metrosTecidos[escolhidoTecido]<tecido)return "Falta esse tecido para produzir o lote.";
            estado.tecido -= tecido; estado.dinheiro -= custo;estado.custosDia+=custo;
            int paraUsar=tecido;
            for(int i=0;i<estado.metrosTecidos.Length&&paraUsar>0;i++)
            {
                if(escolhidoTecido>=0&&escolhidoTecido!=i)continue;
                int utilizada=Math.Min(paraUsar,estado.metrosTecidos[i]);if(utilizada<=0)continue;
                estado.metrosTecidos[i]-=utilizada;paraUsar-=utilizada;
                estado.producao.Add(new Producao{tipo=tipo,quantidade=utilizada*PecasPorMetro,diaEntrega=estado.dia+1,tipoTecido=i,precoVenda=PrecoVendaPorTecido(tipo,i)});
            }
            if(paraUsar>0)estado.producao.Add(new Producao{tipo=tipo,quantidade=paraUsar*PecasPorMetro,diaEntrega=estado.dia+1,tipoTecido=0,precoVenda=PrecoVendaPorTecido(tipo,0)});
            RegistrarNoticia(quantidade + " peças de " + Produtos[tipo] + " enviadas à confecção. Entrega amanhã.");
            return quantidade + " peças de " + Produtos[tipo] + " em produção. Entrega no dia " + (estado.dia+1) + ".";
        }
        public string ComprarTecidoCatalogo(int tipo,int metros)
        {
            if(!Ativo())return "Esta jornada terminou.";
            if(tipo<0||tipo>=PrecosTecidos.Length||metros<1||metros>100)return "Escolha de 1 a 100 metros de um tecido válido.";
            int custo=ObterPrecoTecido(tipo)*metros;
            if(estado.dinheiro<custo)return "Saldo insuficiente para essa compra.";
            estado.dinheiro-=custo;estado.custosDia+=custo;estado.tecido+=metros;estado.metrosTecidos[tipo]+=metros;
            RegistrarNoticia("Compra de "+metros+" m de "+CatalogoTecidos[tipo]+" por R$ "+custo+".");
            return "Compra concluída: "+metros+" m de "+CatalogoTecidos[tipo]+" adicionados ao estoque.";
        }
        void GarantirNegociacoesTecidos()
        {
            if(estado.tecidoNegociacaoDia==null||estado.tecidoNegociacaoDia.Length!=4)estado.tecidoNegociacaoDia=new int[4];
            if(estado.tecidoDescontoDia==null||estado.tecidoDescontoDia.Length!=4)estado.tecidoDescontoDia=new int[4];
        }
        public int ObterPrecoTecido(int tipo)
        {
            GarantirNegociacoesTecidos();
            if(tipo<0||tipo>=4)return 0;
            return estado.tecidoDescontoDia[tipo]==estado.dia?PrecosTecidos[tipo]*9/10:PrecosTecidos[tipo];
        }
        public bool NegociacaoTecidoUtilizada(int tipo)
        {
            GarantirNegociacoesTecidos();
            return tipo>=0&&tipo<4&&estado.tecidoNegociacaoDia[tipo]==estado.dia;
        }
        public bool TentarNegociarTecido(int tipo,out bool aceitou)
        {
            aceitou=false;GarantirNegociacoesTecidos();
            if(!Ativo()||tipo<0||tipo>=4||NegociacaoTecidoUtilizada(tipo))return false;
            estado.tecidoNegociacaoDia[tipo]=estado.dia;
            aceitou=Sortear(0,2)==0;
            if(aceitou)estado.tecidoDescontoDia[tipo]=estado.dia;
            RegistrarNoticia("Negociação de "+CatalogoTecidos[tipo]+": "+(aceitou?"SIM! Desconto de 10% válido hoje.":"NÃO. Preço mantido."));
            return true;
        }
        public bool ProdutoEstaLancado(int tipo)
        {
            return tipo>=0&&tipo<Produtos.Length&&estado.drops.Exists(d=>d.tipo==tipo&&d.restante>0);
        }
        public bool MateriaNoticiaDisponivel(int materia){return materia==0?estado.entregues>0:materia==1&&estado.noticiaDropPublicada;}
        public string LancarProdutoPronto(int tipo)
        {
            if(!Ativo())return "Esta jornada terminou.";
            if(tipo<0||tipo>=QuantidadeProdutosComuns)return "Peça inválida.";
            if(ProdutoEstaLancado(tipo))return "Essa peça já está lançada no site.";
            return Lancar(tipo,estado.estoque[tipo],PrecoVendaEstoque(tipo),false);
        }
        public string Lancar(int tipo, int quantidade, int preco, bool limitado)
        {
            if (!Ativo()) return "Esta jornada terminou.";
            if (tipo < 0 || tipo >= Produtos.Length || quantidade < 1 || quantidade > estado.estoque[tipo]) return "Você precisa de peças prontas no estoque.";
            if (preco < 40 || preco > 300) return "Escolha um preço entre R$ 40 e R$ 300.";
            if (estado.drops.Count >= LimiteAnuncios) return "Limite de 8 lançamentos ativos. Aguarde um encerrar.";
            RemoverEstoquePronto(tipo,quantidade);
            int id = estado.proximoId++;
            estado.drops.Add(new Colecao {id=id, tipo=tipo, restante=quantidade, preco=preco, fimDia=limitado?estado.dia+3:0, limitado=limitado, nome=(limitado?"DROP ":"ESSENTIAL ")+id.ToString("00")});
            if(limitado)estado.noticiaDropPublicada=true; RegistrarNoticia("Novo lançamento no site: " + Produtos[tipo] + (limitado?" · edição limitada!":"."));
            return "Publicado! Os pedidos chegam quando você avança o dia.";
        }
        public static float ValorContabilProduto(int tipo)
        {
            return tipo<QuantidadeProdutosComuns?18f/PecasPorMetro+CustosConfeccao[tipo]:(float)TaxasColaboracoes[tipo-QuantidadeProdutosComuns]/UnidadesColaboracao;
        }
        public string LancarColaboracao(int indice)
        {
            if(!Ativo())return "Esta jornada terminou.";
            if(indice<0||indice>=TaxasColaboracoes.Length)return "Collab inválida.";
            int tipo=QuantidadeProdutosComuns+indice;
            if(ProdutoEstaLancado(tipo))return "Essa collab já está lançada.";
            if(estado.drops.Count>=LimiteAnuncios)return "Limite de 8 lançamentos ativos.";
            if(estado.dinheiro<TaxasColaboracoes[indice])return "Saldo insuficiente para lançar essa collab.";
            int reaproveitar=Math.Min(UnidadesColaboracao,estado.estoque[tipo]);
            estado.dinheiro-=TaxasColaboracoes[indice];estado.custosDia+=TaxasColaboracoes[indice];RemoverEstoquePronto(tipo,reaproveitar);
            int id=estado.proximoId++;
            estado.drops.Add(new Colecao{id=id,tipo=tipo,restante=UnidadesColaboracao,preco=PrecosBase[tipo],fimDia=estado.dia+3,limitado=true,nome="COLLAB / "+Produtos[tipo]});
            estado.noticiaDropPublicada=true; RegistrarNoticia(Produtos[tipo]+": edição especial lançada por R$ "+TaxasColaboracoes[indice]+", 10 unidades por 3 dias.");
            return Produtos[tipo]+" lançado: 10 unidades por 3 dias.";
        }
        public string ContratarPublicidade(int indice)
        {
            if (!Ativo()) return "Esta jornada terminou.";
            if (indice < 0 || indice >= PublicidadeNomes.Length) return "Campanha inválida.";
            if (PublicidadeAtivo(indice)) return "Essa campanha já está contratada.";
            if (estado.dinheiro < PublicidadeCustos[indice]) return "Saldo insuficiente para essa campanha.";
            estado.dinheiro -= PublicidadeCustos[indice];estado.custosDia+=PublicidadeCustos[indice];
            estado.campanhas.Add(new Campanha {nome=PublicidadeNomes[indice],forca=PublicidadeFama[indice],fimDia=estado.dia+PublicidadeDias[indice]});
            estado.fama=Math.Min(100,estado.fama+PublicidadeFama[indice]);
            RegistrarNoticia(PublicidadeNomes[indice]+" contratado. +"+PublicidadeFama[indice]+" de fama por R$ "+PublicidadeCustos[indice]+".");
            VerificarVitoria();
            return PublicidadeNomes[indice]+" contratado por "+PublicidadeDias[indice]+" dia(s). +"+PublicidadeFama[indice]+" de fama.";
        }
        public int Publicidade() { int n=0; foreach(var c in estado.campanhas) n+=c.forca; return n; }
        public string Entregar(int id, int caixa, int adesivo=-1)
        {
            if (!Ativo()) return "Esta jornada terminou.";
            Pedido o=estado.pedidos.Find(x=>x.id==id);
            if(o==null || caixa<0 || caixa>3) return "Pedido indisponível.";
            int bonus = (caixa==o.caixa?1:0);
            int embalagem=4+(caixa==0?0:2)+1;
            estado.receitaDia+=o.preco;estado.custosDia+=embalagem;estado.entregasDia++;estado.dinheiro+=o.preco-embalagem; estado.fama=Math.Min(100,estado.fama+2+bonus); estado.entregues++;
            estado.pedidos.Remove(o);
            RegistrarNoticia("Pedido #"+id+" enviado para "+o.cliente+". +R$ "+(o.preco-embalagem)+" · +"+(2+bonus)+" fama.");
            VerificarVitoria();
            return "Pedido enviado! +R$ "+(o.preco-embalagem)+" e +"+(2+bonus)+" fama.";
        }
        public string PagarDivida(int valor)
        {
            estado.divida=0;
            return "A ajuda do Tio Cleiton não precisa ser devolvida.";
        }
        public string PagarConta(int id)
        {
            if (!Ativo()) return "Esta jornada terminou.";
            Conta b=estado.contas.Find(x=>x.id==id);
            if(b==null || b.pago) return "Conta já resolvida.";
            if(estado.dinheiro<b.valor) return "Saldo insuficiente para pagar essa conta.";
            estado.dinheiro-=b.valor;estado.custosDia+=b.valor; b.pago=true; RegistrarNoticia("Despesa inicial paga: R$ "+b.valor+". Não haverá nova cobrança.");
            return "Tudo pago. O ateliê continua funcionando.";
        }
        public string ComprarMoeda(int valor)
        {
            if (!Ativo()) return "Esta jornada terminou.";
            if(valor<=0||estado.dinheiro<valor) return "Saldo insuficiente para investir.";
            estado.dinheiro-=valor; estado.moedas+=valor/estado.precoMoeda;
            RegistrarNoticia("R$ "+valor+" investidos em DGCoin."); return "DGCoin comprada. A cotação muda a cada dia.";
        }
        public string VenderMoeda()
        {
            if (!Ativo()) return "Esta jornada terminou.";
            if(estado.moedas<=0) return "Você ainda não tem DGCoin.";
            int valor=(int)Math.Floor(estado.moedas*estado.precoMoeda);
            estado.dinheiro+=valor; estado.moedas=0; RegistrarNoticia("Carteira de DGCoin vendida por R$ "+valor+".");
            return "Venda concluída: R$ "+valor+" no caixa.";
        }
        public string ComprarUnidadesMoeda(int quantidade)
        {
            if(!Ativo())return "Esta jornada terminou.";
            if(quantidade<1||quantidade>1000)return "Quantidade inválida.";
            int custo=(int)Math.Ceiling(quantidade*estado.precoMoeda);
            if(estado.dinheiro<custo)return "Saldo insuficiente para comprar moedas.";
            estado.dinheiro-=custo;estado.moedas+=quantidade;RegistrarNoticia(quantidade+" Drip Coin comprada(s) por R$ "+custo+".");
            return quantidade+" Drip Coin comprada(s) por R$ "+custo+".";
        }
        public string VenderUnidadesMoeda(int quantidade)
        {
            if(!Ativo())return "Esta jornada terminou.";
            if(quantidade<1||quantidade>1000||estado.moedas<quantidade)return "Você não tem essa quantidade de moedas.";
            int valor=(int)Math.Floor(quantidade*estado.precoMoeda);
            estado.moedas-=quantidade;estado.dinheiro+=valor;RegistrarNoticia(quantidade+" Drip Coin vendida(s) por R$ "+valor+".");
            return quantidade+" Drip Coin vendida(s) por R$ "+valor+".";
        }
        public string Roleta(int aposta,bool vermelho,out int resultado){return Roleta(aposta,vermelho?0:1,out resultado);}
        public string Roleta(int aposta,int escolha,out int resultado)
        {
            string mensagem=IniciarRoleta(aposta,escolha,out resultado);
            return resultado<0?mensagem:ConcluirRoleta();
        }
        int apostaRoletaPendente,escolhaRoletaPendente,numeroRoletaPendente=-1;
        public string IniciarRoleta(int aposta,int escolha,out int resultado)
        {
            resultado=-1;
            if (!Ativo()) return "Esta jornada terminou.";
            if(numeroRoletaPendente>=0)return "Aguarde o resultado da roleta.";
            if(aposta<10||aposta>500||estado.dinheiro<aposta||escolha<0||escolha>2) return "Aposta inválida ou saldo insuficiente (R$ 10 a R$ 500).";
            apostaRoletaPendente=aposta;escolhaRoletaPendente=escolha;
            resultado=numeroRoletaPendente=Sortear(0,37);
            return "Girando...";
        }
        public string ConcluirRoleta()
        {
            if(numeroRoletaPendente<0)return "Nenhuma aposta aguardando resultado.";
            int resultado=numeroRoletaPendente,aposta=apostaRoletaPendente,escolha=escolhaRoletaPendente;
            numeroRoletaPendente=-1;apostaRoletaPendente=0;
            if(!Ativo()||estado.dinheiro<aposta)return "Aposta cancelada: partida encerrada ou saldo indisponível.";
            estado.dinheiro-=aposta;
            bool resultadoVermelho=EhVermelho(resultado), vitoria=escolha==2?resultado==0:resultado!=0&&resultadoVermelho==(escolha==0);
            int pagamento=escolha==2?MultiplicadorZero:2;if(vitoria)estado.dinheiro+=aposta*pagamento;
            string mensagem="Caiu "+resultado+(resultado==0?" · verde":resultadoVermelho?" · vermelho":" · preto")+". "+(vitoria?"Você ganhou R$ "+(aposta*(pagamento-1))+"!":"Você perdeu R$ "+aposta+".");
            RegistrarNoticia(mensagem); return mensagem;
        }
        public static bool EhVermelho(int n)
        {
            int[] vermelhos={1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36};
            return Array.IndexOf(vermelhos,n)>=0;
        }
        public void VerificarVitoria() { if(estado.fama>=100 && Ativo()) {estado.final="win"; RegistrarNoticia("A DripLab chegou lá! Sua marca atingiu 100 de fama.");} }
        public string AbrirFalencia()
        {
            if(estado.dinheiro>0) return "A falência fica disponível quando o caixa chega a zero.";
            estado.final="bankrupt"; return "Fim desta jornada. Toda marca tem uma história. Tente de novo.";
        }
        public string ProximoDia()
        {
            if (!Ativo()) return "Esta jornada terminou.";
            estado.diaResumo=estado.dia;estado.lucroResumo=estado.receitaDia-estado.custosDia;estado.custosResumo=estado.custosDia;estado.entregasResumo=estado.entregasDia;estado.receitaDia=0;estado.custosDia=0;estado.entregasDia=0;
            int processamentoDia=estado.dia;
            int produzidas=0, recebidos=0, expirados=0;
            // Demand is resolved before the current day ends; campaigns and drops last their advertised duration.
            recebidos=GerarPedidosBalanceados(processamentoDia);
            estado.dia++;
            for(int i=estado.producao.Count-1;i>=0;i--)
                if(estado.producao[i].diaEntrega<=estado.dia) {var p=estado.producao[i];AdicionarEstoquePronto(p.tipo,p.quantidade,p.precoVenda>0?p.precoVenda:PrecosBase[p.tipo]);produzidas+=p.quantidade;estado.producao.RemoveAt(i);}
            for(int i=estado.drops.Count-1;i>=0;i--)
            {
                var d=estado.drops[i];
                if(d.restante==0 || (d.limitado && estado.dia>=d.fimDia))
                {
                    AdicionarEstoquePronto(d.tipo,d.restante,d.preco);
                    RegistrarNoticia(d.nome+(d.restante==0?" esgotou!":" encerrou. "+d.restante+" peças voltaram ao estoque."));
                    estado.drops.RemoveAt(i);
                }
            }
            for(int i=estado.pedidos.Count-1;i>=0;i--)
            {
                var o=estado.pedidos[i];
                if(estado.dia-o.dia>3) {AdicionarEstoquePronto(o.tipo,1,o.preco);estado.pedidos.RemoveAt(i);estado.fama=Math.Max(0,estado.fama-3);expirados++;}
            }
            estado.campanhas.RemoveAll(c=>estado.dia>=c.fimDia);
            // As despesas iniciais não se renovam nem recebem cobranças adicionais.
            float variacao=Sortear(-30,36)/100f;
            estado.precoMoeda=MatematicaJogo.Limitar(estado.precoMoeda*(1+variacao),5,5000);
            estado.historicoCotacoes.Add(estado.precoMoeda); if(estado.historicoCotacoes.Count>16)estado.historicoCotacoes.RemoveAt(0);
            int indiceEvento=Sortear(0,5);
            string[] eventos={"A cena local está de olho no streetwear", "Chuva na cidade, movimento mais lento", "Um look urbano viralizou no feed", "Dia tranquilo no bairro", "Uma feira de moda agitou a comunidade"};
            int[] demandas={3,-3,6,0,4};
            estado.tituloEvento=eventos[indiceEvento];estado.demandaEvento=demandas[indiceEvento];RegistrarNoticia(estado.tituloEvento+".");
            estado.ultimoResumo="Dia "+estado.dia+" começou.\n"+produzidas+" peças chegaram da gráfica.\n"+recebidos+" novos pedidos aguardam embalagem.\n"+expirados+" pedidos cancelados por atraso.";
            if(expirados>0) RegistrarNoticia(expirados+" pedidos cancelados após 3 dias sem envio. -3 fama por pedido.");
            return estado.ultimoResumo;
        }
        int GerarPedidosBalanceados(int dia)
        {
            int vagas=Math.Max(0,LimitePedidos-estado.pedidos.Count),demanda=0,recebidos=0;
            foreach(var colecao in estado.drops)
            {
                if(colecao.restante<=0||demanda>=vagas)continue;
                float proporcao=(float)PrecosBase[colecao.tipo]/colecao.preco;
                float chance=MatematicaJogo.Limitar(.48f+estado.fama*.003f+Publicidade()*.012f+estado.demandaEvento*.01f,.15f,.95f);
                int tentativas=Math.Min(colecao.restante,MatematicaJogo.Limitar(MatematicaJogo.ArredondarParaInteiro((2+estado.fama/15f+Publicidade()*.5f+estado.demandaEvento*.15f)*proporcao),1,20));
                for(int i=0;i<tentativas&&demanda<vagas;i++)if(Sortear(0,100)<chance*100)demanda++;
            }
            while(recebidos<demanda&&estado.pedidos.Count<LimitePedidos)
            {
                var candidatas=new List<Colecao>();int menor=int.MaxValue;
                for(int tipo=0;tipo<Produtos.Length;tipo++)
                {
                    int categoria=tipo;
                    var colecao=estado.drops.Find(d=>d.tipo==categoria&&d.restante>0);
                    if(colecao==null)continue;
                    int pendentes=estado.pedidos.FindAll(p=>p.tipo==categoria).Count;
                    if(pendentes<menor){candidatas.Clear();menor=pendentes;}
                    if(pendentes==menor)candidatas.Add(colecao);
                }
                if(candidatas.Count==0)break;
                var escolhida=candidatas[Sortear(0,candidatas.Count)];
                estado.pedidos.Add(new Pedido{id=estado.proximoId++,tipo=escolhida.tipo,preco=escolhida.preco,dia=dia,cliente=Nomes[Sortear(0,Nomes.Length)],caixa=Sortear(0,4),adesivo=Sortear(0,4)});
                escolhida.restante--;escolhida.vendidas++;recebidos++;
            }
            return recebidos;
        }
        double intervaloCotacao;
        public void AtualizarCotacaoTempoReal(float segundos,float minutoAbsoluto)
        {
            if(estado.final!=""||segundos<=0||float.IsNaN(segundos)||float.IsInfinity(segundos)||float.IsNaN(minutoAbsoluto)||float.IsInfinity(minutoAbsoluto))return;
            if(estado.cotacoesTempoReal==null)estado.cotacoesTempoReal=new List<PontoCotacaoCripto>();
            var pontos=estado.cotacoesTempoReal;
            if(pontos.Count==0)pontos.Add(new PontoCotacaoCripto{minuto=minutoAbsoluto,preco=estado.precoMoeda});
            intervaloCotacao+=Math.Min(segundos,2f);
            if(intervaloCotacao<2)return;
            // One quote tick every two real seconds, without a catch-up burst.
            int ticks=Math.Min(1,(int)(intervaloCotacao/2));intervaloCotacao-=ticks*2;
            for(int i=0;i<ticks;i++)
            {
                estado.sementeCotacao=unchecked(estado.sementeCotacao*1664525+1013904223);
                float oscilacao=((uint)estado.sementeCotacao%2001-1000f)/1000f*.0045f;
                estado.precoMoeda=MatematicaJogo.Limitar(estado.precoMoeda*(1+oscilacao),5,5000);
            }
            // One chronological sample per update, always using the actual trading price.
            float minuto=Math.Max(minutoAbsoluto,pontos[pontos.Count-1].minuto);
            if(minuto==pontos[pontos.Count-1].minuto)pontos[pontos.Count-1].preco=estado.precoMoeda;
            else pontos.Add(new PontoCotacaoCripto{minuto=minuto,preco=estado.precoMoeda});
            int remover=0;while(remover<pontos.Count-2&&pontos[remover+1].minuto<minuto-10080)remover++;
            remover=Math.Max(remover,pontos.Count-100000);if(remover>0)pontos.RemoveRange(0,remover);
        }
    }
}
