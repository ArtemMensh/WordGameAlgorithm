using System;

namespace GenerateSquare
{
    public class Vector2Int
    {
        private int _x;
        private int _y;

        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        internal static Vector2Int Zero => new Vector2Int(0, 0);
        internal static Vector2Int Up => new Vector2Int(1, 0);
        internal static Vector2Int Down => new Vector2Int(-1, 0);
        internal static Vector2Int Left => new Vector2Int(0, 1);
        internal static Vector2Int Right => new Vector2Int(0, -1);

        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
            => new Vector2Int(a._x + b._x, a._y + b._y);

        public static bool operator ==(Vector2Int a, Vector2Int b)
            => a._x == b._x && a._y == b._y;

        public static bool operator !=(Vector2Int a, Vector2Int b)
            => a._x != b._x || a._y != b._y;

        public override bool Equals(object obj)
        {
            if (this == (Vector2Int)obj)
            {
                return true;
            }

            if (this  != (Vector2Int)obj)
            {
                return false;
            }

            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return _x ^ _y;
        }

        public override string ToString()
        {
            return $"({_x},{_y})";
        }
    }
}
