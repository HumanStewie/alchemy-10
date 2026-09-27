using UnityEngine;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class ProjectileGetter : MonoBehaviour
{
    public GameObject trapPrefab;
    [Header("Impact Quad Prefab")]
    [SerializeField] private float surfaceOffset = 0.005f;

    private Vector3 velocity;
    private bool hasHit = false;

    public void Launch(Vector3 direction, float speed)
    {
        velocity = direction.normalized * speed;
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (hasHit) return;

        velocity += Physics.gravity * 0.3f * Time.deltaTime;
        float moveStep = velocity.magnitude * Time.deltaTime;
        Ray ray = new Ray(transform.position, velocity.normalized);

        if (Physics.Raycast(ray, out RaycastHit hit, moveStep))
        {
            HitSurface(hit);
        }
        else
        {
            transform.position += velocity * Time.deltaTime;
        }
    }

    private void HitSurface(RaycastHit hit)
    {
        hasHit = true;

        Quaternion splatRotation = Quaternion.LookRotation(-hit.normal, Vector3.right);
        Vector3 splatPos = hit.point + (hit.normal * surfaceOffset);

        GameObject splat = Instantiate(trapPrefab, splatPos, splatRotation);
        splat.transform.SetParent(hit.collider.transform);

        if (splat.TryGetComponent<JamSpread>(out var splatScript))
        {
            splatScript.InitSplat();
        }
        Destroy(gameObject);

    }

}

