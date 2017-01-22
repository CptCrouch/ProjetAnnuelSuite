Shader "Custom/FadingOut"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_MainColor("_MainColor", Color) = (1,1,1,1)
		_Dissolution("TextureDissolution", 2D) = "white" {}
		_BurnColor("Burn Color", Color) = (1,0,0,1)
		_BurnLength("Burn Length", Range(0, 1)) = 0.1
		_Didi("Dissolution", Range(0, 1)) = 0.5
		_Scale("Scale", vector) = (1, 1, 1, 1)
		_EtirementScale("EtirementScale",float) = 2
		_IlluminationTexture("IllumTex",float) = 0.7
		_ObjectPosition("ObjectPos",vector) = (0,0,0,1)
	}
		SubShader
	{
		//		Tags { "RenderType"="Opaque" }
		Tags{					//Importe la possibilité de jouer avec l'alpha
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
	}
		LOD 100

		Pass
	{
		Blend SrcAlpha OneMinusSrcAlpha	
		cull off	//afficher les faces intérieures et exterieures ("cull back" pour cacher les faces intérieures, "cull front" pour n'afficher que les faces intérieures)

		CGPROGRAM
#pragma vertex vert			
#pragma fragment frag		


#include "UnityCG.cginc"


		struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
		float3 normal : NORMAL;
		float3 viewDir : TEXCOORD1;
		float3 vertexWorld : TEXCOORD2;
		float4 screenUV : TEXCOORD3;
	};

	sampler2D _MainTex;
	sampler2D _Dissolution;
	float4 _MainTex_ST;
	float _Didi;
	float3 _BurnColor;
	float _BurnLength;
	float4 _MainColor;
	float4 _Scale;
	float _EtirementScale;
	float _IlluminationTexture;
	float3 _ObjectPosition;


	v2f vert(appdata_full v)
	{
		v2f o;

		//On applique la convertion de matrice sur l'appdata_full "o" également. La varying "vertexWorld" nous sert plus tard pour appliquer une texture par rapport à sa position dans l'espace
		o.vertexWorld = mul(unity_ObjectToWorld, v.vertex);	//xmatrice du monde (local to global (connecte au component transform))
		o.vertex = UnityObjectToClipPos(v.vertex);	//transforme la pos brut du vertex en pos monde ? (à vérifier à quoi ça sert)
		o.screenUV = ComputeScreenPos(o.vertex);	//transforme la coordonée du vertex sur l'écran (dimention de l'affichage ?) en coordonnée UV de la camera (de 0 à 1)
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);		//uv dans appdata_full c'est texcoord
		o.viewDir = WorldSpaceViewDir(v.vertex);	//transforme la position en direction. On la stocke dans une varying à laquelle on pourra accéder dans le fragment shader
		o.normal = v.normal;
		return o;
	}

	//Le fragment shade renvoie un vecteur4 (rgba). On y importe notre structure v2f que l'on nomme "i". Nous avons donc accès à toutes les varying avec lesquelles on a joué dans le vertex shader
	//SV_Target = cible de rendu. Il existe SV_Target1, SV_Target2, etc. Il y a également un SV_Depth qui permet d'accéder au Zbuffer (profondeur du pixel) (à vérifier)
	fixed4 frag(v2f i) : SV_Target
	{
		//fixed4 col = tex2D(_MainTex, i.uv);		//Importe la texture dans le cannal uv0
		fixed4 col = _MainColor;

		col.rgb *= dot(normalize(i.normal), normalize(i.viewDir)) * _IlluminationTexture + 0.5;	//plus l'angle de la face est different de la vue de la camera, plus c'est assombri (+ Augmentation de la luminance de l'objet faute de savoir faire un rendu metallique)

			
		float lum = Luminance(tex2D(_Dissolution, i.uv/**(_ObjectPosition.x + _ObjectPosition.z)/_EtirementScale)*/));	//recup le grayscale en fonction de l'uv et multiplie par la moyenne des trois composantes de la scale pour étaler la texture sur l'objet et eviter qu'elle ne soit trop étirée

		col.a *= lerp(1, 0, step(_Didi, lum));	//lerp alpha hard grace a un seuil de luminance
	
		fixed3 col3 = _BurnColor;	//Ajout d'une couleur pour les bords

		col.rgb = lerp(col.rgb, col3, step(_Didi - _BurnLength, lum));		//Application de l'effet de bord brulés
		return col;
	}
		ENDCG
	}
	}
}
