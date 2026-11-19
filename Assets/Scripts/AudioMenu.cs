using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        AudioSource fonteMusica;
        float volumeGeral=0.8f, volumeMusica=0.6f, volumeEfeitos=0.75f;
        bool semSom;
        Texture2D principalMenuArte;
        int menuSelecao;
        void ConfigurarAudio()
        {
            IniciarAudioUnderground();






















        }
        void AplicarAudio()
        {
            AtualizarVolumesAudio();


        }
        void SalvarAudio()
        {
            PlayerPrefs.SetFloat("DripLab.Master",volumeGeral);
            PlayerPrefs.SetFloat("DripLab.Music",volumeMusica);
            PlayerPrefs.SetFloat("DripLab.Effects",volumeEfeitos);
            PlayerPrefs.SetInt("DripLab.Muted",semSom?1:0);PlayerPrefs.Save();
        }
        void DesenharMenuPrincipal()
        {
            if(principalMenuArte==null)principalMenuArte=Resources.Load<Texture2D>("UI/MenuPrincipal3D");
            if(principalMenuArte==null)
            {
                Retangulo(0,0,1440,900,Tinta);
                Texto("Imagem do menu não encontrada em Resources/UI/MenuPrincipal3D.",100,100,1240,100,25,Creme);
                return;
            }
            // Fit the supplied artwork intact. Hit areas follow the same transform,
            // including letterboxing, so resizing never detaches them from the art.
            Matrix4x4 anterior=InterfaceCena.matriz;
            InterfaceCena.matriz=Matrix4x4.identity;
            Retangulo(0,0,Screen.width,Screen.height,Color.black);
            float escala=Mathf.Min(Screen.width/(float)principalMenuArte.width,Screen.height/(float)principalMenuArte.height);
            float largura=principalMenuArte.width*escala,altura=principalMenuArte.height*escala;
            var imagemArea=new Rect((Screen.width-largura)/2,(Screen.height-altura)/2,largura,altura);
            // Fundo animado e botões independentes compartilham o mesmo enquadramento.
            DesenharRespiracaoMenu(imagemArea);AtualizarNavegacaoMouseMenu();
            if(InterfaceCena.habilitado && Event.current.type==EventType.KeyDown)
            {
                KeyCode chave=Event.current.keyCode;
                if(chave==KeyCode.DownArrow || chave==KeyCode.UpArrow)
                {
                    menuNavegacaoTeclado=true;menuSelecao=(menuSelecao+(chave==KeyCode.DownArrow?1:2))%3;
                    Toque();Event.current.Use();
                }
                else if(chave==KeyCode.Return || chave==KeyCode.KeypadEnter || chave==KeyCode.Space)
                {
                    AtivarMenuItem(menuSelecao);Event.current.Use();
                }
            }
            MenuArteBotao(0,"Jogar",imagemArea);
            MenuArteBotao(1,"Opções",imagemArea);
            MenuArteBotao(2,"Sair",imagemArea);
            InterfaceCena.matriz=anterior;
        }
        void MenuArteBotao(int indice,string rotulo,Rect imagemArea)
        {
            if(menuBotoesArte==null)menuBotoesArte=Resources.Load<Texture2D>("UI/MenuBotoes");
            Rect area=MenuBotaoArea(indice,imagemArea);
            float escala=1+menuBotaoDestaque[indice]*.045f;
            area=new Rect(area.center-area.size*escala/2,area.size*escala);
            // O clique acompanha a placa; os pingos transparentes não são área de clique.
            Rect cliqueArea=new Rect(area.x+area.width*.02f,area.y+area.height*.02f,area.width*.96f,area.height*.77f);
            bool mouseSobre=InterfaceCena.habilitado&&cliqueArea.Contains(Event.current.mousePosition);
            if(mouseSobre&&!menuNavegacaoTeclado)menuSelecao=indice;
            bool destaque=InterfaceCena.habilitado&&(menuNavegacaoTeclado?menuSelecao==indice:mouseSobre);
            menuBotaoAlvo[indice]=destaque;
            if(menuBotoesArte!=null)InterfaceCena.DesenharTexturaComCoordenadas(area,menuBotoesArte,MenuBotaoUv(indice,destaque));
            else UrbanoTexto(rotulo.ToUpper(),cliqueArea,30,Creme,TextAnchor.MiddleCenter);
            InterfaceCena.DefinirProximoControleNome("Menu"+rotulo);
            if(InterfaceCena.Botao(cliqueArea,new GUIContent("",rotulo),GUIStyle.none))AtivarMenuItem(indice);
        }
        void AtivarMenuItem(int indice)
        {
            Toque();
            if(indice==0)IniciarHistoria();
            else if(indice==1)opcoesAudio=true;
            else SairJogo();
        }
        void SairJogo()
        {
            Salvar(false);SalvarAudio();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying=false;
            #else
            Application.Quit();
            #endif
        }
    }
}
