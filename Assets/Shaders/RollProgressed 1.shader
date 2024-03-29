﻿Shader "Custom/BzKovSoft/RollProgressed"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _Tess ("Tessellation", Range(1,32)) = 4
        _UpDir ("Up Dir", Vector) = (1,0,0,0)
        _RollDir ("Roll Dir", Vector) = (0,1,0,0)
        _Radius ("Radius", float) = 0
        _PointX ("PointX", float) = 0
        _PointY ("PointY", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf Standard addshadow vertex:vert tessellate:tessFixed
        #pragma multi_compile_instancing
        #include "UnityCG.cginc"

        #pragma target 4.6
        #pragma require tessellation tesshw

        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float _Tess;
		float4 _UpDir;
		float4 _RollDir;
		half _Radius;
        half _PointX;
        half _PointY;
        //UNITY_INSTANCING_BUFFER_START(Props)
        //   UNITY_DEFINE_INSTANCED_PROP(float, _CycleY)
        //UNITY_INSTANCING_BUFFER_END(Props)

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_SubstituteBumpMap;
        };

        float4 tessFixed()
        {
            return _Tess;
        }

		void vert(inout appdata_full v)
		{
            float3 v0 = v.vertex.xyz;

            float3 upDir = normalize(_UpDir);
            float3 blendDir = normalize(_RollDir);

			//float y = UNITY_ACCESS_INSTANCED_PROP(Props, _PointY);
            float y = _PointY;
            float dP = dot(v0 - upDir * y, upDir);
            dP = max(0, dP);
            float3 fromInitialPos = upDir * dP;
            v0 -=fromInitialPos;

            float length = 2 * UNITY_PI * _Radius;
            float r = dP / length;
            float a = 2 * r * UNITY_PI;
            
            float s = sin(a);
            float c = cos(a);
            float one_minus_c = 1.0 - c;

            float3 axis = normalize(cross(upDir, blendDir));
            float3x3 rot_mat = 
            {   one_minus_c * axis.x * axis.x + c, one_minus_c * axis.x * axis.y - axis.z * s, one_minus_c * axis.z * axis.x + axis.y * s,
                one_minus_c * axis.x * axis.y + axis.z * s, one_minus_c * axis.y * axis.y + c, one_minus_c * axis.y * axis.z - axis.x * s,
                one_minus_c * axis.z * axis.x - axis.y * s, one_minus_c * axis.y * axis.z + axis.x * s, one_minus_c * axis.z * axis.z + c
            };
            float3 cycleCenter = blendDir * _PointX + blendDir * _Radius + upDir * y;
            v0.xyz = mul(rot_mat,  v0.xyz - cycleCenter) + cycleCenter;

			v.vertex.xyz = v0;
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
        
            o.Albedo = c;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
