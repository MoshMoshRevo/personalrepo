Shader "Unlit/Faceted"
{
    Properties
    {
        _BaseCol("Base colour", Color) = (1,1,1,1)
        _TopCol("Top colour", Color) = (1,1,1,1)
		_CutoffHeight("Cut Off Height", float) = 0
		[MaterialToggle] _CutOffBelow("Cut Off Below Set Height", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "SplatCount" = "4"
            "Queue" = "Geometry-100"
            "RenderType" = "Opaque"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
           
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldPos: TEXCOORD2;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _TopCol, _BaseCol;
			float _CutoffHeight;
			float _CutOffBelow;
           
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				if(_CutOffBelow == 0)
				{
					if(o.worldPos.y >= _CutoffHeight)
					{
						o.vertex.y += 50000;
					}
				}
				else
				{
					if(o.worldPos.y < _CutoffHeight)
					{
						o.vertex.y += 50000;
					}
				}

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
                float3 x = ddx(i.worldPos);
                float3 y = ddy(i.worldPos);
 
                float3 norm = -normalize(cross(x,y));
 
                // Assume basic light shining from above
                float l = saturate(dot(norm, float3(0,1,0)));
                fixed4 col = lerp(_BaseCol, _TopCol, l);
 
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

				if(_CutOffBelow == 0)
				{
					if(i.worldPos.y >= _CutoffHeight)
					{
						col = fixed4(0,0,0,0);
					}
				}
				else
				{
					if(i.worldPos.y < _CutoffHeight+10)
					{
						col = fixed4(0,0,0,0);
					}
				}
				
                return col;
            }
            ENDCG
        }
    }
}