using System.Collections.Generic;
using Tetric3D;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Tiny3D
{
    [UpdateAfter(typeof(CubeDrop))]
    public class ClearCubes : ComponentSystem
    {
        protected override void OnUpdate()
        {
            List<int> layerCubeCounts = new List<int>();
            Entities.WithNone<Dropping>().ForEach((Entity entity, ref Cube cube) =>
            {
                int layer = cube.h;
                while (layerCubeCounts.Count <= layer)
                {
                    layerCubeCounts.Add(0);
                }

                layerCubeCounts[layer] += 1;
            });

            List<int> layersToClear = new List<int>();
            for (var i = 0; i < layerCubeCounts.Count; i++)
            {
                if (layerCubeCounts[i] == 16)
                {
                    layersToClear.Add(i);
                }
            }
            
            if (layersToClear.Count > 0)
            {
                layersToClear.Sort();
                List<int> dropBy = new List<int>();
                int j = 0;
                for (int i = 0; i <= layersToClear[layersToClear.Count-1] + 1; i++)
                {
                    if (layersToClear.Count <= j)
                    {
                        break;
                    }
                    if (layersToClear[j] == i)
                    {
                        dropBy.Add(-1);
                        j += 1;
                    }
                    else // It is assured that layersToClear[j] >= i, so here layersToClear[j] > i
                    {
                        dropBy.Add(j);
                    }
                }
                Entities.WithNone<Dropping>().ForEach((Entity entity, ref Cube cube) =>
                {
                    int toDrop = cube.h >= dropBy.Count ? layersToClear.Count : dropBy[cube.h];
                    if (toDrop == -1)
                    {
                        EntityManager.DestroyEntity(entity);
                    }
                    else
                    {
                        cube.h -= toDrop;
                        EntityManager.SetComponentData(entity, new Translation
                        {
                            Value = new float3
                            {
                                x = cube.x,
                                z = cube.y,
                                y = cube.h
                            }
                        });
                    }
                });
            }
        }
    }
}