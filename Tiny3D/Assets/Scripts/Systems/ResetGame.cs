using Tetric3D;
using Unity.Collections;
using Unity.Entities;

namespace Tiny3D
{
    public class ResetGame : ComponentSystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<Level>();
        }
        
        protected override void OnUpdate()
        {
            var level = GetSingleton<Level>();
            if (level.reset)
            {
                level.reset = false;
                level.nextShape = -1;
                level.started = false;
                var ecb = new EntityCommandBuffer(Allocator.Temp);
                Entities.WithAll<Cube>().ForEach(entity =>
                {
                    ecb.DestroyEntity(entity);
                });
                ecb.Playback(EntityManager);
                SetSingleton(level);
            }
        }
    }
}