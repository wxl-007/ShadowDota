Shader "TSM2/CutoutOutline1"
{
    Properties
    {
		[MaterialToggle(_OUTL_ON)] _Outl ("OFF", Float) = 0 						//0
		[MaterialToggle(_TEX_ON)] _DetailTex ("Enable Detail texture", Float) = 0 	//1
		_MainTex ("Detail", 2D) = "white" {}        								//2
		_ToonShade ("Shade", 2D) = "white" {}  										//3
		[MaterialToggle(_COLOR_ON)] _TintColor ("Enable Color Tint", Float) = 0 	//4
		_Color ("Base Color", Color) = (1,1,1,1)									//5	
		[MaterialToggle(_VCOLOR_ON)] _VertexColor ("Enable Vertex Color", Float) = 0//6        
		_Brightness ("Brightness 2 = neutral", Float) = 2.0							//7	
		[MaterialToggle(_DS_ON)] _DS ("Enable DoubleSided", Float) = 0				//8	
		[Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull mode", Float) = 2		//9	
		_OutlineColor ("Outline Color", Color) = (0.5,0.5,0.5,1.0)					//10
		_Outline ("Outline width", Float) = 0.01									//11
		[MaterialToggle(_ASYM_ON)] _Asym ("Enable Asymmetry", Float) = 0        	//12
		_Asymmetry ("OutlineAsymmetry", Vector) = (0.0,0.25,0.5,0.0)     			//13
		[MaterialToggle(_TRANSP_ON)] _Trans ("Enable Transparency", Float) = 0   	//14
		[Enum(TRANS_OPTIONS)] _TrOp ("Transparency mode", Float) = 0   				//15
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5                                  //16
    }
 
    SubShader
    {
        Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 200
		
        Lighting Off
        Fog { Mode Off }

        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
            Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On ZTest LEqual
			Cull Front
			Lighting Off
            
            CGPROGRAM
			#include "UnityCG.cginc"
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma glsl_no_auto_normalization
            #pragma vertex vert
 			#pragma fragment frag
			#pragma multi_compile _ASYM_OFF _ASYM_ON

            struct appdata_t 
            {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f 
			{
				float4 pos : SV_POSITION;
			};

            fixed _Outline;
            #if _ASYM_ON
            float4 _Asymmetry;
            #endif
            
            v2f vert (appdata_t v) 
            {
                v2f o;
			    o.pos = v.vertex;
			    
			    #if _ASYM_ON
			    o.pos.xyz += (v.normal.xyz + _Asymmetry.xyz) *_Outline*0.01;
			    #else
			    o.pos.xyz += v.normal.xyz *_Outline*0.01;
			    #endif
			    
			    o.pos = mul(UNITY_MATRIX_MVP, o.pos);
			    return o;
            }
            
            fixed4 _OutlineColor;
            
            fixed4 frag(v2f i) :COLOR 
			{
		    	return _OutlineColor;
			}
            
            ENDCG
        }
        
         UsePass "TSM2/Cutout/BASE"
    }
    
    CustomEditor "TSM2"
    Fallback "Transparent/Cutout/Diffuse"
}