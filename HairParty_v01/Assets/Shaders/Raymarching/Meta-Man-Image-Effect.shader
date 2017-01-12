Shader "RayMarching/Meta-Man-Image-Effect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			#define PI 3.14159

			#define RADIANS(x) ((x) * (PI / 180.0))

			#define TETRAHEDRON_DIHEDRAL_ANGLE RADIANS(70.53)
			#define HEXAHEDRON_DIHEDRAL_ANGLE RADIANS(90.0)
			#define OCTAHEDRON_DIHEDRAL_ANGLE RADIANS(109.47)
			#define DODECAHEDRON_DIHEDRAL_ANGLE RADIANS(116.57)
			#define ICOSAHEDRON_DIHEDRAL_ANGLE RADIANS(138.19)

			#define TETRAHEDRON_SCHLAFLI_SYMBOL float2(3.0, 3.0)
			#define HEXAHEDRON_SCHLAFLI_SYMBOL float2(4.0, 3.0)
			#define OCTAHEDRON_SCHLAFLI_SYMBOL float2(3.0, 4.0)
			#define DODECAHEDRON_SCHLAFLI_SYMBOL float2(5.0, 3.0)
			#define ICOSAHEDRON_SCHLAFLI_SYMBOL float2(3.0, 5.0)

			// Provided by our script
			uniform float4x4 _FrustumCornersES;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;
			uniform float4x4 _CameraInvViewMatrix;
			uniform float3 _CameraWS;

			uniform float3 _LeftHand;
			uniform float3 _RightHand;

			// Input to vertex shader
			struct appdata
			{
				// Remember, the z value here contains the index of _FrustumCornersES to use
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			// Output of vertex shader / input to fragment shader
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 ray : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;

				// Index passed via custom blit function in RaymarchGeneric.cs
				half index = v.vertex.z;
				v.vertex.z = 0.1;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv.xy;

				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv.y = 1 - o.uv.y;
				#endif

				// Get the eyespace view ray (normalized)
				o.ray = _FrustumCornersES[(int)index].xyz;

				// Transform the ray from eyespace to worldspace
				// Note: _CameraInvViewMatrix was provided by the script
				o.ray = mul(_CameraInvViewMatrix, o.ray);
				return o;
			}

			samplerCUBE _CubeMap;

			//fragment shader functions
			float cot(float x) {
				return 1.0 / tan(x);
			}

			//basic rotation functions for drawing our meta-solids
			float3 rotx(float3 p, float a)
			{
				float s = sin(a), c = cos(a);
				return float3(p.x, c * p.y - s * p.z, s * p.y + c * p.z);
			}

			float3 roty(float3 p, float a)
			{
				float s = sin(a), c = cos(a);
				return float3(c * p.x + s * p.z, p.y, -s * p.x + c * p.z);
			}

			float3 rotz(float3 p, float a)
			{
				float s = sin(a), c = cos(a);
				return float3(c * p.x - s * p.y, s * p.x + c * p.y, p.z);
			}

			float getInradius(float2 pq, float diha, float radius) {
				float tn = tan(diha * 0.5);
				float a = 2.0 * radius / (tan(PI / pq.y) * tn);
				float r = 0.5 * a * cot(PI / pq.x) * tn;

				return r;
			}

			//Raymarching drawing meta solids
			float tetrahedron(float3 p, float radius) {
				float diha = -RADIANS(180.0 - 70.53); // 180 - "Dihedral angle"
				float tria = -RADIANS(60.0); // triangle angle
				float inra = getInradius(TETRAHEDRON_SCHLAFLI_SYMBOL, TETRAHEDRON_DIHEDRAL_ANGLE, radius);

				float d = p.x - inra;

				p = rotz(p, diha);
				d = max(d, p.x - inra);

				p = rotx(p, tria);
				p = rotz(p, diha);

				d = max(d, p.x - inra);

				p = rotx(p, -tria);
				p = rotz(p, diha);
				d = max(d, p.x - inra);

				return d;
			}

			float hexahedron(float3 p, float radius) {
				float inra = getInradius(HEXAHEDRON_SCHLAFLI_SYMBOL, HEXAHEDRON_DIHEDRAL_ANGLE, radius);

				float d = abs(p.x) - inra;

				p = rotz(p, 1.5708); // 90 degrees
				d = max(d, abs(p.x) - inra);

				p = roty(p, 1.5708); // 90 degrees
				d = max(d, abs(p.x) - inra);

				return d;
			}

			float octahedron(float3 p, float radius) {
				float d = -1e5;

				float inra = getInradius(OCTAHEDRON_SCHLAFLI_SYMBOL, OCTAHEDRON_DIHEDRAL_ANGLE, radius);

				for (float i = 0.0; i < 4.0; i++) {
					p = rotz(p, 1.231); // 70.53110 degrees
					p = rotx(p, 1.047); // 60 degrees

					d = max(d, max(p.x - inra, -p.x - inra));
				}
				return d;
			}

			float dodecahedron(float3 p, float radius) {
				float d = -1e5;

				float inra = getInradius(DODECAHEDRON_SCHLAFLI_SYMBOL, DODECAHEDRON_DIHEDRAL_ANGLE, radius);

				for (float i = 0.0; i <= 4.0; i++) {
					p = roty(p, 0.81); // 46.40958 degrees
					p = rotx(p, 0.759); // 43.48750 degrees
					p = rotz(p, 0.3915); // 22.43130 degrees

					d = max(d, max(p.x - inra, -p.x - inra));
				}

				p = roty(p, 0.577); // 33.05966 degrees
				p = rotx(p, -0.266); // -15.24068 degrees
				p = rotz(p, -0.848); // -48.58682 degrees

				d = max(d, max(p.x - inra, -p.x - inra));

				return d;
			}

			float icosahedron(float3 p, float radius) {
				float d = -1e5;

				//center band
				const float n1 = 0.7297; // 41.80873 degrees
				const float n2 = 1.0472; // 60 degrees

				float inra = getInradius(ICOSAHEDRON_SCHLAFLI_SYMBOL, ICOSAHEDRON_DIHEDRAL_ANGLE, radius);

				for (float i = 0.0; i < 5.0; i++) {

					if (fmod(i, 2.0) == 0.0) {
						p = rotz(p, n1);
						p = rotx(p, n2);
					}
					else {
						p = rotz(p, n1);
						p = rotx(p, -n2);
					}
					d = max(d, max(p.x - inra, -p.x - inra));
				}

				p = roty(p, 1.048); // 60.04598 degrees
				p = rotz(p, 0.8416); // 48.22013 degrees
				p = rotx(p, 0.7772); // 44.53028 degrees

									 //top caps
				for (float j = 0.0; j < 5.0; j++) {
					p = rotz(p, n1);
					p = rotx(p, n2);

					d = max(d, max(p.x - inra, -p.x - inra));
				}
				return d;
			}

			float sphere(float3 p, float r)
			{
				float3 _Centre = float3(0.0, 0.0, 0.0);
				return distance(p, _Centre) - r;
			}

			//meta balls merging function
			float smin(float a, float b, float k)
			{
				float res = exp(-k*a) + exp(-k*b);
				return -log(res) / k;
			}


			float map(float3 pos) {
				float t1 = sphere(pos + _LeftHand * 5.0 + float3(0.0, 0.0, 10.0), 1.0);//dodecahedron(pos + _TargetVector, 0.5);
				t1 = smin(t1, sphere(pos + _RightHand * 5.0 + float3(0.0, 0.0, 10.0), 1.0), 5.0);
				return t1;
			}

			float3 calcNormal(in float3 pos) {
				float3 eps = float3(0.001, 0.0, 0.0);
				float3 nor = float3(
					map(pos + eps.xyy).x - map(pos - eps.xyy).x,
					map(pos + eps.yxy).x - map(pos - eps.yxy).x,
					map(pos + eps.yyx).x - map(pos - eps.yyx).x);
				return normalize(nor);
			}

			void renderColor(float3 viewRay, inout float3 color, float3 currPos)
			{
				float3 originalColor = color;
				float3 normal = calcNormal(currPos);

				float normalDotLight = abs(dot(-viewRay, normal));

				float3 incident = currPos - _WorldSpaceCameraPos;

				float3 reflected = reflect(incident, normal);
				float4 val = texCUBE(_CubeMap, reflected);

				float rim = pow(1.0 - normalDotLight, 3.0);
				color = lerp(val.xyz * 2.0, color, rim);
				color += rim;

				//color = val.xyz;
			}

			fixed4 raymarch(float3 ro, float3 rd, inout float3 color)
			{
				int STEPS = 20;
				float MIN_DISTANCE = 0.01;
				float MAX_DISTANCE = 20.0;
				float t = 0;
				for (int i = 0; i < STEPS; i++)
				{
					float3 position = ro + rd * t;
					float distance = map(position);
					if (distance < MIN_DISTANCE) {
						renderColor(rd, color, position);
						break;
					}
					/*return i / (float)STEPS;*/
					if (distance > MAX_DISTANCE)
						break;
					t += distance;//position += distance * direction;
				}
				return 0;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//ray direction
				float3 rd = normalize(i.ray.xyz);
				//ray origin (camera position)
				float3 ro = _CameraWS;

				float3 color = float3(0.9, 0.9, 0.9);
				raymarch(ro, rd, color);
				return fixed4(color, 1.0);
			}
			ENDCG
		}
	}
}
