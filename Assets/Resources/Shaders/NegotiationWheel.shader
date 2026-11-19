Shader "DripLab/NegotiationWheel"
{
    Properties { _MainTex ("Artwork", 2D) = "white" {} _Angle ("Angle", Float) = 0 }
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
            fixed4 frag(v2f_img input) : SV_Target
            {
                float2 size=float2(1672,941);
                float2 center=float2(836,393);
                float2 wheelOffset=input.uv*size-center;
                float2 sampleUv=input.uv;
                if(dot(wheelOffset,wheelOffset)<257.0*257.0)
                {
                    float s=sin(_Angle),c=cos(_Angle);
                    sampleUv=(center+float2(c*wheelOffset.x-s*wheelOffset.y,s*wheelOffset.x+c*wheelOffset.y))/size;





                }
                return tex2D(_MainTex,sampleUv);
            }
            ENDCG
        }
    }
}
