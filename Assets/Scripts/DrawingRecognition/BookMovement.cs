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

    [Header("Bobbing & Spinning")]
    [SerializeField] private float bobSpeed = 2.5f;
    [SerializeField] private float bobHeight = 0.05f;
    [SerializeField] private float spinSpeed = 40f;

    [Header("Wall Animation Setup")]
    public float WallSpellTime = 0.5f;
    public Transform WallSpawnLoc;

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

    private void ResetToIdleState()
    {
        isInanimation = false;
        isIdle = true;
        currentY = startRot.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SwingJamAttack(Action onSwingApex)
    {
        if (isInanimation) return;

        isInanimation = true;
        isIdle = false;
        transform.DOKill();

        Vector3 prepLoc = new Vector3(startLoc.x - 0.22f, startLoc.y + 0.08f, startLoc.z);
        Vector3 prepRot = new Vector3(10f, -30f, 20f);

        Vector3 swingLoc = new Vector3(startLoc.x + 0.28f, startLoc.y - 0.04f, startLoc.z + 0.12f);
        Vector3 swingRot = new Vector3(-15f, 45f, -35f);

        Sequence swingSeq = DOTween.Sequence();

        swingSeq.Append(transform.DOLocalMove(prepLoc, 0.1f).SetEase(Ease.OutQuad));
        swingSeq.Join(transform.DOLocalRotate(prepRot, 0.1f).SetEase(Ease.OutQuad));

        swingSeq.Append(transform.DOLocalMove(swingLoc, 0.12f).SetEase(Ease.InCubic));
        swingSeq.Join(transform.DOLocalRotate(swingRot, 0.12f).SetEase(Ease.InCubic));

        swingSeq.AppendCallback(() => onSwingApex?.Invoke());

        swingSeq.Append(transform.DOLocalMove(startLoc, 0.25f).SetEase(Ease.OutQuad));
        swingSeq.Join(transform.DOLocalRotate(startRot, 0.25f).SetEase(Ease.OutQuad));

        swingSeq.OnComplete(ResetToIdleState);
    }


    public void WallSpellAnimation(Action onSlamDown = null)
    {
        if (isInanimation) return;
        isInanimation = true;
        isIdle = false;
        transform.DOKill();

        Sequence wallSeq = DOTween.Sequence();

        Vector3 midArcLoc = new Vector3(0.18f, 0.16f, 0.15f);
        Vector3 midArcRot = new Vector3(-20f, -15f, 25f);

        Vector3 centerLoc = new Vector3(0.0f, 0.18f, 0.25f);
        Vector3 centerRot = new Vector3(-35f, 0f, 0f);

        Vector3 slamLoc = new Vector3(0.0f, -0.28f, 0.22f);
        Vector3 slamRot = new Vector3(30f, 0f, 0f);

        wallSeq.Append(transform.DOLocalMove(midArcLoc, 0.18f).SetEase(Ease.OutSine));
        wallSeq.Join(transform.DOLocalRotate(midArcRot, 0.18f).SetEase(Ease.OutSine));

        wallSeq.Append(transform.DOLocalMove(centerLoc, 0.16f).SetEase(Ease.OutQuad));
        wallSeq.Join(transform.DOLocalRotate(centerRot, 0.16f).SetEase(Ease.OutQuad));

        wallSeq.Append(transform.DOLocalMove(slamLoc, 0.12f).SetEase(Ease.InExpo));
        wallSeq.Join(transform.DOLocalRotate(slamRot, 0.12f).SetEase(Ease.InQuad));

        wallSeq.AppendCallback(() => onSlamDown?.Invoke());

        wallSeq.Append(transform.DOLocalMove(startLoc, 0.22f).SetEase(Ease.OutQuad));
        wallSeq.Join(transform.DOLocalRotate(startRot, 0.22f).SetEase(Ease.OutQuad));

        wallSeq.OnComplete(ResetToIdleState);
    }


    public void EatAnimation(Action onEatComplete = null)
    {
        if (isInanimation) return;
        isInanimation = true;
        isIdle = false;
        transform.DOKill();

        Sequence eatSeq = DOTween.Sequence();

        Vector3 mouthLoc = new Vector3(0.08f, -0.05f, -0.1f);
        Vector3 mouthRot = new Vector3(35f, -25f, 15f);

        eatSeq.Append(transform.DOLocalMove(mouthLoc, 0.12f).SetEase(Ease.OutBack));
        eatSeq.Join(transform.DOLocalRotate(mouthRot, 0.12f).SetEase(Ease.OutQuad));

        for (int i = 0; i < 4; i++)
        {
            Vector3 biteDip = mouthLoc + new Vector3(0f, -0.035f, 0.02f);
            eatSeq.Append(transform.DOLocalMove(biteDip, 0.033f).SetEase(Ease.InQuad));
            eatSeq.Append(transform.DOLocalMove(mouthLoc, 0.033f).SetEase(Ease.OutQuad));
        }

        eatSeq.AppendCallback(() =>
        {
            if (MusicManager.Instance != null) MusicManager.Instance.PlayEatingSound(transform.position);
            onEatComplete?.Invoke();
        });

        eatSeq.Append(transform.DOLocalMove(startLoc, 0.12f).SetEase(Ease.OutQuad));
        eatSeq.Join(transform.DOLocalRotate(startRot, 0.12f).SetEase(Ease.OutQuad));

        eatSeq.OnComplete(ResetToIdleState);
    }

 
    public void ThrowTrapAnimation(Action onTrapApex = null)
    {
        if (isInanimation) return;
        isInanimation = true;
        isIdle = false;
        transform.DOKill();

        Sequence throwSeq = DOTween.Sequence();

        Vector3 windBackLoc = new Vector3(startLoc.x + 0.08f, startLoc.y - 0.14f, startLoc.z - 0.12f);
        Vector3 windBackRot = new Vector3(-25f, 20f, -10f);

        Vector3 throwApexLoc = new Vector3(0.05f, 0.12f, 0.32f);
        Vector3 throwApexRot = new Vector3(55f, -10f, 0f);

        throwSeq.Append(transform.DOLocalMove(windBackLoc, 0.12f).SetEase(Ease.OutQuad));
        throwSeq.Join(transform.DOLocalRotate(windBackRot, 0.12f).SetEase(Ease.OutQuad));

        throwSeq.Append(transform.DOLocalMove(throwApexLoc, 0.14f).SetEase(Ease.InCubic));
        throwSeq.Join(transform.DOLocalRotate(throwApexRot, 0.14f).SetEase(Ease.InBack));

        throwSeq.AppendCallback(() => onTrapApex?.Invoke());

        throwSeq.Append(transform.DOLocalMove(startLoc, 0.22f).SetEase(Ease.OutQuad));
        throwSeq.Join(transform.DOLocalRotate(startRot, 0.22f).SetEase(Ease.OutQuad));

        throwSeq.OnComplete(ResetToIdleState);
    }

    public void SpreadCardThrowAnimation(Action onFlick = null)
    {
        if (isInanimation) return;
        isInanimation = true;
        isIdle = false;
        transform.DOKill();

        Sequence flickSeq = DOTween.Sequence();

        Vector3 leftPrepLoc = new Vector3(-0.25f, -0.02f, 0.08f);
        Vector3 leftPrepRot = new Vector3(15f, -40f, 45f);

        Vector3 rightFlickLoc = new Vector3(0.48f, 0.04f, 0.22f);
        Vector3 rightFlickRot = new Vector3(-20f, 50f, -50f);

        flickSeq.Append(transform.DOLocalMove(leftPrepLoc, 0.14f).SetEase(Ease.OutQuad));
        flickSeq.Join(transform.DOLocalRotate(leftPrepRot, 0.14f).SetEase(Ease.OutQuad));

        flickSeq.Append(transform.DOLocalMove(rightFlickLoc, 0.13f).SetEase(Ease.InSine));
        flickSeq.Join(transform.DOLocalRotate(rightFlickRot, 0.13f).SetEase(Ease.InSine));

        flickSeq.AppendCallback(() => onFlick?.Invoke());

        flickSeq.Append(transform.DOLocalMove(startLoc, 0.2f).SetEase(Ease.OutQuad));
        flickSeq.Join(transform.DOLocalRotate(startRot, 0.2f).SetEase(Ease.OutQuad));

        flickSeq.OnComplete(ResetToIdleState);
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

        transform.DOKill();
        transform.DOLocalMove(startLoc, timeChange).SetEase(Ease.OutQuad);
        transform.DOLocalRotate(startRot, timeChange).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                currentY = startRot.y;
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
}