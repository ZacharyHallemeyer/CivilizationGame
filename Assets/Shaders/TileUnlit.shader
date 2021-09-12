Shader "Unlit/NewUnlitShader"
{
	Properties // input data
    { 
        _Color ("Color", Color ) = (1,1,1,1)
    }
    SubShader {
        // subshader tags
        Tags {
            "RenderType"="Opaque" // tag to inform the render pipeline of what type this is
        }
        Pass {
            // pass tags
            
            //Cull Off
            //ZWrite Off
            //Blend One One // additive
            
            
            
            //Blend DstColor Zero // multiply
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            float4 _Color;

            // automatically filled out by Unity
            struct MeshData { // per-vertex mesh data
                float4 vertex : POSITION; // local space vertex position
                float3 normals : NORMAL; // local space normal direction
                float4 uv0 : TEXCOORD0; // uv0 diffuse/normal map textures
            };

            // data passed from the vertex shader to the fragment shader
            // this will interpolate/blend across the triangle!
            struct Interpolators {
                float4 vertex : SV_POSITION; // clip space position
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            Interpolators vert( MeshData v ){
                Interpolators o;
                o.vertex = UnityObjectToClipPos( v.vertex ); // local space to clip space
                o.normal = UnityObjectToWorldNormal( v.normals );
                o.uv = v.uv0; //(v.uv0 + _Offset) * _Scale; // passthrough
                return o;
            }

            float random (float2 uv)
            {
                return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123);
            }

            float4 frag( Interpolators i ) : SV_Target {
                float4 outColor;
                
                //outColor = _Color * (random(i.uv.xx)/10, random(i.uv.xx)/10, random(i.uv.xx)/10, 1);
                float randNum = random(i.uv);
                if(2>1)
                {
                    return _Color;
                }

                return _Color;
            }
            
            ENDCG
        }
    }
}
