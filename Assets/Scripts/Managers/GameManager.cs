using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject simpleFollower;
    public GameObject shootingEnemy;
    [HideInInspector] public GameObject wallSpawner;
    public GameObject charger;
    [HideInInspector] public GameObject sniper;
    [HideInInspector] public GameObject jamToucher;
    [HideInInspector] public GameObject swarmEnemy;
    public GameObject Fatass;

    public List<GameObject> InvisblesWall = new();

    public int currentWave = 1;
    public float addedDifficulty = 50f;
    public bool started = false;
    public bool checking = false;

    [SerializeField] private float floorHeightStep = 17f;
    public float yOffsetForBachMapForNoReason = 141.8f;
    [SerializeField] private LayerMask groundLayer;
    public Transform player;
    [SerializeField] private bool startChecking;

    [Header("Spawn Points")]
    [Tooltip("Assign the parent object for each floor's spawn points. Element 0 = Wave 1, Element 1 = Wave 2, etc.")]
    public List<Transform> floorSpawnParents = new();

    // Kept for Rune, but completely removed from Enemy spawning logic as requested
    [SerializeField] private float enemySpawnHeightOffset = 1.0f;

    [SerializeField] private GameObject runePrefab;
    [SerializeField] private Vector3 runeEntrancePosition = new Vector3(10f, 143f, -247f);

    [SerializeField] private GameObject SkillCanvas;
    [SerializeField] private GameObject GameCanvas;
    [SerializeField] private GameObject LoseCanvas;
    [SerializeField] private GameObject WinCanvas;
    [SerializeField] public GameObject healthBar;
    [SerializeField] private GameObject runeHealthbar;

    [Header("Enemy Tracking & UI")]
    [SerializeField] private TextMeshProUGUI enemyCountText;
    public int activeEnemyCount = 0;
    private Dictionary<EnemyBase, Vector3> spawnedEnemyOrigins = new Dictionary<EnemyBase, Vector3>();
    private List<EnemyBase> deadEnemiesBuffer = new List<EnemyBase>();

    private float lowEnemyTimer = 0f;
    private float waveClearGraceTimer = 0f;

    [HideInInspector] public GameObject burningEffect;
    [HideInInspector] public GameObject poisonEffect;
    [HideInInspector] public GameObject bloodEffect;
    [HideInInspector] public GameObject poofEffect;

    private bool waitingForPlayerToReachNextFloor = false;
    private int targetBarrierIndex = -1;

    private void Awake()
    {
        Instance = this;

        burningEffect = Resources.Load<GameObject>("Particle/burning");
        poisonEffect = Resources.Load<GameObject>("Particle/poison");
        bloodEffect = Resources.Load<GameObject>("Particle/blood");
        poofEffect = Resources.Load<GameObject>("Particle/poof");

        simpleFollower = Resources.Load<GameObject>("Enemy/Simple Follower");
        shootingEnemy = Resources.Load<GameObject>("Enemy/Butter Shooter");
        wallSpawner = Resources.Load<GameObject>("Enemy/Wall Spawner");
        sniper = Resources.Load<GameObject>("Enemy/Sniper");
        charger = Resources.Load<GameObject>("Enemy/Charger");
        jamToucher = Resources.Load<GameObject>("Enemy/Jam Toucher");
        Fatass = Resources.Load<GameObject>("Enemy/Fatass");

        swarmEnemy = new GameObject("Swarm_Marker_Dummy");
        swarmEnemy.transform.SetParent(this.transform);
        swarmEnemy.SetActive(false);
    }

    private void Start()
    {
        if (player == null)
        {
            var p = FindFirstObjectByType<PlayerCharacter>();
            if (p != null) player = p.transform;
        }

        for (int i = 0; i < InvisblesWall.Count; i++)
        {
            if (InvisblesWall[i] != null) InvisblesWall[i].SetActive(true);
        }
    }

    public float GetCurrentFloorY()
    {
        return (currentWave - 1) * floorHeightStep + yOffsetForBachMapForNoReason;
    }

    private float GetCurrentKillPlaneY()
    {
        if (floorSpawnParents != null && floorSpawnParents.Count > 0)
        {
            int floorIndex = Mathf.Clamp(currentWave - 1, 0, floorSpawnParents.Count - 1);
            Transform currentFloorParent = floorSpawnParents[floorIndex];

            if (currentFloorParent != null && currentFloorParent.childCount > 0)
            {
                // Made the kill plane extremely deep (-50f) so nothing accidentally touches it on spawn
                return currentFloorParent.GetChild(0).position.y - 50f;
            }
        }
        return GetCurrentFloorY() - 50f;
    }

    void StartWave(int wave)
    {
        spawnedEnemyOrigins.Clear();
        lowEnemyTimer = 0f;
        waveClearGraceTimer = 0f;

        switch (wave)
        {
            case 1: Wave1(); break;
            case 2: Wave2(); break;
            case 3: Wave3(); break;
            case 4: Wave4(); break;
            case 5: Wave5(); break;
            case 6: Wave6(); break;
            case 7: Wave7(); break;
            case 8: Wave8(); break;
            case 9: Wave9(); break;
            case 10: Wave10(); break;
            default: WaveEndless(); break;
        }
    }

    private void Update()
    {
        UpdateEnemyTrackingAndBounds();

        if (startChecking)
        {
            if (activeEnemyCount == 0)
            {
                waveClearGraceTimer += Time.deltaTime;
                if (waveClearGraceTimer >= 0.5f)
                {
                    if (SkillCanvas != null) SkillCanvas.SetActive(true);
                    if (GameCanvas != null) GameCanvas.SetActive(false);

                    startChecking = false;
                    waveClearGraceTimer = 0f;
                    OnWaveCleared();
                }
            }
            else
            {
                waveClearGraceTimer = 0f;
            }
        }

        if (waitingForPlayerToReachNextFloor && player != null && targetBarrierIndex >= 0 && targetBarrierIndex < InvisblesWall.Count)
        {
            GameObject wall = InvisblesWall[targetBarrierIndex];
            if (wall != null)
            {
                if (player.position.y >= wall.transform.position.y)
                {
                    waitingForPlayerToReachNextFloor = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            StartFirstWave();
        }

        var playerComp = FindFirstObjectByType<PlayerHealthAndStat>();
        if (playerComp != null && healthBar != null)
        {
            var img = healthBar.GetComponent<Image>();
            if (img != null && playerComp.maxHP > 0f)
                img.DOFillAmount(playerComp.currentHP / playerComp.maxHP, 0.1f);
        }

        var runeComp = FindFirstObjectByType<RuneManager>();
        if (runeComp != null && runeHealthbar != null)
        {
            var img = runeHealthbar.GetComponent<Image>();
            if (img != null && runeComp.maxHealth > 0f)
                img.DOFillAmount(runeComp.currentHealth / runeComp.maxHealth, 0.1f);
        }
    }

    private void UpdateEnemyTrackingAndBounds()
    {
        deadEnemiesBuffer.Clear();
        float killPlaneY = GetCurrentKillPlaneY();

        foreach (var kvp in spawnedEnemyOrigins)
        {
            EnemyBase enemy = kvp.Key;

            if (enemy == null || !enemy.gameObject.activeInHierarchy)
            {
                deadEnemiesBuffer.Add(enemy);
                continue;
            }

            if (enemy.transform.position.y < killPlaneY)
            {
                Destroy(enemy.gameObject);
                deadEnemiesBuffer.Add(enemy);
            }
        }

        for (int i = 0; i < deadEnemiesBuffer.Count; i++)
        {
            spawnedEnemyOrigins.Remove(deadEnemiesBuffer[i]);
        }

        EnemyBase[] allActiveEnemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

        activeEnemyCount = 0;
        foreach (var enemy in allActiveEnemies)
        {
            if (enemy != null && !deadEnemiesBuffer.Contains(enemy))
            {
                activeEnemyCount++;
            }
        }

        if (enemyCountText != null)
        {
            enemyCountText.text = $"Enemies Left: {activeEnemyCount}";
        }

        if (startChecking && activeEnemyCount > 0 && activeEnemyCount < 5)
        {
            lowEnemyTimer += Time.deltaTime;

            if (lowEnemyTimer >= 30f)
            {
                foreach (EnemyBase enemy in allActiveEnemies)
                {
                    if (enemy != null && !deadEnemiesBuffer.Contains(enemy))
                    {
                        Destroy(enemy.gameObject);
                    }
                }
                lowEnemyTimer = 0f;
                activeEnemyCount = 0;
            }
        }
        else
        {
            lowEnemyTimer = 0f;
        }
    }

    private void OnWaveCleared()
    {
        if (currentWave == 10)
        {
            Win();
            return;
        }

        int nextWallIndex = currentWave - 1;

        if (nextWallIndex >= 0 && nextWallIndex < InvisblesWall.Count)
        {
            if (InvisblesWall[nextWallIndex] != null)
            {
                InvisblesWall[nextWallIndex].SetActive(false);
            }

            targetBarrierIndex = nextWallIndex;
            waitingForPlayerToReachNextFloor = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void GoNextWave()
    {
        if (currentWave == 10)
        {
            Win();
            return;
        }

        currentWave++;

        if (GameCanvas != null) GameCanvas.SetActive(true);
        if (SkillCanvas != null) SkillCanvas.SetActive(false);

        UpdateFloorAccess();

        StartWave(currentWave);

        RuneManager rune = FindFirstObjectByType<RuneManager>();
        if (rune != null)
        {
            rune.goNextWave(GetCurrentFloorY());
        }

        startChecking = true;
    }

    private void UpdateFloorAccess()
    {
        for (int i = 0; i < InvisblesWall.Count; i++)
        {
            if (InvisblesWall[i] == null) continue;

            if (i == currentWave - 2)
            {
                InvisblesWall[i].SetActive(false);
            }
            else
            {
                InvisblesWall[i].SetActive(true);
            }
        }
    }

    public void StartFirstWave()
    {
        if (started) return;
        started = true;
        currentWave = 1;
        StartWave(currentWave);
        startChecking = true;
    }

    public RuneManager SpawnRuneAtEntrance(bool autoStartWave = true)
    {
        RuneManager activeRune = FindFirstObjectByType<RuneManager>();

        if (activeRune == null)
        {
            if (runePrefab == null)
            {
                runePrefab = Resources.Load<GameObject>("Rune");
            }

            if (runePrefab != null)
            {
                GameObject runeObj = Instantiate(runePrefab, runeEntrancePosition, Quaternion.identity);
                activeRune = runeObj.GetComponent<RuneManager>();
            }
            else
            {
                return null;
            }
        }
        else
        {
            activeRune.transform.position = runeEntrancePosition;
        }

        if (Physics.Raycast(runeEntrancePosition + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f, groundLayer))
        {
            activeRune.transform.position = hit.point;
        }

        activeRune.currentHealth = activeRune.maxHealth;
        activeRune.InitializeRune();

        if (runeHealthbar != null)
        {
            var img = runeHealthbar.GetComponent<Image>();
            if (img != null) img.fillAmount = 1f;
        }

        if (autoStartWave && !started)
        {
            StartFirstWave();
        }

        return activeRune;
    }

    // COMPLETELY REMOVED RAYCASTS & OFFSETS. Just grabs the exact world position.
    private Vector3 GetRandomSpawnPosition()
    {
        if (floorSpawnParents == null || floorSpawnParents.Count == 0)
        {
            Debug.LogError("No floor spawn parents assigned in GameManager! Returning Vector3.zero.");
            return Vector3.zero;
        }

        int floorIndex = Mathf.Clamp(currentWave - 1, 0, floorSpawnParents.Count - 1);
        Transform currentFloorParent = floorSpawnParents[floorIndex];

        if (currentFloorParent == null || currentFloorParent.childCount == 0)
        {
            Debug.LogWarning($"Floor spawn parent at index {floorIndex} is empty! Returning parent position.");
            return currentFloorParent != null ? currentFloorParent.position : Vector3.zero;
        }

        int randomIndex = Random.Range(0, currentFloorParent.childCount);
        Transform randomPoint = currentFloorParent.GetChild(randomIndex);

        // Return EXACT position of the chosen transform point
        return randomPoint.position;
    }

    void InstantiateEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null) return;

        Vector3 spawnPos = GetRandomSpawnPosition();

        if (enemyPrefab == swarmEnemy)
        {
            SpawnSwarmAt(spawnPos);
        }
        else
        {
            GameObject obj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            // FAILSAFE: Force the object to be active in case the prefab was saved as inactive
            obj.SetActive(true);

            if (obj.TryGetComponent<EnemyBase>(out var enemy))
            {
                spawnedEnemyOrigins[enemy] = spawnPos;
            }
        }
    }

    public void SpawnSwarmAt(Vector3 centerPosition)
    {
        if (simpleFollower == null) return;

        for (int i = 0; i < 5; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 1.5f;

            // EXACT center position + flat horizontal spread. No raycasts down.
            Vector3 swarmPos = centerPosition + new Vector3(offset.x, 0f, offset.y);

            GameObject mini = Instantiate(simpleFollower, swarmPos, Quaternion.identity);

            // FAILSAFE
            mini.SetActive(true);
            mini.transform.localScale = Vector3.one * 0.3f;

            if (mini.TryGetComponent<EnemyBase>(out var enemy))
            {
                enemy.maxHP = 1f;
                enemy.currentHP = 1f;
                enemy.damage = 3f;
                spawnedEnemyOrigins[enemy] = swarmPos;
            }
        }
    }

    public void SlowEVERYTHING(int time, int percentage)
    {
        StartCoroutine(slowStuff(time, percentage));
    }

    IEnumerator slowStuff(int time, int percentage)
    {
        Time.timeScale *= (100 - percentage) / 100f;
        yield return new WaitForSeconds(time);
        Time.timeScale = 1f;
    }

    public void Lose()
    {
        if (LoseCanvas != null)
        {
            LoseCanvas.SetActive(true);
            EventTrigger trigger = LoseCanvas.GetComponent<EventTrigger>();
            if (trigger == null) trigger = LoseCanvas.AddComponent<EventTrigger>();

            trigger.triggers.Clear();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => SceneManager.LoadScene("MainMenu"));
            trigger.triggers.Add(entry);
        }

        if (GameCanvas != null) GameCanvas.SetActive(false);
    }

    public void Win()
    {
        if (WinCanvas != null)
        {
            WinCanvas.SetActive(true);
            EventTrigger trigger = WinCanvas.GetComponent<EventTrigger>();
            if (trigger == null) trigger = WinCanvas.AddComponent<EventTrigger>();

            trigger.triggers.Clear();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => SceneManager.LoadScene("MainMenu"));
            trigger.triggers.Add(entry);
        }

        if (GameCanvas != null) GameCanvas.SetActive(false);
    }

    #region Wave Setups
    void Wave1()
    {
        float valueCost = 15f;
        while (valueCost > 0)
        {
            InstantiateEnemy(simpleFollower);
            valueCost -= 1f;
        }
    }

    void Wave2()
    {
        float valueCost = 18f;
        while (valueCost > 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < 60) { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
            else { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
        }
    }

    void Wave3()
    {
        float valueCost = 21f;
        while (valueCost > 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < 50) { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
            else if (roll < 80) { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
            else { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
        }
    }

    void Wave4()
    {
        float valueCost = 24f;
        while (valueCost > 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < 40) { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
            else if (roll < 65) { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
            else if (roll < 85) { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
            else { InstantiateEnemy(charger); valueCost -= 3f; }
        }
    }

    void Wave5()
    {
        float valueCost = 27f;
        InstantiateEnemy(Fatass);
        valueCost -= 4f;

        while (valueCost > 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < 35) { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
            else if (roll < 55) { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
            else if (roll < 70) { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
            else if (roll < 85) { InstantiateEnemy(charger); valueCost -= 3f; }
            else { InstantiateEnemy(sniper); valueCost -= 3f; }
        }
    }

    void Wave6()
    {
        float valueCost = 31f;
        while (valueCost > 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < 25) { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
            else if (roll < 40) { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
            else if (roll < 55) { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
            else if (roll < 70) { InstantiateEnemy(charger); valueCost -= 3f; }
            else if (roll < 80) { InstantiateEnemy(sniper); valueCost -= 3f; }
            else if (roll < 90) { InstantiateEnemy(jamToucher); valueCost -= 2.5f; }
            else { InstantiateEnemy(Fatass); valueCost -= 4f; }
        }
    }

    void Wave7()
    {
        float valueCost = 36f;
        while (valueCost > 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < 20) { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
            else if (roll < 35) { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
            else if (roll < 50) { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
            else if (roll < 65) { InstantiateEnemy(charger); valueCost -= 3f; }
            else if (roll < 75) { InstantiateEnemy(sniper); valueCost -= 3f; }
            else if (roll < 85) { InstantiateEnemy(jamToucher); valueCost -= 2.5f; }
            else if (roll < 95) { InstantiateEnemy(swarmEnemy); valueCost -= 3f; }
            else { InstantiateEnemy(Fatass); valueCost -= 4f; }
        }
    }

    void Wave8()
    {
        float valueCost = 42f;
        while (valueCost > 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < 10) { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
            else if (roll < 25) { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
            else if (roll < 35) { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
            else if (roll < 50) { InstantiateEnemy(charger); valueCost -= 3f; }
            else if (roll < 65) { InstantiateEnemy(sniper); valueCost -= 3f; }
            else if (roll < 75) { InstantiateEnemy(jamToucher); valueCost -= 2.5f; }
            else if (roll < 90) { InstantiateEnemy(swarmEnemy); valueCost -= 3f; }
            else { InstantiateEnemy(Fatass); valueCost -= 4f; }
        }
    }

    void Wave9()
    {
        float valueCost = 48f;
        while (valueCost > 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < 10) { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
            else if (roll < 20) { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
            else if (roll < 35) { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
            else if (roll < 45) { InstantiateEnemy(charger); valueCost -= 3f; }
            else if (roll < 55) { InstantiateEnemy(sniper); valueCost -= 3f; }
            else if (roll < 70) { InstantiateEnemy(jamToucher); valueCost -= 2.5f; }
            else if (roll < 85) { InstantiateEnemy(swarmEnemy); valueCost -= 3f; }
            else { InstantiateEnemy(Fatass); valueCost -= 4f; }
        }
    }

    void Wave10()
    {
        float valueCost = 55f;

        InstantiateEnemy(Fatass);
        InstantiateEnemy(Fatass);
        InstantiateEnemy(swarmEnemy);
        InstantiateEnemy(swarmEnemy);
        valueCost -= 14f;

        while (valueCost > 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < 10) { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
            else if (roll < 20) { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
            else if (roll < 35) { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
            else if (roll < 50) { InstantiateEnemy(charger); valueCost -= 3f; }
            else if (roll < 65) { InstantiateEnemy(sniper); valueCost -= 3f; }
            else if (roll < 75) { InstantiateEnemy(jamToucher); valueCost -= 2.5f; }
            else if (roll < 90) { InstantiateEnemy(swarmEnemy); valueCost -= 3f; }
            else { InstantiateEnemy(Fatass); valueCost -= 4f; }
        }
    }

    void WaveEndless()
    {
        float valueCost = addedDifficulty;
        addedDifficulty += 6f;

        int themeRoll = Random.Range(0, 5);

        while (valueCost > 0)
        {
            int roll = Random.Range(0, 100);

            if (themeRoll == 0)
            {
                if (roll < 30) { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
                else if (roll < 60) { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
                else { InstantiateEnemy(swarmEnemy); valueCost -= 3f; }
            }
            else if (themeRoll == 1)
            {
                if (roll < 45) { InstantiateEnemy(charger); valueCost -= 3f; }
                else if (roll < 80) { InstantiateEnemy(sniper); valueCost -= 3f; }
                else { InstantiateEnemy(Fatass); valueCost -= 4f; }
            }
            else if (themeRoll == 2)
            {
                if (roll < 40) { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
                else if (roll < 70) { InstantiateEnemy(sniper); valueCost -= 3f; }
                else { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
            }
            else if (themeRoll == 3)
            {
                if (roll < 40) { InstantiateEnemy(jamToucher); valueCost -= 2.5f; }
                else if (roll < 75) { InstantiateEnemy(swarmEnemy); valueCost -= 3f; }
                else { InstantiateEnemy(Fatass); valueCost -= 4f; }
            }
            else
            {
                if (roll < 40) { InstantiateEnemy(Fatass); valueCost -= 4f; }
                else if (roll < 70) { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
                else { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
            }
        }
    }
    #endregion
}