using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        readonly Texture2D[] artesNoticias=new Texture2D[2];
        bool diarioNoticias;
        int diarioPagina;
        void DesenharArteNoticias()
        {
            var materias=new System.Collections.Generic.List<int>();for(int i=0;i<2;i++)if(economia.MateriaNoticiaDisponivel(i))materias.Add(i);if(materias.Count==0){DesenharSemNoticias();return;}
            paginaNoticias=Mathf.Clamp(paginaNoticias,0,materias.Count-1);int materia=materias[paginaNoticias];InterfaceCena.DefinirTela("Tablet/Noticias/Materia"+materia);
            if(artesNoticias[materia]==null)artesNoticias[materia]=Resources.Load<Texture2D>("UI/NewsArticle"+materia);
            Matrix4x4 anterior=IniciarReferenciaTela(artesNoticias[materia]);
            if(artesNoticias[materia]==null)Texto("A imagem da notícia não carregou.",400,400,900,90,30,Creme,true);
            if(BotaoVoltarIlustrado(new Rect(36,866,342,62),new GUIContent("","Voltar ao menu do tablet"),GUIStyle.none))
            {Toque();aba=0;diarioNoticias=false;}
            if(Botao("‹",420,874,70,48,Cartao,paginaNoticias>0,30)){paginaNoticias--;Toque();}
            UrbanoPainel(new Rect(501,874,250,48),18);
            Texto("MATÉRIA "+(paginaNoticias+1)+" / "+materias.Count,501,874,250,48,21,Creme,true,TextAnchor.MiddleCenter);
            if(Botao("›",762,874,70,48,Cartao,paginaNoticias<materias.Count-1,30)){paginaNoticias++;Toque();}
            // Only published articles are displayed in the news section.
            /* Navegação pelo botão Voltar; X duplicado removido. */
            if(diarioNoticias)DesenharDiarioNoticias();
            InterfaceCena.matriz=anterior;
        }
        void DesenharSemNoticias()
        {
            InterfaceCena.DefinirTela("Tablet/Noticias/SemNoticias");
            if(compartilhadoTabletFundo==null)compartilhadoTabletFundo=Resources.Load<Texture2D>("UI/OrdersBackground");
            Matrix4x4 anterior=IniciarReferenciaTela(compartilhadoTabletFundo);
            UrbanoPainel(new Rect(370,320,932,220),48);
            Texto("NOTÍCIAS",370,329,932,52,36,UrbanoAmarelo,true,TextAnchor.MiddleCenter);
            Texto("Nenhuma notícia no momento.",370,410,932,90,30,Creme,true,TextAnchor.MiddleCenter);
            if(BotaoVoltarIlustrado(new Rect(36,867,342,60),new GUIContent("","Voltar ao menu do tablet"),GUIStyle.none)){aba=0;Toque();}
            /* Navegação pelo botão Voltar; X duplicado removido. */
            InterfaceCena.matriz=anterior;
        }
        void DesenharDiarioNoticias()
        {
            InterfaceCena.DefinirTela("Tablet/Noticias/Diario");
            Retangulo(0,0,1672,850,new Color(0,0,0,.8f));UrbanoPainel(new Rect(300,200,1072,620),60);
            Texto("DIÁRIO DA LOJA",335,225,900,50,32,Creme,true);
            if(BotaoFecharIlustrado(new Rect(1277,211,62,62),"Fechar diário"))diarioNoticias=false;
            int paginas=Mathf.Max(1,(Estado.noticias.Count+4)/5);diarioPagina=Mathf.Clamp(diarioPagina,0,paginas-1);
            for(int i=0;i<5;i++)
            {
                int indice=diarioPagina*5+i;if(indice>=Estado.noticias.Count)break;
                Texto(Estado.noticias[indice],340,300+i*80,990,72,22,Creme);
            }
            if(Estado.noticias.Count==0)Texto("Os acontecimentos da loja aparecem aqui durante a partida.",340,320,990,100,24,SemSom);
            if(Botao("ANTERIOR",340,735,250,48,Cartao,diarioPagina>0,20))diarioPagina--;
            Texto((diarioPagina+1)+" / "+paginas,630,742,410,35,21,Creme,true,TextAnchor.MiddleCenter);
            if(Botao("PRÓXIMA",1082,735,250,48,Cartao,diarioPagina<paginas-1,20))diarioPagina++;
        }
        void DesenharNoticiasEstoque()
        {
            Matrix4x4 anterior=IniciarReferenciaEstoque();
            UrbanoPainel(new Rect(1080,864,572,67),19);
            Texto("DIA "+Estado.dia+" · "+relogioDia.Exibicao+"     CAPITAL: "+FormatarDinheiro(Estado.dinheiro),1095,870,542,26,17,UrbanoAmarelo,true,TextAnchor.MiddleCenter);
            for(int i=0;i<4;i++)Texto(EconomiaJogo.CatalogoTecidos[i]+": "+Estado.metrosTecidos[i]+" m",1094+i*139,902,137,23,12,Creme,true,TextAnchor.MiddleCenter);
            InterfaceCena.matriz=anterior;
        }
        Matrix4x4 IniciarReferenciaEstoque()
        {
            Matrix4x4 anterior=InterfaceCena.matriz;float escala=Mathf.Min(Screen.width/1672f,Screen.height/941f);
            InterfaceCena.matriz=Matrix4x4.TRS(new Vector3((Screen.width-1672*escala)/2,(Screen.height-941*escala)/2,0),Quaternion.identity,new Vector3(escala,escala,1));return anterior;
        }
    }
}
