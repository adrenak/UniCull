﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UniCull {
    public class CameraDistanceCuller : MonoBehaviour {
        static CameraDistanceCuller instance;
        public static CameraDistanceCuller Instance {
            get {
                if (instance == null)
                    instance = GameObject.FindObjectOfType<CameraDistanceCuller>();
                return instance;
            }
        }

        public int maxChecksPerFrame = 1000;
        List<DistanceCulledRenderer> distCullRenderers = new List<DistanceCulledRenderer>();

        public void Register(DistanceCulledRenderer obj) {
            if(!distCullRenderers.Contains(obj))
                distCullRenderers.Add(obj);
        }

        public void Deregister(DistanceCulledRenderer obj) {
            if(distCullRenderers.Contains(obj))
                distCullRenderers.Remove(obj);
        }

        IEnumerator Start() {
            int index = 0;
            // Update loop replacement
            while (true) {
                // Inner check loop
                while (true) {
                    var isWithinDrawDistance = Vector3.Distance(transform.position, distCullRenderers[index].transform.position) > distCullRenderers[index].renderDistance;
                    if (isWithinDrawDistance)
                        distCullRenderers[index].MakeInvisible();
                    else
                        distCullRenderers[index].MakeVisible();
                    index++;
                    if (index == distCullRenderers.Count) {
                        index = 0;
                        break;
                    }
                    else if (index % maxChecksPerFrame == 0)
                        break;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        private void OnDisable() {
            for (int i = 0; i < distCullRenderers.Count; i++)
                distCullRenderers[i].MakeVisible();
        }
    }
}
