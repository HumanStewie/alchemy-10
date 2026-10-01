using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WallSpell", menuName = "Spells/WallSpell")]
public class WallSpell : SpellTemplate
{
    public GameObject wall;
    public float riseHeight = 2.75f;
    public float lifetime = 10f;
    [SerializeField] private LayerMask groundLayer;

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

        Vector3 spawnLoc = book.WallSpawnLoc != null
            ? book.WallSpawnLoc.position
            : player.transform.position + player.transform.forward * 2f;

        float groundY = spawnLoc.y;
        if (Physics.Raycast(spawnLoc + Vector3.up * 1.5f, Vector3.down, out RaycastHit hit, 10f, groundLayer))
        {
            groundY = hit.point.y;
        }

        Vector3 initialPos = new Vector3(spawnLoc.x, groundY - 1.5f, spawnLoc.z);
        float targetY = groundY + riseHeight;

        Quaternion spawnRot = Quaternion.Euler(-90f, player.transform.eulerAngles.y - 90f, 0f);

        GameObject newWallObj = Instantiate(wall, initialPos, spawnRot);
        newWallObj.transform.DOMoveY(targetY, 0.5f).SetEase(Ease.InOutSine);

        if (newWallObj.TryGetComponent<WallProp>(out var wallScript))
        {
            wallScript.Initialize(lifetime, contactDamage, sizeMultiplier, allowWallWalk, runeHeals);
            activeWalls.Enqueue(wallScript);
        }

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayCreateWallSound(spawnLoc);
        }
        CameraShake.Instance.ShakeLight();
    }

    private void PruneDestroyedWalls()
    {
        while (activeWalls.Count > 0 && activeWalls.Peek() == null)
        {
            activeWalls.Dequeue();
        }
    }
}