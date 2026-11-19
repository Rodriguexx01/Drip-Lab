using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        Texture2D negociacaoArte;
        Material negociacaoMaterial;
        RenderTexture negociacaoQuadro;
        bool negociacaoSombreadorVerificado;
        void DesenharArteNegociacao()
        {
            InterfaceCena.DefinirTela("Tablet/Tecidos/Negociacao");
            if(negociacaoArte==null)negociacaoArte=Resources.Load<Texture2D>("UI/NegotiationWheel");
            if(!negociacaoSombreadorVerificado)
            {
                negociacaoSombreadorVerificado=true;var sombreador=Resources.Load<Shader>("Shaders/NegotiationWheel");
                if(sombreador!=null&&sombreador.isSupported&&negociacaoArte!=null)
                {
                    negociacaoMaterial=new Material(sombreador){hideFlags=HideFlags.HideAndDontSave};
                    negociacaoQuadro=new RenderTexture(negociacaoArte.width,negociacaoArte.height,0){name="Roleta de negociação",hideFlags=HideFlags.HideAndDontSave};
                    if(!negociacaoQuadro.Create()){Destroy(negociacaoQuadro);negociacaoQuadro=null;}
                }
            }
            if(negociacaoQuadro!=null&&Event.current.type==EventType.Repaint)
            {
                negociacaoMaterial.SetFloat("_Angle",tecidoRoletaAngulo*Mathf.Deg2Rad);
                RenderTexture anterior=RenderTexture.active;
                try{Graphics.Blit(negociacaoArte,negociacaoQuadro,negociacaoMaterial);}finally{RenderTexture.active=anterior;}
            }
            if(negociacaoArte!=null)
            {
                InterfaceCena.DesenharTextura(new Rect(0,0,1672,941),negociacaoQuadro!=null?(Texture)negociacaoQuadro:negociacaoArte,ScaleMode.StretchToFill);
                // The pointer stays still while the illustrated wheel turns underneath it.
                DesenharPonteiroRoleta(new Rect(790.5f,245,91,81));
            }
            UrbanoPainel(new Rect(24,335,322,240),36);
            Texto(EconomiaJogo.CatalogoTecidos[tecidoSelecionado].ToUpper(),43,345,284,36,25,Creme,true,TextAnchor.MiddleCenter);
            Texto(tecidoRoletaMensagem,43,416,284,94,21,UrbanoAmarelo,true,TextAnchor.MiddleCenter);
            Texto("50% SIM / 50% NÃO\nUma tentativa por tecido por dia.",43,516,284,45,15,Creme,false,TextAnchor.MiddleCenter);
            bool utilizada=economia.NegociacaoTecidoUtilizada(tecidoSelecionado),habilitado=InterfaceCena.habilitado;
            InterfaceCena.habilitado=habilitado&&!tecidoRoletaGirando;
            if(BotaoVoltarIlustrado(new Rect(21,831,241,91),new GUIContent("","Voltar ao catálogo de tecidos"),GUIStyle.none)){tecidoSelecionado=-1;Toque();}
            if(!utilizada||tecidoRoletaGirando)
            {
                if(NegociacaoAcaoArte(false,!tecidoRoletaGirando))GirarTecidoRoleta();
            }
            else NegociacaoAcaoArte(true,false);
            InterfaceCena.habilitado=habilitado;
        }
        void LiberarArteNegociacao()
        {
            if(negociacaoQuadro!=null){negociacaoQuadro.Release();Destroy(negociacaoQuadro);}
            if(negociacaoMaterial!=null)Destroy(negociacaoMaterial);
        }
        Texture2D roletaPonteiroArte;
        void DesenharPonteiroRoleta(Rect area)
        {
            if(roletaPonteiroArte==null)roletaPonteiroArte=Resources.Load<Texture2D>("UI/RoletaPonteiro");
            if(roletaPonteiroArte==null)return;
            const float proporcao=1.120131512f;
            float largura=Mathf.Min(area.width,area.height*proporcao),altura=largura/proporcao;
            InterfaceCena.DesenharTexturaComCoordenadas(new Rect(area.center.x-largura/2,area.yMax-altura,largura,altura),roletaPonteiroArte,new Rect(0.092163355f,0.094806763f,0.814753495f,0.795893720f));
        }
        readonly Texture2D[] negociacaoAcoesArte=new Texture2D[2];
        static readonly string[] NegociacaoAcoesRecursos={"NegociacaoGirar","NegociacaoRealizada"};
        static readonly Rect[] NegociacaoAcoesUv={new Rect(0.024527839f,0.098601913f,0.951189600f,0.785504047f),new Rect(0.014549791f,0.246511628f,0.970184854f,0.520572451f)};
        static readonly float[] NegociacaoAcoesProporcoes={3.632786885f,5.591065292f};
        bool NegociacaoAcaoArte(bool realizada,bool clicavel)
        {
            int indice=realizada?1:0;
            if(negociacaoAcoesArte[indice]==null)negociacaoAcoesArte[indice]=Resources.Load<Texture2D>("UI/"+NegociacaoAcoesRecursos[indice]);
            Rect area=new Rect(566,838,540,96);
            if(negociacaoAcoesArte[indice]==null)return UrbanoBotao(realizada?"TENTATIVA DE HOJE REALIZADA":"GIRAR",area,true,clicavel,22);
            float largura=Mathf.Min(area.width,area.height*NegociacaoAcoesProporcoes[indice]),altura=largura/NegociacaoAcoesProporcoes[indice];
            Rect desenho=new Rect(area.center.x-largura/2,area.center.y-altura/2,largura,altura);
            InterfaceCena.DesenharTexturaComCoordenadas(desenho,negociacaoAcoesArte[indice],NegociacaoAcoesUv[indice]);
            bool anterior=InterfaceCena.habilitado;InterfaceCena.habilitado=anterior&&clicavel;
            bool pressionado=InterfaceCena.Botao(desenho,new GUIContent("",realizada?"Tentativa de hoje realizada":"Girar roleta de negociação"),GUIStyle.none);
            InterfaceCena.habilitado=anterior;return pressionado;
        }
    }
}
