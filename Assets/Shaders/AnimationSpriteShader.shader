Shader "Custom/AnimationSpriteShader"
{
	Properties
	{

		_MainTex("Sprite Texture", 2D) = "white" {}
		_Cols("Cols Count", Int) = 3
		_Rows("Rows Count", Int) = 1
		_Frame("Per Frame Length", Float) = 0.5
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
				uint _Cols;
				uint _Rows;
				float _Frame;


				fixed4 SampleSpriteTexture(sampler2D tex, float2 texcoord, float dx, float dy, int frame) {
					fixed4 color = tex2D(tex, float2(
						(texcoord.x * dx) + fmod(frame, _Cols) * dx,
						1.0 - ((texcoord.y * dy) + (frame / _Cols) * dy)
						));
					#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
					if (_AlphaSplitEnabled)
						color.a = tex2D(_AlphaTex, uv).r;
					#endif
					return color;
				}

				fixed4 frag(v2f IN) : SV_Target
				{
					int frames = _Rows * _Cols;
					float frame = fmod(_Time.y / _Frame, frames);
					int current = floor(frame);
					float dx = 1.0 / _Cols;
					float dy = 1.0 / _Rows;
					fixed4 color = SampleSpriteTexture(_MainTex, IN.texcoord, dx, dy, current) * IN.color;
					color.rgb *= color.a;
					return color;
				}
			ENDCG
			}
		}
}