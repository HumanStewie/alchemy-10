using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneManager : MonoBehaviour
{
    [Header("Health")]
    public float currentHealth;
    public float maxHealth = 100f;

    [Header("Visuals")]
    public Transform arrowVisual; // Kept so your prefab reference doesn't break in the inspector

    private void Awake()
    {
        if (maxHealth <= 0f) maxHealth = 100f;
        currentHealth = maxHealth;

        // Turn off the arrow since we are stationary now
        if (arrowVisual != null) arrowVisual.gameObject.SetActive(false);
    }

    public void InitializeRune()
    {
        // Randomize location for Wave 1
        TeleportToRandomSpawnPoint();
    }

    private void Update()
    {
        // Death check
        if (currentHealth <= 0f)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Lose();
            }
        }
    }

    // Called by the GameManager when a wave is cleared and the player moves up
    public void goNextWave(float floorY = 0f)
    {
        TeleportToRandomSpawnPoint();
    }

    private void TeleportToRandomSpawnPoint()
    {
        if (GameManager.Instance == null || GameManager.Instance.floorSpawnParents == null || GameManager.Instance.floorSpawnParents.Count == 0)
        {
            Debug.LogWarning("RuneManager: Cannot teleport, GameManager or floorSpawnParents is missing.");
            return;
        }

        // Figure out which floor we are currently on
        int floorIndex = Mathf.Clamp(GameManager.Instance.currentWave - 1, 0, GameManager.Instance.floorSpawnParents.Count - 1);
        Transform currentFloorParent = GameManager.Instance.floorSpawnParents[floorIndex];

        if (currentFloorParent == null || currentFloorParent.childCount == 0)
        {
            Debug.LogWarning($"RuneManager: Spawn parent for floor {floorIndex} is empty.");
            return;
        }

        // Pick a random exact spawn point from the children of the current floor's parent
        int randomIndex = Random.Range(0, currentFloorParent.childCount);
        Transform randomPoint = currentFloorParent.GetChild(randomIndex);

        // Instantly teleport the Rune to the spawn point
        transform.position = randomPoint.position;
    }
}