using System.Collections.Generic;
using Tetric3D;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Transforms;

namespace Tiny3D
{
    public class ProjectionSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<Level>();
        }
        
        protected override void OnUpdate()
        {
            var level = GetSingleton<Level>();
            var projections = new List<Entity>();
            Entities.WithAll<Projection>().ForEach(entity => {
                EntityManager.AddComponent<ProjectionClear>(entity);
            });
            Entities.WithAll<Dropping>().ForEach((ref Cube cube) => {
                var entity = EntityManager.Instantiate(level.projectionPrefab);
                EntityManager.AddComponentData(entity, new Projection {
                    x = cube.x,
                    y = cube.y,
                    h = -1
                });
                EntityManager.SetComponentData(entity, new Translation
                {
                    Value = new float3
                    {
                        x = cube.x,
                        y = -0.5f,
                        z = cube.y
                    }
                });
                projections.Add(entity);
            });
            Entities.WithNone<Dropping>().ForEach((ref Cube cube) =>
            {
                foreach (var entity in projections)
                {
                    var projection = EntityManager.GetComponentData<Projection>(entity);
                    if (projection.x == cube.x &&
                        projection.y == cube.y &&
                        projection.h < cube.h)
                    {
                        EntityManager.SetComponentData(entity, new Projection
                        {
                            x = projection.x,
                            y = projection.y,
                            h = cube.h
                        });
                        EntityManager.SetComponentData(entity, new Translation
                        {
                            Value = new float3
                            {
                                x = projection.x,
                                y = cube.h + 0.5f,
                                z = projection.y
                            }
                        });
                    }
                }
            });
        }
    }
}