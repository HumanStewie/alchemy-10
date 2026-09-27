using UnityEngine;

public class JamToucher : EnemyBase
{
    Transform player;

    public float rotateFast = 5f;
    public float rotateLength = 4f;
    public float contactDamage = 5f;

    protected override void Start()
    {
        base.Start();
        var pc = FindFirstObjectByType<PlayerCharacter>();
        if (pc != null) player = pc.transform;
    }

    void Update()
    {
        if (player == null) return;

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        Vector3 forwardMove = transform.forward * currentspeed * Time.deltaTime;
        Vector3 sideWayMove = transform.right * Mathf.Sin(Time.time * rotateFast) * rotateLength * Time.deltaTime;
        transform.position += forwardMove + sideWayMove;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent<PlayerHealthAndStat>(out PlayerHealthAndStat playerHealth))
        {
            playerHealth.takeDamage(contactDamage > 0 ? contactDamage : currentdamage);
            if (BookMovement.Instance != null)
            {
                BookMovement.Instance.Disabler(4f);
            }
        }
    }
}