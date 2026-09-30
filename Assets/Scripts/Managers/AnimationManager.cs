using System;
using System.Collections;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;
    [SerializeField] private Animator armsAnimator;
    [SerializeField] private Animator jarAnimator;

    public static readonly int DropWallArms = Animator.StringToHash("rig_001|08_Arms Jar DropGround");
    public static readonly int DropWallJar = Animator.StringToHash("Armature|08_Jar Arms DropGround");
    public static readonly int EatArms = Animator.StringToHash("rig_001|07_Arms Jar Eat");
    public static readonly int EatJar = Animator.StringToHash("Armature|07_Jar Arms Eat");
    public static readonly int BazookaArms = Animator.StringToHash("rig_001|05_Arms Jar StartBazooka");
    public static readonly int BazookaJar = Animator.StringToHash("Armature|05_Jar Arms StartBazooka");
    public static readonly int SideWeepArms = Animator.StringToHash("rig_001|13_Arms Jar Splat");
    public static readonly int SideWeepJar = Animator.StringToHash("Armature|13_Jar Arms Splat");

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ArmsJarRunAnimation(int armHash, int jarHash, float transitionDuration, float earlyCutOff, Action onTriggerAction, float triggerDelay = 0.4f)
    {
        StartCoroutine(ArmsAndJarAnimation(armHash, jarHash, transitionDuration, earlyCutOff, onTriggerAction, triggerDelay));
    }

    private IEnumerator ArmsAndJarAnimation(int armHash, int jarHash, float transitionDuration, float earlyCutOff, Action onTriggerAction, float triggerDelay = 0.4f)
    {
        if (armsAnimator != null) armsAnimator.CrossFadeInFixedTime(armHash, transitionDuration);
        if (jarAnimator != null) jarAnimator.CrossFadeInFixedTime(jarHash, transitionDuration);

        yield return new WaitForSeconds(triggerDelay);

        try
        {
            onTriggerAction?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in spell callback: {e}");
        }

        float animLength = 1f;
        if (armsAnimator != null)
        {
            AnimatorStateInfo stateInfo = armsAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.length > 0f) animLength = stateInfo.length;
        }

        float totalDuration = animLength * Mathf.Clamp01(earlyCutOff);
        float remainingTime = Mathf.Max(0f, totalDuration - triggerDelay);

        if (remainingTime > 0f)
        {
            yield return new WaitForSeconds(remainingTime);
        }
    }
}
