Shader "Custom/NeonOutline"
{
    Properties
    {
        [Header(Основная фигура)]
        _MainTex ("Текстура", 2D) = "white" {}
        _Color ("Основной цвет", Color) = (1,1,1,1)
        
        [Header(Режимы)]
        [Toggle] _OutlineOnly("Только обводка", Float) = 0
        // 0 = фигура видна + обводка с неоновым шумом
        // 1 = фигура прозрачная + чистая обводка без шума
        
        [Header(Обводка)]
        _OutlineColor ("Цвет обводки", Color) = (0, 1, 1, 1)
        _OutlineWidth ("Толщина обводки", Range(0.001, 0.5)) = 0.03
        
//        [Header(Неон / Шум)]
        _NoiseSpeed ("Скорость шума", Float) = 2.0
        _NoiseScale ("Масштаб шума", Float) = 25.0
        _NeonIntensity ("Интенсивность неона", Range(1, 10)) = 3.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100
        
        // ============================================
        // PASS 1: ОБВОДКА (рисуем задние грани, расширенные по нормали)
        // ============================================
        Pass
        {
            Name "OUTLINE"
            Cull Front          // Рисуем только задние грани — они будут "оболочкой"
            ZWrite On
            ZTest LEqual
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };
            
            float _OutlineWidth;
            float4 _OutlineColor;
            float _OutlineOnly;
            float _NoiseSpeed;
            float _NoiseScale;
            float _NeonIntensity;
            
            // Простой 2D-шум
            float2 hash22(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * float3(0.1031, 0.1030, 0.0973));
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.xx + p3.yz) * p3.zy);
            }
            
            float noise2D(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f); // smoothstep
                
                float a = dot(hash22(i), f);
                float b = dot(hash22(i + float2(1,0)), f - float2(1,0));
                float c = dot(hash22(i + float2(0,1)), f - float2(0,1));
                float d = dot(hash22(i + float2(1,1)), f - float2(1,1));
                
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                float3 normal = normalize(v.normal);
                // Сдвигаем вершину по нормали — получаем "оболочку"
                float3 pos = v.vertex.xyz + normal * _OutlineWidth;
                o.vertex = UnityObjectToClipPos(float4(pos, 1.0));
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = _OutlineColor;
                
                // Если НЕ в режиме "только обводка" — добавляем анимированный шум для эффекта неона
                if (_OutlineOnly < 0.5)
                {
                    float2 noiseUV = i.worldPos.xy * _NoiseScale + _Time.y * _NoiseSpeed;
                    float n = noise2D(noiseUV);
                    
                    // Пульсация + шум = живое неоновое свечение
                    float pulse = 0.75 + 0.25 * sin(_Time.y * 4.0 + n * 6.28318);
                    col.rgb *= _NeonIntensity * pulse;
                }
                // Если _OutlineOnly == 1 — обводка просто сплошная линия без шума
                
                return col;
            }
            ENDCG
        }
        
        // ============================================
        // PASS 2: ОСНОВНАЯ ФИГУРА
        // ============================================
        Pass
        {
            Name "MAIN"
            Cull Back
            ZWrite On
            ZTest LEqual
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _OutlineOnly;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // В режиме "только обводка" — фигура полностью прозрачная
                if (_OutlineOnly > 0.5)
                {
                    discard;
                }
                
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}