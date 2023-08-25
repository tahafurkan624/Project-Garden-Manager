using System.Collections.Generic;
using HelmetMaster.Main.UI;
using UnityEngine;

namespace _Main._Scripts._General.FarmingSystem
{
    public class FarmArea : MonoBehaviour
    {
        [SerializeField] private float sizeX = 1f, sizeY = 1f;
        [SerializeField] private int width, height;
        [SerializeField] private List<RowSlots> rowSlots = new List<RowSlots>();
        [SerializeField] private SpriteRenderer circle;
        //public Sprite occupiedSprite, normalSprite;
        [SerializeField] private List<FarmAreaRow> unlockableRows = new List<FarmAreaRow>();
        
        private MainCanvas _mainCanvas;
        private MainCanvas MainCanvas => _mainCanvas ??= MainCanvas.Instance;
        
        public FarmSlot[,] Slots;

        [System.Serializable]
        public class RowSlots
        {
            public int minX, maxX, minY, maxY;
        }
        
        protected void Awake()
        {
            Slots = new FarmSlot[(int)(width/sizeX), (int)(height/sizeY)];
        }

        private void Start()
        {
            for (int z = 0; z < height / sizeY; z++)
            {
                for (int x = 0; x < width / sizeX; x++)
                {
                    var pos = transform.position + Vector3.right * x * sizeX + Vector3.forward * z * sizeY;
                    var _circle = Instantiate(circle, pos, circle.transform.rotation);
                    var slot = new FarmSlot
                    {
                        farmArea = this,
                        selfCircle = _circle,
                        IsLocked = true
                    };
                    // foreach (var boxObs in squareObstacles)
                    // {
                    //     if ((x >= boxObs.minX && x <= boxObs.maxX) && (z >= boxObs.minY && z <= boxObs.maxY)) slot.IsObstacle = true;
                    // }
                    Slots[x, z] = slot;
                    slot.Pos = pos;
                    for (var i = 0; i < rowSlots.Count; i++)
                    {
                        var rowSlot = rowSlots[i];

                        if ((rowSlot.minX <= x && x < rowSlot.maxX) && (rowSlot.minY <= z && z <rowSlot.maxY))
                        {
                            var row = unlockableRows[i];

                            row.AddSlots(slot);
                            row.FarmArea = this;
                        }
                    }

                    
                }
            }
        }

        public FarmSlot GetNearestGridSlot(Vector3 position)
        {
            position -= transform.position;

            int xCount = Mathf.RoundToInt(position.x / sizeX);
            int zCount = Mathf.RoundToInt(position.z / sizeY);

            xCount = Mathf.Clamp(xCount, 0, width - 1);
            zCount = Mathf.Clamp(zCount, 0, height - 1);

            return Slots[xCount, zCount];
        }

        public Vector3 GetNearestPointOnGrid(Vector3 position)
        {
            position -= transform.position;

            int xCount = Mathf.RoundToInt(position.x / sizeX);
            int zCount = Mathf.RoundToInt(position.z / sizeY);

            xCount = Mathf.Clamp(xCount, 0, width - 1);
            zCount = Mathf.Clamp(zCount, 0, height - 1);

            Vector3 result = new Vector3(xCount * sizeX, 0, zCount * sizeY);

            result += transform.position;

            return result;
        }

        public void OnRowUnlocked()
        {
            foreach (var farmAreaRow in unlockableRows)
            {
                if (!farmAreaRow.IsUnlocked)
                {
                    return;
                }
            }
            
            MainCanvas.scrollButton.ChangeInteractability(true);
        }

        public void SpeedUpRows()
        {
            foreach (var farmAreaRow in unlockableRows)
            {
                farmAreaRow.SetRowSpeed(2f);
            }
        }

        public void ResetRowSpeeds()
        {
            foreach (var farmAreaRow in unlockableRows)
            {
                farmAreaRow.SetRowSpeed(1f);
            }
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (sizeX < .1f) return;
            if (sizeY < .1f) return;

            var pos = transform.position;
            Gizmos.color = Color.yellow;
            for (float x = 0; x < width; x += sizeX)
            {
                for (float z = 0; z < height; z += sizeY)
                {
                    if (Slots != null)
                    {
                        if (!Slots[(int)(x / sizeX), (int)(z / sizeY)].IsObstacle)
                        {
                            if ( Slots[(int)(x / sizeX), (int)(z / sizeY)].IsOccupied)
                            {
                                Gizmos.color = Color.black;
                            }
                            else
                            {
                                Gizmos.color = Color.yellow;
                            }
                        }
                        else Gizmos.color = Color.red;
                    }
                    var point = GetNearestPointOnGrid(new Vector3(pos.x + x, 0f, pos.z + z));
                    Gizmos.DrawSphere(point, 0.1f);
                }
            }
        }
#endif
    }
}