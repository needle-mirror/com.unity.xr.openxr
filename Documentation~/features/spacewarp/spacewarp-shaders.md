---
uid: openxr-spacewarp-shaders
---

# Shaders and SpaceWarp

Application SpaceWarp requires shaders that record XR motion vectors, which are used to predict the position of pixels in the extrapolated frames. If you use a shader that doesn't record XR motion vectors, rendered objects that use it won't update in the synthesized frames. These objects might appear to stutter because they are effectively rendered at half the framerate. Refer to [Understand Application SpaceWarp](xref:openxr-spacewarp-overview) for information about how SpaceWarp works.

## Compatible URP shaders {#compatible-urp-shaders}

The following URP base shaders are SpaceWarp compatible:

* [Lit](xref:urp-lit-shader)
* [Unlit](xref:urp-unlit-shader)
* [Complex Lit](xref:urp-shader-complex-lit)
* [Simple Lit](xref:urp-simple-lit-shader)
* [Baked Lit](xref:urp-baked-lit-shader)

For Shadergraph, the following **Material** options for the **Universal** [Graph Target](https://docs.unity3d.com/Packages/com.unity.shadergraph@17.0/manual/Graph-Target.html) are SpaceWarp compatible:

* Lit
* Unlit

### Compatible UGUI shaders {#compatible-ugui-shaders}

The following UGUI base shaders are SpaceWarp compatible:

* [UI-Default](xref:ui-default)
* [UI-DefaultETC1](xref:ui-defaultetc1-shader)
* [UI Unlit Detail](xref:ui-unlit-detail)

### Compatible TMP shaders {#compatible-tmp-shaders}

The following TMP shader is SpaceWarp compatible:

* [Mobile/Distance Field With SpaceWarp Compatibility](xref:tmp-sdf-mobile-spacewarp)

> [!NOTE]
> If you are unable to use SpaceWarp-compatible UI shaders refer to [UI incompatibility workaround](xref:openxr-spacewarp-ui#ui-incompatibility-workaround) for a workaround that can help minimize visual artifacts.

## Modify custom shaders {#modify-custom-shaders}

To enable your custom shaders to work with Application SpaceWarp, you must add support for the XRMotionVectors render pass.

First, add the `_XRMotionVectorsPass` property to your shader. This property allows you to enable SpaceWarp on a per-material basis. (Refer to [Configure Materials for SpaceWarp](xref:openxr-spacewarp-materials) for information about how it works with the URP shaders that support SpaceWarp.)

``` lang-hlsl
[HideInInspector] _XRMotionVectorsPass("_XRMotionVectorsPass", Float) = 1.0
```

Next, add the following shader subpass declaration:

``` lang-hlsl
Pass
{
        Name "XRMotionVectors"
        Tags { "LightMode" = "XRMotionVectors" }
        ColorMask RGBA

        // Stencil write for obj motion pixels
        Stencil
        {
        WriteMask 1
                Ref 1
                Comp Always
                Pass Replace
        }

        HLSLPROGRAM
        #pragma shader_feature_local _ALPHATEST_ON
        #pragma multi_compile _ LOD_FADE_CROSSFADE
        #pragma shader_feature_local_vertex _ADD_PRECOMPUTED_VELOCITY
        #define APPLICATION_SPACE_WARP_MOTION 1

        #include "Packages/com.unity.render-pipelines.universal/Shaders/BakedLitInput.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ObjectMotionVectors.hlsl"
        ENDHLSL
}
```

## Modify custom UI shaders {#modify-custom-ui-shaders}

To enable your custom shaders to work with Application SpaceWarp, you must add support for the XRMotionVectors render pass.

First, add the `_XRMotionVectorsPass` property to your shader. This property allows you to enable SpaceWarp on a per-material basis. (Refer to [Configure Materials for SpaceWarp](xref:openxr-spacewarp-materials) for information about how it works with the URP shaders that support SpaceWarp.)

``` lang-hlsl
[HideInInspector] _XRMotionVectorsPass("_XRMotionVectorsPass", Float) = 1.0
```

Next, add the following shader subpass declaration. Include the following important elements:

* The necessary declarations and defines at the start of the new subshader pass (ZWrite On is also important since spacewarp requires to write to depth)
* The necessary members to the structs of the vertex and pixel shader
* The vertex/pixel shader code

> [!NOTE]
> This shader code is only an example and you must base your implementation on the default subshader pass so that your UI gets rendered the same way during the spacewarp pass.

``` lang-hlsl
Pass
{
        Name "XRMotionVectors"
        Tags { "LightMode" = "XRMotionVectors" }
        ZWrite On

        ///////////////////////////////
        // Includes/Pragmas/Defines from the default pass of the UI shader
        ///////////////////////////////

        float4x4 unity_MatrixPreviousM;
        #define UNITY_MATRIX_M     unity_ObjectToWorld
        #define UNITY_PREV_MATRIX_M   unity_MatrixPreviousM
        float4x4 _PrevViewProjMatrixStereo[2];
        float4x4 _NonJitteredViewProjMatrixStereo[2];
        #define  _PrevViewProjMatrix  _PrevViewProjMatrixStereo[unity_StereoEyeIndex]
        #define  _NonJitteredViewProjMatrix _NonJitteredViewProjMatrixStereo[unity_StereoEyeIndex]
        float _SpaceWarpNDCModifier;
        float _XRMotionVectorsPass;

        ///////////////////////////////
        // This is the struct for the input parameters of the vertex shader
        ///////////////////////////////
        struct vertex_t {
            UNITY_VERTEX_INPUT_INSTANCE_ID
            ///////////////////////////////
            // The other parameters
            ///////////////////////////////
            float4    oldvertex        : TEXCOORD4; // This needs to be added in this struct
        };

        ///////////////////////////////
        // This is the struct for the input parameters of the pixel shader
        ///////////////////////////////
        struct pixel_t{
            UNITY_VERTEX_INPUT_INSTANCE_ID
            ///////////////////////////////
            // The other parameters
            ///////////////////////////////
            // These two need to be added in this struct
            float4 positionCSNoJitter         : POSITION_CS_NO_JITTER;
            float4 previousPositionCSNoJitter : PREV_POSITION_CS_NO_JITTER;
        };

pixel_t VertShader(vertex_t input)
{
    ///////////////////////////////
    // This is an example of the code you need to add.
    // This will vary based on the code of the original shader pass
    // You need to adapt it
    // The important part is that for the computation of the CS position, you NEED to use
    // the UNJITTERED version of the ViewProj matrix to compute the current position
    ///////////////////////////////
    float4 vert = input.vertex;
    vert.x += _VertexOffsetX;
    vert.y += _VertexOffsetY;
    float4 oldvert = input.oldvertex;
    oldvert.x += _VertexOffsetX;
    oldvert.y += _VertexOffsetY;
    // We do not need jittered position in ASW
    float4 vPosition = mul(_NonJitteredViewProjMatrix, mul(UNITY_MATRIX_M, vert));
    output.positionCSNoJitter = vPosition;
    output.previousPositionCSNoJitter = mul(_PrevViewProjMatrix, mul(UNITY_PREV_MATRIX_M, oldvert));

    ///////////////////////////////
    // The rest of the vertex shader code
    ///////////////////////////////

}

float4 PixShader(pixel_t input) : SV_Target
{
    ///////////////////////////////
    // The rest of the pixel shader code
    ///////////////////////////////

    ///////////////////////////////
    // This is an example of the code you need to add.
    // This will vary based on the code of the original shader pass
    // You need to adapt it
    ///////////////////////////////

    // This especially important for shaders that render a quad but draw a different shape, like text
    clip(c.a * _XRMotionVectorsPass - 0.001);

    float3 posNDC = input.positionCSNoJitter.xyz * rcp(input.positionCSNoJitter.w);
    float3 prevPosNDC = input.previousPositionCSNoJitter.xyz * rcp(input.previousPositionCSNoJitter.w);

    float3 velocity;
    // Calculate forward velocity
    velocity = (posNDC.xyz - prevPosNDC.xyz);

    #if UNITY_UV_STARTS_AT_TOP
    velocity.y = velocity.y * _SpaceWarpNDCModifier;
    #endif

    return float4(velocity.xyz, 1.0);
}
        ENDHLSL
}
```
