using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        Texture2D gestaoInicioArte,falenciaBotoesArte;
        static readonly Rect FalenciaAmareloUv=new Rect(.028f,.052f,.947f,.403f);
        static readonly Rect FalenciaAzulUv=new Rect(.028f,.542f,.947f,.403f);
        void DesenharInicioGestao()
        {
            if(gestaoInicioArte==null)gestaoInicioArte=Resources.Load<Texture2D>("UI/ManagementHome");
            Matrix4x4 anterior=IniciarReferenciaTela(gestaoInicioArte);
            if(gestaoInicioArte==null)
                Texto("O menu de gestão não carregou. Feche com Esc e tente novamente.",250,350,1172,100,32,Creme,true);
            else
            {
                GestaoLigacao(new Rect(463,326,744,120),"Tecidos",9);
                GestaoLigacao(new Rect(463,482,744,116),"Confecção",1);
                GestaoLigacao(new Rect(463,638,744,113),"Publicidade",4);
                GestaoLigacao(new Rect(463,782,744,117),"Despesas",5);
                GestaoLigacao(new Rect(42,841,367,62),"Voltar para o menu",0);
            }
            DesenharFalenciaGestao();/* Navegação pelo botão Voltar; X duplicado removido. */
            InterfaceCena.matriz=anterior;
        }
        void GestaoLigacao(Rect area,string rotulo,int destino)
        {
            if((destino==0?BotaoVoltarIlustrado(area,new GUIContent("",rotulo),GUIStyle.none):InterfaceCena.Botao(area,new GUIContent("",rotulo),GUIStyle.none))){Toque();aba=destino;if(destino==9)tecidoSelecionado=-1;if(destino==1)produtoConfeccao=-1;}
        }
        void DesenharFalenciaGestao()
        {
            Rect area=new Rect(1300,846,330,72);
            bool disponivel=Estado.dinheiro<=0;
            if(falenciaBotoesArte==null)falenciaBotoesArte=Resources.Load<Texture2D>("UI/BankruptcyButtons");
            if(falenciaBotoesArte!=null)
            {
                Color anteriorCor=InterfaceCena.cor;
                if(!disponivel)InterfaceCena.cor=new Color(.48f,.48f,.48f,1);
                InterfaceCena.DesenharTexturaComCoordenadas(area,falenciaBotoesArte,disponivel&&area.Contains(Event.current.mousePosition)?FalenciaAzulUv:FalenciaAmareloUv);
                InterfaceCena.cor=anteriorCor;
            }
            else {UrbanoArredondado(area,12,Hex("A71927"));Texto("FALÊNCIA",area.x,area.y,area.width,area.height,28,Color.white,true,TextAnchor.MiddleCenter);}
            bool anterior=InterfaceCena.habilitado;InterfaceCena.habilitado=anterior&&disponivel;
            bool pressionado=InterfaceCena.Botao(area,new GUIContent("","Declarar falência"),GUIStyle.none);
            InterfaceCena.habilitado=anterior;
            if(pressionado)
            {Executar(economia.AbrirFalencia());tabletAberto=false;}
        }
    }
}
