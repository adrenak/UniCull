using System;
using UnityEngine;

namespace UniCull {
    public class DistanceCulledRenderer : MonoBehaviour {
        // INTERNAL STRUCTURES
        // Instead of keeping an array of Renderers, we store renderers in a struct
        // Reason : Class object arrays are not stored contigiously but struct object arrays are
        // Whether this makes much difference here is yet to be tested.
        // Good video for reference : https://www.youtube.com/watch?v=tGmnZdY5Y-E
        [Serializable]
        public struct RendererContainer {
            public Renderer renderer;
        }

        public enum RendererCachingScheme {
            SELF,
            MANUAL,
            CHILDREN
        }

        // FIELDS
        public bool debug;
        public float renderDistance;
        public RendererContainer[] rendererContainers;
        public RendererCachingScheme scheme;

        // ================================================
        // PUBLIC METHODS
        // ================================================
        /// <summary>
        /// Enables all the renderer containers
        /// </summary>
        public void MakeVisible() {
            for (int i = 0; i < rendererContainers.Length; i++)
                rendererContainers[i].renderer.enabled = true;
        }

        /// <summary>
        /// Disables all the renderer components
        /// </summary>
        public void MakeInvisible() {
            for (int i = 0; i < rendererContainers.Length; i++)
                rendererContainers[i].renderer.enabled = false;
        }

        /// <summary>
        /// Returns if any of the renderers in the container array is currently visible
        /// </summary>
        /// <returns></returns>
        public bool IsVIsible() {
            for (int i = 0; i < rendererContainers.Length; i++)
                if (rendererContainers[i].renderer.isVisible)
                    return true;
            return false;
        }

        // ================================================
        // INITIALIZATION
        // ================================================
        private void Start() {
            CacheRenderers();
            DisableAllRendererContainers();
        }

        void CacheRenderers() {
            switch (scheme) {
                case RendererCachingScheme.SELF:
                    var _renderer = GetComponent<Renderer>();
                    if (_renderer != null) {
                        rendererContainers = new RendererContainer[1];
                        rendererContainers[0].renderer = _renderer;
                    }
                    else {
                        string warningMsg = gameObject.name + " has DistanceCulledRenderer component with SELF caching scheme but has no renderer component!";
                        Debug.LogWarning(warningMsg);
                    }
                    break;

                case RendererCachingScheme.CHILDREN:
                    Renderer[] _renderers = GetComponentsInChildren<Renderer>();
                    if(_renderers.Length > 0) {
                        rendererContainers = new RendererContainer[_renderers.Length];
                        for (int i = 0; i < _renderers.Length; i++)
                            rendererContainers[i].renderer = _renderers[i];
                    }
                    break;

                case RendererCachingScheme.MANUAL:
                    break;
            }
        }

        void DisableAllRendererContainers() {
            foreach (RendererContainer c in rendererContainers)
                c.renderer.enabled = true;
        }

        // ================================================
        // LIFECYCLE
        // ================================================
        private void OnEnable() {
            CameraCuller culler;
            if(GetCullerInstance(out culler))
                culler.Register(this);
        }

        private void OnDisable() {
            CameraCuller culler;
            if (GetCullerInstance(out culler))
                culler.Deregister(this);
        }

        bool GetCullerInstance(out CameraCuller culler) {
            culler = CameraCuller.Instance;
            if (culler != null) 
                return true;
            return false;
        }

        private void OnDrawGizmosSelected() {
            if (!debug)
                return;
            Gizmos.color = new Color(1, 0, 0, .1F);
            Gizmos.DrawSphere(transform.position, renderDistance);
        }
    }
}
