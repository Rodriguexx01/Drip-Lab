Shader "DripLab/RoletaApostas"
{
    Properties { _MainTex ("Arte", 2D) = "white" {} _Angle ("Angulo", Float) = 0 }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float _Angle;
            fixed4 frag(v2f_img entrada) : SV_Target
            {
                float2 tamanho=float2(1672,941);
                // As coordenadas UV começam na parte inferior da textura.
                float2 centro=float2(627,475);
                float2 deslocamento=entrada.uv*tamanho-centro;
                float raio=length(deslocamento);
                fixed4 original=tex2D(_MainTex,entrada.uv);
                // Somente o anel de números e casas gira; a arte restante fica intacta.
                if(raio<=132.0 || raio>=204.0)return original;
                float seno=sin(_Angle),cosseno=cos(_Angle);
                float2 uv=(centro+float2(cosseno*deslocamento.x-seno*deslocamento.y,seno*deslocamento.x+cosseno*deslocamento.y))/tamanho;
                float mascara=smoothstep(132.0,133.0,raio)*(1.0-smoothstep(203.0,204.0,raio));
                return lerp(original,tex2D(_MainTex,uv),mascara);
            }
            ENDCG
        }
    }
}
