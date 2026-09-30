using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class BookMovement : MonoBehaviour
{
    public static BookMovement Instance;

    [Header("Positions")]
    [SerializeField] private Vector3 startLoc = new Vector3(0.45f, -0.07f, 0.05f);
    [SerializeField] private Vector3 startRot = Vector3.zero;

    [SerializeField] private Vector3 endLoc = new Vector3(0f, 0.09f, -0.3f);
    [SerializeField] private Vector3 endRot = Vector3.zero;

    public bool isInanimation = false;
    public bool isIdle = true;
 
    
    public float timeChange = 0.2f;


    [SerializeField] public GameObject jam1;
    [SerializeField] private GameObject jam2;

    [Header("Bobbing & Spinning")]
    [SerializeField] private float bobSpeed = 2.5f;
    [SerializeField] private float bobHeight = 0.05f;
    [SerializeField] private float spinSpeed = 40f;

    [SerializeField] private GameObject healthbar;
    [SerializeField] private GameObject Runehealthbar;

    [Header("Wall Animation Setup")]
    public float WallSpellTime = 0.5f;
    public Transform WallSpawnLoc;
    [SerializeField] private Animator armsAnimator;
    [SerializeField] private Animator jarAnimator;

    private static readonly int DropWallArms = Animator.StringToHash("rig_001|08_Arms Jar DropGround");
    private static readonly int DropWallJar = Animator.StringToHash("Armature|08_Jar Arms DropGround");
    private static readonly int EatArms = Animator.StringToHash("rig_001|07_Arms Jar Eat");
    private static readonly int EatJar = Animator.StringToHash("Armature|07_Jar Arms Eat");
    private static readonly int BazookaArms = Animator.StringToHash("rig_001|05_Arms Jar StartBazooka");
    private static readonly int BazookaJar = Animator.StringToHash("Armature|05_Jar Arms StartBazooka");
    private static readonly int SideWeepArms = Animator.StringToHash("rig_001|13_Arms Jar Splat");
    private static readonly int SideWeepJar = Animator.StringToHash("Armature|13_Jar Arms Splat");

    [Header("Player Character Reference")]
    [SerializeField] private PlayerCharacter playerCharacter;

    private float currentY;
    public bool isDisabled = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        transform.localPosition = startLoc;
        transform.localRotation = Quaternion.Euler(startRot);
        currentY = startRot.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCharacter == null)
        {
            playerCharacter = FindFirstObjectByType<PlayerCharacter>();
        }
    }

    void Update()
    {
        if (!isDisabled && isIdle && !isInanimation)
        {
            Spinning();
        }
    }

    void Spinning()
    {
        float newY = startLoc.y + (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        transform.localPosition = new Vector3(startLoc.x, newY, startLoc.z);

        currentY += spinSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(startRot.x, currentY, startRot.z);
    }

    private void OnStartSpellAnimation()
    {
        isInanimation = true;
        isIdle = false;
        transform.DOKill();

        if (playerCharacter != null)
        {
            playerCharacter.SetHandVisibility(false);
        }
    }

    private void ResetToIdleState()
    {
        isInanimation = false;
        isIdle = true;
        currentY = startRot.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCharacter != null)
        {
            playerCharacter.SetHandVisibility(true);
        }
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

        ResetToIdleState();
    }

    public void SwingJamAttack(Action onSwingApex)
    {
        if (isInanimation) return;
        OnStartSpellAnimation();

        StartCoroutine(ArmsAndJarAnimation(SideWeepArms, SideWeepJar, 0.2f, 1.0f, onSwingApex, 0.4f));
    }

    public void WallSpellAnimation(Action onSlamDown = null)
    {
        if (isInanimation) return;
        OnStartSpellAnimation();

        StartCoroutine(ArmsAndJarAnimation(DropWallArms, DropWallJar, 0.2f, 0.8f, onSlamDown, 0.4f));
    }

    public void EatAnimation(Action onEatComplete = null)
    {
        if (isInanimation) return;
        OnStartSpellAnimation();

        if (MusicManager.Instance != null) MusicManager.Instance.PlayEatingSound(transform.position);
        StartCoroutine(ArmsAndJarAnimation(EatArms, EatJar, 0.2f, 1.0f, () => {
            onEatComplete?.Invoke();
        }, 0.4f));
    }

    public void ThrowTrapAnimation(Action onTrapApex = null)
    {
        if (isInanimation) return;
        OnStartSpellAnimation();

        StartCoroutine(ArmsAndJarAnimation(SideWeepArms, SideWeepJar, 0.2f, 1.0f, onTrapApex, 0.4f));
    }

    public void SpreadCardThrowAnimation(Action onFlick = null)
    {
        if (isInanimation) return;
        OnStartSpellAnimation();

        StartCoroutine(ArmsAndJarAnimation(SideWeepArms, SideWeepJar, 0.2f, 1.0f, onFlick, 0.4f));
    }

    public void ToggleBookState()
    {
        if (isInanimation || isDisabled) return;

        if (isIdle)
        {
            transform.DOKill();
            isIdle = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            transform.DOLocalMove(endLoc, timeChange).SetEase(Ease.OutBack);
            transform.DOLocalRotate(endRot, timeChange).SetEase(Ease.OutBack);

            if (DrawingGuidanceUI.Instance != null)
            {
                DrawingGuidanceUI.Instance.ShowGuidance();
            }
            if (Runehealthbar != null) Runehealthbar.GetComponent<RectTransform>().DOAnchorPosX(-500, 0.1f);
            if (healthbar != null) healthbar.GetComponent<RectTransform>().DOAnchorPosX(-500, 0.1f);
        }
        else
        {
            ReturnToIdle();
        }
    }

    public void ReturnToIdle()
    {
        if (isInanimation) return;

        isIdle = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (DrawingGuidanceUI.Instance != null)
        {
            DrawingGuidanceUI.Instance.HideGuidance();
        }

        if (Runehealthbar != null) Runehealthbar.GetComponent<RectTransform>().DOAnchorPosX(0, 0.1f);
        if (healthbar != null) healthbar.GetComponent<RectTransform>().DOAnchorPosX(5, 0.1f);

        transform.DOKill();
        transform.DOLocalMove(startLoc, timeChange).SetEase(Ease.OutQuad);
        transform.DOLocalRotate(startRot, timeChange).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                currentY = startRot.y;
                if (playerCharacter != null)
                {
                    playerCharacter.SetHandVisibility(true);
                }
            });
    }

    public void Disabler(float time) => StartCoroutine(TemporaryDisable(time));

    public IEnumerator TemporaryDisable(float time)
    {
        isDisabled = true;
        if (!isIdle) ReturnToIdle();
        yield return new WaitForSeconds(time);
        isDisabled = false;
    }
    public void SetJamMaterial(Material newMat)
    {
        if (newMat == null) return;

        if (jam1 != null && jam1.TryGetComponent<Renderer>(out var rend1))
        {
            rend1.material = newMat;
        }

        if (jam2 != null && jam2.TryGetComponent<Renderer>(out var rend2))
        {
            rend2.material = newMat;
        }
    }
}