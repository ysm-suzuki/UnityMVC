using Atagoal.Core;

namespace UnityMVC
{
    public class Position : Point
    {
        public float z;

        public static Position Create(UnityEngine.Vector3 vector3)
        {
            return new Position
            {
                x = vector3.x,
                y = vector3.y,
                z = vector3.z,
            };
        }
        public static Position Create(byte[] bytes)
        {
            UnityEngine.Debug.Assert(bytes.Length == ByteSize, "Invalid bytes size.");

            int head = 0;
            return new Position
            {
                x = System.BitConverter.ToSingle(bytes, head),
                y = System.BitConverter.ToSingle(bytes, head + 4 * 1),
                z = System.BitConverter.ToSingle(bytes, head + 4 * 2),
            };
        }

        new public static Position Create()
        {
            var basePoint = Point.Create();
            return new Position { x = basePoint.x, y = basePoint.y, z = 0};
        }
        new public static Position Create(float x, float y, float z = 0)
        {
            return new Position { x = x, y = y, z = z};
        }
        new public static Position CreateInvalidPoint()
        {
            var basePoint = Point.CreateInvalidPoint();
            return new Position { x = basePoint.x, y = basePoint.y, z = 0};
        }
        new public Position Clone()
        {
            return Position.Create(x, y, z);
        }

        public Position()
        {
            x = 0;
            y = 0;
            z = 0;
        }
        public Position(float argX, float argY, float argZ = 0)
        {
            x = argX;
            y = argY;
            z = argZ;
        }

        public UnityEngine.Vector2 ToVector2()
        {
            return new UnityEngine.Vector2(x, y);
        }

        public UnityEngine.Vector3 ToVector3()
        {
            return new UnityEngine.Vector3(x, y, z);
        }


        public bool Equals(Position point)
        {
            return x == point.x
                && y == point.y
                && z == point.z;
        }

        public static Position operator +(Position point1, Position point2)
        {
            return new Position
            {
                x = point1.x + point2.x,
                y = point1.y + point2.y,
                z = point1.z + point2.z,
            };
        }
        public static Position operator -(Position point1, Position point2)
        {
            return new Position
            {
                x = point1.x - point2.x,
                y = point1.y - point2.y,
                z = point1.z - point2.z,
            };
        }
        public static Position operator *(Position position, float scale)
        {
            return new Position
            {
                x = position.x * scale,
                y = position.y * scale,
                z = position.z * scale
            };
        }

        public static Position operator /(Position position, float scale)
        {
            return new Position
            {
                x = position.x / scale,
                y = position.y / scale,
                z = position.z / scale
            };
        }


        public byte[] ToBytes()
        {
            var xBytes = System.BitConverter.GetBytes(x);
            var yBytes = System.BitConverter.GetBytes(y);
            var zBytes = System.BitConverter.GetBytes(z);
            return new byte[12]
            {
                xBytes[0],
                xBytes[1],
                xBytes[2],
                xBytes[3],
                yBytes[0],
                yBytes[1],
                yBytes[2],
                yBytes[3],
                zBytes[0],
                zBytes[1],
                zBytes[2],
                zBytes[3],
            };
        }

        public static int ByteSize
        {
            get
            {
                return
                    4 +     // x as float
                    4 +     // y as float
                    4;      // z as float
            }
        }
    }
}