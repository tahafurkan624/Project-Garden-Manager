using TMPro;
using UnityEngine;

namespace _Main._Scripts.Utilities
{
    public static class Helper
    {
        /// <summary>
        ///   <para>Remaps t from between a and b to newA and newB.</para>
        /// </summary>
        /// <param name="a">The start value.</param>
        /// <param name="b">The end value.</param>
        /// <param name="newA">The New start value.</param>
        /// <param name="newB">The New end value.</param>
        /// <param name="t">The value between the first two floats.</param>
        /// <returns>
        ///   <para>Remapped value of t between newA and newB.</para>
        /// </returns>
        public static float Remap(float a, float b, float newA, float newB, float t)
        {
            return Mathf.Lerp(newA, newB,Mathf.InverseLerp(a, b, t));
        }

        /// <summary>
        /// Destroy all child objects of this transform (Unintentionally evil sounding).
        /// Use it like so:
        /// <code>
        /// transform.DestroyChildren();
        /// </code>
        /// </summary>
        public static void DestroyChildren(this Transform t) {
            foreach (Transform child in t) Object.Destroy(child.gameObject);
        }

        public static Vector2 AngleToDir(float angle)
        {
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        public static float DirToAngle(Vector2 dir)
        {
            return Mathf.Atan2(dir.y, dir.x);
        }

        public static Vector3 SetX(this Transform trans, float x)
        {
            Vector3 position = trans.position;
            position.x = x;
            trans.position = position;
            return position;
        }
    
        public static Vector3 SetY(this Transform trans, float y)
        {
            Vector3 position = trans.position;
            position.y = y;
            trans.position = position;
            return position;
        }
    
        public static Vector3 SetZ(this Transform trans, float z)
        {
            Vector3 position = trans.position;
            position.z = z;
            trans.position = position;
            return position;
        }

        public static Material GetMaterial(this Transform trans)
        {
            return trans.TryGetComponent(out MeshRenderer mr) ? mr.material : null;
        }

        public static string SetText(this TMP_Text tmpText, string text)
        {
            tmpText.text = text;
            return text;
        }
    
        public static string SetText(this TMP_Text tmpText, int text)
        {
            tmpText.text = $"{text}";
            return $"{text}";
        }
    
        public static string SetText(this TMP_Text tmpText, float text)
        {
            tmpText.text = $"{text}";
            return $"{text}";
        }
    
        public static string SetText(this TMP_Text tmpText, double text)
        {
            tmpText.text = $"{text}";
            return $"{text}";
        }
    
        public static string SetText(this TMP_Text tmpText, byte text)
        {
            tmpText.text = $"{text}";
            return $"{text}";
        }
    }
}
