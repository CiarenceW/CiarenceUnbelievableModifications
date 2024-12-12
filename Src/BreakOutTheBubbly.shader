Shader "Custom/BreakOutTheBubbly"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        [NoScaleOffset] _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Roughness ("Roughness", 2D) = "black" {}
		_NoiseTex ("Noise Texture", 2D) = "white" {}
		_BubblyMultiplier ("Bubbly Multiplier", float) = 1
		[Normal] [NoScaleOffset] _Normal ("Normal Map", 2D) = "bump" {}
		[HDR] [NoScaleOffset] _Emission ("Emission", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _Normal;
		sampler2D _Emission;
		sampler2D _NoiseTex;

        struct Input
        {
            float2 uv_MainTex;
        };

		half _BubblyMultiplier;
        half _Roughness;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
			half moveTime = (_Time.x * _BubblyMultiplier) % 1;
			float2 offsetMainTex = IN.uv_MainTex;
			offsetMainTex.y += moveTime;
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color * (tex2D (_NoiseTex, offsetMainTex) * 2);
            o.Albedo = c.rgb;
            o.Metallic = 0;
            o.Smoothness = 1 - _Roughness; //invert :)
			o.Emission = tex2D (_Emission, IN.uv_MainTex) - ((tex2D (_NoiseTex, offsetMainTex) + 0.01) * 0.5);
			o.Normal = tex2D (_Normal, IN.uv_MainTex);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
