using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        static readonly int[] RoletaNumeros={0,32,15,19,4,21,2,25,17,34,6,27,13,36,11,30,8,23,10,5,24,16,33,1,20,14,31,9,22,18,29,7,28,12,35,3,26};
        Texture2D roletaRoletaArte;
        bool roletaGirando;
        float anguloRoleta,roletaInicial,roletaAlvo,roletaTempoDecorrido;
        string mensagemRoleta="Escolha vermelho ou preto e gire a roleta.";

        void IniciarGiroRoleta()
        {
            if(roletaGirando)return;
            string resultado=economia.IniciarRoleta(aposta,apostaZero?2:apostaVermelho?0:1,out roletaNumero);
            if(roletaNumero<0){AdicionarTecidoAviso(resultado,false);return;}
            int setor=System.Array.IndexOf(RoletaNumeros,roletaNumero);
            roletaInicial=anguloRoleta;roletaAlvo=GeometriaRoleta.AnguloParada(anguloRoleta,setor,RoletaNumeros.Length);
            roletaTempoDecorrido=0;roletaGirando=true;mensagemRoleta="Girando...";
        }
        void AtualizarRoletaApostas()
        {
            if(!roletaGirando||menuAberto||!sessaoTemFoco||sessaoPausada)return;
            roletaTempoDecorrido+=Time.unscaledDeltaTime;
            float t=Mathf.Clamp01(roletaTempoDecorrido/3f);
            anguloRoleta=Mathf.Lerp(roletaInicial,roletaAlvo,1-Mathf.Pow(1-t,3));
            if(t>=1)
            {
                mensagemRoleta=economia.ConcluirRoleta();roletaGirando=false;anguloRoleta=Mathf.Repeat(roletaAlvo,360);
                AdicionarTecidoAviso(mensagemRoleta,true,"RESULTADO DA ROLETA");Toque();
            }
        }
        void CriarArteRoleta()
        {
            if(roletaRoletaArte!=null)return;
            const int tamanho=512;var pixels=new Color[tamanho*tamanho];
            float setorAngulo=360f/RoletaNumeros.Length;
            for(int y=0;y<tamanho;y++)for(int x=0;x<tamanho;x++)
            {
                float dx=x+.5f-256,dy=y+.5f-256,d=Mathf.Sqrt(dx*dx+dy*dy);
                float angulo=Mathf.Repeat(Mathf.Atan2(dx,dy)*Mathf.Rad2Deg,360);
                int setor=Mathf.FloorToInt(angulo/setorAngulo+.5f)%RoletaNumeros.Length;
                int numero=RoletaNumeros[setor];
                Color cor=numero==0?Hex("009D54"):EconomiaJogo.EhVermelho(numero)?Hex("D9232B"):UrbanoPreto;
                if(d>242)cor=UrbanoAmarelo;
                if(d<237&&Mathf.Abs(Mathf.DeltaAngle(angulo,(setor+.5f)*setorAngulo))<.35f)cor=UrbanoAmarelo;
                cor.a=Mathf.Clamp01(256-d);pixels[y*tamanho+x]=cor;
            }
            roletaRoletaArte=new Texture2D(tamanho,tamanho,TextureFormat.RGBA32,false){name="Roleta DRIP LAB",filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp,hideFlags=HideFlags.HideAndDontSave};
            roletaRoletaArte.SetPixels(pixels);roletaRoletaArte.Apply();
        }
        void DesenharRoletaApostas()
        {
            InterfaceCena.DefinirTela("Tablet/Investimentos/Roleta");
            CriarArteRoleta();
            UrbanoPainel(new Rect(350,105,972,740),66);
            UrbanoTexto("ROLETA / DRIP LAB",new Rect(390,128,892,46),34,Color.white,TextAnchor.MiddleCenter);
            Texto("Vermelho ou preto paga 2× a aposta. Zero perde. Chance: 18 em 37.",390,197,892,36,19,Creme,false,TextAnchor.MiddleCenter);
            Matrix4x4 anterior=InterfaceCena.matriz;
            Vector2 centro=new Vector2(836,470.5f);
            GUIUtility.RotateAroundPivot(anguloRoleta,centro);
            InterfaceCena.DesenharTextura(new Rect(centro.x-190,centro.y-190,380,380),roletaRoletaArte);
            for(int i=0;i<RoletaNumeros.Length;i++)
            {
                float angulo=i*2*Mathf.PI/RoletaNumeros.Length;
                Vector2 posicao=centro+new Vector2(Mathf.Sin(angulo),-Mathf.Cos(angulo))*159;
                UrbanoTexto(RoletaNumeros[i].ToString(),new Rect(posicao.x-15,posicao.y-13,30,26),17,Color.white,TextAnchor.MiddleCenter);
            }
            InterfaceCena.matriz=anterior;
            Disco(centro.x,centro.y,70,UrbanoAzul);UrbanoTexto("DG",new Rect(centro.x-35,centro.y-28,70,56),37,UrbanoAmarelo,TextAnchor.MiddleCenter);
            UrbanoTexto("▼",new Rect(810,248.5f,52,45),39,UrbanoAmarelo,TextAnchor.MiddleCenter);
            Texto(mensagemRoleta,390,674,892,36,20,Creme,true,TextAnchor.MiddleCenter);
            Texto("APOSTA / R$",423,699,154,20,13,SemSom,true);
            bool habilitado=InterfaceCena.habilitado;InterfaceCena.habilitado=habilitado&&!roletaGirando;
            aposta=SeletorQuantidade(aposta,423,722,10,500,10);
            if(Botao("VERMELHO",601,722,191,40,apostaVermelho?Hex("DC6577"):Cartao,true,18))apostaVermelho=true;
            if(Botao("PRETO",810,722,165,40,!apostaVermelho?Azul:Cartao,true,18))apostaVermelho=false;
            if(Botao("GIRAR",995,717,255,48,Amarelo,Estado.dinheiro>=aposta&&Estado.final=="",23))IniciarGiroRoleta();
            if(Botao("VOLTAR",423,783,827,40,Cartao,true,19))riscoRoleta=false;
            InterfaceCena.habilitado=habilitado;
        }
    }
}
