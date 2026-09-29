using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Enemies")]
    [HideInInspector] public GameObject simpleFollower;
    [HideInInspector] public GameObject shootingEnemy;
    [HideInInspector] public GameObject wallSpawner;
    [HideInInspector] public GameObject charger;
    [HideInInspector] public GameObject sniper;
    [HideInInspector] public GameObject jamToucher;
    [HideInInspector] public GameObject swarmEnemy;
    [HideInInspector] public GameObject Fatass;

    [Header("Wave State")]
    public int currentWave = 1;
    public float addedDifficulty = 40f;
    public bool started = false;
    public bool checking = false;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 15f;
    public Transform player;
    [SerializeField] private bool startChecking;

    [SerializeField] private GameObject SkillCanvas;
    [SerializeField] private GameObject GameCanvas;

    [HideInInspector] public GameObject burningEffect;
    [HideInInspector] public GameObject poisonEffect;
    [HideInInspector] public GameObject bloodEffect;
    [HideInInspector] public GameObject poofEffect;

    [SerializeField] private GameObject healthBar;

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

        swarmEnemy = simpleFollower;
    }

    void StartWave(int wave)
    {
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
        if (startChecking)
        {
            if (!FindFirstObjectByType<EnemyBase>())
            {
                if (SkillCanvas != null) SkillCanvas.SetActive(true);
                if (GameCanvas != null) GameCanvas.SetActive(false);
                startChecking = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            StartFirstWave();
        }

        healthBar.GetComponent<Image>().DOFillAmount(FindFirstObjectByType<PlayerHealthAndStat>().currentHP / FindFirstObjectByType<PlayerHealthAndStat>().maxHP, 0.1f);
    }

    public void GoNextWave()
    {
        currentWave++;
        if (GameCanvas != null) GameCanvas.SetActive(true);
        StartWave(currentWave);
        startChecking = true;
    }

    void Wave1()
    {
        float valueCost = 5f;
        while (valueCost > 0)
        {
            InstantiateEnemy(simpleFollower);
            valueCost -= 1f;
        }
    }

    void Wave2()
    {
        float valueCost = 8f;
        while (valueCost > 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < 60) { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
            else { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
        }
    }

    void Wave3()
    {
        float valueCost = 11f;
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
        float valueCost = 14f;
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
        float valueCost = 17f;
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
        float valueCost = 21f;
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
        float valueCost = 26f;
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
        float valueCost = 32f;
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
        float valueCost = 38f;
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
        float valueCost = 45f;

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

    public void StartFirstWave()
    {
        if (started) return;
        started = true;
        currentWave = 1;
        StartWave(currentWave);
        startChecking = true;
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

    void InstantiateEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null) return;

        Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = new Vector3(randomPoint.x, currentWave * 2, randomPoint.y);

        if (enemyPrefab == swarmEnemy)
        {
            SpawnSwarmAt(spawnPos);
        }
        else
        {
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    public void SpawnSwarmAt(Vector3 centerPosition)
    {
        if (simpleFollower == null) return;

        for (int i = 0; i < 5; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 1.5f;
            Vector3 spawnPos = centerPosition + new Vector3(offset.x, 0f, offset.y);

            GameObject mini = Instantiate(simpleFollower, spawnPos, Quaternion.identity);
            mini.transform.localScale = Vector3.one * 0.3f;

            if (mini.TryGetComponent<EnemyBase>(out var enemy))
            {
                enemy.maxHP = 1f;
                enemy.currentHP = 1f;
                enemy.damage = 3f;
            }
        }
    }
}