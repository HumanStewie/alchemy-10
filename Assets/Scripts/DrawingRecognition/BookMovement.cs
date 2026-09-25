using DG.Tweening;
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

    [Header("WallSpell Animation")]
    public float WallSpellTime = 0.5f;
    private Vector3 WallSpellLoc = new Vector3(0.3f, -0.1f, 0.1f);
    private Vector3 WallSpellRot = new Vector3(0, 0, 180);

    private float currentY;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        transform.localPosition = startLoc;
        transform.localRotation = Quaternion.Euler(startRot);
        currentY = startRot.y;
    }

    void Update()
    {
        if (isIdle)
        {
            Spinning();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleBookState();
        }
    }

    void Spinning()
    {
        float newY = startLoc.y + (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        transform.localPosition = new Vector3(startLoc.x, newY, startLoc.z);

        currentY += spinSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(startRot.x, currentY, startRot.z);
    }

    public void ToggleBookState()
    {
        if (isInanimation) return;


        Cursor.lockState = CursorLockMode.None;

        if (isIdle)
        {
            transform.DOKill();
            isIdle = false;

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

        Cursor.lockState = CursorLockMode.Locked;
        transform.DOKill();
        transform.DOLocalMove(startLoc, timeChange).SetEase(Ease.OutQuad);
        transform.DOLocalRotate(startRot, timeChange).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                currentY = startRot.y;
                isIdle = true;
            });
    }

    public void WallSpellAnimation()
    {
        isInanimation = true;
        transform.DOKill();

        transform.DOLocalMove(WallSpellLoc, WallSpellTime).SetEase(Ease.OutBack);
        transform.DOLocalRotate(WallSpellRot, WallSpellTime).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transform.DOLocalMove(startLoc, timeChange).SetEase(Ease.OutQuad);
                transform.DOLocalRotate(startRot, timeChange).SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        isInanimation = false;
                        isIdle = true;
                        currentY = startRot.y;
                    });
            });
    }


    public void SwingJamAttack(System.Action onSwingApex)
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
        swingSeq.OnComplete(() =>
        {
            isInanimation = false;
            isIdle = true;
            currentY = startRot.y;
        });
    }
}