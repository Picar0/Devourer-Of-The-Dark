using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Manager
{
    public class AnimatorManager : MonoBehaviour
    {
        public Animator anim;
        public bool canRotate;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        // this method enables the root motion for animation that I need to play for doing certain task
        public void PlayTargetAnimation(string targetAnim, bool isInteracting)
        {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("canRotate", false);
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.2f);
        }
    }
}
