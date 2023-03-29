// Colormaps texture generated using this python script:

// from PIL import Image
// import matplotlib.cm as cm
// import numpy as np
// names = ['viridis', 'plasma', 'inferno', 'jet', 'rainbow', 'coolwarm', 'flag', 'gray']
// def cm_array(m, size=256):
//     return cm.ScalarMappable(cmap=getattr(cm, m)).to_rgba(range(size), bytes=True).reshape(1, size, 4)
// Image.fromarray(np.vstack(map(cm_array, names)), mode='RGBA').save('colormaps.png')
// print ','.join(map(lambda x: x.title(), names))

Shader "Custom/Depth" {
	Properties {
		
		[PerRendererData]
		_MainTex ("MainTex", 2D) = "black" {}
		
		[HideInInspector]
		_Colormaps ("Colormaps", 2D) = "" {}

		_DepthScale("Depth Multiplier Factor to Meters", float) = 0.001 
		
		_MinRange("Min Range(m)", Range(0, 10)) = 0.15
		_MaxRange("Max Range(m)", Range(0, 20)) = 10.0

		[KeywordEnum(Viridis,Plasma,Inferno,Jet,Rainbow,Coolwarm,Flag,Gray)]
		_Colormap("Colormap", Float) = 0

	}
	SubShader {
		Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" }
		Pass {
			ZWrite Off
			Cull Off
			Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			// Depth texture
			sampler2D _MainTex;
			// Color coding colormaps
			sampler2D _Colormaps;
			float4 _Colormaps_TexelSize;
			
			// Selected colormap
			float _Colormap;
			// Minimum visible range
			float _MinRange;
			// Maximum visible range
			float _MaxRange;
			// Depth scale
			float _DepthScale;

			// Fragment shader
			half4 frag (v2f_img pix) : SV_Target
			{
				// Depth value to meters
				// [0..1] -> ushort -> meters
				float z = tex2D(_MainTex, pix.uv).r * 0xffff * _DepthScale;
				
				// If out of range -> discard fragment
				if (z > _MaxRange || z < _MinRange) {
					discard;
					return 0;
				}

				// Compute color coordinates
				z = (z - _MinRange) / (_MaxRange - _MinRange);
				if (z <= 0) {
					discard;
					return 0;
				}

				// Get color from colormap
				return tex2D(_Colormaps, float2(z, 1 - (_Colormap + 0.5) * _Colormaps_TexelSize.y));
			}
			ENDCG
		}
	}
	FallBack Off
}
