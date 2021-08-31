using Godot;

namespace Morozurfu.Dijkstra
{
    public class Map
    {
        public Cell[,] _maze;
        public readonly int Height;
        public readonly int Width;

        public enum Cell
        {
            Wall = -1,
            Ground,
            Water,
            Fire,
            Ice
        }

        public Map(Cell[,] maze)
        {
            Width = maze.GetLength(1);
            Height = maze.GetLength(0);
            _maze = Transpose(maze);

        }

        public Cell this[Vector2 point]
        {
            get => _maze[(int)point.x, (int)point.y];
            private set => _maze[(int)point.x, (int)point.y] = value;
        }
        public Cell this[int x, int y] => _maze[x, y];

        public void ChangeCell(int changedTo, Vector2 tilePosition)
        {
            this[tilePosition] = (Map.Cell) changedTo;
        }

        public bool IsWall(Vector2 point)
            => _maze[(int)point.x, (int)point.y] == Cell.Wall;

        public bool IsInBounds(Vector2 point)
            => (int)point.x >= 0
               && (int)point.x < Width
               && (int)point.y >= 0
               && (int)point.y < Height;

        private static T[,] Transpose<T>(T[,] matrix)
        {
            var width = matrix.GetLength(1);
            var height = matrix.GetLength(0);
            var transposed = new T[width, height];

            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    transposed[x, y] = matrix[y, x];

            return transposed;
        }
    }
}