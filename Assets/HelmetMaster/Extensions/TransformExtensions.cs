using System;
using UnityEngine;

namespace HelmetMaster.Extensions
{
    public static class TransformExtensions
    {
        public static Vector3 ChangeX(this Transform trans, float x)
        {
            var position = trans.position;
            position.x = x;
            trans.position = position;
            return position;
        }

        public static Vector3 ChangeY(this Transform trans, float y)
        {
            var position = trans.position;
            position.y = y;
            trans.position = position;
            return position;
        }

        public static Vector3 ChangeZ(this Transform trans, float z)
        {
            var position = trans.position;
            position.z = z;
            trans.position = position;
            return position;
        }
        
        public static Transform CloneTransform(this Transform transform, Vector3 ?newPosition = null, string name = null) {
            Transform clone = GameObject.Instantiate(transform, transform.parent);
            newPosition = newPosition != null ? newPosition.Value : Vector3.zero;
            clone.position = newPosition.Value;
            clone.name = name != null ? name : transform.name;
            
            return clone;
        }
        
        public static Transform CloneTransform(this Transform transform, Transform newParent, Vector3 ?newPosition = null, string name = null) {
            Transform clone = GameObject.Instantiate(transform, newParent);
            newPosition = newPosition != null ? newPosition.Value : Vector3.zero;
            clone.position = newPosition.Value;
            clone.name = name != null ? name : transform.name;

            return clone;
        }
        
        // Destroy all children of this parent
        public static void DestroyChildren(this Transform parent) {
            foreach (Transform transform in parent)
                GameObject.Destroy(transform.gameObject);
        }
        
        // Destroy all children except the ones with these names
        public static void DestroyChildren(this Transform parent, params string[] ignoreArr) {
            foreach (Transform transform in parent) {
                if (Array.IndexOf(ignoreArr, transform.name) == -1)
                    // Don't ignore
                    GameObject.Destroy(transform.gameObject);
            }
        }
        
        // Set all parent and all children to this layer
        public static void SetAllChildrenLayer(this Transform parent, int layer) {
            parent.gameObject.layer = layer;
            foreach (Transform trans in parent) {
                SetAllChildrenLayer(trans, layer);
            }
        }
        
        public static void ResetTransformation(this Transform trans)
        {
            trans.position = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = new Vector3(1, 1, 1);
        }
    }
}

