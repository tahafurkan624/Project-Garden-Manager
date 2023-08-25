using System.Collections.Generic;
using System.Linq;
using _Main._Scripts.Managers;
using HelmetMaster.Extensions;
using HelmetMaster.Main;
using UnityEngine;

namespace _Main._Scripts._General.FarmingSystem
{
    public class FarmSlot
    {
        public bool IsObstacle;
        public Vector3 Pos;

        public bool IsOccupied
        {
            get => _isOccupied;
            set
            {
                //selfCircle.sprite = (value)? farmArea.occupiedSprite : farmArea.normalSprite;
                _isOccupied = value;
            }
        }

        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                selfCircle.gameObject.SetActive(!value);
                _isLocked = value;
            }
        }

        public bool AnimalAttackingMe { get; set; }
        
        public Seed currentSeed;
        public Vector3 SpawnPos { get; set; }

        public FarmArea farmArea;
        public SpriteRenderer selfCircle;
        
        private bool _isOccupied;
        private bool _isLocked;

        public void SetSpeed(float speed)
        {
            if (currentSeed == null) return;
            
            currentSeed.SetSpeed(speed);
        }
        
        public bool CanPlaceObject(SeedBag seedBag)
        {
            var seed = seedBag.Seed;
            var size = seed.size;
            var pos = Pos;

            var sizeX = Mathf.CeilToInt(size.x);
            var sizeZ = Mathf.CeilToInt(size.z);
            var halfSizeZ = Mathf.CeilToInt(size.z/2f);
            var halfSizeX = Mathf.CeilToInt(size.x/2f);
            Vector3 checkPos;
            
            if (UsefulFunctions.IsEven(sizeX))
            {
                if (UsefulFunctions.IsEven(sizeZ))
                {
                    for (int i = 0; i < halfSizeZ; i++)
                    {
                        checkPos = pos + new Vector3(1f, 0, 0.5f + i);
                        if (CheckGrid(checkPos, sizeX, sizeZ, seed))
                        {
                            SpawnPos = checkPos + new Vector3(1f, 0,0);
                            //Debug.Log("Right");
                            //Debug.Log("Down"); Flipped
                            return true;
                        }

                        checkPos = pos + new Vector3(-1f, 0, 0.5f + i);
                        if (CheckGrid(checkPos, sizeX, sizeZ, seed))
                        {
                            SpawnPos = checkPos + new Vector3(1f, 0,0);
                            //Debug.Log("Left");
                            //Debug.Log("Up"); Flipped
                            return true;
                        }
                        checkPos = pos + new Vector3(1f, 0,-0.5f - i);
                        if (CheckGrid(checkPos, sizeX, sizeZ, seed))
                        {
                            SpawnPos = checkPos + new Vector3(1f, 0,0);
                            //Debug.Log("Right");
                            //Debug.Log("Down"); Flipped
                            return true;
                        }
                        checkPos = pos + new Vector3(-1f, 0,-0.5f - i);
                        if (CheckGrid(checkPos, sizeX, sizeZ, seed))
                        {
                            SpawnPos = checkPos + new Vector3(1f, 0,0);
                            //Debug.Log("Left");
                            //Debug.Log("Up"); Flipped
                            return true;
                        }
                    }
                }
                else // ODD Z
                {
                    for (int i = 0; i < halfSizeZ; i++)
                    {
                        checkPos = pos + new Vector3(1f, 0, i);
                        if (CheckGrid(checkPos, sizeX, sizeZ, seed))
                        {
                            SpawnPos = checkPos + new Vector3(1f, 0,0);
                            //Debug.Log("Right");
                            //Debug.Log("Down"); Flipped
                            return true;
                        }
                        checkPos = pos + new Vector3(1f, 0, -i);
                        if (CheckGrid(checkPos, sizeX, sizeZ, seed))
                        {
                            SpawnPos = checkPos + new Vector3(1f, 0,0);
                            //Debug.Log("Right");
                            //Debug.Log("Down"); Flipped
                            return true;
                        }
                        checkPos = pos + new Vector3(-1f, 0,i);
                        if (CheckGrid(checkPos, sizeX, sizeZ, seed))
                        {
                            SpawnPos = checkPos + new Vector3(1f, 0,0);
                            //Debug.Log("Left");
                            //Debug.Log("Up"); Flipped
                            return true;
                        }
                        checkPos = pos + new Vector3(-1f, 0,-i);
                        if (CheckGrid(checkPos, sizeX, sizeZ, seed))
                        {
                            SpawnPos = checkPos + new Vector3(1f, 0,0);
                            //Debug.Log("Left");
                            //Debug.Log("Up"); Flipped
                            return true;
                        }
                    }
                }
            }
            else // IsOdd X
            {
                if (UsefulFunctions.IsEven(sizeZ))
                {
                    for (int i = 0; i < halfSizeZ; i++)
                    {
                        for (int j = 0; j < halfSizeX; j++)
                        {
                            checkPos = pos + new Vector3(j, 0,0.5f + i);
                            if (CheckGrid(checkPos, sizeX, sizeZ, seed))
                            {
                                SpawnPos = checkPos;
                                //Debug.Log("Mid");
                                return true;
                            }
                            checkPos = pos + new Vector3(j, 0,-0.5f + i);
                            if (CheckGrid(checkPos, sizeX, sizeZ, seed))
                            {
                                SpawnPos = checkPos;
                                //Debug.Log("Mid");
                                return true;
                            }
                            
                            if (j > 0)
                            {
                                checkPos = pos + new Vector3(-j, 0,0.5f + i);
                                if (CheckGrid(checkPos, sizeX, sizeZ, seed))
                                {
                                    SpawnPos = checkPos;
                                    //Debug.Log("Left");
                                    return true;
                                } 
                                
                                checkPos = pos + new Vector3(-j, 0,-0.5f + i);
                                if (CheckGrid(checkPos, sizeX, sizeZ, seed))
                                {
                                    SpawnPos = checkPos;
                                    //Debug.Log("Left");
                                    return true;
                                } 
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < halfSizeZ; i++)
                    {
                        for (int j = 0; j < halfSizeX; j++)
                        {
                            checkPos = pos + new Vector3(j, 0, i);
                            if (CheckGrid(checkPos, sizeX, sizeZ, seed))
                            {
                                SpawnPos = checkPos;
                                //Debug.Log("Mid");
                                return true;
                            }
                        
                            checkPos = pos + new Vector3(j, 0, -i);
                            if (CheckGrid(checkPos, sizeX, sizeZ, seed))
                            {
                                SpawnPos = checkPos;
                                //Debug.Log("Mid");
                                return true;
                            }

                            if (j > 0)
                            {
                                checkPos = pos + new Vector3(-j, 0, i);
                                if (CheckGrid(checkPos, sizeX, sizeZ, seed))
                                {
                                    SpawnPos = checkPos;
                                    //Debug.Log("Mid");
                                    return true;
                                }
                        
                                checkPos = pos + new Vector3(-j, 0, -i);
                                if (CheckGrid(checkPos, sizeX, sizeZ, seed))
                                {
                                    SpawnPos = checkPos;
                                    //Debug.Log("Mid");
                                    return true;
                                } 
                            }
                        }
                    }
                }
            }
            return false;
        }

        bool CheckGrid(Vector3 pos, float width, float length, Seed seed) 
        {
            var slots = farmArea.Slots;

            List<FarmSlot> plantedSlots = new List<FarmSlot>();

            for (int x = 0; x < slots.GetLength(0); x++)
            {
                for (int z = 0; z < slots.GetLength(1); z++)
                {
                    var slot = slots[x, z];
                    if (pos.IsCloserThen(slot.Pos, width/2, length/2))
                    {
                        if (slot.IsObstacle || slot.IsOccupied || slot.IsLocked) return false;

                        plantedSlots.Add(slot);
                    }
                }
            }

            if (plantedSlots.Count < width * length) return false;
            
            seed.plantedSlots = plantedSlots.ToList();
            var currentCrate = DragArea.Instance.SelectedCreateSlot;
            foreach (var grid in plantedSlots)
            {
                grid.IsOccupied = true;

                if(currentCrate.HasSeed)
                    grid.currentSeed = currentCrate.NextSeedBag().Seed;
            }

            return true;
        }
    }
}