Shader "Sprites/Outline"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0

			// Add values to determine if outlining is enabled and outline color.
			[PerRendererData] _Outline("Outline", Float) = 0
			[PerRendererData] _NewOutlineColor("New Outline Color", Color) = (1,1,1,1)
			[PerRendererData] _OldOutlineColor("Old Outline Color", Color) = (1,1,1,1)
			[PerRendererData] _OutlineSize("Outline Size", int) = 1
			[PerRendererData] _SpriteColor("Sprite Color", Color) = (1,1,1,0)
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
				#pragma vertex SpriteVert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile _ PIXELSNAP_ON
				#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
				#include "UnitySprites.cginc"

				float _Outline;
				fixed4 _NewOutlineColor;
				fixed4 _OldOutlineColor;
				fixed4 _SpriteColor;
				int _OutlineSize;
				float4 _MainTex_TexelSize;
				float _SpriteFading;

				fixed4 frag(v2f IN) : SV_Target
				{
					fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;

					// If outline is enabled and there is a pixel, try to draw an outline.
				/*if (_Outline > 0 && c.a == 0)
				{
					//float totalAlpha = 0.0;

					[unroll(16)]
					for (int i = 1; i < _OutlineSize + 1; i++)
					{
						fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + fixed2(0, i * _MainTex_TexelSize.y));
						fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0, i * _MainTex_TexelSize.y));
						fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2(i * _MainTex_TexelSize.x, 0));
						fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2(i * _MainTex_TexelSize.x, 0));

						totalAlpha = totalAlpha + pixelUp.a + pixelDown.a + pixelRight.a + pixelLeft.a;
					}

					if (totalAlpha != 0)
					{
						c.rgba = fixed4(1, 1, 1, 1) * _OutlineColor;
					}
				}
				else if (c.a != 0)
					c.rgba = fixed4(_SpriteColor.r, _SpriteColor.g, _SpriteColor.b, 1 - _SpriteColor.a) + tex2D(_MainTex, IN.texcoord) * _SpriteColor.a;*/
					
					if(_Outline > 0 && c.r == _OldOutlineColor.r && c.g == _OldOutlineColor.g && c.b == _OldOutlineColor.b && c.a > 0)
						c.rgba = fixed4(1,1,1,1) * _NewOutlineColor;
					else if (c.a != 0)
						c.rgba = (fixed4(_SpriteColor.r, _SpriteColor.g, _SpriteColor.b, 1 - _SpriteColor.a) + tex2D(_MainTex, IN.texcoord) * _SpriteColor.a);

						if(c.a>0)
							c.a = _SpriteFading;

					c.rgb *= c.a;

					return c;
				}
		ENDCG
		}
		}
}
