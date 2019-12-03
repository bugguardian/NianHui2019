Shader "NIANHUI/PShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ColorOL("Outline Color", Color) = (1,1,1,1)
		_SizeOL("Outline Size", Range(0.01, 0.5)) = 0.15
    }
    SubShader
    {
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent"}

		ColorMask RGB
		ZWrite Off
		Cull Off
		LOD 100

		CGINCLUDE
		#include "UnityCG.cginc"
		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
			float3 normal : NORMAL;
			fixed4 color : COLOR;
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			fixed4 color : COLOR;
			float4 vertex : SV_POSITION;
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;
		fixed4 _ColorOL;
		float _SizeOL;

		v2f vert(appdata v)
		{
			v2f o;
			float4 pos = v.vertex + float4(v.normal * _SizeOL, 0) * v.color.r;
			o.vertex = UnityObjectToClipPos(pos);
			o.uv = TRANSFORM_TEX(v.uv, _MainTex); 
			o.color = v.color;
			return o;
		}
		fixed4 frag(v2f i) : SV_Target
		{
			float mask = i.color.r;
			fixed4 col = (float4(i.uv,1,1) * i.color.r + tex2D(_MainTex, i.uv) * i.color.g * 1) * i.color.a;
			return col;
		}
		ENDCG

		Pass // Picture
		{
			//Blend SrcAlpha OneMinusSrcAlpha
			Blend SrcAlpha One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
    }
}
