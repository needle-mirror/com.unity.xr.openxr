#if XR_COMPOSITION_LAYERS
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Provider;
using Unity.XR.CompositionLayers.Services;

namespace UnityEngine.XR.OpenXR.CompositionLayers
{
    /// <summary>
    /// Manages communication of changes between an application and the UnityOpenXR lib for all
    /// <see cref="Unity.XR.CompositionLayers.Layers.LayerData"/> objects.
    /// </summary>
    /// <remarks>
    /// OpenXR providers or extensions that create custom composition layer types or that override how the built-in
    /// layer types are handled, must implement the <see cref="ILayerProvider"/> interface and register instances of
    /// these implementations with the <c>OpenXRLayerProvider</c> via <see cref="RegisterLayerHandler(Type, OpenXRLayerProvider.ILayerHandler)"/>.
    /// </remarks>
    public class OpenXRLayerProvider : ILayerProvider, IDisposable
    {
        /// <summary>
        /// An interface used by the <see cref="OpenXRLayerProvider"/> to communicate layer data changes to
        /// registered layer handlers.
        /// </summary>
        /// <remarks>
        /// <c>ILayerHandler</c> instances must register themselves via
        /// <see cref="OpenXRLayerProvider.RegisterLayerHandler(Type, OpenXRLayerProvider.ILayerHandler)"/>
        /// to specify the <see cref="LayerData"/> type to handle.
        /// If more than one object registers itself as a handler for a specific <see cref="LayerData"/>
        /// type, the last registered handler is used.
        ///
        /// The <see cref="OpenXRCustomLayerHandler{T}"/> class provides a partial, base implementation of this interface that you can
        /// use to create custom layer handlers.
        /// </remarks>
        public interface ILayerHandler
        {

            /// <summary>
            /// Called by the <see cref="OpenXRLayerProvider"/> during the Unity Update loop.
            /// All implementations must call <see cref="OpenXRLayerUtility.AddActiveLayersToEndFrame(void*,void*,int,int)"/> every frame
            /// to add their native layer structs to the <c>endFrameInfo</c> struct inside the UnityOpenXR lib.
            /// </summary>
            public void OnUpdate();

            /// <summary>
            /// Called by the <see cref="OpenXRLayerProvider"/> when a new <see cref="LayerData"/>
            /// object of the type registered to this <c>ILayerHandler</c> instance has been created.
            /// </summary>
            public void CreateLayer(CompositionLayerManager.LayerInfo layerInfo);

            /// <summary>
            /// Called by the <see cref="OpenXRLayerProvider"/> when a <see cref="LayerData"/> object
            /// of the type registered to this <c>ILayerHandler</c> instance has been destroyed or disabled.
            /// </summary>
            public void RemoveLayer(int id);

            /// <summary>
            /// Called by the <see cref="OpenXRLayerProvider"/> when a <see cref="LayerData"/> object
            /// or any attached extension components have had a member modified.
            /// </summary>
            public void ModifyLayer(CompositionLayerManager.LayerInfo layerInfo);

            /// <summary>
            /// Called every frame by the <see cref="OpenXRLayerProvider"/> for all currently active <see cref="LayerData"/> objects
            /// of the type registered to this <c>ILayerHandler</c> instance.
            /// </summary>
            public void SetActiveLayer(CompositionLayerManager.LayerInfo layerInfo);
        }

        /// <summary>
        /// Initializes and returns an instance of <c>OpenXRLayerProvider</c>.
        /// Initializes and registers all the default, built-in layer handlers.
        /// </summary>
        /// <remarks>
        /// The <c>OpenXRLayerProvider</c> is created and disposed by the <see cref="Management.XRLoader"/>.
        /// You do not need to create an instance of <c>OpenXRLayerProvider</c> yourself. Layer handlers
        /// should only use the static methods and properties of this class
        /// </remarks>
        public OpenXRLayerProvider() => InitializeAndRegisterBuiltInHandlers();

        /// <summary>
        /// Calls the methods in its invocation list when the <c>OpenXRLayerProvider</c> has started and registered it's built-in layer handlers.
        /// </summary>
        /// <remarks>
        /// You can use this event to wait for the <c>OpenXRLayerProvider</c> to finish registering its built-in layer handlers
        /// so that you can override them with your own custom layer handlers.
        /// </remarks>
        public static event Action Started;

        /// <summary>
        /// Calls the methods in its invocation list when the <c>OpenXRLayerProvider</c> has stopped and is disposed.
        /// </summary>
        public static event Action Stopped;

        /// <summary>
        /// Reports whether the <c>OpenXRLayerProvider</c> has already been created and started.
        /// </summary>
        public static bool isStarted { get; private set; }

        private static Dictionary<Type, ILayerHandler> LayerHandlers = new Dictionary<Type, ILayerHandler>();

        /// <summary>
        /// Registers a concrete <see cref="ILayerHandler"/> object as the handler for all layers of a specific
        /// <see cref="LayerData"/> subclass.
        /// </summary>
        /// <remarks>
        /// If more than one object registers itself as a handler for a specific <see cref="LayerData"/>
        /// type, the last registered handler is used.
        ///
        /// The <c>OpenXRLayerProvider</c> invokes the registered layer handler's <see cref="ILayerHandler"/> methods
        /// when any object of the associated <see cref="LayerData"/> type is updated in some way.
        /// </remarks>
        /// <param name="layerDataType">The <see cref="LayerData"/> subclass to handle.</param>
        /// <param name="handler">The concrete <c>ILayerHandler</c> instance> to register.</param>
        public static void RegisterLayerHandler(Type layerDataType, ILayerHandler handler)
        {
            if (handler == null)
            {
                LayerHandlers.Remove(layerDataType);
                return;
            }

            LayerHandlers[layerDataType] = handler;
        }

        /// <summary>
        /// Sets the layer provider state on first assignment to the <see cref="CompositionLayerManager" />.
        /// </summary>
        public void SetInitialState(List<CompositionLayerManager.LayerInfo> layers)
        {
            if (OpenXRLayerUtility.mainCameraCache == null)
                OpenXRLayerUtility.mainCameraCache = Camera.main;
            UpdateLayers(layers, null, null, null);
        }

        /// <summary>
        /// Called by the <see cref="CompositionLayerManager" /> to update the current state of layers.
        /// </summary>
        public void UpdateLayers(List<CompositionLayerManager.LayerInfo> createdLayers, List<int> removedLayers, List<CompositionLayerManager.LayerInfo> modifiedLayers, List<CompositionLayerManager.LayerInfo> activeLayers)
        {
            if (createdLayers != null && createdLayers.Count != 0)
            {
                foreach (var created in createdLayers)
                {
                    var layerDataType = created.Layer.LayerData.GetType();
                    if (LayerHandlers.TryGetValue(layerDataType, out ILayerHandler handler))
                        handler?.CreateLayer(created);
                }
            }

            if (modifiedLayers != null && modifiedLayers.Count != 0)
            {
                 foreach (var modified in modifiedLayers)
                 {
                    var layerDataType = modified.Layer.LayerData.GetType();

                    if (LayerHandlers.TryGetValue(layerDataType, out ILayerHandler handler))
                        handler?.ModifyLayer(modified);
                 }
            }

            if (activeLayers != null && activeLayers.Count != 0)
            {
                foreach (var active in activeLayers)
                {
                    // Covers case where layer is destroyed (scene loading)
                    if(active.Layer == null)
                        continue;

                    var layerDataType = active.Layer.LayerData.GetType();
                    if (LayerHandlers.TryGetValue(layerDataType, out ILayerHandler handler))
                        handler?.SetActiveLayer(active);
                }
            }

            foreach (var handler in LayerHandlers.Values)
            {
                if (removedLayers != null && removedLayers.Count != 0)
                {
                    foreach (var removed in removedLayers)
                    {
                        handler?.RemoveLayer(removed);
                    }
                }

                handler?.OnUpdate();
            }
        }

        /// <summary>
        /// Used to clean up.
        /// </summary>
        public void Dispose()
        {
            foreach (var handler in LayerHandlers.Values)
            {
                if (handler is IDisposable)
                {
                    ((IDisposable)handler)?.Dispose();
                }
            }

            LayerHandlers.Clear();
            isStarted = false;
            Stopped?.Invoke();
        }

        ///<summary>For internal use only.</summary>
        public void CleanupState()
        {
        }

        ///<summary>For internal use only.</summary>
        public void LateUpdate()
        {
        }

        private void InitializeAndRegisterBuiltInHandlers()
        {
            var quadLayerHandler = new OpenXRQuadLayer();
            var projectionLayerHandler = new OpenXRProjectionLayer();
            var cylinderLayerHandler = OpenXRCylinderLayer.ExtensionEnabled ? new OpenXRCylinderLayer() : null;
            var cubeLayerHandler = OpenXRCubeLayer.ExtensionEnabled ? new OpenXRCubeLayer() : null;
            ILayerHandler equirectLayerHandler = OpenXREquirect2Layer.ExtensionEnabled ? new OpenXREquirect2Layer() :  OpenXREquirectLayer.ExtensionEnabled ? new OpenXREquirectLayer() : null;

            RegisterLayerHandler(typeof(QuadLayerData), quadLayerHandler);
            RegisterLayerHandler(typeof(CylinderLayerData), cylinderLayerHandler);
            RegisterLayerHandler(typeof(ProjectionLayerData), projectionLayerHandler);
            RegisterLayerHandler(typeof(ProjectionLayerRigData), projectionLayerHandler);
            RegisterLayerHandler(typeof(CubeProjectionLayerData), cubeLayerHandler);
            RegisterLayerHandler(typeof(EquirectMeshLayerData), equirectLayerHandler);

            isStarted = true;
            Started?.Invoke();
        }
    }
}

#endif
