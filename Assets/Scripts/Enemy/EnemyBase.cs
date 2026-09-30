using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Base Stats")]
    public float maxHP = 10f;
    public float currentHP;
    public float damage = 5f;
    public float moveSpeed = 4f;
    public float currentspeed;

    [Header("Detection & Targeting")]
    public float attackRange = 2f;
    public float detectionRange = 30f;
    public bool preferRune = false;

    public GameObject runeSymbol;

    [Header("Hit Flash Settings")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.08f;

    protected Transform player;
    protected Transform runeTarget;
    protected Transform currentTarget;
    protected float attackCooldownTimer = 0f;

    private GameObject burningEffect;
    private GameObject poisonEffect;
    private GameObject bloodEffect;
    private GameObject poofEffect;

    private Coroutine _poisonRoutine;
    private Coroutine _burnRoutine;
    private GameObject _activePoisonVFX;
    private GameObject _activeBurnVFX;

    private List<Renderer> _renderers = new List<Renderer>();
    private MaterialPropertyBlock _propBlock;
    private Coroutine _flashRoutine;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    [SerializeField] protected Animator enemyAnimator; 
    [SerializeField] protected Rigidbody rb;

    protected static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    protected static readonly int AttackHash = Animator.StringToHash("Attack");

    protected virtual void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        GetComponentsInChildren<Renderer>(true, _renderers);
    }

    protected virtual void Start()
    {
        enemyAnimator = GetComponent<Animator>();

        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponentInChildren<Animator>();
        }
        rb = GetComponent<Rigidbody>();

        if (enemyAnimator == null)
        {
            rb = GetComponentInChildren<Rigidbody>();
        }
        runeSymbol = (GameObject)Resources.Load("Enemy/Rune");
        currentHP = maxHP;
        currentspeed = moveSpeed;

        var p = Object.FindFirstObjectByType<PlayerCharacter>();
        if (p != null) player = p.transform;

        var rune = Object.FindFirstObjectByType<RuneManager>();
        if (rune != null) runeTarget = rune.transform;

        if (GameManager.Instance != null)
        {
            burningEffect = GameManager.Instance.burningEffect;
            poisonEffect = GameManager.Instance.poisonEffect;
            bloodEffect = GameManager.Instance.bloodEffect;
            poofEffect = GameManager.Instance.poofEffect;
        }

        ChooseTarget();
    }

    protected virtual void Update()
    {
        if (currentTarget == null) ChooseTarget();
        if (currentTarget == null) return;

        attackCooldownTimer -= Time.deltaTime;
        BehaviorUpdate();
    }

    protected virtual void ChooseTarget()
    {
        if (preferRune && runeTarget != null && Random.value < 0.5f)
        {
            currentTarget = runeTarget;
            if (runeSymbol != null)
            {
                Instantiate(runeSymbol, new Vector3(0, 2, 0), Quaternion.identity, transform);
            }
        }
        else
        {
            currentTarget = player;
            if (runeSymbol != null) runeSymbol.SetActive(false);
        }
    }

    protected virtual void BehaviorUpdate() { }

    public virtual void takeDamage(float amount)
    {
        currentHP -= amount;

        TriggerHitFlash();

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayEnemyHurtSound(transform.position);

        if (bloodEffect != null)
            Instantiate(bloodEffect, transform.position, Quaternion.identity);

        if (currentHP <= 0) Die();
    }

    public virtual void Die()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayDieSound(transform.position);

        if (poofEffect != null)
            Instantiate(poofEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }


    public virtual void Poisoned(float dps, float duration)
    {
        if (_poisonRoutine != null)
        {
            StopCoroutine(_poisonRoutine);
        }
        _poisonRoutine = StartCoroutine(PoisonRoutine(dps, duration));
    }

    public virtual void Burnt(float dps, float duration)
    {
        if (_burnRoutine != null)
        {
            StopCoroutine(_burnRoutine);
        }
        _burnRoutine = StartCoroutine(BurnRoutine(dps, duration));
    }

    public virtual void Freeze(float duration)
    {
        float original = currentspeed;
        currentspeed = 0f;
        Invoke(nameof(Unfreeze), duration);
        void Unfreeze() => currentspeed = original;
    }

    private IEnumerator PoisonRoutine(float dps, float duration)
    {
        if (_activePoisonVFX == null && poisonEffect != null)
        {
            _activePoisonVFX = Instantiate(poisonEffect, transform.position, Quaternion.identity, transform);
        }

        float t = 0f;
        while (t < duration)
        {
            takeDamage(dps * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }

        if (_activePoisonVFX != null)
        {
            Destroy(_activePoisonVFX);
            _activePoisonVFX = null;
        }
        _poisonRoutine = null;
    }

    private IEnumerator BurnRoutine(float dps, float duration)
    {
        if (_activeBurnVFX == null && burningEffect != null)
        {
            _activeBurnVFX = Instantiate(burningEffect, transform.position, Quaternion.identity, transform);
        }

        float t = 0f;
        while (t < duration)
        {
            takeDamage(dps * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }

        if (_activeBurnVFX != null)
        {
            Destroy(_activeBurnVFX);
            _activeBurnVFX = null;
        }
        _burnRoutine = null;
    }


    protected void MoveTowards(Vector3 targetPos, float speed)
    {
        Vector3 dir = (targetPos - transform.position);
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.01f && speed > 0f)
        {
            if (rb != null)
            {
                Vector3 moveVelocity = dir.normalized * speed;
                moveVelocity.y = rb.linearVelocity.y; 
                rb.linearVelocity = moveVelocity;
            }

            Quaternion targetRot = Quaternion.LookRotation(dir) * Quaternion.Euler(0f, 90f, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 8f * Time.deltaTime);

            if (enemyAnimator != null) enemyAnimator.SetBool(IsMovingHash, true);
        }
        else
        {
            if (rb != null) rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            if (enemyAnimator != null) enemyAnimator.SetBool(IsMovingHash, false);
        }
    }

    public void TriggerAttackAnimation()
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool(IsMovingHash, false);
            enemyAnimator.SetTrigger(AttackHash);
        }
    }

    public void TriggerHitFlash()
    {
        if (_flashRoutine != null)
        {
            StopCoroutine(_flashRoutine);
        }
        _flashRoutine = StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        SetRendererColors(flashColor);
        yield return new WaitForSeconds(flashDuration);
        ResetRendererColors();
        _flashRoutine = null;
    }

    private void SetRendererColors(Color color)
    {
        for (int i = 0; i < _renderers.Count; i++)
        {
            Renderer rend = _renderers[i];
            if (rend == null) continue;

            rend.GetPropertyBlock(_propBlock);
            _propBlock.SetColor(BaseColorId, color);
            _propBlock.SetColor(ColorId, color);
            rend.SetPropertyBlock(_propBlock);
        }
    }

    private void ResetRendererColors()
    {
        for (int i = 0; i < _renderers.Count; i++)
        {
            Renderer rend = _renderers[i];
            if (rend == null) continue;

            rend.SetPropertyBlock(null);
        }
    }

    protected virtual void OnDisable()
    {
        ResetRendererColors();

        if (_activePoisonVFX != null) Destroy(_activePoisonVFX);
        if (_activeBurnVFX != null) Destroy(_activeBurnVFX);
    }
}