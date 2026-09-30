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
    [SerializeField] private Animator swordAnimator;

    [Header("Animation Delay")]
    [SerializeField] private float bazookaDelay = 1.5f;
    [SerializeField] private float dropWallDelay = 0.3f;
    [SerializeField] private float eatDelay = 0.4f;
    [SerializeField] private float dropCurveDelay = 0.2f;
    [SerializeField] private float splatDelay = 0.2f;

    [SerializeField] private float swordPullDelay = 0.4f;
    [SerializeField] private float swordLeftDelay = 0.4f;
    [SerializeField] private float swordRightDelay = 0.4f;
    [SerializeField] private float swordEndDelay = 0.4f;


    private static readonly int DropWallArms = Animator.StringToHash("rig_001|08_Arms Jar DropGround");
    private static readonly int DropWallJar = Animator.StringToHash("Armature|08_Jar Arms DropGround");
    private static readonly int EatArms = Animator.StringToHash("rig_001|07_Arms Jar Eat");
    private static readonly int EatJar = Animator.StringToHash("Armature|07_Jar Arms Eat");
    private static readonly int BazookaArms = Animator.StringToHash("rig_001|05_Arms Jar StartBazooka");
    private static readonly int BazookaJar = Animator.StringToHash("Armature|05_Jar Arms StartBazooka");
    private static readonly int SideWeepArms = Animator.StringToHash("rig_001|13_Arms Jar Splat");
    private static readonly int SideWeepJar = Animator.StringToHash("Armature|13_Jar Arms Splat");
    private static readonly int PullSwordArms = Animator.StringToHash("rig_001|09_Arms Jar PullSword");
    private static readonly int PullSwordJar = Animator.StringToHash("Armature|09_Jar Arms PullSword");
    private static readonly int PullSwordSword = Animator.StringToHash("Armature_001|09_Sword Arms PullSword");
    private static readonly int SwingLeftArms = Animator.StringToHash("rig_001|11_Arms Sword AttackLeft");
    private static readonly int SwingLeftSword = Animator.StringToHash("Armature_001|11_Sword Arms AttackLeft");
    private static readonly int SwingLeftJar = Animator.StringToHash("Armature|11_Jar Arms AttackLeft");

    private static readonly int SwingRightArms = Animator.StringToHash("rig_001|10_Arms Sword AttackRight");
    private static readonly int SwingRightSword = Animator.StringToHash("Armature_001|10_Sword Arms AttackRight");
    private static readonly int SwingRightJar = Animator.StringToHash("Armature|10_Jar Arms AttackRight");
    private static readonly int StopSwordArms = Animator.StringToHash("rig_001|12_Arms Sword EndSword");
    private static readonly int StopSwordSword = Animator.StringToHash("Armature_001|12_Sword Arms EndSword");
    private static readonly int StopSwordJar = Animator.StringToHash("Armature|12_Jar Arms EndSword");


    [Header("Player Character Reference")]
    [SerializeField] private PlayerCharacter playerCharacter;

    private float currentY;
    public bool isDisabled = false;

    // Checks if player is currently in sword stance based on the arms animator parameter
    public bool IsHoldingSword => armsAnimator != null && armsAnimator.GetBool("IsHoldingSword");

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
        swordAnimator.gameObject.SetActive(false);
        jarAnimator.gameObject.SetActive(true);

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

    private IEnumerator ArmsAndJarAndSwordAnimation(int armHash, int jarHash, int swordHash, float transitionDuration, float earlyCutOff, Action onTriggerAction, float triggerDelay = 0.4f, Action onAnimationComplete = null)
    {
        if (armsAnimator != null) armsAnimator.CrossFadeInFixedTime(armHash, transitionDuration);
        if (jarAnimator != null) jarAnimator.CrossFadeInFixedTime(jarHash, transitionDuration);
        if (swordAnimator != null) swordAnimator.CrossFadeInFixedTime(swordHash, transitionDuration);

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

        // Invoke completion callback if provided (e.g., to clean up sword state after sheathing)
        onAnimationComplete?.Invoke();

        ResetToIdleState();
    }


    public void JamBazooka(Action onSwingApex)
    {
        if (isInanimation) return;
        OnStartSpellAnimation();

        StartCoroutine(ArmsAndJarAnimation(BazookaArms, BazookaJar, 0.2f, 1.0f, onSwingApex, bazookaDelay));
    }

    public void JamSwordSwing(SwordState state, Action onSwingApex)
    {
        if (isInanimation) return;
        swordAnimator.gameObject.SetActive(true);
        isInanimation = true;
        isIdle = false;
        transform.DOKill();
        transform.localPosition = startLoc;
        transform.localRotation = Quaternion.Euler(startRot);

        if (state is SwordState.Pull)
        {   
            armsAnimator.SetBool("IsHoldingSword", true);
            jarAnimator.SetBool("IsHoldingSword", true);
            StartCoroutine(ArmsAndJarAndSwordAnimation(PullSwordArms, PullSwordJar, PullSwordSword, 0.2f, 1f, onSwingApex, swordPullDelay));
        }
        else if (state is SwordState.SwingRight)
        {
            jarAnimator.gameObject.SetActive(false);
            StartCoroutine(ArmsAndJarAndSwordAnimation(SwingRightArms, SwingRightJar, SwingRightSword, 0.2f, 1f, onSwingApex, swordRightDelay));
        }
        else if (state is SwordState.SwingLeft)
        {
            jarAnimator.gameObject.SetActive(false);
            StartCoroutine(ArmsAndJarAndSwordAnimation(SwingLeftArms, SwingLeftJar, SwingLeftSword, 0.2f, 1f, onSwingApex, swordLeftDelay));
        }
        else if (state is SwordState.Stop)
        {
            // Putting away the sword:
            // Ensure both jar and sword are visible so they can animate stowing into the jar together
            jarAnimator.gameObject.SetActive(true);
            swordAnimator.gameObject.SetActive(true);

            StartCoroutine(ArmsAndJarAndSwordAnimation(
                StopSwordArms, StopSwordJar, StopSwordSword, 0.2f, 1f, () => {
                    // Clean up once the putting-away animation has fully finished:
                    // 1. Reset sword holding animation parameters so arms/jar return to regular idle
                    if (armsAnimator != null) armsAnimator.SetBool("IsHoldingSword", false);
                    if (jarAnimator != null) {
                        jarAnimator.SetBool("IsHoldingSword", false);
                        jarAnimator.SetBool("IsDrawing", false);
                    }
                    
                    // 2. Hide the sword mesh now that it's back inside the jar
                    if (swordAnimator != null) swordAnimator.gameObject.SetActive(false);
                }, swordEndDelay));
        }
    }

    public void WallSpellAnimation(Action onSlamDown = null)
    {
        if (isInanimation) return;
        OnStartSpellAnimation();

        StartCoroutine(ArmsAndJarAnimation(DropWallArms, DropWallJar, 0.2f, 0.8f, onSlamDown, dropWallDelay));
    }

    public void EatAnimation(Action onEatComplete = null)
    {
        if (isInanimation) return;
        OnStartSpellAnimation();

        if (MusicManager.Instance != null) MusicManager.Instance.PlayEatingSound(transform.position);
        StartCoroutine(ArmsAndJarAnimation(EatArms, EatJar, 0.2f, 1.0f, () => {
            onEatComplete?.Invoke();
        }, eatDelay));
    }

    public void ThrowTrapAnimation(Action onTrapApex = null)
    {
        if (isInanimation) return;
        OnStartSpellAnimation();

        StartCoroutine(ArmsAndJarAnimation(SideWeepArms, SideWeepJar, 0.5f, 1.0f, onTrapApex, dropCurveDelay));
    }

    public void SpreadCardThrowAnimation(Action onFlick = null)
    {
        if (isInanimation) return;
        OnStartSpellAnimation();

        StartCoroutine(ArmsAndJarAnimation(SideWeepArms, SideWeepJar, 0.2f, 1.0f, onFlick, splatDelay));
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