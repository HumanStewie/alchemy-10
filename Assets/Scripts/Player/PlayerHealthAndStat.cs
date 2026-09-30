using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthAndStat : MonoBehaviour
{
    [Header("Health Stats")]
    public float maxHP = 100f;
    public float currentHP;

    [Header("Attack Stats")]
    public float damage = 10f;
    public float currentdamage;

    [Header("Modifiers")]
    public float cooldownMultiplier = 1f;
    public float maxSpeed = 10f;

    [SerializeField] private GameObject bloodParticle;
    [SerializeField] private GameObject PoofParticle;

    private PlayerCharacter playerCharacter;
    private bool isDead = false;

    protected virtual void Awake()
    {
        if (maxHP <= 0f) maxHP = 100f;
        currentHP = maxHP;
        currentdamage = damage;
    }

    protected virtual void Start()
    {
        playerCharacter = GetComponent<PlayerCharacter>();
        if (playerCharacter != null && maxSpeed <= 0f)
            maxSpeed = playerCharacter.walkSpeed;

        if (GameManager.Instance != null)
        {
            bloodParticle = GameManager.Instance.bloodEffect;
            PoofParticle = GameManager.Instance.poofEffect;
        }

        UpdateHealthBar();
    }

    private void LateUpdate()
    {
        if (!isDead && currentHP <= 0f)
        {
            Die();
        }
    }

    public void ChangeAttack(float multiplier = 1f)
    {
        if (multiplier != 1f)
            currentdamage *= multiplier;
    }

    public void AttackNormal()
    {
        currentdamage = damage;
    }

    public void ChangeSpeed(float multiplier = 1f)
    {
        if (playerCharacter != null && multiplier != 1f)
            playerCharacter.walkSpeed *= multiplier;
    }

    public void SpeedNormal()
    {
        if (playerCharacter != null)
            playerCharacter.walkSpeed = maxSpeed;
    }

    public void takeDamage(float dmg)
    {
        if (isDead) return;

        currentHP = Mathf.Max(currentHP - dmg, 0f);
        UpdateHealthBar();

        if (bloodParticle != null)
            Instantiate(bloodParticle, transform.position, Quaternion.identity);

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayPlayerHurtSound(transform.position);

        if (CameraShake.Instance != null)
            CameraShake.Instance.ShakeLight();

        if (currentHP <= 0f)
            Die();
    }

    public void heal(float amount)
    {
        if (isDead) return;
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (GameManager.Instance == null || GameManager.Instance.healthBar == null) return;

        var barImage = GameManager.Instance.healthBar.GetComponent<Image>();
        if (barImage != null && maxHP > 0f)
            barImage.fillAmount = Mathf.Clamp01(currentHP / maxHP);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayDieSound(transform.position);

        if (PoofParticle != null)
            Instantiate(PoofParticle, transform.position, Quaternion.identity);

        if (CameraShake.Instance != null)
            CameraShake.Instance.ShakeHeavy();

        if (GameManager.Instance != null)
            GameManager.Instance.Lose();

           Destroy(gameObject);
    }
}