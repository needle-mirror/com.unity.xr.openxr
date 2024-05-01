---
uid: openxr-foveated-rendering
---
# Foveated Rendering

Unity OpenXR provides support for various techniques of foveated rendering and will attempt to enable the following foveation techniques in this order depending on availability:

1. Gaze-based Fragment Density Map (GFDM) from provider.
2. Fixed Fragment Density Map (FFDM) from provider.
3. Fragment Shading Rate (FSR) using a provider's texture.
4. Fragment Shading Rate using a compute shader calculated from the asymetric FOVs the provider gives.
