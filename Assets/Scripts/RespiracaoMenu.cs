using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        [UnityEngine.Serialization.FormerlySerializedAs("menuBreathingStrength")] [Header("Animação do menu")]
        [SerializeField,Range(0,2)] float intensidadeRespiracaoMenu=1;
        [UnityEngine.Serialization.FormerlySerializedAs("menuBreathingSpeed")] [SerializeField,Range(.25f,2)] float velocidadeRespiracaoMenu=1;
        Material menuRespiracaoMaterial;
        RenderTexture menuRespiracaoQuadro;
        bool menuAnimacaoVerificado;

        void Update()
        {
            AtualizarCarregamento();
            AtualizarHistoria();
            AtualizarBancada();
            AtualizarTecidoNegociacao();
            AtualizarRelogioDia();
            AtualizarRoletaApostas();
            AtualizarPaisagemSonora();
            AtualizarDestaqueMenu();AtualizarDestaqueVoltarAudio(); // Crescimento suave ao destacar cada botão.
            if(!menuAberto || principalMenuArte==null)return;
            if(!menuAnimacaoVerificado)
            {
                menuAnimacaoVerificado=true;
                var sombreador=Resources.Load<Shader>("Shaders/MenuBreathing");
                if(sombreador!=null && sombreador.isSupported)
                {
                    menuRespiracaoMaterial=new Material(sombreador){hideFlags=HideFlags.HideAndDontSave};
                    menuRespiracaoQuadro=new RenderTexture(principalMenuArte.width,principalMenuArte.height,0,RenderTextureFormat.ARGB32)
                    {
                        name="DripLab - Menu animado",
                        filterMode=FilterMode.Bilinear,
                        wrapMode=TextureWrapMode.Clamp,
                        useMipMap=false,
                        hideFlags=HideFlags.HideAndDontSave
                    };
                    if(!menuRespiracaoQuadro.Create())
                    {
                        Destroy(menuRespiracaoQuadro);menuRespiracaoQuadro=null;
                        Debug.LogWarning("Renderização do menu animado indisponível. Exibindo a arte original.");
                    }
                }
                else Debug.LogWarning("Shader do menu animado indisponível. Exibindo a arte original.");
            }
            if(menuRespiracaoMaterial==null || menuRespiracaoQuadro==null)
                return;
            menuRespiracaoMaterial.SetFloat("_MenuTime",Time.unscaledTime*velocidadeRespiracaoMenu);
            menuRespiracaoMaterial.SetFloat("_Strength",intensidadeRespiracaoMenu);
            // Os botões são desenhados separadamente e não sofrem a pulsação do fundo.
            RenderTexture anterior=RenderTexture.active;
            // One source sample per pixel, no per-frame textures or readbacks.
            try {Graphics.Blit(principalMenuArte,menuRespiracaoQuadro,menuRespiracaoMaterial);}
            finally {RenderTexture.active=anterior;}
        }
        void DesenharRespiracaoMenu(Rect area)
        {
            InterfaceCena.DesenharTextura(area,menuRespiracaoQuadro!=null?(Texture)menuRespiracaoQuadro:principalMenuArte,ScaleMode.StretchToFill);
        }
        void OnDestroy()
        {
            LiberarPaisagemSonora(); LiberarArteNegociacao(); LiberarArteRoletaApostas(); if(menuRespiracaoQuadro!=null){menuRespiracaoQuadro.Release();Destroy(menuRespiracaoQuadro);}
            if(menuRespiracaoMaterial!=null)Destroy(menuRespiracaoMaterial);
            if(capsulaMascara!=null)Destroy(capsulaMascara);
            if(discoMascara!=null)Destroy(discoMascara);
            if(tecidoRoletaArte!=null)Destroy(tecidoRoletaArte);
            if(roletaRoletaArte!=null)Destroy(roletaRoletaArte);
        }
    }
}
