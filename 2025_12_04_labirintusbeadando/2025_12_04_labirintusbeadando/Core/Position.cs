using System;

namespace _2025_12_04_labirintusbeadando.Core
{
    public class Position : IEquatable<Position>
    {
        public int X { get; }
        public int Y { get; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Position other)
        {
            if (other is null) return false;
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj) => Equals(obj as Position);

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public static bool operator ==(Position left, Position right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);
            return left.Equals(right);
        }

        public static bool operator !=(Position left, Position right) => !(left == right);

        public override string ToString() => $"({X}, {Y})";

        public double DistanceTo(Position other)
        {
            if (other == null) return double.MaxValue;
            return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
        }
    }
}