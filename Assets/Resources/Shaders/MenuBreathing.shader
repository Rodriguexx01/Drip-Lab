Shader "DripLab/MenuBreathing"
{
    Properties
    {
        _MainTex ("Fundo do menu", 2D) = "white" {}
        _MenuTime ("Tempo", Float) = 0
        _Strength ("Intensidade", Range(0,2)) = 1
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float _MenuTime, _Strength;

            float region(float2 p,float2 center,float2 radius)
            {
                float2 d=(p-center)/radius;
                return 1-smoothstep(.30,1.0,dot(d,d));
            }
            float2 breathingOffset(float2 p,float time)
            {
                // Coordenadas com origem no canto superior esquerdo da nova arte.
                float2 logo=float2(.214,.165),cristo=float2(.745,.155);
                float logoMask=region(p,logo,float2(.215,.19));
                float cristoMask=region(p,cristo,float2(.175,.19));
                float2 dg=float2(.30,.63);
                float dgMask=region(p,dg,float2(.24,.385));
                float cachorroMask=region(p,float2(.53,.84),float2(.16,.17));
                dgMask*=(1-logoMask)*(1-cachorroMask);
                float pulso=sin(time*1.25);
                float2 motion=-(p-logo)*(.012*pulso*logoMask);
                motion-=(p-cristo)*(.018*pulso*cristoMask);
                motion-=(p-dg)*(.010*pulso*dgMask);
                motion.y-=.0012*pulso*dgMask;
                float borda=min(min(p.x,1-p.x),min(p.y,1-p.y));
                return motion*smoothstep(0,.025,borda)*_Strength;
            }
            fixed4 frag(v2f_img input) : SV_Target
            {
                float2 p=float2(input.uv.x,1-input.uv.y);
                float2 moved=p+breathingOffset(p,_MenuTime);
                return tex2D(_MainTex,float2(moved.x,1-moved.y));
            }
            ENDCG
        }
    }
    Fallback Off
}
