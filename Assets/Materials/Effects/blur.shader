// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BLUR" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
        _BumpAmt("Distortion", Range(0,128)) = 10
        _MainTex("Tint Color (RGB)", 2D) = "white" {}
        _BumpMap("Normalmap", 2D) = "bump" {}
        _Size("Size", Range(0, 20)) = 1
    }

        Category{

            Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Opaque" }


            SubShader {

                GrabPass {
                    Tags { "LightMode" = "Always" }
                }
                Pass {
                    Tags { "LightMode" = "Always" }

                    CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma fragmentoption ARB_precision_hint_fastest
                    #include "UnityCG.cginc"

                    struct appdata_t {
                        float4 vertex : POSITION;
                        float2 texcoord: TEXCOORD0;
                    };

                    struct v2f {
                        float4 vertex : POSITION;
                        float4 uvgrab : TEXCOORD0;
                    };

                    v2f vert(appdata_t v) {
                        v2f o;
                        o.vertex = UnityObjectToClipPos(v.vertex);
                        #if UNITY_UV_STARTS_AT_TOP
                        float scale = -1.0;
                        #else
                        float scale = 1.0;
                        #endif
                        o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                        o.uvgrab.zw = o.vertex.zw;
                        return o;
                    }

                    sampler2D _GrabTexture;
                    float4 _GrabTexture_TexelSize;
                    float _Size;

                    half4 frag(v2f i) : COLOR {

                        half4 sum = half4(0,0,0,0);
                        #define GRABPIXEL(weight,kernelx) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx*_Size, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight
                        sum += GRABPIXEL(0.0000000000000000000000000, -25);
                        sum += GRABPIXEL(0.0000000000000000000000000, -24);
                        sum += GRABPIXEL(0.0000000000000000000000003, -23);
                        sum += GRABPIXEL(0.0000000000000000000000350, -22);
                        sum += GRABPIXEL(0.0000000000000000000029714, -21);
                        sum += GRABPIXEL(0.0000000000000000002053215, -20);
                        sum += GRABPIXEL(0.0000000000000000115391702, -19);
                        sum += GRABPIXEL(0.0000000000000005274538641, -18);
                        sum += GRABPIXEL(0.0000000000000196093913211, -17);
                        sum += GRABPIXEL(0.0000000000005929437332604, -16);
                        sum += GRABPIXEL(0.0000000000145825202670845, -15);
                        sum += GRABPIXEL(0.0000000002916899252026457, -14);
                        sum += GRABPIXEL(0.0000000047454784404767840, -13);
                        sum += GRABPIXEL(0.0000000627925782354606400, -12);
                        sum += GRABPIXEL(0.0000006757815976064989000, -11);
                        sum += GRABPIXEL(0.0000059152641376688660000, -10);
                        sum += GRABPIXEL(0.0000421125615104609840000, -9);
                        sum += GRABPIXEL(0.0002438478381099517000000, -8);
                        sum += GRABPIXEL(0.0011484071718654244000000, -7);
                        sum += GRABPIXEL(0.0043988850933909120000000, -6);
                        sum += GRABPIXEL(0.0137043699621821230000000, -5);
                        sum += GRABPIXEL(0.0347252408113939800000000, -4);
                        sum += GRABPIXEL(0.0715650852812925900000000, -3);
                        sum += GRABPIXEL(0.1199573478380590700000000, -2);
                        sum += GRABPIXEL(0.1635393444602833100000000, -1);
                        sum += GRABPIXEL(0.1813374001824693800000000, 0);
                        sum += GRABPIXEL(0.1635393444602833100000000, 1);
                        sum += GRABPIXEL(0.1199573478380590700000000, 2);
                        sum += GRABPIXEL(0.0715650852812925900000000, 3);
                        sum += GRABPIXEL(0.0347252408113939800000000, 4);
                        sum += GRABPIXEL(0.0137043699621821230000000, 5);
                        sum += GRABPIXEL(0.0043988850933909120000000, 6);
                        sum += GRABPIXEL(0.0011484071718654244000000, 7);
                        sum += GRABPIXEL(0.0002438478381099517000000, 8);
                        sum += GRABPIXEL(0.0000421125615104609840000, 9);
                        sum += GRABPIXEL(0.0000059152641376688660000, 10);
                        sum += GRABPIXEL(0.0000006757815976064989000, 11);
                        sum += GRABPIXEL(0.0000000627925782354606400, 12);
                        sum += GRABPIXEL(0.0000000047454784404767840, 13);
                        sum += GRABPIXEL(0.0000000002916899252026457, 14);
                        sum += GRABPIXEL(0.0000000000145825202670845, 15);
                        sum += GRABPIXEL(0.0000000000005929437332604, 16);
                        sum += GRABPIXEL(0.0000000000000196093913211, 17);
                        sum += GRABPIXEL(0.0000000000000005274538641, 18);
                        sum += GRABPIXEL(0.0000000000000000115391702, 19);
                        sum += GRABPIXEL(0.0000000000000000002053215, 20);
                        sum += GRABPIXEL(0.0000000000000000000029714, 21);
                        sum += GRABPIXEL(0.0000000000000000000000350, 22);
                        sum += GRABPIXEL(0.0000000000000000000000003, 23);
                        sum += GRABPIXEL(0.0000000000000000000000000, 24);
                        sum += GRABPIXEL(0.0000000000000000000000000, 25);

                        return sum;
                    }
                    ENDCG
                }

                GrabPass {
                    Tags { "LightMode" = "Always" }
                }
                Pass {
                    Tags { "LightMode" = "Always" }

                    CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma fragmentoption ARB_precision_hint_fastest
                    #include "UnityCG.cginc"

                    struct appdata_t {
                        float4 vertex : POSITION;
                        float2 texcoord: TEXCOORD0;
                    };

                    struct v2f {
                        float4 vertex : POSITION;
                        float4 uvgrab : TEXCOORD0;
                    };

                    v2f vert(appdata_t v) {
                        v2f o;
                        o.vertex = UnityObjectToClipPos(v.vertex);
                        #if UNITY_UV_STARTS_AT_TOP
                        float scale = -1.0;
                        #else
                        float scale = 1.0;
                        #endif
                        o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                        o.uvgrab.zw = o.vertex.zw;
                        return o;
                    }

                    sampler2D _GrabTexture;
                    float4 _GrabTexture_TexelSize;
                    float _Size;

                    half4 frag(v2f i) : COLOR {

                        half4 sum = half4(0,0,0,0);
                        #define GRABPIXEL(weight,kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely*_Size, i.uvgrab.z, i.uvgrab.w))) * weight

                        sum += GRABPIXEL(0.0000000000000000000000000, -25);
                        sum += GRABPIXEL(0.0000000000000000000000000, -24);
                        sum += GRABPIXEL(0.0000000000000000000000003, -23);
                        sum += GRABPIXEL(0.0000000000000000000000350, -22);
                        sum += GRABPIXEL(0.0000000000000000000029714, -21);
                        sum += GRABPIXEL(0.0000000000000000002053215, -20);
                        sum += GRABPIXEL(0.0000000000000000115391702, -19);
                        sum += GRABPIXEL(0.0000000000000005274538641, -18);
                        sum += GRABPIXEL(0.0000000000000196093913211, -17);
                        sum += GRABPIXEL(0.0000000000005929437332604, -16);
                        sum += GRABPIXEL(0.0000000000145825202670845, -15);
                        sum += GRABPIXEL(0.0000000002916899252026457, -14);
                        sum += GRABPIXEL(0.0000000047454784404767840, -13);
                        sum += GRABPIXEL(0.0000000627925782354606400, -12);
                        sum += GRABPIXEL(0.0000006757815976064989000, -11);
                        sum += GRABPIXEL(0.0000059152641376688660000, -10);
                        sum += GRABPIXEL(0.0000421125615104609840000, -9);
                        sum += GRABPIXEL(0.0002438478381099517000000, -8);
                        sum += GRABPIXEL(0.0011484071718654244000000, -7);
                        sum += GRABPIXEL(0.0043988850933909120000000, -6);
                        sum += GRABPIXEL(0.0137043699621821230000000, -5);
                        sum += GRABPIXEL(0.0347252408113939800000000, -4);
                        sum += GRABPIXEL(0.0715650852812925900000000, -3);
                        sum += GRABPIXEL(0.1199573478380590700000000, -2);
                        sum += GRABPIXEL(0.1635393444602833100000000, -1);
                        sum += GRABPIXEL(0.1813374001824693800000000, 0);
                        sum += GRABPIXEL(0.1635393444602833100000000, 1);
                        sum += GRABPIXEL(0.1199573478380590700000000, 2);
                        sum += GRABPIXEL(0.0715650852812925900000000, 3);
                        sum += GRABPIXEL(0.0347252408113939800000000, 4);
                        sum += GRABPIXEL(0.0137043699621821230000000, 5);
                        sum += GRABPIXEL(0.0043988850933909120000000, 6);
                        sum += GRABPIXEL(0.0011484071718654244000000, 7);
                        sum += GRABPIXEL(0.0002438478381099517000000, 8);
                        sum += GRABPIXEL(0.0000421125615104609840000, 9);
                        sum += GRABPIXEL(0.0000059152641376688660000, 10);
                        sum += GRABPIXEL(0.0000006757815976064989000, 11);
                        sum += GRABPIXEL(0.0000000627925782354606400, 12);
                        sum += GRABPIXEL(0.0000000047454784404767840, 13);
                        sum += GRABPIXEL(0.0000000002916899252026457, 14);
                        sum += GRABPIXEL(0.0000000000145825202670845, 15);
                        sum += GRABPIXEL(0.0000000000005929437332604, 16);
                        sum += GRABPIXEL(0.0000000000000196093913211, 17);
                        sum += GRABPIXEL(0.0000000000000005274538641, 18);
                        sum += GRABPIXEL(0.0000000000000000115391702, 19);
                        sum += GRABPIXEL(0.0000000000000000002053215, 20);
                        sum += GRABPIXEL(0.0000000000000000000029714, 21);
                        sum += GRABPIXEL(0.0000000000000000000000350, 22);
                        sum += GRABPIXEL(0.0000000000000000000000003, 23);
                        sum += GRABPIXEL(0.0000000000000000000000000, 24);
                        sum += GRABPIXEL(0.0000000000000000000000000, 25);

                        return sum;
                    }
                    ENDCG
                }

                GrabPass {
                    Tags { "LightMode" = "Always" }
                }
                Pass {
                    Tags { "LightMode" = "Always" }

                    CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma fragmentoption ARB_precision_hint_fastest
                    #include "UnityCG.cginc"

                    struct appdata_t {
                        float4 vertex : POSITION;
                        float2 texcoord: TEXCOORD0;
                    };

                    struct v2f {
                        float4 vertex : POSITION;
                        float4 uvgrab : TEXCOORD0;
                        float2 uvbump : TEXCOORD1;
                        float2 uvmain : TEXCOORD2;
                    };

                    float _BumpAmt;
                    float4 _BumpMap_ST;
                    float4 _MainTex_ST;

                    v2f vert(appdata_t v) {
                        v2f o;
                        o.vertex = UnityObjectToClipPos(v.vertex);
                        #if UNITY_UV_STARTS_AT_TOP
                        float scale = -1.0;
                        #else
                        float scale = 1.0;
                        #endif
                        o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                        o.uvgrab.zw = o.vertex.zw;
                        o.uvbump = TRANSFORM_TEX(v.texcoord, _BumpMap);
                        o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);
                        return o;
                    }

                    fixed4 _Color;
                    sampler2D _GrabTexture;
                    float4 _GrabTexture_TexelSize;
                    sampler2D _BumpMap;
                    sampler2D _MainTex;

                    half4 frag(v2f i) : COLOR {

                        half2 bump = UnpackNormal(tex2D(_BumpMap, i.uvbump)).rg;
                        float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy;
                        i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;

                        half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
                        half4 tint = tex2D(_MainTex, i.uvmain) * _Color;

                        return col * tint;
                    }
                    ENDCG
                }
            }
        }
}