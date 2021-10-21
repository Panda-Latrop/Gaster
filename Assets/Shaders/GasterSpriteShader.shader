Shader "Custom/GasterSpriteShader"
{
	Properties
	{

		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		[MaterialToggle] _SolidColor("Solid Color", Float) = 0
		[MaterialToggle] _GrayShades("Gray Shades", Float) = 0
		_GrayShadeColor("Gray Shade Color", Color) = (1,1,1,1)
		[MaterialToggle] _HalfColor("Half Color", Float) = 0
	}

		SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_MainTex, uv);
			#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D(_AlphaTex, uv).r;
			#endif
				return color;
			}

			float _SolidColor;
			float _GrayShades;
			float4 _GrayShadeColor;
			float _HalfColor;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
				if (_SolidColor > 0) {
					c = IN.color * IN.color.a * c.a;
				}
				else {
					if (_GrayShades > 0) {
						float m = max(c.r, max(c.g, c.b));
						c.r = m * _GrayShadeColor.r;
						c.g = m * _GrayShadeColor.g;
						c.b = m * _GrayShadeColor.b;
					}
					if (_HalfColor > 0) {
						c.rgb *= 0.5f;
					}
				}				
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}