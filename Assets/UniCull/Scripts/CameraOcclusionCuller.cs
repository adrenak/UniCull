using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        public int rayCols;
        public int rayRows;
        public int updateRate;
        List<OcclusionCulledRenderer> occCullRenderers = new List<OcclusionCulledRenderer>();
        HashSet<OcclusionCulledRenderer> scannedRenderers = new HashSet<OcclusionCulledRenderer>();

        new Camera camera;
        RaycastHit hit;
        Ray ray;
        List<Ray> rays = new List<Ray>();

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
        }

        IEnumerator Start() {
            camera = GetComponent<Camera>();
            while (true) {
                scannedRenderers.Clear();
                for (int i = 0; i < rayRows; i++) {
                    for (int j = 0; j < rayCols; j++) {
                        var v = new Vector3(i * (float)Screen.width / rayRows, j * (float)Screen.height / rayCols);
                        ray = camera.ScreenPointToRay(v);
                        rays.Add(ray);
                        if (Physics.Raycast(ray, out hit, camera.farClipPlane)) {
                            var collider = hit.collider;
                            if (collider == null)
                                break;
                            var ocr = collider.GetComponent<OcclusionCulledRenderer>();
                            if (ocr == null)
                                break;

                            if (!scannedRenderers.Contains(ocr))
                                scannedRenderers.Add(ocr);
                        }
                    }
                    if (i % updateRate == 0)
                        yield return new WaitForEndOfFrame();
                }

                for (int i = 0; i < occCullRenderers.Count; i++) {
                    if (scannedRenderers.Contains(occCullRenderers[i]))
                        occCullRenderers[i].MakeVisible();
                    else
                        occCullRenderers[i].MakeInvisible();
                }

                //for (int i = 0; i < updateRate; i++)
                //    yield return new WaitForEndOfFrame();
            }
        }

        private void OnDisable() {
            for (int i = 0; i < occCullRenderers.Count; i++)
                occCullRenderers[i].MakeVisible();
        }
    }
}