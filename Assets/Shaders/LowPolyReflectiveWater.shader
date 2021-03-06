﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/LowPolyReflectiveWater" {
	Properties
	{
		_Diffuse("Diffuse Color", Color) = (0,0.4431,0.7333,0.5)
		_Specular("Specular Color", Color) = (1,1,1,1)
		_Shininess("Shininess", Float) = 50.0
		//
		_WaveLength("Wave Length", Float) = 0.1
		_WaveHeight("Wave Height", Float) = 0.1
		_WaveSpeed("Wave Speed", Float) = 0.1
		//
		_RandomHeight("Random Height", Float) = 1
		_RandomSpeed("Random Speed", Float) = 5
		//
		_ShoreColor("Shore Color", Color) = (0.8431,1,1,1)
		_ShoreIntensity("Shore Intensity", Range(-1,1)) = 0.9
		_ShoreDistance("Shore Distance", Float) = 0.1
		//
		_DistortDistance("Distortion Distance", Float) = 0.12
		_DistortIntensity("Distortion Intensity", Float) = 100
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		GrabPass
		{
			"_GrabTex"
		}

		Pass
		{

			CGPROGRAM
			#include "UnityCG.cginc"
			#include "UnityStandardUtils.cginc"
			#include "UnityLightingCommon.cginc"
			
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			
			#if UNITY_VERSION < 540
				#define UNITY_VERTEX_INPUT_INSTANCE_ID
				#define UNITY_VERTEX_OUTPUT_STEREO
				#define UNITY_SETUP_INSTANCE_ID(i)
				#define UNITY_TRANSFER_INSTANCE_ID(i, output)
				#define UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output)
				#define COMPUTESCREENPOS ComputeScreenPos
			#else
				#define COMPUTESCREENPOS ComputeNonStereoScreenPos
			#endif
			
			float rand(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(21, 8, 14))) * 11);
			}
			
			float rand2(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(20, 12, 23))) * 15);
			}

			float _WaveLength;
			float _WaveHeight;
			float _WaveSpeed;
			float _RandomHeight;
			float _RandomSpeed;
			float _DistortDistance;
			float _DistortIntensity;

			uniform float4 _Diffuse;
			uniform float4 _Specular;
			uniform float _Shininess;
			sampler2D _CameraDepthTexture;
			float _ShoreIntensity, _ShoreDistance;
			float4 _ShoreColor;
			sampler2D _GrabTex;

			struct v2g
			{
				float4 pos : POSITION0;
				float4 screenPos : TEXCOORD3;
				float3 norm : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct g2f
			{
				float4 pos : POSITION0;
				float3 norm : NORMAL;
				float2 uv : TEXCOORD0;
				float4 screenPos : TEXCOORD3;
				float4 diffuseColor : TEXCOORD1;
				float4 specularColor : TEXCOORD2;
			};

			float waveRipple(float time, float3 vertex)
			{
				return _WaveHeight * sin((time * _WaveSpeed) + (vertex.x * _WaveLength) + (vertex.z * _WaveLength) + rand(vertex.xzz));
			}

			float randomRipple(float time, float3 vertex)
			{
				return _RandomHeight * sin(cos(rand2(vertex.xzz) * _RandomHeight * cos(time * _RandomSpeed * sin(rand2(vertex.xxz)))));
			}

			v2g vert(appdata_full i)
			{

				float ripple1 = waveRipple(_Time[1], i.vertex);
				float ripple2 = randomRipple(_Time[1], i.vertex);

				i.vertex.y = ripple1 + ripple2 - 0.9 + _WaveHeight;

				half4 vpos = mul(unity_ObjectToWorld, i.vertex);
				vpos = mul(UNITY_MATRIX_VP, vpos);

				v2g output;
				output.pos = i.vertex;
				output.screenPos = COMPUTESCREENPOS(vpos);
				output.screenPos.z = lerp(vpos.w, mul(UNITY_MATRIX_V, vpos).z, unity_OrthoParams.w);
				output.norm = i.normal;
				output.uv = i.texcoord;
				
				return output;
			}

			[maxvertexcount(3)]
			void geom(triangle v2g i[3], inout TriangleStream<g2f> triangles)
			{
				float3 v0 = i[0].pos.xyz;
				float3 v1 = i[1].pos.xyz;
				float3 v2 = i[2].pos.xyz;
				float3 centerPos = (v0 + v1 + v2) / 3.0;

				float3 vn = normalize(cross(v1 - v0, v2 - v0));

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				float3 normalDirection = normalize(mul(float4(vn, 0.0), modelMatrixInverse).xyz);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(modelMatrix, float4(centerPos, 0.0)).xyz);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float attenuation = 1.0;

				float4 ambientLighting = float4(0.5,0.5,0.5,1.0) * _Diffuse.rgba;

				float4 diffuseReflection = attenuation * _LightColor0.rgba * _Diffuse.rgba * max(0.0, dot(normalDirection, lightDirection));

				float4 specularReflection;
				if (dot(normalDirection, lightDirection) < 0.0)
				{
					specularReflection = float4(0.0, 0.0, 0.0, 0.0);
				}
				else
				{
					specularReflection = attenuation * _LightColor0.rgba * _Specular.rgba * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
				}

				g2f o;

				for (int index = 0; index < 3; index++)
				{
					o.pos = UnityObjectToClipPos(i[index].pos);
					o.screenPos = i[index].screenPos;
					o.screenPos.x += (sin(_Time[1] * _WaveSpeed * _WaveLength * i[index].screenPos.x) * (1/_DistortIntensity));
					o.screenPos.y += (sin(_Time[1] * _WaveSpeed * _WaveLength * i[index].screenPos.y) * (1 / _DistortIntensity));
					o.norm = vn;
					o.uv = i[index].uv;
					o.diffuseColor = ambientLighting + diffuseReflection;
					o.specularColor = specularReflection;
					triangles.Append(o);
				}

			}

			float4 frag(g2f i) : COLOR
			{
				float ar = _ScreenParams.x / _ScreenParams.y;

				float4 distortOffset = float4(_DistortDistance, _DistortDistance, 0, 0);
				float4 proj = UNITY_PROJ_COORD(i.screenPos) + distortOffset;

				half4 distortionResult = half4(tex2Dproj(_GrabTex, proj).rgb, 1.0);

				float4 c = float4(i.specularColor + i.diffuseColor);
				c = lerp(float4(c.rgb, 1), distortionResult, c.a);

				float sceneZ = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));

				float perpectiveZ = LinearEyeDepth(sceneZ);
				
				float orthoZ = sceneZ * (_ProjectionParams.y - _ProjectionParams.z) - _ProjectionParams.y;

				sceneZ = lerp(perpectiveZ, orthoZ, unity_OrthoParams.w);

				float difference = abs(sceneZ - i.screenPos.z) / _ShoreDistance;

				difference = smoothstep(_ShoreIntensity, 1, difference);

				c = lerp(lerp(c, _ShoreColor, _ShoreColor.a), c, difference);
				c.a = 1.0;
				
				return c;
			}

			ENDCG

		}
	}
}
