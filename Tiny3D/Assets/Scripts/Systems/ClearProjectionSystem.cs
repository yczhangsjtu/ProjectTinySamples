using Tetric3D;
using Unity.Entities;

namespace Tiny3D
{
    [UpdateBefore(typeof(ProjectionSystem))]
    public class ClearProjectionSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<Projection, ProjectionClear>().ForEach(entity => {
                EntityManager.DestroyEntity(entity);
            });
        }
    }
}