Shader "Custom/CloakSurfaceShader" 
{
	Properties 
    {
		[NoScaleOffset] _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
        
        _Cloak ("Cloak", Range(0,1)) = 0.0
        
        _NoiseSettings("Noise settings : scale xy, offset xy", Vector) = (1, 1, 0, 0)
        _NoiseSettings2("Noise settings gain, frequencyMul, baseWeight, na", Vector) = (0.7, 2, 1, 1)
	}

	SubShader 
    {
		
        Tags 
        { 
            "RenderType"="Opaque" 
        }

        
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vert 
        #pragma surface surf Standard fullforwardshadows

		#pragma target 4.0
        #include "Assets/Project/Shaders/Include/snoise.cginc"

        uniform sampler2D _MainTex;
        
        uniform float _Cloak;
        uniform float4 _NoiseSettings;
        uniform float4 _NoiseSettings2;
            
        
		struct Input 
        {
			float2 uv_MainTex : TEXCOORD0;
            float2 noiseUV : TEXCOORD1;
		};

		uniform float _Glossiness;
        uniform float _Metallic;
        
        
        void vert(inout appdata_full v, out Input o) 
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.uv_MainTex = v.texcoord.xy;
            o.noiseUV = (v.texcoord.xy + _NoiseSettings.zw) * _NoiseSettings.xy;
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
            return abs(baseNoiseValue);
        }

		void surf (Input IN, inout SurfaceOutputStandard o) 
        {
            float noise = 0;
            float weight = _NoiseSettings2.z;
            float frequency = 1;
            const float gain = _NoiseSettings2.x;
            const float frequencyMul = _NoiseSettings2.y;
            for (uint i = 0; i < 8; ++i)
            {
                noise += weight * NoiseFunction(snoise(IN.noiseUV * frequency), i);
                weight *= gain;
                frequency *= frequencyMul;
            }
         
            float postNoise = saturate(PoseNoiseFunction(noise));
            clip( postNoise - _Cloak);
        
			float4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
            o.Emission = 0;
            o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}

	FallBack Off
}
