using System.Collections.Generic;
using Tetric3D;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Tiny3D
{
    public class CubeDrop : ComponentSystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<Level>();
        }
        
        protected override void OnUpdate()
        {
            var level = GetSingleton<Level>();
            level.timeLeft -= level.speed * Time.DeltaTime;
            SetSingleton(level);
            
            if (level.timeLeft > 0)
            {
                return;
            }

            level.timeLeft = 1;
            SetSingleton(level);
            
            List<Cube> cubes = new List<Cube>();
            bool hitBottom = false;
            Entities.ForEach((ref Dropping dropping, ref Cube cube) =>
            {
                cubes.Add(cube);
                if (cube.h <= 0)
                {
                    hitBottom = true;
                }
            });
            if (cubes.Count == 0)
            {
                return;
            }
            if (!hitBottom)
            {
                Entities.WithNone<Dropping>().ForEach((ref Cube cube) =>
                {
                    if (hitBottom)
                    {
                        return;
                    }
                    foreach (var droppingCube in cubes)
                    {
                        if (cube.x == droppingCube.x &&
                            cube.y == droppingCube.y &&
                            cube.h == droppingCube.h - 1)
                        {
                            hitBottom = true;
                            return;
                        }
                    }
                });
            }

            if (!hitBottom)
            {
                Entities.ForEach((Entity entity, ref Dropping dropping, ref Cube cube) =>
                {
                    cube.h -= 1;
                    EntityManager.SetComponentData(entity, new Translation
                    {
                        Value = new float3
                        {
                            x = cube.x,
                            z = cube.y,
                            y = cube.h
                        }
                    });
                });
            }
            else
            {
                Entities.ForEach((Entity entity, ref Dropping dropping) =>
                {
                    EntityManager.RemoveComponent<Dropping>(entity);
                });
            }
        }
    }
}