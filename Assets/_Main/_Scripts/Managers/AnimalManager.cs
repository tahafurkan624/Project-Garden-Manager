using System;
using System.Collections;
using System.Collections.Generic;
using _Main._Scripts._General;
using _Main._Scripts._General.AnimalSystem;
using _Main._Scripts._General.FarmingSystem;
using HelmetMaster.Main.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main._Scripts.Managers
{
    public class AnimalManager : Utilities.Singleton<AnimalManager>
    {
        private Coroutine delayRoutine;
        [SerializeField] private Animal animal;
        [SerializeField] private List<SpawningPositions> farmAreaSpawnPoints = new List<SpawningPositions>();
        private bool isReadyToSend;
        [SerializeField] private List<FarmArea> farmAreas = new List<FarmArea>();

        private void Start()
        {
            if (LevelProgress.Instance.ReachedLevelIdx >= 4)
            {
                OnAnimalReturned();
            }
        }

        public void OnSomethingPlanted(FarmArea farmArea, FarmSlot slot)
        {
            if(!isReadyToSend) return;

            isReadyToSend = false;

            animal.transform.position = GetSpawningPoint(farmArea, slot).position;
            animal.CurrentSlot = slot;
            animal.gameObject.SetActive(true);
            SendNextAnimal();
        }

        private Transform GetSpawningPoint(FarmArea farmArea, FarmSlot slot)
        {
            var farmAreaIndex = farmAreas.IndexOf(farmArea);
            Transform spawningPoint = farmAreaSpawnPoints[farmAreaIndex].spawningPoints[0];
            float closestDistance = 1000f;
            foreach (var spawnPoint in farmAreaSpawnPoints[farmAreaIndex].spawningPoints)
            {
                var distance = Vector3.Distance(spawnPoint.position, slot.SpawnPos);
                if (distance < closestDistance)
                {
                    spawningPoint = spawnPoint;
                    closestDistance = distance;
                }
            }

            return spawningPoint;
        }
        private void SendNextAnimal()
        {
            animal.StartStealing();
            MainCanvas.Instance.changeSceneButton.AnimalAttacked();
        }
        
        public void OnAnimalReturned()
        {
            MainCanvas.Instance.changeSceneButton.AnimalRunoff();
            animal.gameObject.SetActive(false);

            if (delayRoutine != null)
            {
                StopCoroutine(delayRoutine);
            }
            
            delayRoutine = StartCoroutine(DelayNextAnimal());
        }

        IEnumerator DelayNextAnimal()
        {
            var delay = Random.Range(80, 160);
            yield return new WaitForSeconds(delay);
            
            TryFindingTarget();
            delayRoutine = null;
        }

        private void TryFindingTarget()
        {
            foreach (var farmArea in farmAreas)
            {
                foreach (var slot in farmArea.Slots)
                {
                    if (slot.IsOccupied && slot.currentSeed != null)
                    {
                        animal.transform.position =  GetSpawningPoint(farmArea, slot).position;
                        animal.CurrentSlot = slot;
                        animal.gameObject.SetActive(true);
                        SendNextAnimal();
                        return;
                    }
                }
            }
            isReadyToSend = true;
        }

        public void FarmCollected()
        {
            animal.OnClick();
        }
        
        [ContextMenu("set")]
        public void OnReachedLevel4()
        {
            isReadyToSend = true;
        }

        [Serializable]
        public class SpawningPositions
        {
            public List<Transform> spawningPoints = new List<Transform>();
        }
    }
}