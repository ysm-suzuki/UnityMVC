using Atagoal.Core;

namespace UnityMVC
{
    public class Position : Point
    {
        public static Position Create(UnityEngine.Vector3 vector3)
        {
            return new Position
            {
                x = vector3.x,
                y = vector3.y,
            };
        }
        public static Position Create(byte[] bytes)
        {
            UnityEngine.Debug.Assert(bytes.Length == ByteSize, "Invalid bytes size.");

            int head = 0;
            return new Position
            {
                x = System.BitConverter.ToSingle(bytes, head),
                y = System.BitConverter.ToSingle(bytes, head + 4),
            };
        }

        new public static Position Create()
        {
            var basePoint = Point.Create();
            return new Position { x = basePoint.x, y = basePoint.y };
        }
        new public static Position Create(float x, float y)
        {
            return new Position { x = x, y = y };
        }
        new public static Position CreateInvalidPoint()
        {
            var basePoint = Point.CreateInvalidPoint();
            return new Position { x = basePoint.x, y = basePoint.y };
        }
        new public Position Clone()
        {
            return (Position)base.Clone();
        }

        public UnityEngine.Vector2 ToVector2()
        {
            return new UnityEngine.Vector2(x, y);
        }

        public UnityEngine.Vector3 ToVector3()
        {
            return new UnityEngine.Vector3(x, y, 0);
        }


        public static Position operator +(Position point1, Position point2)
        {
            return new Position
            {
                x = point1.x + point2.x,
                y = point1.y + point2.y,
            };
        }
        public static Position operator -(Position point1, Position point2)
        {
            return new Position
            {
                x = point1.x - point2.x,
                y = point1.y - point2.y,
            };
        }


        public byte[] ToBytes()
        {
            var xBytes = System.BitConverter.GetBytes(x);
            var yBytes = System.BitConverter.GetBytes(y);
            return new byte[8]
            {
                xBytes[0],
                xBytes[1],
                xBytes[2],
                xBytes[3],
                yBytes[0],
                yBytes[1],
                yBytes[2],
                yBytes[3],
            };
        }

        public static int ByteSize
        {
            get
            {
                return
                    4 +     // x as float
                    4;      // y as float
            }
        }
    }
}