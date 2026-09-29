using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Tween currentShakeTween;
    private Vector3 initialLocalPos;

    private void Awake()
    {
        Instance = this;
        initialLocalPos = transform.localPosition;
    }

    public void Shake(float duration = 0.15f, float strength = 0.2f, int vibrato = 20, float randomness = 90f)
    {
        if (currentShakeTween != null && currentShakeTween.IsActive())
        {
            currentShakeTween.Kill();
            transform.localPosition = initialLocalPos;
        }

        currentShakeTween = transform.DOShakePosition(duration, strength, vibrato, randomness, false, true)
            .OnComplete(() =>
            {
                transform.localPosition = initialLocalPos;
            });
    }

    public void ShakeLight() => Shake(0.12f, 0.12f, 25, 90f);
    public void ShakeHeavy() => Shake(0.28f, 0.35f, 30, 90f);

    private void OnDisable()
    {
        if (currentShakeTween != null && currentShakeTween.IsActive())
        {
            currentShakeTween.Kill();
            transform.localPosition = initialLocalPos;
        }
    }
}