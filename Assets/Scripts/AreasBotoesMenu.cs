using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        static readonly Rect[] MenuBotaoAreasClique={new Rect(1160,350,460,190),new Rect(1160,535,460,190),new Rect(1160,710,460,190)};
        static readonly float[] MenuBotoesLinhas={390,1600,2810};
        Texture2D menuBotoesArte;
        readonly float[] menuBotaoDestaque=new float[3];
        readonly bool[] menuBotaoAlvo=new bool[3];
        bool menuNavegacaoTeclado,menuMouseConhecido;
        Vector2 menuUltimoMouse;

        Rect MenuBotaoArea(int indice,Rect imagemArea)
        {
            Rect origem=MenuBotaoAreasClique[indice];
            float sx=imagemArea.width/1672f,sy=imagemArea.height/941f;
            Vector2 centro=new Vector2(imagemArea.x+origem.center.x*sx,imagemArea.y+origem.center.y*sy);
            Vector2 tamanho=new Vector2(origem.width*sx,origem.height*sy);
            return new Rect(centro-tamanho/2,tamanho);
        }
        Rect MenuBotaoUv(int indice,bool laranja)
        {
            // Células do atlas original, sem recortar ou alterar a imagem fornecida.
            return new Rect((laranja?3260f:260f)/6263f,1-(MenuBotoesLinhas[indice]+1140)/4175f,2760f/6263f,1140f/4175f);
        }
        void AtualizarNavegacaoMouseMenu()
        {
            Vector2 posicao=Event.current.mousePosition;
            if(menuMouseConhecido&&posicao!=menuUltimoMouse)menuNavegacaoTeclado=false;
            menuMouseConhecido=true;menuUltimoMouse=posicao;
        }
        void AtualizarDestaqueMenu()
        {
            for(int i=0;i<3;i++)
            {
                if(!menuAberto||opcoesAudio)menuBotaoAlvo[i]=false;
                menuBotaoDestaque[i]=Mathf.MoveTowards(menuBotaoDestaque[i],menuBotaoAlvo[i]?1:0,Time.unscaledDeltaTime*10);
            }
        }
    }
}
