Shader "Custom/ObjectIDShader"
{

	Properties {
	}


    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            Name "ObjectPositionToColor"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

		    sampler2D _CameraDepthTexture;


            struct Attributes
            {
                float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };


		    float Hash(float3 p)
            {
				p = p + float3(1.1723930,1.1723930,1.1723930);
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }
            Varyings vert(Attributes input)
            {
                Varyings output;
                float4 worldPos = mul(unity_ObjectToWorld, input.positionOS);
                output.positionCS = mul(UNITY_MATRIX_VP, worldPos);
				    output.normalWS = normalize(mul((float3x3)unity_ObjectToWorld, input.normalOS));
				output.screenPos = ComputeScreenPos(UnityObjectToClipPos(input.positionOS));
				return output;
            }

            half4 frag(Varyings input) : SV_Target
            {

				float3 normal = normalize(input.normalWS);
				//Hash an object id by position
                float3 objectPosition = unity_ObjectToWorld._m03_m13_m23;
				float hashValue = Hash(objectPosition);
			    float2 screenUV = input.screenPos.xy / input.screenPos.w;
			   float eyeDepth = tex2D(_CameraDepthTexture, screenUV);
                //Store object ID and depth
				return half4(hashValue, eyeDepth,0,1);
            }


            ENDHLSL
        }
    }
    FallBack "Diffuse"
}

