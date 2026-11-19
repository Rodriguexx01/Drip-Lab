using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DripLab
{
    // The hierarchy objects drive these controls; disabling or editing an object changes rendering.
    public static class InterfaceCena
    {
        static readonly Dictionary<string,ElementoInterfaceCena> elementos=new Dictionary<string,ElementoInterfaceCena>();
        static readonly Dictionary<string,int> contadores=new Dictionary<string,int>();
        static readonly List<ElementoInterfaceCena> visivel=new List<ElementoInterfaceCena>();
        static Transform raiz;
        static bool indexado; public static Action AoClicar;
        static int profundidade;
        static string tela="Atelie";
        public static Color cor {get=>UnityEngine.GUI.color;set=>UnityEngine.GUI.color=value;}
        public static Matrix4x4 matriz {get=>UnityEngine.GUI.matrix;set=>UnityEngine.GUI.matrix=value;}
        public static bool habilitado {get=>UnityEngine.GUI.enabled;set=>UnityEngine.GUI.enabled=value;}
        public static GUISkin tema=>UnityEngine.GUI.skin;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reiniciar(){AoClicar=null;elementos.Clear();contadores.Clear();visivel.Clear();raiz=null;indexado=false;profundidade=0;}
        public static void IniciarQuadro()
        {
            contadores.Clear();profundidade=0;
            foreach(var item in visivel)if(item!=null)item.visivelNaTelaAtual=false;
            visivel.Clear();
        }
        public static void DefinirTela(string nome){tela=nome;}
        static Transform Grupo(string caminho)
        {
            if(raiz==null)
            {
                foreach(var transformacao in UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include))
                    if(transformacao.parent==null&&transformacao.name=="DRIP LAB - Interface"){raiz=transformacao;break;}
                if(raiz==null)raiz=new GameObject("DRIP LAB - Interface").transform;
            }
            Transform pai=raiz;
            foreach(string parte in caminho.Split('/'))
            {var filho=pai.Find(parte);if(filho==null){filho=new GameObject(parte).transform;filho.SetParent(pai,false);}pai=filho;}
            return pai;
        }
        public readonly struct ElementoEscopo : IDisposable
        {
            public readonly Rect Area;
            public readonly string Texto;
            public readonly ElementoInterfaceCena No;
            public bool Visivel=>No==null||(!No.oculto&&No.enabled&&No.gameObject.activeInHierarchy);
            public Texture Textura(Texture alternativa)=>No!=null&&No.texturaPersonalizada!=null?No.texturaPersonalizada:alternativa;
            public ElementoEscopo(Rect area,string texto,ElementoInterfaceCena no){Area=area;Texto=texto;No=no;}
            public void Dispose(){profundidade--;}
        }
        public static ElementoEscopo Elemento(string categoria,Rect area,string rotulo="",Texture textura=null,[CallerFilePath]string arquivo="",[CallerLineNumber]int linha=0)
        {
            if(profundidade++>0)return new ElementoEscopo(area,rotulo,null);
            if(!indexado)
            {
                foreach(var no in UnityEngine.Object.FindObjectsByType<ElementoInterfaceCena>(FindObjectsInactive.Include))
                    if(!string.IsNullOrEmpty(no.vinculo))elementos[no.vinculo]=no;
                indexado=true;
            }
            string prefixo=tela+"/"+System.IO.Path.GetFileNameWithoutExtension(arquivo)+":"+linha+":"+categoria;
            int ocorrencia;contadores.TryGetValue(prefixo,out ocorrencia);contadores[prefixo]=ocorrencia+1;
            string chave=prefixo+":"+ocorrencia;
            ElementoInterfaceCena elemento;
            if(!elementos.TryGetValue(chave,out elemento)||elemento==null)
            {
                string nome=string.IsNullOrEmpty(rotulo)?categoria:categoria+" - "+rotulo.Replace('\n',' ');
                if(nome.Length>75)nome=nome.Substring(0,75);
                var objeto=new GameObject(nome);objeto.transform.SetParent(Grupo(tela),false);
                elemento=objeto.AddComponent<ElementoInterfaceCena>();elemento.vinculo=chave;elementos[chave]=elemento;
            }
            elemento.areaOriginal=area;elemento.textoOriginal=rotulo;elemento.texturaOriginal=textura;elemento.visivelNaTelaAtual=true;visivel.Add(elemento);
            return new ElementoEscopo(elemento.Area(area),elemento.substituirTexto?elemento.texto:rotulo,elemento);
        }
        public static void DesenharTextura(Rect r,Texture textura,ScaleMode modo=ScaleMode.StretchToFill,bool alfaMistura=true,float proporcao=0,[CallerFilePath]string arquivo="",[CallerLineNumber]int linha=0)
        {
            if(Event.current.type!=EventType.Repaint)return;
            using(var e=Elemento("Imagem",r,textura!=null?textura.name:"Imagem",textura,arquivo,linha))
            {if(!e.Visivel)return;Color anterior=cor;if(e.No!=null)cor*=e.No.tonalidade;UnityEngine.GUI.DrawTexture(e.Area,e.Textura(textura),modo,alfaMistura,proporcao);cor=anterior;}
        }
        public static void DesenharTexturaComCoordenadas(Rect r,Texture textura,Rect uv,bool alfaMistura=true,[CallerFilePath]string arquivo="",[CallerLineNumber]int linha=0)
        {
            if(Event.current.type!=EventType.Repaint)return;
            using(var e=Elemento("Imagem",r,textura!=null?textura.name:"Imagem",textura,arquivo,linha))
            {if(!e.Visivel)return;Color anterior=cor;if(e.No!=null)cor*=e.No.tonalidade;UnityEngine.GUI.DrawTextureWithTexCoords(e.Area,e.Textura(textura),uv,alfaMistura);cor=anterior;}
        }
        public static void Rotulo(Rect r,string texto,GUIStyle estilo,[CallerFilePath]string arquivo="",[CallerLineNumber]int linha=0)
        {if(Event.current.type!=EventType.Repaint)return;using(var e=Elemento("Texto",r,texto,null,arquivo,linha)){if(!e.Visivel)return;UnityEngine.GUI.Label(e.Area,e.Texto,estilo);}}
        public static bool Botao(Rect r,GUIContent conteudo,GUIStyle estilo,[CallerFilePath]string arquivo="",[CallerLineNumber]int linha=0)
        {using(var e=Elemento("Botao",r,string.IsNullOrEmpty(conteudo.text)?conteudo.tooltip:conteudo.text,null,arquivo,linha)){bool pressionado=e.Visivel&&UnityEngine.GUI.Button(e.Area,new GUIContent(e.No!=null&&e.No.substituirTexto?e.Texto:conteudo.text,conteudo.tooltip),estilo);if(pressionado)AoClicar?.Invoke();return pressionado;}}
        public static bool Botao(Rect r,string texto,GUIStyle estilo,[CallerFilePath]string arquivo="",[CallerLineNumber]int linha=0)=>Botao(r,new GUIContent(texto),estilo,arquivo,linha);
        public static void Caixa(Rect r,GUIContent conteudo,GUIStyle estilo,[CallerFilePath]string arquivo="",[CallerLineNumber]int linha=0)
        {if(Event.current.type!=EventType.Repaint)return;using(var e=Elemento("Painel",r,conteudo.text,null,arquivo,linha)){if(e.Visivel)UnityEngine.GUI.Box(e.Area,conteudo,estilo);}}
        public static float HorizontalDeslizante(Rect r,float valor,float esquerda,float direita,GUIStyle trilho,GUIStyle marcador,[CallerFilePath]string arquivo="",[CallerLineNumber]int linha=0)
        {using(var e=Elemento("Slider",r,"Volume",null,arquivo,linha)){return e.Visivel?UnityEngine.GUI.HorizontalSlider(e.Area,valor,esquerda,direita,trilho,marcador):valor;}}
        public static void IniciarGrupo(Rect area){UnityEngine.GUI.BeginGroup(area);}
        public static void FimGrupo(){UnityEngine.GUI.EndGroup();}
        public static void DefinirProximoControleNome(string nome){UnityEngine.GUI.SetNextControlName(nome);}
    }
}
