using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorRuneTrigger : MonoBehaviour
{
    [SerializeField] private bool autoStartWave = true;
    [SerializeField] private GameObject doorToCloseAfterEntering;

    private bool _hasTriggered = false;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;

        if (other.CompareTag("Player") || other.GetComponentInParent<PlayerCharacter>() != null)
        {
            _hasTriggered = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SpawnRuneAtEntrance(autoStartWave);
            }

            if (doorToCloseAfterEntering != null)
            {
                doorToCloseAfterEntering.SetActive(true);
            }

            gameObject.SetActive(false);
        }
    }
}