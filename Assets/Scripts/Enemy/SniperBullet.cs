using UnityEngine;

public class SniperBullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    public float damage = 20f;

    void Start()
    {
        var pc = FindFirstObjectByType<PlayerCharacter>();
        if (pc != null)
        {
            transform.LookAt(pc.transform);
        }
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerHealthAndStat>(out PlayerHealthAndStat player))
        {
            player.takeDamage(damage);
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}