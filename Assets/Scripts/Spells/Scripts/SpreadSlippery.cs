using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WallSlope", menuName = "Spells/WallSlope")]
public class WallSlope : SpellTemplate
{
    [Header("Prefab & Position")]
    public GameObject slopePrefab;         
    public float yFinal = 0.1f;            
    public float lifetime = 12f;
    public float spawnForwardOffset = 2.5f; 

    [Header("Base & Upgrade Stats")]
    public int maxSlopes = 1;
    public float sizeMultiplier = 1f;
    public bool allowWallWalk = true;  

    private Queue<WallProp> activeSlopes = new();

    public override void Cast(GameObject caster, Vector3 targetPoint, float scale)
    {
        BookMovement book = Object.FindAnyObjectByType<BookMovement>();
        PlayerCharacter player = Object.FindAnyObjectByType<PlayerCharacter>();

        if (book == null || player == null || slopePrefab == null) return;

        // Same animation timing as WallSpell
        book.WallSpellAnimation(() =>
        {
            SpawnSlopeInstance(book, player);
        });
    }

    private void SpawnSlopeInstance(BookMovement book, PlayerCharacter player)
    {
        PruneDestroyedSlopes();

        // Remove oldest if we are at the limit
        while (activeSlopes.Count >= maxSlopes)
        {
            WallProp oldest = activeSlopes.Dequeue();
            if (oldest != null)
                oldest.DespawnWall();
        }

        Vector3 spawnLoc;
        if (book.WallSpawnLoc != null)
            spawnLoc = book.WallSpawnLoc.position;
        else
            spawnLoc = player.transform.position + player.transform.forward * spawnForwardOffset;

        Quaternion spawnRot = Quaternion.Euler(0, player.transform.eulerAngles.y - 180f, 0f);

        GameObject newSlopeObj = Instantiate(slopePrefab, spawnLoc, spawnRot);

        newSlopeObj.transform.DOMoveY(spawnLoc.y + yFinal, 0.45f).SetEase(Ease.OutSine);

        if (newSlopeObj.TryGetComponent<WallProp>(out var slopeScript))
        {
            slopeScript.Initialize(lifetime, 0f, sizeMultiplier, allowWallWalk, false);
            activeSlopes.Enqueue(slopeScript);
        }

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayCreateWallSound(spawnLoc);
    }

    private void PruneDestroyedSlopes()
    {
        while (activeSlopes.Count > 0 && activeSlopes.Peek() == null)
            activeSlopes.Dequeue();
    }
}