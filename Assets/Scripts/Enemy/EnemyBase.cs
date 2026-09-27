using System.Collections;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
 
public abstract class EnemyBase : MonoBehaviour
{
    public float maxHP;
    public float currentHP;

    public float damage;
    public float currentdamage;

    public float speed;
    public float currentspeed;

    public float attackCooldown;

    [SerializeField] private GameObject bloodParticle;
    [SerializeField] private GameObject PoofParticle;
    [SerializeField] private GameObject poisonParticle;
    [SerializeField] private GameObject burnParticle;

    public bool isPoisoned = false;
    public bool isBurning;

    private Coroutine freezeCoroutine;


    protected virtual void Start()
    {
        currentHP = maxHP;
        currentdamage = damage;
        currentspeed = speed;
    }

    public void ChangeSpeed(float multiplier = 1f)
    {
        if (multiplier != 1)
        {
            currentspeed *= multiplier;
        }
    }
    public void SpeedNormal()
    {
        currentspeed = speed;
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

    public void Freeze(float duration)
    {
        if (freezeCoroutine != null) StopCoroutine(freezeCoroutine);
        freezeCoroutine = StartCoroutine(Freezing(duration));
    }

    private IEnumerator Freezing(float duration)
    {
        float originalSpeed = currentspeed;
        currentspeed = 0f; // Halt movement

        yield return new WaitForSeconds(duration);

        currentspeed = originalSpeed; // Restore speed
        freezeCoroutine = null;
    }
    public void Poisoned(float damage, float time)
    {
        if (isPoisoned) return;
        StartCoroutine(poisoning(damage, time));
    }

    IEnumerator poisoning(float damage, float time)
    {
        isPoisoned = true;

        float timer = 0;

        while (timer < time)
        {
            takeDamage(damage);
            timer += 1f;
            yield return new WaitForSeconds(1f);
        }
        isPoisoned = false;
    }
    public void Burnt(float damage, float time)
    {
        if (isBurning) return;
        StartCoroutine(burning(damage, time));
    }

    IEnumerator burning(float damage, float time)
    {
        isBurning = true;

        float timer = 0;

        while (timer < time)
        {
            takeDamage(damage);
            timer += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }
        isBurning = false;
    }
}
