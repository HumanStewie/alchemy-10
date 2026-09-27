using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float hitRadius = 0.8f;
    public int damage = 5;
    public float lifetime = 5f;

    public LayerMask layermask;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.forward, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out EnemyBase enemy))
        {
            enemy.takeDamage(damage);
            Destroy(gameObject);
        }
        else if (!other.isTrigger && !other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}