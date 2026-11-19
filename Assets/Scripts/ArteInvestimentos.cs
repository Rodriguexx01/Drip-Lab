using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        Texture2D investimentoAtalhoArte,investimentoMenuArte,roletaPainelArte,criptoPainelArte;
        int paginaInvestimentos,fichaRoleta=50,quantidadeFichas=1,quantidadeCripto=1,periodoCripto=1;
        bool apostaZero;
        void DesenharInvestimentoAtalho()
        {
            if(investimentoAtalhoArte==null)investimentoAtalhoArte=Resources.Load<Texture2D>("UI/ExtrasButtons");
            if(investimentoAtalhoArte==null)return;
            Rect area=new Rect(24,241,340,114);
            DesenharBotaoDoisEstados(new Rect(24,250,340,96),investimentoAtalhoArte,AtalhoAmareloUv,AtalhoAzulUv);
            if(InterfaceCena.Botao(area,new GUIContent("","Abrir extras"),GUIStyle.none))
            {aba=6;paginaInvestimentos=0;riscoRoleta=false;tabletAberto=true;Toque();}
        }
        void DesenharArteInvestimentos()
        {
            if(investimentoMenuArte==null)investimentoMenuArte=Resources.Load<Texture2D>("UI/InvestmentMenu");
            if(roletaPainelArte==null)roletaPainelArte=Resources.Load<Texture2D>("UI/RoulettePanel");
            if(criptoPainelArte==null)criptoPainelArte=Resources.Load<Texture2D>("UI/CryptoPanel");
            InterfaceCena.DefinirTela(paginaInvestimentos==1?"Tablet/Investimentos/Roleta":paginaInvestimentos==2?"Tablet/Investimentos/Cripto":"Tablet/Investimentos/Inicio");
            Matrix4x4 anterior=IniciarReferenciaTela(paginaInvestimentos==1?roletaPainelArte:paginaInvestimentos==2?criptoPainelArte:investimentoMenuArte);
            if(paginaInvestimentos==0)
            {
                if(InterfaceCena.Botao(new Rect(299,743,487,117),new GUIContent("","Aposta na roleta"),GUIStyle.none)){paginaInvestimentos=1;riscoRoleta=true;Toque();}
                if(InterfaceCena.Botao(new Rect(887,743,488,117),new GUIContent("","Criptomoeda"),GUIStyle.none)){paginaInvestimentos=2;riscoRoleta=false;Toque();}
                if(BotaoVoltarIlustrado(new Rect(21,831,241,91),new GUIContent("","Voltar à bancada"),GUIStyle.none)){tabletAberto=false;Toque();}
            }
            else
            {
                if(paginaInvestimentos==1)DesenharArteRoleta();else DesenharArteCripto();
                bool habilitado=InterfaceCena.habilitado;InterfaceCena.habilitado=habilitado&&!roletaGirando;
                if(BotaoVoltarIlustrado(paginaInvestimentos==2?new Rect(11,852,240,82):new Rect(11,852,245,82),new GUIContent("","Voltar aos investimentos"),GUIStyle.none)){paginaInvestimentos=0;riscoRoleta=false;Toque();}
                InterfaceCena.habilitado=habilitado;
            }
            InterfaceCena.matriz=anterior;
        }
        void InvestimentoValor(string rotulo,Rect area,int tamanho=32,Color? tinta=null)
        {
            UrbanoArredondado(area,8,Hex("090D12"));
            Texto(rotulo,area.x+3,area.y+2,area.width-6,area.height-4,tamanho,tinta??UrbanoAmarelo,true,TextAnchor.MiddleCenter);
        }
        void DesenharArteRoleta()
        {
            DesenharPainelRoletaAnimado();DesenharPonteiroRoleta(new Rect(598,228,58,52));
            Texto("R$ "+Estado.dinheiro.ToString("N2",System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),1385,134,238,53,34,UrbanoAmarelo,true,TextAnchor.MiddleCenter);
            bool habilitado=InterfaceCena.habilitado;InterfaceCena.habilitado=habilitado&&!roletaGirando;
            int[] fichas={10,50,100};float[] xs={1206,1326,1457};
            for(int i=0;i<3;i++)if(InterfaceCena.Botao(new Rect(xs[i],348,106,106),new GUIContent("","Ficha de R$ "+fichas[i]),GUIStyle.none)){fichaRoleta=fichas[i];quantidadeFichas=Mathf.Min(quantidadeFichas,500/fichaRoleta);Toque();} DesenharFichasAposta();
            if(InterfaceCena.Botao(new Rect(1223,574,65,52),new GUIContent("","Diminuir quantidade"),GUIStyle.none)){quantidadeFichas=Mathf.Max(1,quantidadeFichas-1);Toque();}
            if(InterfaceCena.Botao(new Rect(1477,574,65,52),new GUIContent("","Aumentar quantidade"),GUIStyle.none)){quantidadeFichas=Mathf.Min(500/fichaRoleta,quantidadeFichas+1);Toque();}
            aposta=fichaRoleta*quantidadeFichas;
            Texto("R$ "+aposta.ToString("N2",System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),1220,478,330,48,36,UrbanoAmarelo,true,TextAnchor.MiddleCenter);
            Texto(quantidadeFichas.ToString(),1294,577,177,47,36,Creme,true,TextAnchor.MiddleCenter);
            UrbanoTexto("FICHA R$ "+fichaRoleta,new Rect(1203,321,352,24),16,UrbanoAmarelo,TextAnchor.MiddleCenter);
            if(InterfaceCena.Botao(new Rect(1177,644,138,69),new GUIContent("","Apostar no vermelho"),GUIStyle.none)){apostaVermelho=true;apostaZero=false;Toque();}
            if(InterfaceCena.Botao(new Rect(1320,644,136,69),new GUIContent("","Apostar no preto"),GUIStyle.none)){apostaVermelho=false;apostaZero=false;Toque();}
            if(InterfaceCena.Botao(new Rect(1465,644,120,69),new GUIContent("","Apostar no zero"),GUIStyle.none)){apostaZero=true;Toque();}
            DesenharCoresAposta();
            if(InterfaceCena.Botao(new Rect(1178,729,405,87),new GUIContent("","Girar a roleta"),GUIStyle.none))IniciarGiroRoleta();
            InterfaceCena.habilitado=habilitado;
            InvestimentoValor(roletaGirando?"GIRANDO...":apostaZero?"ZERO · PAGA 3×":apostaVermelho?"VERMELHO · PAGA 2×":"PRETO · PAGA 2×",new Rect(1181,824,399,29),17,Creme);
            UrbanoArredondado(new Rect(269,872,798,46),10,UrbanoPreto);
            Texto(mensagemRoleta,281,878,774,35,18,Creme,true,TextAnchor.MiddleCenter);
        }
        void DesenharArteCripto()
        {
            DesenharCriptoTempoReal();















        }
        void DesenharGraficoCotacao()
        {
            DesenharGraficoCriptoVivo();











        }
        void InvestimentoContorno(Rect area)
        {
            DesenharLinha(new Vector2(area.x,area.y),new Vector2(area.xMax,area.y),UrbanoAmarelo,3);
            DesenharLinha(new Vector2(area.x,area.yMax),new Vector2(area.xMax,area.yMax),UrbanoAmarelo,3);
            DesenharLinha(new Vector2(area.x,area.y),new Vector2(area.x,area.yMax),UrbanoAmarelo,3);
            DesenharLinha(new Vector2(area.xMax,area.y),new Vector2(area.xMax,area.yMax),UrbanoAmarelo,3);
        }
        Material materialRoletaApostas;
        RenderTexture quadroRoletaApostas;
        bool sombreadorRoletaApostasVerificado;
        void DesenharPainelRoletaAnimado()
        {
            if(roletaPainelArte==null)return;
            if(!sombreadorRoletaApostasVerificado)
            {
                sombreadorRoletaApostasVerificado=true;
                var sombreador=Resources.Load<Shader>("Shaders/RoletaApostas");
                if(sombreador!=null&&sombreador.isSupported)
                {
                    materialRoletaApostas=new Material(sombreador){hideFlags=HideFlags.HideAndDontSave};
                    quadroRoletaApostas=new RenderTexture(roletaPainelArte.width,roletaPainelArte.height,0){name="Painel da roleta de apostas",hideFlags=HideFlags.HideAndDontSave};
                    if(!quadroRoletaApostas.Create()){Destroy(quadroRoletaApostas);quadroRoletaApostas=null;}
                }
            }
            if(quadroRoletaApostas==null)return;
            if(Event.current.type==EventType.Repaint)
            {
                materialRoletaApostas.SetFloat("_Angle",anguloRoleta*Mathf.Deg2Rad);
                var anterior=RenderTexture.active;
                try{Graphics.Blit(roletaPainelArte,quadroRoletaApostas,materialRoletaApostas);}
                finally{RenderTexture.active=anterior;}
            }
            // O sombreador altera somente a faixa circular dos números e casas.
            // A matriz da interface, o ponteiro, o eixo e a moldura permanecem fixos.
            InterfaceCena.DesenharTextura(new Rect(0,0,1672,941),quadroRoletaApostas,ScaleMode.StretchToFill);
        }
        void LiberarArteRoletaApostas()
        {
            if(quadroRoletaApostas!=null){quadroRoletaApostas.Release();Destroy(quadroRoletaApostas);}
            if(materialRoletaApostas!=null)Destroy(materialRoletaApostas);if(criptoLinhaTextura!=null)Destroy(criptoLinhaTextura);
        }
        void DesenharCoresAposta()
        {
            DesenharBotoesCoresIlustrados();
        }

















        void DesenharFichasAposta()
        {
            DesenharFichasIlustradas();
        }





















        Texture2D criptoPeriodosArte;
        static readonly Rect[] CriptoPeriodosUv={new Rect(0.693680858f,0.623445156f,0.253184713f,0.258763664f),new Rect(0.026148173f,0.622314361f,0.306151525f,0.264983038f),new Rect(0.356520282f,0.626460611f,0.286540396f,0.289483603f),new Rect(0.030254777f,0.096871466f,0.296932618f,0.289295138f),new Rect(0.652363393f,0.057293630f,0.333640630f,0.334715417f),new Rect(0.390881663f,0.128722201f,0.216644318f,0.226159065f)};
        void DesenharPeriodosCripto(float[] posicoes)
        {
            if(criptoPeriodosArte==null)criptoPeriodosArte=Resources.Load<Texture2D>("UI/CriptoPeriodos");
            // Cover the three fixed controls baked into the background, including the old selection.
            // Transparent surroundings: the new background has no baked period controls.
            for(int i=0;i<3;i++)
            {
                Rect area=new Rect(posicoes[i],350,92,52);
                if(criptoPeriodosArte!=null)InterfaceCena.DesenharTexturaComCoordenadas(area,criptoPeriodosArte,CriptoPeriodosUv[(periodoCripto==i?3:0)+i]);
                else {UrbanoArredondado(area,8,periodoCripto==i?UrbanoAmarelo:UrbanoPreto);Texto(new[]{"1H","1D","1S"}[i],area.x,area.y,area.width,area.height,23,periodoCripto==i?Color.black:Color.white,true,TextAnchor.MiddleCenter);}
            }
        }
        int InicioPeriodoCripto()
        {
            var pontos=Estado.cotacoesTempoReal;if(pontos==null||pontos.Count==0)return 0;
            float janela=periodoCripto==0?60:periodoCripto==1?1440:10080;
            float corte=pontos[pontos.Count-1].minuto-janela;int inicio=0;
            while(inicio<pontos.Count-1&&pontos[inicio].minuto<corte)inicio++;
            return inicio;
        }
        void DesenharCriptoTempoReal()
        {
            var cultura=System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
            Texto(FormatarDinheiro(Estado.dinheiro),1380,135,245,52,34,UrbanoAmarelo,true,TextAnchor.MiddleCenter);
            Texto(Estado.precoMoeda.ToString("N2",cultura),687,269,151,62,32,Creme,true,TextAnchor.MiddleCenter);
            var pontos=Estado.cotacoesTempoReal;
            float referencia=pontos!=null&&pontos.Count>0?pontos[InicioPeriodoCripto()].preco:Estado.precoMoeda;
            float variacao=referencia>0?(Estado.precoMoeda/referencia-1)*100:0;
            // Reuse the artwork's clean interior to replace its fixed + and % glyphs.
            if(criptoPainelArte!=null)InterfaceCena.DesenharTexturaComCoordenadas(new Rect(868,276,148,46),criptoPainelArte,new Rect(912f/1672,1-319f/941,30f/1672,39f/941));
            Texto((variacao>=0?"+":"")+variacao.ToString("F1",cultura)+"%",868,276,148,46,28,variacao>=0?Hex("00D34F"):Hex("E13E42"),true,TextAnchor.MiddleCenter);
            float[] periodos={847,946,1046};DesenharPeriodosCripto(periodos);
            for(int i=0;i<3;i++)if(InterfaceCena.Botao(new Rect(periodos[i],350,92,52),new GUIContent("","Período "+new[]{"1 hora","1 dia","1 semana"}[i]),GUIStyle.none)){periodoCripto=i;Toque();}
            DesenharGraficoCotacao();
            if(InterfaceCena.Botao(new Rect(1225,568,66,58),new GUIContent("","Diminuir quantidade"),GUIStyle.none)){quantidadeCripto=Mathf.Max(1,quantidadeCripto-1);Toque();}
            if(InterfaceCena.Botao(new Rect(1488,568,68,58),new GUIContent("","Aumentar quantidade"),GUIStyle.none)){quantidadeCripto=Mathf.Min(1000,quantidadeCripto+1);Toque();}
            Texto(quantidadeCripto.ToString(),1299,571,180,52,38,Creme,true,TextAnchor.MiddleCenter);
            Texto(FormatarDinheiro(Mathf.CeilToInt(quantidadeCripto*Estado.precoMoeda)),1360,644,207,47,29,UrbanoAmarelo,true,TextAnchor.MiddleCenter);
            if(InterfaceCena.Botao(new Rect(1198,706,190,76),new GUIContent("","Comprar moedas"),GUIStyle.none))Executar(economia.ComprarUnidadesMoeda(quantidadeCripto));
            if(InterfaceCena.Botao(new Rect(1398,706,186,76),new GUIContent("","Vender moedas"),GUIStyle.none))Executar(economia.VenderUnidadesMoeda(quantidadeCripto));
            Texto(Estado.moedas.ToString("0.###",cultura),1440,797,128,32,22,Creme,true,TextAnchor.MiddleCenter);
        }
        void DesenharGraficoCriptoVivo()
        {
            var pontos=Estado.cotacoesTempoReal;int inicio=InicioPeriodoCripto(),fim=pontos!=null?pontos.Count-1:-1;
            float minimo=Estado.precoMoeda,maximo=minimo;
            if(fim>=0)for(int i=inicio;i<=fim;i++){minimo=Mathf.Min(minimo,pontos[i].preco);maximo=Mathf.Max(maximo,pontos[i].preco);}
            float margem=Mathf.Max(1,(maximo-minimo)*.18f);minimo-=margem;maximo+=margem;
            for(int i=0;i<5;i++)
            {
                float y=435+i*76;Retangulo(166,y,954,1,Hex("16354F"));
                Texto("R$ "+Mathf.Lerp(maximo,minimo,i/4f).ToString("F2",System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),87,y-13,72,26,13,Creme,false,TextAnchor.MiddleRight);
            }
            float t0=fim>=0?pontos[inicio].minuto:0,t1=fim>=0?pontos[fim].minuto:1;
            for(int i=0;i<5;i++)
            {
                float x=166+i*954/4f;Retangulo(x,435,1,304,Hex("16354F"));
                int minuto=Mathf.RoundToInt(Mathf.Lerp(t0,t1,i/4f));string horario=(minuto/60%24).ToString("00")+":"+(minuto%60).ToString("00");
                Texto(periodoCripto==2?"D"+(minuto/1440+1)+" "+horario:horario,x-45,746,90,23,13,Creme,false,TextAnchor.MiddleCenter);
            }
            Vector2 ultimo=new Vector2(1120,Mathf.Lerp(739,435,Mathf.InverseLerp(minimo,maximo,Estado.precoMoeda)));
            DesenharLinhaCriptoContinua(pontos,inicio,fim,minimo,maximo,t0,t1);
            Disco(ultimo.x,ultimo.y,13+3*Mathf.Sin(Time.unscaledTime*4),new Color(.09f,.74f,1,.25f));Disco(ultimo.x,ultimo.y,6,Creme);
            Texto("TEMPO REAL · "+new[]{"ÚLTIMA HORA","ÚLTIMAS 24 HORAS","ÚLTIMOS 7 DIAS"}[periodoCripto],180,787,910,26,17,Creme,true,TextAnchor.MiddleCenter);
        }
        Texture2D criptoLinhaTextura;
        Color32[] criptoLinhaPixels;
        int criptoLinhaInicio=-1,criptoLinhaFim=-2,criptoLinhaPeriodo=-1;
        float criptoLinhaPreco=-1;
        void DesenharLinhaCriptoContinua(System.Collections.Generic.List<PontoCotacaoCripto> pontos,int inicio,int fim,float minimo,float maximo,float t0,float t1)
        {
            const int largura=955,altura=305;
            if(criptoLinhaTextura==null)
            {
                criptoLinhaTextura=new Texture2D(largura,altura,TextureFormat.RGBA32,false){name="Linha contínua Drip Coin",filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp,hideFlags=HideFlags.HideAndDontSave};
                criptoLinhaPixels=new Color32[largura*altura];criptoLinhaFim=-2;
            }
            if(Event.current.type==EventType.Repaint&&(criptoLinhaInicio!=inicio||criptoLinhaFim!=fim||criptoLinhaPeriodo!=periodoCripto||criptoLinhaPreco!=Estado.precoMoeda))
            {
                System.Array.Clear(criptoLinhaPixels,0,criptoLinhaPixels.Length);
                int segmentos=Mathf.Max(1,Mathf.Min(192,fim-inicio));
                Vector2 anterior=Vector2.zero;
                for(int j=0;j<=segmentos;j++)
                {
                    float x,y;
                    if(fim>inicio)
                    {
                        int k=inicio+Mathf.RoundToInt((fim-inicio)*j/(float)segmentos);
                        x=t1>t0?(pontos[k].minuto-t0)/(t1-t0)*(largura-1):j*(largura-1)/(float)segmentos;
                        y=Mathf.InverseLerp(minimo,maximo,pontos[k].preco)*(altura-1);
                    }
                    else {x=j*(largura-1)/(float)segmentos;y=Mathf.InverseLerp(minimo,maximo,Estado.precoMoeda)*(altura-1);}
                    Vector2 atual=new Vector2(x,y);
                    if(j>0)
                    {
                        int passos=Mathf.Max(1,Mathf.CeilToInt(Vector2.Distance(anterior,atual)*2));
                        for(int passo=0;passo<=passos;passo++)PintarPontoCripto(Vector2.Lerp(anterior,atual,passo/(float)passos),largura,altura);
                    }
                    anterior=atual;
                }
                criptoLinhaTextura.SetPixels32(criptoLinhaPixels);criptoLinhaTextura.Apply(false);
                criptoLinhaInicio=inicio;criptoLinhaFim=fim;criptoLinhaPeriodo=periodoCripto;criptoLinhaPreco=Estado.precoMoeda;
            }
            // One transparent image for the entire polyline: no independently rotated segments.
            InterfaceCena.DesenharTextura(new Rect(166,435,954,304),criptoLinhaTextura,ScaleMode.StretchToFill);
        }
        void PintarPontoCripto(Vector2 ponto,int largura,int altura)
        {
            int cx=Mathf.RoundToInt(ponto.x),cy=Mathf.RoundToInt(ponto.y);
            for(int y=Mathf.Max(0,cy-2);y<=Mathf.Min(altura-1,cy+2);y++)
                for(int x=Mathf.Max(0,cx-2);x<=Mathf.Min(largura-1,cx+2);x++)
                {
                    float cobertura=Mathf.Clamp01(1.8f-Vector2.Distance(new Vector2(x,y),ponto));
                    int indice=y*largura+x;byte alfa=(byte)Mathf.RoundToInt(cobertura*255);
                    if(alfa>criptoLinhaPixels[indice].a)criptoLinhaPixels[indice]=new Color32(23,189,255,alfa);
                }
        }
        readonly Texture2D[] roletaCoresArte=new Texture2D[6];
        static readonly string[] RoletaCoresRecursos={"RoletaVermelho","RoletaPreto","RoletaZero","RoletaVermelhoSelecionado","RoletaPretoSelecionado","RoletaZeroSelecionado"};
        static readonly Rect[] RoletaCoresUv={new Rect(0.135438596f,0.235789474f,0.727894737f,0.522456140f),new Rect(0.135964912f,0.274385965f,0.727543860f,0.446666667f),new Rect(0.158975259f,0.262807018f,0.681347605f,0.473333333f),new Rect(0.102280702f,0.158947368f,0.791052632f,0.677192982f),new Rect(0.103684211f,0.201052632f,0.788070175f,0.594736842f),new Rect(0.100719424f,0.164912281f,0.794876294f,0.668771930f)};
        void DesenharBotoesCoresIlustrados()
        {
            int selecionada=apostaZero?2:apostaVermelho?0:1;
            Rect[] areas={new Rect(1177,644,138,69),new Rect(1320,644,136,69),new Rect(1465,644,120,69)};
            for(int i=0;i<3;i++)
            {
                bool selecionado=i==selecionada;int indice=i+(selecionado?3:0);
                if(roletaCoresArte[indice]==null)roletaCoresArte[indice]=Resources.Load<Texture2D>("UI/"+RoletaCoresRecursos[indice]);
                Rect area=areas[i];
                // A aura ocupa a margem externa; o corpo do botão mantém o tamanho.
                if(selecionado){float w=area.width*1.14f,h=area.height*1.30f;area=new Rect(area.center.x-w/2,area.center.y-h/2,w,h);}
                if(roletaCoresArte[indice]!=null)InterfaceCena.DesenharTexturaComCoordenadas(area,roletaCoresArte[indice],RoletaCoresUv[indice]);
            }
        }
        readonly Texture2D[] roletaFichasArte=new Texture2D[6];
        static readonly string[] RoletaFichasRecursos={"RoletaFicha10","RoletaFicha50","RoletaFicha100","RoletaFicha10Selecionada","RoletaFicha50Selecionada","RoletaFicha100Selecionada"};
        static readonly Rect[] RoletaFichasUv={new Rect(0.117592421f,0.109975850f,0.760728218f,0.770574029f),new Rect(0.125394761f,0.123537061f,0.743265837f,0.753111648f),new Rect(0.140999443f,0.139141743f,0.713914174f,0.715957644f),new Rect(0.107746610f,0.113877020f,0.764815159f,0.782277540f),new Rect(0.099944269f,0.094371168f,0.784321011f,0.801783392f),new Rect(0.057031395f,0.059074865f,0.870146758f,0.878134869f)};
        void DesenharFichasIlustradas()
        {
            int[] valores={10,50,100};float[] centros={1259,1379,1510};
            for(int i=0;i<3;i++)
            {
                bool selecionada=fichaRoleta==valores[i];int indice=i+(selecionada?3:0);
                if(roletaFichasArte[indice]==null)roletaFichasArte[indice]=Resources.Load<Texture2D>("UI/"+RoletaFichasRecursos[indice]);
                float diametro=selecionada?132:106;
                if(roletaFichasArte[indice]!=null)InterfaceCena.DesenharTexturaComCoordenadas(new Rect(centros[i]-diametro/2,401-diametro/2,diametro,diametro),roletaFichasArte[indice],RoletaFichasUv[indice]);
            }
        }
    }
}
