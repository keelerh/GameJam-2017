using Improbable;
using UnityEngine;

namespace Assets.Gamelogic.Utils
{
    public static class MathUtils {

        //    public static Quaternion ToUnityQuaternion(Improbable.Core.Quaternion quaternion)
        //    {
        //        return new Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        //    }

        //    public static Improbable.Core.Quaternion ToNativeQuaternion(Quaternion quaternion)
        //    {
        //        return new Improbable.Core.Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        //    }

        public static Vector3 FlattenVector(this Vector3 vector3)
        {
            return new Vector3(vector3.x, 0f, vector3.z);
        }

        public static bool CompareEqualityEpsilon(float a, float b)
        {
            return Mathf.Abs(a - b) < Mathf.Epsilon;
        }

        public static bool CompareEqualityEpsilon(Vector3 a, Vector3 b)
        {
            return (a - b).sqrMagnitude < Mathf.Epsilon;
        }

        public static bool CompareEqualityEpsilon(Vector3 a, Vector3 b, float epsilon)
        {
            return (a - b).sqrMagnitude < epsilon;
        }

        public static bool CompareEqualityEpsilon(Coordinates a, Coordinates b)
        {
            return Coordinates.SquareDistance(a, b) < Mathf.Epsilon;
        }
    }

    public static class CoordinatesExtensions
    {
        public static Vector3 ToVector3(this Coordinates coordinates)
        {
            return new Vector3((float)coordinates.X, (float)coordinates.Y, (float)coordinates.Z);
        }
    }
}
