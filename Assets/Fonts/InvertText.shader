// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ddShaders/InvertText" {
    Properties
    {
            _Color ("Tint Color", Color) = (1,1,1,1)
            _MainTex("Main Texture", 2D) = "white"{}
    }
   
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
     
        Lighting Off 
        Cull Off
        ZWrite Off
        Fog { Mode Off }
        //ColorMask 0
       
        Pass
        {
            AlphaTest Greater 0.5
            Blend OneMinusDstColor OneMinusSrcColor //invert blending, so long as FG color is 1,1,1,1
            BlendOp Sub
            SetTexture [_MainTex]
            {
                constantColor [_Color]
                combine texture * constant
            }
        }
       
    } //end subshader
}
 