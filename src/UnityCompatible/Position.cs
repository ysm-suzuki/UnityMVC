
namespace UnityMVC
{
    public class Position : Atagoal.Core.Point
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
            UnityEngine.Debug.Assert(bytes.Length == GetBytesSize(), "Invalid bytes size.");

            int head = 0;
            return new Position
            {
                x = System.BitConverter.ToSingle(bytes, head),
                y = System.BitConverter.ToSingle(bytes, head + 4),
            };
        }
        
        public UnityEngine.Vector2 ToVector2()
        {
            return new UnityEngine.Vector2(x, y);
        }

        public UnityEngine.Vector3 ToVector3()
        {
            return new UnityEngine.Vector3(x, y, 0);
        }

        public byte[] ToBytes()
        {
            var xBytes = System.BitConverter.GetBytes(_x);
            var yBytes = System.BitConverter.GetBytes(_y);
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

        public static int GetBytesSize()
        {
            return
                4 +     // x as float
                4;      // y as float
        }
    }
}