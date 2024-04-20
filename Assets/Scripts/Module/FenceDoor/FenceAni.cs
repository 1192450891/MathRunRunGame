using System;
using Struct;
using Unity.VisualScripting;
using UnityEngine;

namespace BrokenVector.LowPolyFencePack
{
    public class FenceAni:MonoBehaviour
    {

        private bool IsFenceOpen { get { return CurrentState == FenceState.Open; } }
        private bool IsFenceClosed { get { return CurrentState == FenceState.Closed; } }

        private float initAnimationSpeed = 9999;
        private float animationSpeed = 1;

        private float AnimationSpeed
        {
            get
            {
                animationSpeed = GameStaticData.CurSpeedNum/40;
                return animationSpeed;
            }
        }

        private Animation animator;
        private FenceState currentState;
        
        public FenceAni(Transform transform)
        {

        }

        private void Awake()
        {
            animator=transform.AddComponent<Animation>();
            animator.playAutomatically = false;

            var openAnimation = GameStart.Instance.openAnimation;
            var closeAnimation = GameStart.Instance.closeAnimation;
            openAnimation.legacy = true;
            closeAnimation.legacy = true;
            animator.AddClip(openAnimation, FenceState.Open.ToString());
            animator.AddClip(closeAnimation, FenceState.Closed.ToString());
            
            animator[FenceState.Closed.ToString()].speed = initAnimationSpeed;
        }

        private void Start()
        {
            animator.Play(FenceState.Closed.ToString());
        }


        private void CloseFence()
        {
            if (IsFenceClosed)
                return;

            CurrentState = FenceState.Closed;
        }

        private void OpenFence()
        {
            if (IsFenceOpen)
                return;

            CurrentState = FenceState.Open;
        }

        public void ToggleFence()
        {
            if (IsFenceOpen)
                CloseFence();
            else
                OpenFence();
        }

        private void Animate()
        {
            var clip = GetCurrentAnimation();
            animator[clip].speed = AnimationSpeed;
            animator.Play(clip);
        }

        private string GetCurrentAnimation()
        {
            return CurrentState.ToString();
        }
        
        private enum FenceState
        {
            Open,
            Closed
        }
        private FenceState CurrentState {
            get
            {
                return currentState;
            }
            set
            {
                currentState = value;
                Animate();
            }
        }
    }
}