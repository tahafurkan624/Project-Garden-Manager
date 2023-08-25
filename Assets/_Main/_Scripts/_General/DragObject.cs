using UnityEngine;

namespace _Main._Scripts._General
{
    public class DragObject : MonoBehaviour
    {
        //public Vector3 StartPos;
        private CreateSlotUnlockable _createSlotUnlockable;
        public CreateSlotUnlockable CreateSlotUnlockable => _createSlotUnlockable ??= GetComponent<CreateSlotUnlockable>();

        private CreateSlot _createSlot;
        public CreateSlot CreateSlot => _createSlot ??= GetComponent<CreateSlot>();
        public float objectScale;

        // public void Init(float x = 0f)
        // {
        //     StartPos = transform.position;
        //
        //     if (x > 0)
        //     {
        //         StartPos.x = x;
        //     }
        // }
    }
}