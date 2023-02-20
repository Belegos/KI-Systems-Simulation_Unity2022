Shader "Unlit/ScreenCutoutShader_HDRP"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }

        SubShader
    {
        Tags
        {
            "RenderPipeline" = "HDRP"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite On
        ZTest LEqual

        HLSLPROGRAM
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Core.hlsl"

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            float4 screenPos : TEXCOORD1;
            float2 uv : TEXCOORD0;
        };

        sampler2D _MainTex;

        v2f vert(appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.screenPos = ComputeScreenPos(o.vertex);
            o.uv = v.uv;
            return o;
        }

        fixed4 frag(v2f i) : SV_Target
        {
            i.screenPos /= i.screenPos.w;
            fixed4 col = tex2D(_MainTex, float2(i.screenPos.x, i.screenPos.y));
            col.a = (col.r + col.g + col.b) / 3; // Set the alpha channel to the average of RGB values
            return col;
        }
        ENDHLSL
    }
        FallBack "Diffuse"
}
