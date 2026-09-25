using UnityEngine;

public class PlayerHealthAndStat : MonoBehaviour
{
    public float maxHP;
    public float currentHP;

    public float damage;
    public float currentdamage;

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

    public void takeDamage(float damage)
    {
        currentHP -= damage;
        Instantiate(bloodParticle, transform.position, Quaternion.identity);
        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Instantiate(PoofParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
