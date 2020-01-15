using Unity.Entities;

namespace Tetric3D
{
    public struct Dropping : IComponentData
    {
        public int cx;
        public int cy;
        public int cz;
    }
}