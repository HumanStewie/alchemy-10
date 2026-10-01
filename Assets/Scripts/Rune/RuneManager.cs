using DG.Tweening;
using System.Collections;
using UnityEngine;

public class RuneManager : MonoBehaviour
{
    [Header("Timings & Speeds")]
    public float waitDuration = 5f;
    public float moveDuration = 5f;
    public float moveSpeed = 6f;

    [Header("Health")]
    public float currentHealth;
    public float maxHealth = 100f;

    [Header("Detection Settings")]
    public LayerMask obstacleMask;
    public float castRadius = 0.4f;
    public float raycastCheckDist = 0.5f;
    public float groundSnapDistance = 1.5f;

    [Header("Visuals")]
    public Transform arrowVisual;

    [Header("Position & Wave Transitions")]
    private Vector3 centerPosition = new Vector3(9.75f, 143.179993f, -247.979996f);
    [SerializeField] private float floatHeightOffset = 18f;
    [SerializeField] private float transitionDuration = 1.5f;
    [SerializeField] private Ease transitionEase = Ease.InOutSine;

    private Vector3 _currentMoveDirection;
    private bool _isMoving = false;
    private Coroutine _routine;

    public Vector3 CurrentMoveDirection => _currentMoveDirection;

    private void Awake()
    {
        if (maxHealth <= 0f) maxHealth = 100f;
        currentHealth = maxHealth;
    }

    public void InitializeRune()
    {
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(RuneRoutine());
    }

    private void Update()
    {
        if (currentHealth <= 0f)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Lose();
            }
        }
    }

    private IEnumerator RuneRoutine()
    {
        while (true)
        {
            PickNewDirection();
            if (arrowVisual != null) arrowVisual.gameObject.SetActive(true);

            _isMoving = false;
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.SetRuneMovementSound(false);
            }

            yield return new WaitForSeconds(waitDuration);

            if (arrowVisual != null) arrowVisual.gameObject.SetActive(false);
            _isMoving = true;

            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.SetRuneMovementSound(true, transform.position);
            }

            float elapsed = 0f;
            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                PerformMovement();

                if (MusicManager.Instance != null)
                {
                    MusicManager.Instance.SetRuneMovementSound(true, transform.position);
                }

                yield return null;
            }

            _isMoving = false;
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.SetRuneMovementSound(false);
            }
        }
    }

    private void PerformMovement()
    {
        Vector3 moveDir = _currentMoveDirection;
        float stepDistance = moveSpeed * Time.deltaTime;

        Vector3 origin = transform.position + Vector3.up * castRadius;
        if (Physics.SphereCast(origin, castRadius, moveDir, out RaycastHit wallHit, raycastCheckDist, obstacleMask))
        {
            Vector3 wallTangent = Vector3.Cross(wallHit.normal, Vector3.up);
            Vector3 climbDir = Vector3.Cross(wallTangent, wallHit.normal).normalized;

            if (climbDir.y < 0) climbDir = -climbDir;

            transform.position += climbDir * stepDistance;
        }
        else
        {
            Vector3 nextPos = transform.position + moveDir * stepDistance;

            if (Physics.Raycast(nextPos + Vector3.up * 0.5f, Vector3.down, out RaycastHit groundHit, groundSnapDistance, obstacleMask))
            {
                nextPos.y = groundHit.point.y;
            }

            transform.position = nextPos;
        }
    }

    public void PickNewDirection()
    {
        float angle = Random.Range(0f, 360f);
        _currentMoveDirection = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;

        if (arrowVisual != null)
        {
            arrowVisual.rotation = Quaternion.LookRotation(_currentMoveDirection, Vector3.up);
        }
    }

    public void Redirect(Vector3 newDirection)
    {
        newDirection.y = 0f;
        _currentMoveDirection = newDirection.normalized;

        if (arrowVisual != null)
        {
            arrowVisual.rotation = Quaternion.LookRotation(_currentMoveDirection, Vector3.up);
        }
    }

    private void OnDisable()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetRuneMovementSound(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * castRadius, castRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(new Vector3(centerPosition.x, transform.position.y, centerPosition.z), 0.5f);
    }

    public void goNextWave(float floorY = 0f)
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        _isMoving = false;
        if (arrowVisual != null) arrowVisual.gameObject.SetActive(false);

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetRuneMovementSound(false);
        }

        Vector3 targetPos = new Vector3(centerPosition.x, floorY + floatHeightOffset, centerPosition.z);

        transform.DOKill();
        transform.DOMove(targetPos, transitionDuration).SetEase(transitionEase);
    }

    public void ResumeWave()
    {
        transform.DOKill();

        float snapY = transform.position.y;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit groundHit, 50f, obstacleMask))
        {
            snapY = groundHit.point.y;
        }

        Vector3 groundPos = new Vector3(transform.position.x, snapY, transform.position.z);

        transform.DOMove(groundPos, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(RuneRoutine());
        });
    }
}