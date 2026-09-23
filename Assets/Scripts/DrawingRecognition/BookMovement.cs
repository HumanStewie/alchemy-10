using DG;
using DG.Tweening;
using System.Threading;
using UnityEngine;

public class BookMovement : MonoBehaviour
{
    Vector3 startLoc = new Vector3(0.5f, 0.2f, -0.36f);
    Vector3 endLoc = new Vector3(-30f, -18f, -10f);

    Vector3 startRot = new Vector3(0.5f, 0.2f, -0.36f);
    Vector3 endRot = new Vector3(90f, 0f, 0f);
    bool isIdle = true;

    public float timeChange = 0.2f;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (isIdle)
            {
                transform.DOMove(endLoc, timeChange).SetEase(Ease.OutBack);
                transform.DORotate(endRot, timeChange).SetEase(Ease.OutBack);
            }
            else
            {
                transform.DOMove(startLoc, timeChange).SetEase(Ease.OutBack);
                transform.DORotate(startRot, timeChange).SetEase(Ease.OutBack);
            }
        }    
    }

}
