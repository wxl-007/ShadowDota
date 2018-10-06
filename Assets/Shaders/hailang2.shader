// Shader created with Shader Forge Beta 0.35 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.35;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:False,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32624,y:32676|emission-1447-OUT,alpha-93-OUT,clip-410-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:33959,y:32693,ptlb:1,ptin:_1,tex:f1c269e2d792c8646bf2452f72c358b2,ntxv:0,isnm:False|UVIN-1042-UVOUT;n:type:ShaderForge.SFN_Panner,id:3,x:34465,y:32707,spu:0,spv:0|UVIN-1050-UVOUT;n:type:ShaderForge.SFN_Vector1,id:4,x:34450,y:32892,v1:2;n:type:ShaderForge.SFN_Multiply,id:5,x:34353,y:32612|A-3-UVOUT,B-4-OUT;n:type:ShaderForge.SFN_Multiply,id:33,x:34280,y:33006|A-35-UVOUT,B-37-OUT;n:type:ShaderForge.SFN_Panner,id:35,x:34465,y:32954,spu:0,spv:0.2;n:type:ShaderForge.SFN_Vector1,id:37,x:34450,y:33186,v1:2;n:type:ShaderForge.SFN_Multiply,id:38,x:33721,y:32906|A-2-R,B-1017-R;n:type:ShaderForge.SFN_Multiply,id:93,x:33379,y:32967|A-38-OUT,B-94-OUT;n:type:ShaderForge.SFN_Vector1,id:94,x:33564,y:33152,v1:3;n:type:ShaderForge.SFN_Power,id:410,x:33231,y:32761|VAL-93-OUT,EXP-93-OUT;n:type:ShaderForge.SFN_Tex2d,id:1017,x:34072,y:33144,ptlb:2,ptin:_2,tex:d6eba89cdfc42fa4293167bf4a0cc9c6,ntxv:0,isnm:False|UVIN-33-OUT,MIP-1107-OUT;n:type:ShaderForge.SFN_Rotator,id:1042,x:34151,y:32641|UVIN-5-OUT,ANG-1067-OUT;n:type:ShaderForge.SFN_Vector1,id:1043,x:34312,y:32904,v1:0;n:type:ShaderForge.SFN_TexCoord,id:1050,x:34619,y:32736,uv:0;n:type:ShaderForge.SFN_Vector1,id:1067,x:34325,y:32857,v1:3.35;n:type:ShaderForge.SFN_Vector1,id:1107,x:34343,y:33186,v1:3.35;n:type:ShaderForge.SFN_Multiply,id:1447,x:33072,y:32655|A-1448-OUT,B-410-OUT;n:type:ShaderForge.SFN_Vector3,id:1448,x:33356,y:32629,v1:0.6838235,v2:0.9345843,v3:1;proporder:2-1017;pass:END;sub:END;*/

Shader "Shader Forge/hailang2" {
    Properties {
        _1 ("1", 2D) = "white" {}
        _2 ("2", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers d3d9 d3d11 xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform sampler2D _1; uniform float4 _1_ST;
            uniform sampler2D _2; uniform float4 _2_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float node_1042_ang = 3.35;
                float node_1042_spd = 1.0;
                float node_1042_cos = cos(node_1042_spd*node_1042_ang);
                float node_1042_sin = sin(node_1042_spd*node_1042_ang);
                float2 node_1042_piv = float2(0.5,0.5);
                float4 node_1757 = _Time + _TimeEditor;
                float2 node_1042 = (mul(((i.uv0.rg+node_1757.g*float2(0,0))*2.0)-node_1042_piv,float2x2( node_1042_cos, -node_1042_sin, node_1042_sin, node_1042_cos))+node_1042_piv);
                float2 node_33 = ((i.uv0.rg+node_1757.g*float2(0,0.2))*2.0);
                float node_93 = ((tex2D(_1,TRANSFORM_TEX(node_1042, _1)).r*tex2Dlod(_2,float4(TRANSFORM_TEX(node_33, _2),0.0,3.35)).r)*3.0);
                clip(pow(node_93,node_93) - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = (float3(0.6838235,0.9345843,1)*pow(node_93,node_93));
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,node_93);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCollector"
            Tags {
                "LightMode"="ShadowCollector"
            }
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCOLLECTOR
            #define SHADOW_COLLECTOR_PASS
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcollector
            #pragma exclude_renderers d3d9 d3d11 xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform sampler2D _1; uniform float4 _1_ST;
            uniform sampler2D _2; uniform float4 _2_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_COLLECTOR;
                float2 uv0 : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float node_1042_ang = 3.35;
                float node_1042_spd = 1.0;
                float node_1042_cos = cos(node_1042_spd*node_1042_ang);
                float node_1042_sin = sin(node_1042_spd*node_1042_ang);
                float2 node_1042_piv = float2(0.5,0.5);
                float4 node_1759 = _Time + _TimeEditor;
                float2 node_1042 = (mul(((i.uv0.rg+node_1759.g*float2(0,0))*2.0)-node_1042_piv,float2x2( node_1042_cos, -node_1042_sin, node_1042_sin, node_1042_cos))+node_1042_piv);
                float2 node_33 = ((i.uv0.rg+node_1759.g*float2(0,0.2))*2.0);
                float node_93 = ((tex2D(_1,TRANSFORM_TEX(node_1042, _1)).r*tex2Dlod(_2,float4(TRANSFORM_TEX(node_33, _2),0.0,3.35)).r)*3.0);
                clip(pow(node_93,node_93) - 0.5);
                SHADOW_COLLECTOR_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Cull Off
            Offset 1, 1
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers d3d9 d3d11 xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform sampler2D _1; uniform float4 _1_ST;
            uniform sampler2D _2; uniform float4 _2_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float node_1042_ang = 3.35;
                float node_1042_spd = 1.0;
                float node_1042_cos = cos(node_1042_spd*node_1042_ang);
                float node_1042_sin = sin(node_1042_spd*node_1042_ang);
                float2 node_1042_piv = float2(0.5,0.5);
                float4 node_1761 = _Time + _TimeEditor;
                float2 node_1042 = (mul(((i.uv0.rg+node_1761.g*float2(0,0))*2.0)-node_1042_piv,float2x2( node_1042_cos, -node_1042_sin, node_1042_sin, node_1042_cos))+node_1042_piv);
                float2 node_33 = ((i.uv0.rg+node_1761.g*float2(0,0.2))*2.0);
                float node_93 = ((tex2D(_1,TRANSFORM_TEX(node_1042, _1)).r*tex2Dlod(_2,float4(TRANSFORM_TEX(node_33, _2),0.0,3.35)).r)*3.0);
                clip(pow(node_93,node_93) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
