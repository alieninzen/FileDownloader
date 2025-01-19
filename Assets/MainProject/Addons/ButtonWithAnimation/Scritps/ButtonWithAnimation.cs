using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public enum AnimationType
{
    Flip,
    Pressed,
}

[RequireComponent(typeof(Button))]

public class ButtonWithAnimation : MonoBehaviour
{
    private Animator animator;
    private Button button;
    [SerializeField] private AnimationType animType = AnimationType.Pressed;
    [SerializeField] private bool withIdle = false;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        AnimatorController animatorController = UnityEngine.Resources.Load<AnimatorController>("animations/Button");
        animator.runtimeAnimatorController = animatorController;
        if (withIdle)
        {
            ShowIdle();
        }
    }

    public void ShowIdle()
    {
        GetAnimator();
        animator.SetBool("idle", true);
    }
    private void GetAnimator()
    {
        if (animator == null)
        {
            Awake();
        }
    }
    public void ShowButtons()
    {
        GetAnimator();
        animator.SetTrigger("show");
    }
    public void HideButtons()
    {
        GetAnimator();
        animator.SetTrigger("hide");
    }
    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
    public void OnButtonClick()
    {
        GetAnimator();
        animator.SetTrigger(animType.ToString());
    }
}