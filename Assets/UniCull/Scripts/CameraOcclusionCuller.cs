using UnityEngine;
using System.Collections.Generic;
using UniPrep.Utils;

namespace UniCull {
    public class CameraOcclusionCuller : MonoBehaviour {
        static CameraOcclusionCuller instance;
        public static CameraOcclusionCuller Instance {
            get {
                if (instance == null)
                    instance = GameObject.FindObjectOfType<CameraOcclusionCuller>();
                return instance;
            }
        }

        public float scale;
        public int frameLength;

        int rayCols, rayRows;
        List<OcclusionCulledRenderer> occCullRenderers = new List<OcclusionCulledRenderer>();
        HashSet<OcclusionCulledRenderer> scannedRenderers = new HashSet<OcclusionCulledRenderer>();

        new Camera camera;
        RaycastHit hit;
        Ray ray;
        List<Ray> rays = new List<Ray>();
        bool doScan = true;

        public void Register(OcclusionCulledRenderer obj) {
            if (!occCullRenderers.Contains(obj))
                occCullRenderers.Add(obj);
        }

        public void Deregister(OcclusionCulledRenderer obj) {
            if (occCullRenderers.Contains(obj))
                occCullRenderers.Remove(obj);
        }

        private void Update() {
            foreach (var r in rays)
                Debug.DrawRay(r.origin, r.direction * camera.farClipPlane, Color.red);
            rays.Clear();
            if (doScan) Scan();
        }

        void Scan() {
            doScan = false;

            OverFrames.Get().For(0, rayRows, frameLength, i => {
                for(int j = 0; j < rayCols; j++) {
                    var screenPoint = new Vector3(j * (float)Screen.width / rayCols, i  * (float)Screen.height / rayRows);
                    var ray = camera.ScreenPointToRay(screenPoint);
                    rays.Add(ray);

                    if (Physics.Raycast(ray, out hit, camera.farClipPlane)) {
                        var collider = hit.collider;
                        if (collider == null)
                            break;
                        var ocr = collider.GetComponent<OcclusionCulledRenderer>();
                        if (ocr != null) {
                            if (!scannedRenderers.Contains(ocr))
                                scannedRenderers.Add(ocr);
                        }

                    }
                }
            },
            () => {
                for (int i = 0; i < occCullRenderers.Count; i++) {
                    if (scannedRenderers.Contains(occCullRenderers[i]))
                        occCullRenderers[i].MakeVisible();
                    else
                        occCullRenderers[i].MakeInvisible();
                }
                scannedRenderers.Clear();
                doScan = true;
            });
        }

        void Start() {
            camera = GetComponent<Camera>();
            rayCols = (int)(Screen.width * scale);
            rayRows = (int)(Screen.height * scale);
        }

        private void OnDisable() {
            for (int i = 0; i < occCullRenderers.Count; i++)
                occCullRenderers[i].MakeVisible();
        }
    }
}