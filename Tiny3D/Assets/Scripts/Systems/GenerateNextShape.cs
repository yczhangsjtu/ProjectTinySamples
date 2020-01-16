using Tetric3D;
using Unity.Entities;
using Random = Unity.Mathematics.Random;

namespace Tiny3D
{
    public class GenerateNextShape : ComponentSystem
    {
        public static Cube[][] candidates =
        {
            new Cube[]
            {
                new Cube {x = 0, y = 1, h = 0},
                new Cube {x = 1, y = 1, h = 0},
                new Cube {x = 2, y = 1, h = 0},
                new Cube {x = 3, y = 1, h = 0},
            },
            new Cube[]
            {
                new Cube {x = 0, y = 1, h = 0},
                new Cube {x = 1, y = 1, h = 0},
                new Cube {x = 1, y = 2, h = 0},
                new Cube {x = 2, y = 2, h = 0},
            }, 
            new Cube[]
            {
                new Cube {x = 0, y = 1, h = 0},
                new Cube {x = 1, y = 1, h = 0},
                new Cube {x = 2, y = 1, h = 0},
                new Cube {x = 1, y = 2, h = 0},
            }, 
            new Cube[]
            {
                new Cube {x = 0, y = 1, h = 0},
                new Cube {x = 1, y = 1, h = 0},
                new Cube {x = 2, y = 1, h = 0},
                new Cube {x = 2, y = 2, h = 0},
            }, 
            new Cube[]
            {
                new Cube {x = 1, y = 1, h = 0},
                new Cube {x = 1, y = 2, h = 0},
                new Cube {x = 2, y = 1, h = 0},
                new Cube {x = 2, y = 2, h = 0},
            }, 
            new Cube[]
            {
                new Cube {x = 1, y = 1, h = 0},
                new Cube {x = 1, y = 2, h = 0},
                new Cube {x = 2, y = 1, h = 0},
            }, 
            new Cube[]
            {
                new Cube {x = 1, y = 1, h = 0},
                new Cube {x = 1, y = 2, h = 0},
                new Cube {x = 1, y = 3, h = 0},
            }, 
            new Cube[]
            {
                new Cube {x = 1, y = 1, h = 0},
                new Cube {x = 1, y = 2, h = 0},
            }, 
//            new Cube[]
//            {
//                new Cube {x = 0, y = 1, h = 0},
//                new Cube {x = 1, y = 1, h = 0},
//                new Cube {x = 1, y = 2, h = 0},
//                new Cube {x = 1, y = 2, h = -1},
//            }, 
        };
        
        public static Random random;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<Level>();
            random = new Random(1);
        }

        protected override void OnUpdate()
        {
            var level = GetSingleton<Level>();
            if (!level.started)
            {
                return;
            }
            if (level.nextShape >= 0)
            {
                return;
            }

            level.nextShape = random.NextInt(0, candidates.Length);
            SetSingleton(level);
        }
    }
}