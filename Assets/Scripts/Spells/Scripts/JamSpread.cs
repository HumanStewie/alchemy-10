using UnityEngine;

using UnityEngine.Rendering.Universal;

using DG.Tweening;
using Unity.VisualScripting;



[RequireComponent(typeof(DecalProjector))]

[RequireComponent(typeof(BoxCollider))]

public class JamSpread : MonoBehaviour

{

    [Header("Life Cycle")]

    [SerializeField] private float duration = 8f;

    [SerializeField] private float fadeDuration = 1.2f;



    [Header("Pop Animation")]

    [SerializeField] private float popDuration = 0.1f;



    [Header("Trigger Settings")]

    [SerializeField] private float triggerThickness = 0.15f;



    [Header("Gameplay Effects")]

    [SerializeField] private float slowMultiplier = 0.4f;



    private DecalProjector projector;

    private BoxCollider triggerBox;

    private Vector3 targetSize;



    void Awake()

    {

        transform.rotation = Quaternion.Euler(90f, FindAnyObjectByType<PlayerCharacter>().transform.eulerAngles.y, 0f);

        projector = GetComponent<DecalProjector>();

        triggerBox = GetComponent<BoxCollider>();



        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");



        triggerBox.isTrigger = true;

        targetSize = projector.size;



        triggerBox.size = new Vector3(targetSize.x, targetSize.y, triggerThickness);

        triggerBox.center = Vector3.zero;

    }



    public void InitSplat()
    {

        projector.size = new Vector3(0f, 0f, targetSize.z);

        triggerBox.size = new Vector3(0f, 0f, triggerThickness);

        MusicManager.Instance.PlaySpreadSound(transform.position);

        DOTween.To(() => projector.size, s =>

        {

            projector.size = s;

            triggerBox.size = new Vector3(s.x, s.y, triggerThickness);

        }, targetSize, popDuration).SetEase(Ease.OutBack);



        DOVirtual.DelayedCall(Mathf.Max(0.1f, duration - fadeDuration), StartFadeOut);

    }



    private void StartFadeOut()
    {

        if (triggerBox != null)

        {

            triggerBox.enabled = false;
        }
        DOTween.To(() => projector.fadeFactor, f => projector.fadeFactor = f, 0f, fadeDuration)

            .OnComplete(() => Destroy(gameObject));

    }
}