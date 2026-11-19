using UnityEngine;

namespace DripLab
{
    [DisallowMultipleComponent]
    public sealed class ElementoInterfaceCena : MonoBehaviour
    {
        [UnityEngine.Serialization.FormerlySerializedAs("binding")] [HideInInspector] public string vinculo;
        [UnityEngine.Serialization.FormerlySerializedAs("hidden")] [Header("Elemento real da interface")]
        public bool oculto;
        [UnityEngine.Serialization.FormerlySerializedAs("overrideLayout")] [Tooltip("Substitui a área padrão. Coordenadas em pixels da tela de referência.")]
        public bool substituirDisposicao;
        [UnityEngine.Serialization.FormerlySerializedAs("layout")] public Rect disposicao;
        [UnityEngine.Serialization.FormerlySerializedAs("overrideText")] public bool substituirTexto;
        [UnityEngine.Serialization.FormerlySerializedAs("text")] [TextArea] public string texto;
        [UnityEngine.Serialization.FormerlySerializedAs("textureOverride")] public Texture texturaPersonalizada;
        [UnityEngine.Serialization.FormerlySerializedAs("tint")] public Color tonalidade=Color.white;
        [UnityEngine.Serialization.FormerlySerializedAs("sourceLayout")] [Header("Dados recebidos do jogo")]
        public Rect areaOriginal;
        [UnityEngine.Serialization.FormerlySerializedAs("sourceText")] [TextArea] public string textoOriginal;
        [UnityEngine.Serialization.FormerlySerializedAs("sourceTexture")] public Texture texturaOriginal;
        [UnityEngine.Serialization.FormerlySerializedAs("visibleInCurrentScreen")] public bool visivelNaTelaAtual;
        public Rect Area(Rect origem)
        {
            Rect area=substituirDisposicao?disposicao:origem;
            Vector3 deslocamento=transform.position,escala=transform.lossyScale;
            Vector2 centro=area.center+new Vector2(deslocamento.x,-deslocamento.y);
            Vector2 tamanho=new Vector2(area.width*Mathf.Abs(escala.x),area.height*Mathf.Abs(escala.y));
            return new Rect(centro-tamanho/2,tamanho);
        }
        void OnDrawGizmosSelected()
        {
            Rect r=Area(areaOriginal);Gizmos.color=new Color(1,.7f,0,1);
            Gizmos.DrawWireCube(new Vector3((r.center.x-836)/100,(470.5f-r.center.y)/100,0),new Vector3(r.width/100,r.height/100,.01f));
        }
    }
}
