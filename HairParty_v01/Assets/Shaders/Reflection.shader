Shader "Custom/Reflection"
{
	Properties
	{
		_CubeMap("Cubemap", Cube) = "" {}
		//_RefractionIndex("Refraction Index", Float) = 0
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				half3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				half3 worldNormal : TEXCOORD1;
				float4 worldVertex : POSITION1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			samplerCUBE _CubeMap;
			float _RefractionIndex;

			fixed4 frag(v2f i) : SV_Target
			{
				// 
				float3 pos = i.worldVertex;
				float3 incident = pos - _WorldSpaceCameraPos;

				float3 normal = normalize(i.worldNormal);//normalize(float3(-i.worldNormal.x, i.worldNormal.y, i.worldNormal.z));
				float3 reflected = reflect(incident, normal);
				float4 col = 1.0;
				float4 val = texCUBE(_CubeMap, reflected);//UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, refracted);
				//col.xyz = DecodeHDR(val, unity_SpecCube0_HDR);
				//col.w = 1.0;
				col = val;
				return col;
			}
			ENDCG
		}
	}
}
