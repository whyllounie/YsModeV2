Shader "Custom/URP_OutlineNeon_Fixed"
{
    Properties
    {
        _Color ("Shape Color", Color) = (1,1,1,1)
        _MainTex ("Texture (optional)", 2D) = "white" {}

        _OutlineColor ("Outline Color", Color) = (0,1,1,1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.02

        [Toggle] _FirstMode ("First Mode (shape invisible, outline only, no noise)", Float) = 0

        _NoiseSpeed ("Neon Noise Speed", Float) = 2.0
        _NoiseScale ("Neon Noise Scale", Float) = 10.0
        _GlowIntensity ("Neon Glow Intensity", Range(0, 5)) = 1.5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }

        // ---------- PASS 1: ОСНОВНАЯ ФИГУРА (рендерится ПЕРВОЙ, пишет глубину всегда) ----------
        Pass
        {
            Name "Main"
            Tags { "LightMode"="UniversalForward" }
            Cull Back
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _FirstMode;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half4 col = tex * _Color;

                if (_FirstMode > 0.5)
                {
                    // ВАЖНО: не discard! Фрагмент остаётся и пишет глубину,
                    // просто становится полностью прозрачным по цвету.
                    // Это нужно, чтобы обводка (Pass 2) правильно "пряталась"
                    // за силуэтом объекта и не заливала всю площадь.
                    col.a = 0;
                }

                return col;
            }
            ENDHLSL
        }

        // ---------- PASS 2: ОБВОДКА (рендерится ВТОРОЙ, тестируется по глубине) ----------
        Pass
        {
            Name "Outline"
            Tags { "LightMode"="UniversalForward" }
            Cull Front
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineWidth;
                float _FirstMode;
                float _NoiseSpeed;
                float _NoiseScale;
                float _GlowIntensity;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 normalOS = normalize(IN.normalOS);
                float3 offsetPosOS = IN.positionOS.xyz + normalOS * _OutlineWidth;
                OUT.positionCS = TransformObjectToHClip(offsetPosOS);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            // простой псевдо-случайный шум на основе мировых координат + времени
            float hashNoise(float3 p)
            {
                return frac(sin(dot(p, float3(12.9898, 78.233, 45.164))) * 43758.5453);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 col = _OutlineColor;

                if (_FirstMode < 0.5)
                {
                    // Mode 0: объект виден, добавляем "неоновый" шум/пульсацию на обводку
                    float n = hashNoise(IN.positionWS * _NoiseScale + _Time.y * _NoiseSpeed);
                    float pulse = sin(_Time.y * _NoiseSpeed * 3.0) * 0.5 + 0.5;
                    float glow = saturate(n * 0.5 + pulse * 0.5) * _GlowIntensity;

                    col.rgb += _OutlineColor.rgb * glow;
                }
                // Mode 1: объект невидим, обводка - чистая линия без шума

                return col;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
