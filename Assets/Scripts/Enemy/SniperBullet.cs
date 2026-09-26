using UnityEngine;

public class SniperBullet : MonoBehaviour
{
    public float speed = 10f;

    void Start()
    {
        transform.LookAt(FindFirstObjectByType<PlayerCharacter>().transform);        
        
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerHealthAndStat>(out PlayerHealthAndStat player))
        {
            player.takeDamage(20);
        }
    }
}
