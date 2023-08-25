using UnityEngine;

namespace HelmetMaster.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 Flattened(this Vector3 vector)
        {
            return new Vector3(vector.x, 0f, vector.z);
        }
        
        public static Vector3 AddX(this Vector3 vector, float x)
        {
            return new Vector3(vector.x + x, vector.y, vector.z);
        }

        public static Vector3 AddY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, vector.y + y, vector.z);
        }
        
        public static Vector3 AddZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, vector.z + z);
        }
        public static float FlatDistance(this Vector3 origin, Vector3 destination)
        {
            return Vector3.Distance(origin.Flattened(), destination.Flattened());
        }

        public static float ManhattanDistance(this Vector3 startPos, Vector3 endPos)
        {
            return Mathf.Abs(startPos.x - endPos.x) + Mathf.Abs(startPos.y - endPos.y) + Mathf.Abs(startPos.z - endPos.z);
        }
        
        public static Vector3 Abs(this Vector3 vector)
        {
            vector.x = Mathf.Abs(vector.x);
            vector.y = Mathf.Abs(vector.y);
            vector.z = Mathf.Abs(vector.z);
            return vector;
        }

        public static Vector3 ApplyRotationToVector(this Vector3 vec, Vector3 vecRotation)
        {
            return ApplyRotationToVector(vec, GetAngleFromVectorFloat(vecRotation));
        }

        public static Vector3 ApplyRotationToVector(this Vector3 vec, float angle)
        {
            return Quaternion.Euler(0, 0, angle) * vec;
        }

        public static Vector3 ApplyRotationToVectorXZ(this Vector3 vec, float angle)
        {
            return Quaternion.Euler(0, angle, 0) * vec;
        }
        
        public static float GetAngleFromVectorFloat(this Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }
        public static int GetAngleFromVector(this Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }
        
        public static float GetAngleFromVectorFloatXZ(this Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }

        public static int GetAngleFromVector180(this Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }

        public static bool IsCloserThen(this Vector3 pos, Vector3 gridPos, float width, float length)
        {
            if (Mathf.Abs(gridPos.x - pos.x) > width)
            {
                return false;
            }
            
            if (Mathf.Abs(gridPos.z - pos.z) > length)
            {
                return false;
            }
            return true;
        }
    }
}