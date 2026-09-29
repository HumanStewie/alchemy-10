using KinematicCharacterController;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalTeleporter : MonoBehaviour
{
    public Transform player;
    public Transform receiver;

    // make invisible/visible portals
    public MeshRenderer thisPortal;
    public MeshRenderer otherPortal;
    private bool playerIsOverlap = false;
    public bool leadsInside = true;
    private MeshRenderer[] toggleableRenderers;

    private static float teleportCooldown = 0f;

    void Start()
    {
        GameObject[] toggleableObjects = GameObject.FindGameObjectsWithTag("Toggleable");
        toggleableRenderers = new MeshRenderer[toggleableObjects.Length];
        for (int i = 0; i < toggleableObjects.Length; i++)
        {
            toggleableRenderers[i] = toggleableObjects[i].GetComponent<MeshRenderer>();
        }
    }

    void Update()
    {
        if (teleportCooldown > 0f)
        {
            teleportCooldown -= Time.deltaTime;
        }

        if (playerIsOverlap && teleportCooldown <= 0f)
        {
            teleportCooldown = 0.4f;

            
            Vector3 portalToPlayer = player.position - transform.position;
            float rotationDiff = -Quaternion.Angle(transform.rotation, receiver.rotation);
            rotationDiff += 180f;

            
            Quaternion targetRotation = Quaternion.Euler(0f, rotationDiff, 0f) * player.rotation;
            Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
            Vector3 targetPosition = receiver.position + positionOffset;

            
            KinematicCharacterMotor motor = player.GetComponent<KinematicCharacterMotor>();
            if (motor != null)
            {
                motor.SetPositionAndRotation(targetPosition, targetRotation);
            }
            else
            {
                
                player.SetPositionAndRotation(targetPosition, targetRotation);
            }

            if (thisPortal != null) thisPortal.enabled = false;
            if (otherPortal != null) otherPortal.enabled = true;

            foreach (MeshRenderer rend in toggleableRenderers)
            {
                if (rend != null)
                {
                    rend.shadowCastingMode = leadsInside ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player") && teleportCooldown <= 0f)
        {
            playerIsOverlap = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            playerIsOverlap = false;
        }
    }
}