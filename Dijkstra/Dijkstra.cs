using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Morozurfu.Dijkstra
{
    public static class Dijkstra
    {
        private static readonly Vector2[] Offsets = new[] { (0, -1), (-1, 0), (1, 0), (0, 1) }
            .Select(point => new Vector2(point.Item1, point.Item2))
            .ToArray();

        private class DijkstraData
        {
            public readonly Vector2 Current;
            public readonly DijkstraData Previous;
            public readonly int Price;

            public DijkstraData(Vector2 current, DijkstraData previous, int price)
            {
                Current = current;
                Previous = previous;
                Price = price;
            }
        }


        public static Vector2 FindShortestWay(Map maze, Vector2 start, Vector2 finish, Dictionary<Map.Cell, int> prices)
        {
            var data = new DijkstraData[maze.Width, maze.Height];
            data[(int)start.x, (int)start.y] = new DijkstraData(start, null, 0);
            var queue = new Queue<DijkstraData>(new[] { data[(int)start.x, (int)start.y] });
            while (queue.Count > 0)
            {
                var currentPoint = queue.Dequeue();
                if (currentPoint.Current == finish)
                    break;
                foreach (var nextPoint in GetNextPoints(maze, currentPoint.Current))
                {
                    prices.TryGetValue(maze[nextPoint], out var nextPointPrice);
                    var price = currentPoint.Price + nextPointPrice;
                    if (data[(int)nextPoint.x, (int)nextPoint.y] != null &&
                        data[(int)nextPoint.x, (int)nextPoint.y].Price <= price) continue;
                    data[(int)nextPoint.x, (int)nextPoint.y] = new DijkstraData(nextPoint, currentPoint, price);
                    queue.Enqueue(data[(int)nextPoint.x, (int)nextPoint.y]);
                }
            }

            var current = data[(int)finish.x, (int)finish.y];
            var result = new List<Vector2>();
            while (current != null)
            {
                result.Add(current.Current);
                current = current.Previous;
            }

            result.Reverse();
            return result.Count > 1
                ? result[1] - result[0]
                : Vector2.Zero;
        }

        private static IEnumerable<Vector2> GetNextPoints(Map maze, Vector2 currentPoint)
            => Offsets
                .Select(offset => offset + currentPoint)
                .Where(point => maze.IsInBounds(point) && !maze.IsWall(point));
    }
}