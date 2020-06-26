// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASE/ASE_StandartCutout2Sided"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_AlbedoColor("AlbedoColor", Color) = (1,1,1,1)
		_EmissionMap("EmissionMap", 2D) = "white" {}
		_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		_EmissionMultiplayer("EmissionMultiplayer", Float) = 0
		_NormalMap("NormalMap", 2D) = "bump" {}
		[Toggle]_SmoothFromMapSwitch("SmoothFromMapSwitch", Float) = 1
		[Toggle]_EmissionSwitch("EmissionSwitch", Float) = 0
		[Toggle]_SmoothRough("Smooth/Rough", Float) = 1
		_SmoothnessMap("SmoothnessMap", 2D) = "white" {}
		_MetallicMap("MetallicMap", 2D) = "white" {}
		_HeightMap("HeightMap", 2D) = "white" {}
		_ParalaxOffset("ParalaxOffset", Float) = 0.001
		_Snoothness("Snoothness", Float) = 1
		_Metallic("Metallic", Float) = 1
		_NormalMapDepth("NormalMapDepth", Float) = 1
		_Tiling("Tiling", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 texcoord_0;
			float2 uv_texcoord;
			float3 viewDir;
			INTERNAL_DATA
		};

		uniform float _NormalMapDepth;
		uniform sampler2D _NormalMap;
		uniform float _Tiling;
		uniform sampler2D _HeightMap;
		uniform float4 _HeightMap_ST;
		uniform float _ParalaxOffset;
		uniform sampler2D _Albedo;
		uniform float4 _AlbedoColor;
		uniform float4 _EmissionColor;
		uniform float _EmissionMultiplayer;
		uniform float _EmissionSwitch;
		uniform sampler2D _EmissionMap;
		uniform float _Metallic;
		uniform sampler2D _MetallicMap;
		uniform float _SmoothRough;
		uniform float _SmoothFromMapSwitch;
		uniform sampler2D _SmoothnessMap;
		uniform float _Snoothness;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 temp_cast_0 = (_Tiling).xx;
			o.texcoord_0.xy = v.texcoord.xy * temp_cast_0 + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_HeightMap = i.uv_texcoord * _HeightMap_ST.xy + _HeightMap_ST.zw;
			float temp_output_63_0 = ( 0.001 * _ParalaxOffset );
			float2 Offset16 = ( ( tex2D( _HeightMap, uv_HeightMap ).r - 1.0 ) * i.viewDir.xy * temp_output_63_0 ) + i.texcoord_0;
			float2 Offset26 = ( ( tex2D( _HeightMap, Offset16 ).r - 1.0 ) * i.viewDir.xy * temp_output_63_0 ) + Offset16;
			float2 Offset31 = ( ( tex2D( _HeightMap, Offset26 ).r - 1.0 ) * i.viewDir.xy * temp_output_63_0 ) + Offset26;
			float2 Offset29 = ( ( tex2D( _HeightMap, Offset31 ).r - 1.0 ) * i.viewDir.xy * temp_output_63_0 ) + Offset31;
			float2 Offset20 = ( ( tex2D( _HeightMap, Offset29 ).r - 1.0 ) * i.viewDir.xy * temp_output_63_0 ) + Offset29;
			float2 Offset21 = ( ( tex2D( _HeightMap, Offset20 ).r - 1.0 ) * i.viewDir.xy * temp_output_63_0 ) + Offset20;
			float2 Offset53 = ( ( tex2D( _HeightMap, Offset21 ).r - 1.0 ) * i.viewDir.xy * temp_output_63_0 ) + Offset21;
			float2 Offset55 = ( ( tex2D( _HeightMap, Offset53 ).r - 1.0 ) * i.viewDir.xy * temp_output_63_0 ) + Offset53;
			float2 Offset58 = ( ( tex2D( _HeightMap, Offset55 ).r - 1.0 ) * i.viewDir.xy * temp_output_63_0 ) + Offset55;
			float2 Offset60 = ( ( tex2D( _HeightMap, Offset58 ).r - 1.0 ) * i.viewDir.xy * temp_output_63_0 ) + Offset58;
			float2 Offset62 = ( ( tex2D( _HeightMap, Offset60 ).r - 1.0 ) * i.viewDir.xy * temp_output_63_0 ) + Offset60;
			float2 Offset22 = ( ( tex2D( _HeightMap, Offset62 ).r - 1.0 ) * i.viewDir.xy * temp_output_63_0 ) + Offset62;
			o.Normal = UnpackScaleNormal( tex2D( _NormalMap, Offset22 ) ,_NormalMapDepth );
			float4 tex2DNode1 = tex2D( _Albedo, Offset22 );
			float4 temp_output_3_0 = ( tex2DNode1 * _AlbedoColor );
			o.Albedo = temp_output_3_0.rgb;
			float4 tex2DNode33 = tex2D( _EmissionMap, Offset22 );
			o.Emission = ( _EmissionColor * ( _EmissionMultiplayer * lerp(tex2DNode33,( temp_output_3_0 * float4( 0,0,0,0 ) * tex2DNode33.a ),_EmissionSwitch) ) ).rgb;
			float4 tex2DNode5 = tex2D( _MetallicMap, Offset22 );
			o.Metallic = ( _Metallic * tex2DNode5.r );
			o.Smoothness = ( lerp(lerp(tex2DNode5.a,tex2D( _SmoothnessMap, Offset22 ).r,_SmoothFromMapSwitch),( 1.0 - lerp(tex2DNode5.a,tex2D( _SmoothnessMap, Offset22 ).r,_SmoothFromMapSwitch) ),_SmoothRough) * _Snoothness );
			o.Alpha = 1;
			clip( tex2DNode1.a - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			# include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				float4 texcoords01 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.texcoords01 = float4( v.texcoord.xy, v.texcoord1.xy );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord.xy = IN.texcoords01.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
218;290;1523;863;1036.105;1008.891;1.141955;True;True
Node;AmplifyShaderEditor.RangedFloatNode;67;-2507.129,-935.2653;Float;False;Property;_Tiling;Tiling;17;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;18;-3309.308,265.2168;Float;False;Property;_ParalaxOffset;ParalaxOffset;13;0;0.001;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;64;-3545.771,133.1267;Float;False;Constant;_ParalaxDepthCorrection;ParalaxDepthCorrection;20;0;0.001;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;17;-2552.477,-1265.36;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;19;-2181.574,26.19169;Float;False;Tangent;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;15;-2553.399,-723.864;Float;True;Property;_HeightMap;HeightMap;12;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-2142.688,279.9562;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ParallaxMappingNode;16;-1715.648,-1029.003;Float;False;Normal;4;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT3;0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;27;-2559.562,-477.743;Float;True;Property;_TextureSample6;Texture Sample 6;12;0;None;True;0;False;white;Auto;False;Instance;15;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ParallaxMappingNode;26;-1723.194,-815.5413;Float;False;Normal;4;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT3;0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;30;-2545.458,-214.85;Float;True;Property;_TextureSample4;Texture Sample 4;12;0;None;True;0;False;white;Auto;False;Instance;15;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ParallaxMappingNode;31;-1714.93,-571.4125;Float;False;Normal;4;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT3;0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;28;-2555.921,7.728275;Float;True;Property;_TextureSample1;Texture Sample 1;12;0;None;True;0;False;white;Auto;False;Instance;15;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ParallaxMappingNode;29;-1725.393,-348.834;Float;False;Normal;4;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT3;0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;23;-2553.803,215.1232;Float;True;Property;_TextureSample1;Texture Sample 1;12;0;None;True;0;False;white;Auto;False;Instance;15;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ParallaxMappingNode;20;-1723.276,-141.439;Float;False;Normal;4;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT3;0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;24;-2553.803,429.2001;Float;True;Property;_TextureSample2;Texture Sample 2;12;0;None;True;0;False;white;Auto;False;Instance;15;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ParallaxMappingNode;21;-1721.541,42.3968;Float;False;Normal;4;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT3;0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;54;-2552.627,666.0082;Float;True;Property;_TextureSample5;Texture Sample 5;12;0;None;True;0;False;white;Auto;False;Instance;15;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ParallaxMappingNode;53;-1720.365,279.2048;Float;False;Normal;4;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT3;0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;56;-2552.626,878.4504;Float;True;Property;_TextureSample7;Texture Sample 7;12;0;None;True;0;False;white;Auto;False;Instance;15;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ParallaxMappingNode;55;-1720.365,491.647;Float;False;Normal;4;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT3;0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;57;-2545.169,1110.064;Float;True;Property;_TextureSample8;Texture Sample 8;12;0;None;True;0;False;white;Auto;False;Instance;15;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ParallaxMappingNode;58;-1712.908,723.2602;Float;False;Normal;4;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT3;0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;59;-2545.168,1320.407;Float;True;Property;_TextureSample9;Texture Sample 9;12;0;None;True;0;False;white;Auto;False;Instance;15;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ParallaxMappingNode;60;-1712.908,933.6038;Float;False;Normal;4;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT3;0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;61;-2551.093,1527.788;Float;True;Property;_TextureSample10;Texture Sample 10;12;0;None;True;0;False;white;Auto;False;Instance;15;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ParallaxMappingNode;62;-1718.833,1140.985;Float;False;Normal;4;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT3;0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;25;-2575.662,1769.426;Float;True;Property;_TextureSample3;Texture Sample 3;12;0;None;True;0;False;white;Auto;False;Instance;15;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ParallaxMappingNode;22;-1748.579,1338.499;Float;False;Normal;4;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT3;0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.ColorNode;4;-407.9034,-673.7851;Float;False;Property;_AlbedoColor;AlbedoColor;2;0;1,1,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-331.8671,-927.1003;Float;True;Property;_Albedo;Albedo;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;6;-1291.196,248.5266;Float;True;Property;_SmoothnessMap;SmoothnessMap;10;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;33;-352.3483,-472.2335;Float;True;Property;_EmissionMap;EmissionMap;3;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;0.9208739,-662.1165;Float;False;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;5;-1285.996,5.426482;Float;True;Property;_MetallicMap;MetallicMap;11;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;2;-896.6956,186.1265;Float;False;Property;_SmoothFromMapSwitch;SmoothFromMapSwitch;7;0;1;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;229.7413,-699.8065;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;38;420.8224,-589.8196;Float;False;Property;_EmissionMultiplayer;EmissionMultiplayer;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;32;280.0221,-432.5562;Float;False;Property;_EmissionSwitch;EmissionSwitch;8;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.OneMinusNode;12;-658.3749,320.7062;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;36;684.2137,-982.8802;Float;False;Property;_EmissionColor;EmissionColor;4;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;9;-461.2012,333.9779;Float;False;Property;_Snoothness;Snoothness;14;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;588.9876,-480.4108;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;8;-644.9746,-14.00033;Float;False;Property;_Metallic;Metallic;15;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;11;-508.8754,175.1064;Float;False;Property;_SmoothRough;Smooth/Rough;9;0;1;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;66;-1428.504,-467.3266;Float;False;Property;_NormalMapDepth;NormalMapDepth;16;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;903.0311,-401.3935;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-196.8749,160.8066;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;13;-778.8251,-414.4076;Float;True;Property;_NormalMap;NormalMap;6;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-293.0748,8.706559;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-24.70001,49.40001;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;ASE/ASE_StandartCutout2Sided;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;0;False;0;0;Custom;0.5;True;True;0;True;TransparentCutout;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;17;0;67;0
WireConnection;63;0;64;0
WireConnection;63;1;18;0
WireConnection;16;0;17;0
WireConnection;16;1;15;1
WireConnection;16;2;63;0
WireConnection;16;3;19;0
WireConnection;27;1;16;0
WireConnection;26;0;16;0
WireConnection;26;1;27;1
WireConnection;26;2;63;0
WireConnection;26;3;19;0
WireConnection;30;1;26;0
WireConnection;31;0;26;0
WireConnection;31;1;30;1
WireConnection;31;2;63;0
WireConnection;31;3;19;0
WireConnection;28;1;31;0
WireConnection;29;0;31;0
WireConnection;29;1;28;1
WireConnection;29;2;63;0
WireConnection;29;3;19;0
WireConnection;23;1;29;0
WireConnection;20;0;29;0
WireConnection;20;1;23;1
WireConnection;20;2;63;0
WireConnection;20;3;19;0
WireConnection;24;1;20;0
WireConnection;21;0;20;0
WireConnection;21;1;24;1
WireConnection;21;2;63;0
WireConnection;21;3;19;0
WireConnection;54;1;21;0
WireConnection;53;0;21;0
WireConnection;53;1;54;1
WireConnection;53;2;63;0
WireConnection;53;3;19;0
WireConnection;56;1;53;0
WireConnection;55;0;53;0
WireConnection;55;1;56;1
WireConnection;55;2;63;0
WireConnection;55;3;19;0
WireConnection;57;1;55;0
WireConnection;58;0;55;0
WireConnection;58;1;57;1
WireConnection;58;2;63;0
WireConnection;58;3;19;0
WireConnection;59;1;58;0
WireConnection;60;0;58;0
WireConnection;60;1;59;1
WireConnection;60;2;63;0
WireConnection;60;3;19;0
WireConnection;61;1;60;0
WireConnection;62;0;60;0
WireConnection;62;1;61;1
WireConnection;62;2;63;0
WireConnection;62;3;19;0
WireConnection;25;1;62;0
WireConnection;22;0;62;0
WireConnection;22;1;25;1
WireConnection;22;2;63;0
WireConnection;22;3;19;0
WireConnection;1;1;22;0
WireConnection;6;1;22;0
WireConnection;33;1;22;0
WireConnection;3;0;1;0
WireConnection;3;1;4;0
WireConnection;5;1;22;0
WireConnection;2;0;5;4
WireConnection;2;1;6;1
WireConnection;34;0;3;0
WireConnection;34;2;33;4
WireConnection;32;0;33;0
WireConnection;32;1;34;0
WireConnection;12;0;2;0
WireConnection;39;0;38;0
WireConnection;39;1;32;0
WireConnection;11;0;2;0
WireConnection;11;1;12;0
WireConnection;37;0;36;0
WireConnection;37;1;39;0
WireConnection;10;0;11;0
WireConnection;10;1;9;0
WireConnection;13;1;22;0
WireConnection;13;5;66;0
WireConnection;7;0;8;0
WireConnection;7;1;5;1
WireConnection;0;0;3;0
WireConnection;0;1;13;0
WireConnection;0;2;37;0
WireConnection;0;3;7;0
WireConnection;0;4;10;0
WireConnection;0;10;1;4
ASEEND*/
//CHKSM=3A193BDF0BF82D716C9836080F635EED1070B7C7