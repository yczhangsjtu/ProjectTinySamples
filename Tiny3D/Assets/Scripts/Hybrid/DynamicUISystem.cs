using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;
using Unity.Tiny.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;

namespace Tiny3D
{
    public class DynamicUISystem : ComponentSystem
    {
        private float rotation = 0;
        private Texture2D texture;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            // init();
        }

        protected override void OnUpdate()
        {
            // update();
        }

        private void init()
        {
            var mesh = new Mesh()
            {
                vertices = new Vector3[]
                {
                    new Vector3(-2.0f, -0.5f),
                    new Vector3(-2.0f, 0.5f),
                    new Vector3(2.0f, 0.5f),
                    new Vector3(2.0f, -0.5f),
                },
                uv = new Vector2[]
                {
                    new Vector2(0.0f, 0.0f),
                    new Vector2(0.0f, 1.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(1.0f, 0.0f),
                },
                triangles = new int[]
                {
                    0, 1, 2, 0, 2, 3
                }
            };
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            var material = new Material(shader);
            texture = Resources.Load<Texture2D>("SkyGradient");
            material.SetColor("_BaseColor", Color.blue);
            material.SetFloat("_Cull", 0.0f);
            material.SetTexture("_BaseMap", texture);
            
            var renderMesh = new RenderMesh
            {
                mesh = mesh,
                castShadows = ShadowCastingMode.Off,
                receiveShadows = false,
                layer = 0,
                material = material,
            };
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponent<RenderMesh>(entity);
            EntityManager.AddSharedComponentData(entity, renderMesh);
            EntityManager.AddComponentData(entity, new LocalToWorld()
            {
                Value = float4x4.RotateX(rotation)
            });
            EntityManager.AddComponent<DynamicUI>(entity);
            
        }

        private void update()
        {
            rotation += Time.DeltaTime * 1.5f;
            while (rotation > 360)
            {
                rotation -= 360;
            }
            Entities.WithAll<DynamicUI>().ForEach(entity =>
            {
                EntityManager.SetComponentData(entity, new LocalToWorld()
                {
                    Value = float4x4.RotateX(rotation)
                });
            });
            
        }
    }
}