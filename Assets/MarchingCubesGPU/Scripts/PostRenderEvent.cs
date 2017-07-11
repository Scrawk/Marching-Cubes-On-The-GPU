using UnityEngine;
using System;

namespace MarchingCubesGPUProject
{

    [RequireComponent(typeof(Camera))]
    public class PostRenderEvent : CameraEvent
    {

        public CameraEventHandler OnEvent = delegate { };

        void OnPostRender()
        {
            OnEvent(Camera);
        }

        public static void AddEvent(Camera camera, CameraEventHandler onPostRenderEvent)
        {
            if (camera == null) return;

            PostRenderEvent e = camera.GetComponent<PostRenderEvent>();
            if (e != null)
                e.OnEvent += onPostRenderEvent;
        }

        public static void RemoveEvent(Camera camera, CameraEventHandler onPostRenderEvent)
        {
            if (camera == null) return;

            PostRenderEvent e = camera.GetComponent<PostRenderEvent>();
            if (e != null)
                e.OnEvent -= onPostRenderEvent;
        }

    }

}
