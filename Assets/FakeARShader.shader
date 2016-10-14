Shader "Custom/FakeAR"
{
	Properties{
		_MainTex("", 2D) = "white" {}
	}

	SubShader{

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc" 
			//we include "UnityCG.cginc" to use the appdata_img struct

			struct v2f {
				float4 pos : POSITION;
				half2 uv : TEXCOORD0;
			};

			//Our Vertex Shader 
			v2f vert(appdata_img v) {
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
				return o;
			}

			sampler2D _MainTex;
	
			fixed4 frag(v2f i) : COLOR {
				// Get the orginal rendered color
				fixed4 orgCol = tex2D(_MainTex, abs(i.uv * 2.0 - 1.0));
				float avg = (orgCol.r + orgCol.g + orgCol.b) / 3.0;
				fixed4 col = fixed4(avg, avg, avg, 1);

				return orgCol;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
