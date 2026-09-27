using UnityEngine;

public class PlayerHealthAndStat : MonoBehaviour
{
    public float maxHP;
    public float currentHP;

    public float damage;
    public float currentdamage;

    public float cooldownMultiplier = 1f;

    public float maxSpeed;


    [SerializeField] private GameObject bloodParticle;
    [SerializeField] private GameObject PoofParticle;

    protected virtual void Start()
    {
        currentHP = maxHP;
        currentdamage = damage;
    }
    public void ChangeAttack(float multiplier = 1f)
    {
        if (multiplier != 1)
        {
            currentdamage *= multiplier;
        }
    }
    public void AttackNormal()
    {
        currentdamage = damage;
    }

    public void ChangeSpeed(float multiplier = 1f)
    {
        if (multiplier != 1)
        {
            GetComponent<PlayerCharacter>().walkSpeed *= multiplier;
        }
    }
    public void SpeedNormal()
    {
        GetComponent<PlayerCharacter>().walkSpeed = maxSpeed;
    }

    public void takeDamage(float damage)
    {
        currentHP -= damage;
        Instantiate(bloodParticle, transform.position, Quaternion.identity);
        if (currentHP <= 0)
        {
            Die();
        }
    }
    public void heal(float damage)
    {
        currentHP += damage;
        if (currentHP > maxHP) {
            currentHP = maxHP;
        }
    }

    public void Die()
    {
        Instantiate(PoofParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
