/*
* Copyright (c) 2019, Collaprime Oy.
* All rights reserved.
*
* This software is the confidential and proprietary information
* of the Collaprime Oy.
* You shall not disclose such Confidential Information and shall use
* it only in accordance with the terms of the license agreement
* you entered into with the Collaprime Oy.
*
* http://www.collaprime.com
*/


Shader "Hidden/GeometryCorrection"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

			float gradientAmount;
			float bottomMaskAmount;
			float maskAmount;

			half4 gradientColor;
			half4 maskColor;

			float leftMaskSlope;
			float leftMaskAmount;
			float rightMaskSlope;
			float rightMaskAmount;

			float2 corner1;
			float2 corner2;
			float2 corner3;
			float2 corner4;

			fixed4 frag (v2f i) : SV_Target
			{

				half4 color;
				float2 coords = i.uv;

				float x2 = corner1.x + (corner2.x - corner1.x) * coords.x;
				float x1 = corner3.x + (corner4.x - corner3.x) * coords.x;

				float x22 = corner1.x + (corner2.x - corner1.x) * (coords.x / coords.y);
				float x21 = corner3.x;

				float k2 = 1 / (x22 - x21);
				float yt2 = k2 * coords.x - k2 * x21;

				coords.x = x2;

				if (x2 - x1 != 0)
				{
					float k1 = 1 / (x2 - x1);
					float yt1 = k1 * coords.x - k1 * x1;

					coords.x = (k1 * x1 - k2 * x21) / (k1 - k2);
				}

				coords.y = k2 * coords.x - k2 * x21;

				/////////////////////////

				float x2V = corner4.y + (corner2.y - corner4.y) * coords.y;
				float x1V = corner3.y + (corner1.y - corner3.y) * coords.y;

				float x22V = corner4.y + (corner2.y - corner4.y) * (coords.y / coords.x);
				float x21V = corner3.y;

				float k2V = 1 / (x22V - x21V);
				float yt2V = k2V * coords.y - k2V * x21V;

				coords.y = x2V;

				if (x2V - x1V != 0)
				{
					float k1V = 1 / (x2V - x1V);
					float yt1V = k1V * coords.y - k1V * x1V;
					coords.y = (k1V * x1V - k2V * x21V) / (k1V - k2V);
				}

				coords.x = k2V * coords.y - k2V * x21V;
				color = tex2D(_MainTex, coords);

				if (i.uv.x > i.uv.y * (-rightMaskSlope) + (1 - rightMaskAmount) || i.uv.x < i.uv.y * (leftMaskSlope)+leftMaskAmount || i.uv.y <(bottomMaskAmount) || maskAmount > 0)
				{
					return maskColor;
				}
				else
				{
					return lerp(color, gradientColor, 1 - saturate((1- i.uv.y) + (1 - gradientColor.a)));
				}
			}
			ENDCG
		}
	}
}

