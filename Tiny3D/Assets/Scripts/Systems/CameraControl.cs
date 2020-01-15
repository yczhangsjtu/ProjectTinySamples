using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Tiny3D
{
    public class CameraControl : ComponentSystem
    {
        public static float cameraHeight = math.PI / 4;
        public static float cameraDistance = 20;
        public static float cameraAngle = math.PI / 4;
        public const float minCameraHeight = math.PI / 8;
        public const float maxCameraHeight = math.PI / 2 - 0.1f;
        
#if !UNITY_DOTSPLAYER
        UnityEngine.Transform cameraTransform;
#endif
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<Unity.Tiny.Rendering.Camera>();
        }
        
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
#if !UNITY_DOTSPLAYER
            cameraTransform.position = new float3(
                cameraDistance * math.sin(cameraAngle) * math.cos(cameraHeight),
                cameraDistance * math.sin(cameraHeight),
                cameraDistance * math.cos(cameraAngle) * math.cos(cameraHeight));
            cameraTransform.LookAt(new float3(0, 0, 0));
//#else
//            var cameraEntity = GetSingletonEntity<Unity.Tiny.Rendering.Camera>();
//            var cameraRot = EntityManager.GetComponentData<Rotation>(cameraEntity).Value;
//            EntityManager.SetComponentData(cameraEntity, new Translation {Value = new float3(
//                cameraDistance * math.sin(cameraAngle),
//                cameraHeight,
//                cameraDistance * math.cos(cameraAngle))});
//            
//            EntityManager.SetComponentData(cameraEntity, new Rotation
//            {
//                Value = quaternion.EulerXYZ(math.PI + cameraAngle, math.PI + cameraHeight, 0)
//            });
#endif
        }
    }
}