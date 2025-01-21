using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopUpBase : MonoBehaviour
{
    private Fader screenFade;
    private Animator animator;
    public UnityAction actionOnClose;
    private bool neeedShowFader = true;
    [SerializeField] private bool withOpenAnim = true;
    [SerializeField] private bool withSound = true;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }
        animator.enabled = withOpenAnim;
        animator.runtimeAnimatorController = UnityEngine.Resources.Load<RuntimeAnimatorController>("animations/Popup");
        animator.SetBool("withOpenAnim", withOpenAnim);
    }
    
    public void HideFader()
    {
        neeedShowFader = false;
        if (screenFade != null)
        {
            screenFade.Hide();
        }
    }
    public void ShowFader(bool overlayCanvas = false)
    {
        if (!neeedShowFader) return;
        if (screenFade == null)
        {
            screenFade = new Fader();
            screenFade.Show(overlayCanvas);
            gameObject.transform.SetParent(screenFade.FaderObj.transform);
            gameObject.transform.localPosition = Vector3.zero;
           /* if (AudioManager.instance != null && withSound)
            {
                AudioManager.instance.PlaySound(AudioManager.SoundId.PopUpShow);
            }*/
        }
    }
    public void Close()
    {
        gameObject.SetActive(false);
        actionOnClose?.Invoke();
        screenFade.Hide();
        Destroy(screenFade.FaderObj, 0.59f);
        Destroy(gameObject, 0.6f);
    }

}
