Shader "Custom/Pacha"
{
	Properties
	{
		//_CubeMap("Cubemap", Cube) = "" {}
		//_RefractionIndex("Refraction Index", Float) = 0
		_LightPosition("Light Position", Vector) = (0, 0, 0)
		_Specular("specular", Vector) = (1, 1, 1)
		_Shininess("Shininess", Float) = 1

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

			float3 _Specular;
			float _Shininess;
			float3 _LightPosition;

			fixed4 frag(v2f i) : SV_Target
			{
				float3 pos = i.worldVertex;
				float3 normal = normalize(i.worldNormal);
				float3 lightDir = normalize(pos - _LightPosition);

				float3 surf2light = normalize(lightDir - pos);

				float3 surf2view = normalize(-pos);
				float3 reflection = reflect(-surf2light, normal);
				float scont = pow(1.0 - max(0.0, dot(surf2view, reflection)), _Shininess);
				float attenuation = distance(pos, _LightPosition);

				float4 col = fixed4(1.0, 0.0, 0.0, 1.0);
				if (scont > 0.9999)
					col = fixed4(1.0, 1.0, 1.0, 1.0);
				//float4 col = 1.0;
				//float4 val = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, refracted);
				//col.xyz = DecodeHDR(val, unity_SpecCube0_HDR);
				//col.w = 1.0;
				return col;
			}
			ENDCG
		}
	}
}
