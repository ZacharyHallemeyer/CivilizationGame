Shader "Custom/Tile"
{
	    Properties { // input data
        _ColorA ("Color A", Color ) = (1,1,1,1)
        _ColorB ("Color B", Color ) = (1,1,1,1)
        _amplitude ("Amplitude", Float ) = .01
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

            #define TAU 6.28318530718

            float4 _ColorA;
            float4 _ColorB;
            float _amplitude;
            static float _nextX = 0;
            static float _nextY = 0;

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
                /*
                float randNum = random(i.uv);
                if(randNum <= .1)
                {
                    if(random(i.uv) < 1)
                    {
                        return float4(_Color.r + random(i.uv), _Color.g + random(i.uv), 
                                      _Color.b + random(i.uv), 1);
                    }
                }
                */
                float colorB = float4(_ColorA.r + random(i.uv), _ColorA.g + random(i.uv), 
                                      _ColorA.b + random(i.uv), 1);

                float4 gradient = lerp(_ColorA, _ColorB, i.uv.y);

                float xOffset = cos( i.uv.x * TAU * 8 ) * _amplitude;                
                float t = cos( (i.uv.y + xOffset - _Time.y * 0.1) * TAU * 5) * 0.5 + 0.5;
                t *= 1-i.uv.y;

                
                if(abs(i.uv.x - i.uv.y) < .01)
                {
                    return float4(0,0,0,1);
                }
                else if(i.uv.x + i.uv.y > .965 && i.uv.x + i.uv.y < .99)
                {
                    return float4(0,0,0,1);
                }

                return gradient * t;
            }
            
            ENDCG
        }
    }
}
