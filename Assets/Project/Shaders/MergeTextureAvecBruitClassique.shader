Shader "Custom/MergeTextureAvecBruitClassique"
{
    Properties
    {
        _Texture0("Texture 0", 2D) = "white" {}
        _Texture1("Texture 1", 2D) = "black" {}
        _TextureSettings("Texture settings : scale xy, offset xy", Vector) = (1, 1, 0, 0)
        _NoiseSettings("Noise settings : scale xy, offset xy", Vector) = (1, 1, 0, 0)
        _NoiseSettings2("Noise settings gain, frequencyMul, baseWeight, na", Vector) = (0.7, 2, 1, 1)
		
    }
    
    SubShader
    {
        Tags
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent+0" 
        }

        Pass
        {
            Blend One OneMinusSrcAlpha
            Cull Off 
            Lighting Off 
            ZWrite On
            ZTest LEqual
        
            CGPROGRAM
            #pragma target 3.0
			#include "Assets/Project/Shaders/Include/snoise.cginc"
            #pragma vertex vert
            #pragma fragment frag

            
            
            uniform sampler2D _Texture0;
            uniform sampler2D _Texture1;
            uniform float4 _NoiseSettings;
            uniform float4 _NoiseSettings2;
            uniform float4 _TextureSettings;
		
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0; // En spécifiant TEXCOORD0 on indique a Unity qu'il s'agit du premier canal de texture.
                
            };

            struct v2f
            {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0; // On est obligé d'assigner une semantique a chacun des canaux. ici TEXCOORD0.
                float2 uv1 : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv1 = (v.uv0 + _TextureSettings.zw) * _TextureSettings.xy;
                o.uv0 = (v.uv0 + _NoiseSettings.zw) * _NoiseSettings.xy;
                return o;
            }
            
            float NoiseFunction(float baseNoiseValue, uint generation)
            {
                if (generation < 2)
                {
                    return 1 - abs(baseNoiseValue);
                }
                else
                {
                    return baseNoiseValue * baseNoiseValue;
                }
                
            }
            
            float PoseNoiseFunction(float baseNoiseValue)
            {
                const float baseValue = max(0, baseNoiseValue - 0.3);
                return baseValue * baseValue;
            }
            
            float4 frag (v2f IN) : COLOR
            {
                float noise = 0;
                float weight = _NoiseSettings2.z;
                float frequency = 1;
                const float gain = _NoiseSettings2.x;
                const float frequencyMul = _NoiseSettings2.y;
                for (uint i = 0; i < 8; ++i)
                {
                    noise += weight * NoiseFunction(snoise(IN.uv0 * frequency), i);
                    weight *= gain;
                    frequency *= frequencyMul;
                }
             
                float postNoise = saturate(PoseNoiseFunction(noise));
                
                // on peut tester avec un seuil abrupt ou bien un smoothstep pour minimiser les zones ou il y a les deux textures:
                postNoise = smoothstep(0.4, 0.6, postNoise);
                
                // return float4(postNoise, postNoise, postNoise, 1); // line for debug
                
                
                float texture0 = 0;
                float texture1 = 0;
                if (postNoise > 0) // ici le test vaut le coup car consulter une texture est cher.
                {
                    texture1 = tex2D(_Texture1, IN.uv1);
                }
                
                if (postNoise < 1)
                {
                    texture0 = tex2D(_Texture0, IN.uv1);
                }
                
                float4 baseResult = lerp(texture0, texture1, postNoise);
                
                
                
                return float4(baseResult.rgb, 1);
                //return float4(postNoise, 0, -postNoise, 1);
            }
            ENDCG
        }
    }
}
