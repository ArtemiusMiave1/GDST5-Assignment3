Shader "Custom/SeamBlendPost"
{

	Properties {
	   [Header(Quality)]
		_KernelSize("Blend Iterations",int) = 10
		_DepthTreshold("Depth Margin", float) = 0.05
	   [Header(Look)]
	    _KernelRadius("Blend Radius", float) = 0.1
	    _DepthFalloff("Depth Falloff",float) = 3

	   [Header(Range)]
	    _DepthRangeMin("Min Active Distance",float) = 3
	    _DepthRangeMax("Max Active Distance",float) = 50
	    _DepthRangeFall("Outside Active Distance falloff radius",float) = 3
	}


    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            Name "FullscreenPass"

            ZWrite Off
            Cull Off
            ZTest Always
            Blend Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
			#include "UnityCG.cginc"

		    CBUFFER_START(UnityPerMaterial)
			int _KernelSize;
			float _KernelRadius;
			float _DistanceFalloff;
			float _DepthFalloff;
			float _DepthRangeMin;
			float _DepthRangeMax;
			float _DepthRangeFall;
			float _DepthTreshold;
			CBUFFER_END

			sampler2D _ObjectIDTexture;
			sampler2D _CameraOpaqueTexture;
		    sampler2D _CameraDepthTexture;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

			struct Varyings
			{
				float4 positionHCS : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			Varyings Vert(uint vertexID : SV_VertexID)
			{
				Varyings o;

				float2 pos = float2((vertexID << 1) & 2, vertexID & 2);
				o.positionHCS = float4(pos * 2.0 - 1.0, 0, 1);
				o.uv = pos;

				return o;
			}

            half4 Frag(Varyings input) : SV_Target
            {	

				float2 UV = float2(input.uv.x, input.uv.y);
			   	float actualDepth = tex2D(_CameraDepthTexture, UV);
			    
				float4 sceneColor = tex2D(_CameraOpaqueTexture, UV);
				
				float4 objectIDColor = tex2D(_ObjectIDTexture, UV);

				float sceneDepth = objectIDColor.y;

				float2 closestSeamLocation = float2(0,0);
				float minDist = 9999999;
				//Kernel to find location of object seam
				for(int x = -_KernelSize; x < _KernelSize; x++) {
					for(int y = -_KernelSize; y < _KernelSize; y++) {
						float2 offset = float2(x,y)*_KernelRadius*sceneDepth/_KernelSize;
						float2 SampleUV = UV + offset;
						float4 id = tex2D(_ObjectIDTexture, SampleUV);
						if (id.x != objectIDColor.x) {
							float sqrDist = dot(offset,offset);
							if (sqrDist < minDist) {
								minDist = sqrDist;
								closestSeamLocation = offset;
							}

						}
					}
				}
				

				float4 otherID = tex2D(_ObjectIDTexture, UV + closestSeamLocation*2);
				float4 otherColor = tex2D(_CameraOpaqueTexture, UV + closestSeamLocation*2);
				float otherDepth = otherID.y;

				//Weighing for blendradius and depth ignoring
				float depthDiff = abs(otherDepth-sceneDepth);
				float maxFalloffDist = (_KernelRadius)*sceneDepth;
				float spatialWeight = saturate(0.5 - sqrt(minDist) / maxFalloffDist);
				float depthWeight = 1-saturate(depthDiff / (_DepthFalloff*_KernelRadius));
				float finalWeight = spatialWeight * depthWeight;
				
				//Falloff outside ranges
				if (sceneDepth > _DepthRangeMax) {
					finalWeight *= 1-saturate((sceneDepth-_DepthRangeMax)/_DepthRangeFall);
				}
				if (sceneDepth < _DepthRangeMin) {
					finalWeight *= 1-saturate((_DepthRangeMin-sceneDepth)/_DepthRangeFall);
				}

				//Check if not behind an ignored object
				if (sceneDepth > actualDepth+_DepthTreshold) finalWeight = 0;

			    //Mirrored color, with lerp for transition
			    return lerp(sceneColor, otherColor, finalWeight );

            }
            ENDHLSL
        }
    }
}
