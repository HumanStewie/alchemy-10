using UnityEngine;

public class JamToucher : EnemyBase
{
    Transform player;

    public float rotateFast = 5;
    public float rotateLength = 4;
    protected override void Start()
    {
        base.Start();
        player = FindFirstObjectByType<PlayerCharacter>().transform;
    }

    void Update()
    {
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        Vector3 forwardMove = transform.forward * currentspeed * Time.deltaTime;

        Vector3 sideWayMove = transform.right * Mathf.Sin(Time.deltaTime * rotateFast) * rotateLength * Time.deltaTime;
        transform.position += forwardMove + sideWayMove;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent<PlayerHealthAndStat>(out PlayerHealthAndStat player))
        {
            BookMovement.Instance.Disabler(4);
        }
    }



}
