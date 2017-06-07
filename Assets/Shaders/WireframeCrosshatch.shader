Shader "GrimTrigger/WireframeCrosshatch" {

	Properties {
		_Thickness ("Thickness", Float) = 1
		_Color ("Color", Color) = (1,1,1,1)
		_LineColor ("Line Color", Color) = (1,1,1,1)
	}

	SubShader {

		Tags {
			"RenderType"="Opaque"
			"Queue"="Transparent"
			"LightMode" = "ForwardBase"
			"DisableBatching" = "True"
		}
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
//			#pragma multi_compile_fwdbase
			#include "UnityCG.cginc"
//			#include "AutoLight.cginc"

			//Material Properties
			uniform float _Thickness;
			uniform float4 _Color;
			uniform float4 _LineColor;

			//Pipeline Structs
			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct v2g {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float2 screenUV : TEXCOORD1;
				float4 tangent : TANGENT;
			};

			struct g2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float2 screenUV : TEXCOORD1;
				float3 dist : TEXCOORD2;
			};

			//Vertex Shader
			v2g vert(appdata v) {
				v2g output;
				output.pos = UnityObjectToClipPos(v.vertex);
				output.worldNormal = UnityObjectToWorldNormal (v.normal);
				output.screenUV = output.pos.xy / output.pos.w * 0.5 + 0.5;	           
	            output.tangent = v.tangent;
				return output;
			}

			// Geometry Shader
			[maxvertexcount(3)]
			void geom(triangle v2g p[3], inout TriangleStream<g2f> triStream)
			{
				//Points in Screen Space
				float2 p0 = _ScreenParams.xy * p[0].pos.xy / p[0].pos.w;
				float2 p1 = _ScreenParams.xy * p[1].pos.xy / p[1].pos.w;
				float2 p2 = _ScreenParams.xy * p[2].pos.xy / p[2].pos.w;	

				//Edge Vectors
				float2 v0 = p2 - p1;
				float2 v1 = p2 - p0;
				float2 v2 = p1 - p0;

				//Area of the Triangle
			 	float area = abs(v1.x * v2.y - v1.y * v2.x);

				//values based on distance to the edges
				float dist0 = area / length(v0);
				float dist1 = area / length(v1);
				float dist2 = area / length(v2);
			
				g2f pIn;

				//First Point
				pIn.pos = p[0].pos;
				pIn.worldNormal = p[0].worldNormal;
				pIn.screenUV = p[0].screenUV;	
				pIn.dist = float3(dist0,0,0);
				triStream.Append(pIn);

				//Second Point
				pIn.pos =  p[1].pos;
				pIn.worldNormal = p[1].worldNormal;
				pIn.screenUV = p[1].screenUV;
				pIn.dist = float3(0,dist1,0);
				triStream.Append(pIn);
				
				//Third Point
				pIn.pos = p[2].pos;
				pIn.worldNormal = p[2].worldNormal;
				pIn.screenUV = p[2].screenUV;
				pIn.dist = float3(0,0,dist2);
				triStream.Append(pIn);

			}

			float4 frag(g2f input) : COLOR {

				//Crosshatch
//				float4 crosshatchCol = 1;
//				float dotProduct = dot (input.worldNormal, normalize(_WorldSpaceLightPos0.xyz));
//
//				if (dotProduct > 0.65) {
//					crosshatchCol = 1;
//				} else if (dotProduct > 0.2) {
//					crosshatchCol = tex2D(_Crosshatch_Light_Tex, input.screenUV);
//				} else {
//					crosshatchCol = tex2D(_Crosshatch_Heavy_Tex, input.screenUV);
//				}

				//Wireframe
				float wireframeCol = min(input.dist.x, min( input.dist.y, input.dist.z));
				wireframeCol = 1 - exp2(-1/_Thickness * wireframeCol * wireframeCol) * 10;

				return (wireframeCol * _Color + (1 - wireframeCol) * _LineColor);

//				return (wireframeCol * crosshatchCol * _Color + (1 - wireframeCol) * _LineColor);
			}			
			ENDCG
		}

//		Pass {
//			Blend SrcAlpha OneMinusSrcAlpha
//			BlendOp Min
//
//			CGPROGRAM
//			#pragma vertex vert
//			#pragma fragment frag
//			#pragma multi_compile_fwdbase 
//			#include "UnityCG.cginc"
//			#include "AutoLight.cginc"
//
//			sampler2D _Crosshatch_Shadow_Tex;
//			fixed4 _LightColor0;
//
//			struct v2f
//			{
//				float4 pos : SV_POSITION;
//				float2 screenUV : TEXCOORD0;
//				LIGHTING_COORDS(1, 2)
//			};
//
//			v2f vert(appdata_base v) {
//				v2f o;
//				o.pos = UnityObjectToClipPos(v.vertex);
//				o.screenUV = o.pos.xy / o.pos.w * 0.5 + 0.5;	           
//				TRANSFER_VERTEX_TO_FRAGMENT(o);
//				return o;
//			}
//
//			fixed4 frag(v2f i) : COLOR {
//				float4 crosshatchCol = tex2D(_Crosshatch_Shadow_Tex, i.screenUV);
//				float attenuation = LIGHT_ATTENUATION(i);
//				float4 shadowCol = 1-(1-attenuation) * crosshatchCol;
//				shadowCol.a = 1-attenuation;
//				return shadowCol;
//			}
//			ENDCG
//		}
	}
	Fallback "VertexLit" 
}