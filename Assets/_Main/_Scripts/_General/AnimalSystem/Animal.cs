using System;
using System.Collections;
using _Main._Scripts._General.FarmingSystem;
using _Main._Scripts.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main._Scripts._General.AnimalSystem
{
    public class Animal : MonoBehaviour
    {
        public bool IsActive { get; set; }
        [SerializeField] private Animator animator;
        private static readonly int Attack1 = Animator.StringToHash("Attack");
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");
        [SerializeField] private Transform handTransform;
        [SerializeField] private GameObject timer;
        [SerializeField] private TMP_Text timerText;

        private Vector3 startPos;

        public FarmSlot CurrentSlot { get; set; }
        private Coroutine runningRoutine, attackingRoutine;
        private bool isReturning;
        private static readonly int Bounce = Animator.StringToHash("Bounce");

        public void StartStealing()
        {
            startPos = transform.position;
            IsActive = true;
            isReturning = false;
            runningRoutine = StartCoroutine(RunToTarget());
            CurrentSlot.AnimalAttackingMe = true;
        }

        IEnumerator RunToTarget()
        {
            animator.SetBool(IsRunning, true);
            var target = CurrentSlot.currentSeed.transform.position + new Vector3(-0.5f, 0, 1.5f);
            transform.DOLookAt(target, 0.2f, AxisConstraint.Y);
            yield return transform.DOMove(target, 5f).SetEase(Ease.Linear).SetSpeedBased().WaitForCompletion();
            if (CurrentSlot == null || CurrentSlot.currentSeed == null)
            {
                RunBack();
                runningRoutine = null;
                yield break;
            }
            transform.DOLookAt(CurrentSlot.currentSeed.transform.position, 0.2f, AxisConstraint.Y);
            animator.SetBool(IsRunning, false);
            RunningOver();
            runningRoutine = null;
        }
        
        private void RunningOver()
        {
            attackingRoutine = StartCoroutine(AttackPhase());
            timer.gameObject.SetActive(true);
        }

        private void SetTime(int time)
        {
            timerText.text = $"{Mathf.Max(time, 0)}";
        }
        
        IEnumerator AttackPhase()
        {
            int time = 10;
            SetTime(time);
            for (int i = 0; i < 5; i++)
            {
                Attack();
                yield return new WaitForSeconds(1f);
                time--;
                SetTime(time);
                yield return new WaitForSeconds(1f);
                time--;
                SetTime(time);
            }
            
            if (CurrentSlot == null || CurrentSlot.currentSeed == null)
            {
                RunBack();
                attackingRoutine = null;
                yield break;
            }
            
            CurrentSlot.currentSeed.Steal(handTransform);
            RunBack();

            attackingRoutine = null;
        }

        private void Attack()
        {
            animator.SetTrigger(Attack1);
        }
        
        private void RunBack()
        {
            isReturning = true;
            timer.gameObject.SetActive(false);
            StartCoroutine(RunToBase());
        }

        IEnumerator RunToBase()
        {
            animator.SetBool(IsRunning, true);

            transform.DOLookAt(startPos, 0.2f, AxisConstraint.Y);
            yield return transform.DOMove(startPos, 8f).SetEase(Ease.Linear).SetSpeedBased().WaitForCompletion();
            animator.SetBool(IsRunning, false);
            if (handTransform.childCount > 0)
            {
                Destroy(handTransform.GetChild(0).gameObject);
            }

            isReturning = false;
            AnimalManager.Instance.OnAnimalReturned();
            IsActive = false;
        }

        public void OnClick()
        {
            if (!IsActive) return;
            
            if (isReturning) return;
            
            isReturning = true;

            if (runningRoutine != null)
            {
                StopCoroutine(runningRoutine);
                runningRoutine = null;
                animator.SetBool(IsRunning, false);
            }

            if (attackingRoutine != null)
            {
                StopCoroutine(attackingRoutine);
                attackingRoutine = null;
            }

            transform.DOKill();
            if (gameObject.activeSelf)
            {
                StartCoroutine(RunAway());
            }
        }

        IEnumerator RunAway()
        {
            animator.SetTrigger(Bounce);
            yield return new WaitForSeconds(.5f);
            RunBack();
            IsActive = false;
        }
    }
}