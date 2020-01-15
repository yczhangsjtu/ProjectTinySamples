using System;
using Unity.Entities;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Tiny.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tiny3D
{
    [UpdateInGroup(typeof(GameObjectConversionGroup))]
    public class DynamicUIConversion : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            var mesh = new Mesh()
            {
                vertices = new Vector3[]
                {
                    new Vector3(-3.0f, -1.0f),
                    new Vector3(-3.0f, 1.0f),
                    new Vector3(3.0f, 1.0f),
                    new Vector3(3.0f, -1.0f),
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
            var texture = Resources.Load<Texture2D>("SkyGradient");
            material.SetColor("_BaseColor", Color.blue);
            material.SetFloat("_Cull", 0.0f);
            material.SetTexture("_BaseMap", texture);
            
            Entities.WithNone<RenderMesh>().WithAll<DynamicUI>().ForEach((UnityEngine.MeshRenderer uMeshRenderer) =>
            {
                var primaryEntity = CreateAdditionalEntity(uMeshRenderer);
                var meshEntity = CreateAdditionalEntity(mesh);
                var entityMaterial = CreateAdditionalEntity(material);

                DstEntityManager.AddComponentData(primaryEntity, new Unity.Tiny.Rendering.MeshRenderer()
                {
                    material = entityMaterial,
                    startIndex = 0,
                    indexCount = 1
                });
                DstEntityManager.AddComponentData(primaryEntity, new WorldBounds());
                if (DstEntityManager.HasComponent<LitMaterial>(entityMaterial))
                {
                    DstEntityManager.AddComponentData(primaryEntity, new LitMeshReference() { mesh = meshEntity });
                }
                else if (DstEntityManager.HasComponent<SimpleMaterial>(entityMaterial))
                {
                    DstEntityManager.AddComponentData(primaryEntity, new SimpleMeshReference() { mesh = meshEntity });
                }
                var renderMesh = new RenderMesh
                {
                    mesh = mesh,
                    castShadows = ShadowCastingMode.Off,
                    receiveShadows = false,
                    layer = 0,
                    material = material,
                };
                DstEntityManager.AddComponent<RenderMesh>(primaryEntity);
                DstEntityManager.AddSharedComponentData(primaryEntity, renderMesh);
                DstEntityManager.AddComponentData(primaryEntity, new LocalToWorld()
                {
                    Value = float4x4.identity
                });
                DstEntityManager.AddComponent<DynamicUI>(primaryEntity);
            });
            
        }
    }
}