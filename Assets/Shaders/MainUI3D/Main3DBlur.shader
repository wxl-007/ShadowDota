Shader "Custom/Blur4" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "black" {}
		_BlurValur("BlurValue",Range(0,0.2)) = 0.03
		_AlphaValur("AlphaValur",Range(0,1)) = 1
	}
	SubShader {
		Tags
		 {
		 	  "Queue" = "Transparent"
		 	  "RenderType"="Transparent"
		 }
		LOD 200
//		Cull Off
//		Lighting Off
//		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha
		pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _BlurValur;
			float _AlphaValur;
			float blurSize = 0.03f;
			float BlurDistance2 = 0.003f;
			float4 _MainTex_ST;

			struct appdata_t
			{
				float4 _pos : POSITION;
				float2 _texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 _pos : POSITION;
				float2 _texcoord : TEXCOORD0;
				float2 worldPos : TEXCOORD1;
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o._pos =  mul(UNITY_MATRIX_MVP, v._pos);
				o._texcoord = TRANSFORM_TEX(v._texcoord, _MainTex);
				return o;
			}

			half4 frag (v2f IN) : COLOR
			{
				half4 col;
				//x
//		        col += tex2D(_MainTex, float2(IN._texcoord.x - 0.03 * _BlurValur, IN._texcoord.y)) *  1.0/47.0;  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x - 0.02 * _BlurValur, IN._texcoord.y)) *  2.0/47.0;  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x - 0.01 * _BlurValur, IN._texcoord.y)) *  3.0/47.0;  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x -   _BlurValur, IN._texcoord.y)) * 10.0/47.0;  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x , IN._texcoord.y)) * 15.0/47.0;  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x +  _BlurValur, IN._texcoord.y)) * 10.0/47.0;  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x + 0.01 * _BlurValur, IN._texcoord.y)) *  3.0/47.0;  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x + 0.02 * _BlurValur, IN._texcoord.y)) *  2.0/47.0;  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x + 0.03 * _BlurValur, IN._texcoord.y)) *  1.0/47.0;  

//		        col += tex2D(_MainTex, float2(IN._texcoord.x - 0.03 * _BlurValur, IN._texcoord.y));  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x - 0.02 * _BlurValur, IN._texcoord.y));  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x - 0.01 * _BlurValur, IN._texcoord.y));  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x -   _BlurValur, IN._texcoord.y));  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x , IN._texcoord.y));  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x +  _BlurValur, IN._texcoord.y));  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x + 0.01 * _BlurValur, IN._texcoord.y));  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x + 0.02 * _BlurValur, IN._texcoord.y));  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x + 0.03 * _BlurValur, IN._texcoord.y));  
			    

//			    col += tex2D(_MainTex, float2(IN._texcoord.x, IN._texcoord.y - 4.0 * _BlurValur)) *  1.0/47.0;  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x, IN._texcoord.y - 3.0 * _BlurValur)) *  2.0/47.0;  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x, IN._texcoord.y - 2.0 * _BlurValur)) *  3.0/47.0;  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x, IN._texcoord.y -  _BlurValur)) * 10.0/47.0;  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x, IN._texcoord.y  )) * 15.0/47.0;  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x, IN._texcoord.y +  _BlurValur)) * 10.0/47.0;  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x, IN._texcoord.y + 2.0 * _BlurValur)) *  3.0/47.0;  
//			    col += tex2D(_MainTex, float2(IN._texcoord.x, IN._texcoord.y + 3.0 * _BlurValur)) *  2.0/47.0;  
//   			col += tex2D(_MainTex, float2(IN._texcoord.x, IN._texcoord.y + 4.0 * _BlurValur)) *  1.0/47.0; 
			    
//			    col  = tex2D( _MainTex, float2(IN._texcoord.x+_BlurValur, IN._texcoord.y+_BlurValur));
//				col += tex2D( _MainTex, float2(IN._texcoord.x-_BlurValur, IN._texcoord.y-_BlurValur));
//				col += tex2D( _MainTex, float2(IN._texcoord.x+_BlurValur, IN._texcoord.y-_BlurValur));
//				col += tex2D( _MainTex, float2(IN._texcoord.x-_BlurValur, IN._texcoord.y+_BlurValur));
//			    col = col/4;
//#if UNITY_IOS || UNITY_IPHONE
//				col = col/9;
//#endif				
				col = tex2D (_MainTex, IN._texcoord);
 			    col.a = _AlphaValur;
			    
				return col;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
