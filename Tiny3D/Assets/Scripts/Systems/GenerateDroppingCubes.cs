using Tetric3D;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Tiny3D
{
    [UpdateAfter(typeof(GenerateNextShape))]
    public class GenerateDroppingCubes : ComponentSystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<Level>();
        }

        protected override void OnUpdate()
        {
            int droppingCubeCount = 0;
            Entities.WithAll<Cube, Dropping>().ForEach(entity => { droppingCubeCount += 1; });
            if (droppingCubeCount == 0)
            {
                var level = GetSingleton<Level>();
                if (!level.started)
                {
                    return;
                }
                if (level.nextShape < 0)
                {
                    return;
                }

                foreach (var cube in GenerateNextShape.candidates[level.nextShape])
                {
                    var entity = EntityManager.Instantiate(level.cubePrefab);
                    EntityManager.SetComponentData(entity, new Cube()
                    {
                        x = cube.x,
                        y = cube.y,
                        h = cube.h + level.initialHeight
                    });
                    EntityManager.SetComponentData(entity, new Translation
                    {
                        Value = new float3
                        {
                            x = cube.x,
                            z = cube.y,
                            y = cube.h + level.initialHeight
                        }
                    });
                    EntityManager.AddComponentData(entity, new Dropping
                    {
                        cx = cube.x - 1,
                        cy = cube.y - 1,
                        cz = cube.h
                    });
                }

                level.nextShape = -1;
                level.timeLeft = 1;
                SetSingleton(level);
            }
        }
    }
}