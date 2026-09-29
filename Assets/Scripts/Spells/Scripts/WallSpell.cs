using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WallSpell", menuName = "Spells/WallSpell")]
public class WallSpell : SpellTemplate
{
    [Header("Prefab & Position")]
    public GameObject wall;
    public float yFinal = 2.75f;
    public float lifetime = 10f;

    [Header("Base & Upgrade Stats")]
    public int maxWalls = 1;
    public float contactDamage = 0f;
    public float sizeMultiplier = 1f;
    public bool allowWallWalk = false;
    public bool runeHeals = false;

    private Queue<WallProp> activeWalls = new();

    

    public override void Cast(GameObject caster, Vector3 targetPoint, float scale)
    {
        BookMovement book = Object.FindAnyObjectByType<BookMovement>();
        PlayerCharacter player = Object.FindAnyObjectByType<PlayerCharacter>();

        if (book == null || player == null || wall == null) return;

        // Spawns exactly when the hand completes its arc and slams down to place the jam
        book.WallSpellAnimation(() =>
        {
            SpawnWallInstance(book, player);
        });
    }

    private void SpawnWallInstance(BookMovement book, PlayerCharacter player)
    {
        PruneDestroyedWalls();

        while (activeWalls.Count >= maxWalls)
        {
            WallProp oldest = activeWalls.Dequeue();
            if (oldest != null)
            {
                oldest.DespawnWall();
            }
        }

        Vector3 spawnLoc = book.WallSpawnLoc != null ? book.WallSpawnLoc.position : player.transform.position + player.transform.forward * 2f;
        Quaternion spawnRot = Quaternion.Euler(-90f, player.transform.eulerAngles.y - 90f, 0f);

        GameObject newWallObj = Instantiate(wall, spawnLoc, spawnRot);
        newWallObj.transform.DOMoveY(yFinal, 0.5f).SetEase(Ease.InOutSine);

        if (newWallObj.TryGetComponent<WallProp>(out var wallScript))
        {
            wallScript.Initialize(lifetime, contactDamage, sizeMultiplier, allowWallWalk, runeHeals);
            activeWalls.Enqueue(wallScript);
        }

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayCreateWallSound(spawnLoc);
        }
    }

    private void PruneDestroyedWalls()
    {
        while (activeWalls.Count > 0 && activeWalls.Peek() == null)
        {
            activeWalls.Dequeue();
        }
    }
}