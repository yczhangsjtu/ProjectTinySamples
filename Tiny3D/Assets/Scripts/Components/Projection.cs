using Unity.Entities;

namespace Tetric3D
{
    [GenerateAuthoringComponent]
    public struct Projection : IComponentData
    {
        public int x;
        public int y;
        public int h;
    }
}