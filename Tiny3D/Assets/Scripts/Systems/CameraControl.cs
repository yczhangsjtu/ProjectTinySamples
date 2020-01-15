using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tiny3D
{
    public class CameraControl : ComponentSystem
    {
        public static float cameraHeight = 15;
        public static float cameraDistance = 20;
        public static float cameraAngle = math.PI / 4;
        public const float minCameraHeight = 2;
        public const float maxCameraHeight = 30;
        
#if !UNITY_DOTSPLAYER
        UnityEngine.Transform cameraTransform;
#endif
        
        protected override void OnStartRunning()
        {
#if !UNITY_DOTSPLAYER
            cameraTransform = UnityEngine.Camera.main.transform;
#endif
        }
        
        protected override void OnUpdate()
        {
            if (cameraHeight > maxCameraHeight)
            {
                cameraHeight = maxCameraHeight;
            }

            if (cameraHeight < minCameraHeight)
            {
                cameraHeight = minCameraHeight;
            }
            cameraTransform.position = new Vector3(
                cameraDistance * math.sin(cameraAngle),
                cameraHeight,
                cameraDistance * math.cos(cameraAngle));
            cameraTransform.LookAt(new Vector3(0, 0, 0));
        }
    }
}