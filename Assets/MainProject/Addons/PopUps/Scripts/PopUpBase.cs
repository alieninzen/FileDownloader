using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.Events;

public class PopUpBase : MonoBehaviour
{
    private Fader screenFade;
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }
        AnimatorController animatorController = UnityEngine.Resources.Load<AnimatorController>("animations/Popup");
        animator.runtimeAnimatorController = animatorController;
    }
    void Start()
    {
        screenFade = new Fader();
        screenFade.Show();
        gameObject.transform.SetParent(screenFade.FaderObj.transform);
        gameObject.transform.localPosition = Vector3.zero;
      //  gameObject.transform.SetAsLastSibling();
    }
    public void Close()
    {
        animator.SetTrigger("Close");
        screenFade.Hide();
        Destroy(screenFade.FaderObj, 0.9f);
        Destroy(gameObject, 1);
    }

}
