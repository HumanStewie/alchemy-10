using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Enemies")]
    public GameObject simpleFollower;
    public GameObject shootingEnemy;
    public GameObject wallSpawner;
    public GameObject charger;
    public GameObject sniper;
    public GameObject jamToucher;
    public GameObject swarmEnemy;

    [Header("Wave State")]
    public int currentWave = 1;
    public float addedDifficulty = 40f;
    public bool started = false;
    public bool checking = false;

    [Header("Spawn Settings")]
    public float spawnRadius = 15f;
    public Transform player;
    public bool startChecking;

    public GameObject SkillPanel;

    private void Awake()
    {
        Instance = this;
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
                SkillPanel.SetActive(true);
                startChecking = false;
            }
        }
    }

    public void GoNextWave()
    {
        currentWave++;
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
        float valueCost = 20f;
        while (valueCost > 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < 30) { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
            else if (roll < 45) { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
            else if (roll < 60) { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
            else if (roll < 75) { InstantiateEnemy(charger); valueCost -= 3f; }
            else if (roll < 90) { InstantiateEnemy(sniper); valueCost -= 3f; }
            else { InstantiateEnemy(jamToucher); valueCost -= 2.5f; }
        }
    }

    void Wave7()
    {
        float valueCost = 25f;
        while (valueCost > 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < 20) { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
            else if (roll < 40) { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
            else if (roll < 55) { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
            else if (roll < 70) { InstantiateEnemy(charger); valueCost -= 3f; }
            else if (roll < 85) { InstantiateEnemy(sniper); valueCost -= 3f; }
            else if (roll < 95) { InstantiateEnemy(jamToucher); valueCost -= 2.5f; }
            else { InstantiateEnemy(swarmEnemy); valueCost -= 3f; }
        }
    }

    void Wave8()
    {
        float valueCost = 30f;
        while (valueCost > 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < 10) { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
            else if (roll < 30) { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
            else if (roll < 40) { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
            else if (roll < 60) { InstantiateEnemy(charger); valueCost -= 3f; }
            else if (roll < 80) { InstantiateEnemy(sniper); valueCost -= 3f; }
            else if (roll < 90) { InstantiateEnemy(jamToucher); valueCost -= 2.5f; }
            else { InstantiateEnemy(swarmEnemy); valueCost -= 3f; }
        }
    }

    void Wave9()
    {
        float valueCost = 35f;
        while (valueCost > 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < 15) { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
            else if (roll < 25) { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
            else if (roll < 45) { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
            else if (roll < 55) { InstantiateEnemy(charger); valueCost -= 3f; }
            else if (roll < 65) { InstantiateEnemy(sniper); valueCost -= 3f; }
            else if (roll < 80) { InstantiateEnemy(jamToucher); valueCost -= 2.5f; }
            else { InstantiateEnemy(swarmEnemy); valueCost -= 3f; }
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
        Time.timeScale *= (100 - percentage)/100;
        yield return new WaitForSeconds(time);
        Time.timeScale = 1f;
    }

    void Wave10()
    {
        float valueCost = 40f;

        InstantiateEnemy(swarmEnemy);
        InstantiateEnemy(swarmEnemy);
        valueCost -= 6f;

        while (valueCost > 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < 10) { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
            else if (roll < 25) { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
            else if (roll < 40) { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
            else if (roll < 55) { InstantiateEnemy(charger); valueCost -= 3f; }
            else if (roll < 70) { InstantiateEnemy(sniper); valueCost -= 3f; }
            else if (roll < 85) { InstantiateEnemy(jamToucher); valueCost -= 2.5f; }
            else { InstantiateEnemy(swarmEnemy); valueCost -= 3f; }
        }
    }

    void WaveEndless()
    {
        float valueCost = addedDifficulty;
        addedDifficulty += 5f; 

        int themeRoll = Random.Range(0, 4); 

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
                if (roll < 50) { InstantiateEnemy(charger); valueCost -= 3f; }
                else { InstantiateEnemy(sniper); valueCost -= 3f; }
            }
            else if (themeRoll == 2)
            {
                if (roll < 40) { InstantiateEnemy(shootingEnemy); valueCost -= 1.5f; }
                else if (roll < 70) { InstantiateEnemy(sniper); valueCost -= 3f; }
                else { InstantiateEnemy(wallSpawner); valueCost -= 2f; }
            }
            else 
            {
                if (roll < 40) { InstantiateEnemy(jamToucher); valueCost -= 2.5f; }
                else if (roll < 80) { InstantiateEnemy(swarmEnemy); valueCost -= 3f; }
                else { InstantiateEnemy(simpleFollower); valueCost -= 1f; }
            }
        }
    }

    void InstantiateEnemy(GameObject enemyPrefab)
    {
        Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;

        Vector3 spawnPos = new Vector3(randomPoint.x, currentWave * 2, randomPoint.y);


        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}