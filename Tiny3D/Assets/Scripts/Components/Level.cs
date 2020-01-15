using Unity.Entities;

namespace Tetric3D
{
    [GenerateAuthoringComponent]
    public struct Level : IComponentData
    {
        public int score;
        public Entity cubePrefab;
        public Entity projectionPrefab;
        public int initialHeight;
        public float speed;
        public int level;
        public int nextShape;
        public float timeLeft;
    }
}