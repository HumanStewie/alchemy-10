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

    // Hit Flash Internal Variables
    private List<Renderer> _renderers = new List<Renderer>();
    private MaterialPropertyBlock _propBlock;
    private Coroutine _flashRoutine;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    protected virtual void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        GetComponentsInChildren<Renderer>(true, _renderers);
    }

    protected virtual void Start()
    { 
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
            if (runeSymbol != null) runeSymbol.SetActive(true);
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

        if (currentHP <= 0) Die();
    }

    public virtual void Die()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayDieSound(transform.position);

        Destroy(gameObject);
    }

    public virtual void Poisoned(float dps, float duration) => StartCoroutine(StatusDamage(dps, duration));
    public virtual void Burnt(float dps, float duration) => StartCoroutine(StatusDamage(dps, duration));
    public virtual void Freeze(float duration)
    {
        float original = currentspeed;
        currentspeed = 0f;
        Invoke(nameof(Unfreeze), duration);
        void Unfreeze() => currentspeed = original;
    }

    IEnumerator StatusDamage(float dps, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            takeDamage(dps * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }
    }

    protected void MoveTowards(Vector3 targetPos, float speed)
    {
        Vector3 dir = (targetPos - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.01f)
        {
            transform.position += dir.normalized * speed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 8f * Time.deltaTime);
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
    }
}