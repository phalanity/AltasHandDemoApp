Shader "Custom/PureTextureShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		_uvTranslation("uvTranslation", Vector) = (0,0,0,0)
		_uvScale("uvScale", Vector) = (1,1,1,1)
		_uvRotation("uvRotation", Float) = 0
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

		Lighting Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf NoLighting  fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;
		float4 _uvTranslation;
		float4 _uvScale;
		float _uvRotation;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
		
		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
			return fixed4(s.Albedo, s.Alpha);
		}
		float2 rotateVec2(float2 Base, float Degree)
		{
			float2 rtn;
			float rad = 0.0174532925 * Degree;
			rtn.x = cos(rad) * Base.x - sin(rad) * Base.y;
			rtn.y = sin(rad) * Base.x + cos(rad) * Base.y;

			return rtn;
		}

        void surf (Input IN, inout SurfaceOutput o)
        {
			float2 uvMinusOneToPlusOne = (IN.uv_MainTex - 0.5) * 2;

			float2 uvFinal = rotateVec2(uvMinusOneToPlusOne, _uvRotation) / 2 + 0.5;
			uvFinal.x /= _uvScale.x;
			uvFinal.y /= _uvScale.y;
			uvFinal += _uvTranslation.xy;
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, uvFinal) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }		
        ENDCG
    }
    FallBack "Diffuse"
}
