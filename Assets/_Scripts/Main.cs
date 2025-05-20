using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;
    public bool bossSpawn;

    [Header("Score Settings")]
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;
    private int currentScore = 0;
    private int highScore = 0;

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies;
    public GameObject boss;
    public float enemySpawnPerSecond = 0.5f;
    public float enemySpawnPadding = 1.5f;
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[]
    {
        WeaponType.blaster, WeaponType.blaster,
        WeaponType.spread,
        WeaponType.shield,
        WeaponType.laser, WeaponType.laser
    };

    [Header("Progress Bar Settings")]
    public Slider progressBar; // ������Slider���
    public int totalEnemiesToDefeat = 20; // ��Ҫ���ܵĵ�������
    private int enemiesDefeated = 0; // �ѻ��ܵĵ�������

    private BoundsCheck bndCheck;

    public void ShipDestoryed(Enemy e)
    {
        enemiesDefeated++;
        currentScore += e.score;
        UpdateScoreDisplay();
        print(enemiesDefeated);
        UpdateProgressBar();
        if(Random.value < e.powerUpDropChance)
        {
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];

            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp powerUp = go.GetComponent<PowerUp>();
            powerUp.SetType(puType);
            powerUp.transform.position = e.transform.position;
        }
    }

    void UpdateScoreDisplay()
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = "Score: " + currentScore.ToString();
        }

        // ��鲢������߷�
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            if (highScoreText != null)
            {
                highScoreText.text = "High Score: " + highScore.ToString();
            }
        }
    }

    void UpdateProgressBar()
    {
        if (progressBar != null)
        {
            float value = (float)enemiesDefeated / totalEnemiesToDefeat;
            if (value >= 1 && !bossSpawn) { 
                SpawnBoss();
                bossSpawn = true;
            }
            progressBar.value = Mathf.Clamp01(value);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach(WeaponDefinition definition in weaponDefinitions)
        {
            WEAP_DICT[definition.type] = definition;
        }
        // ��ʼ��������
        if (progressBar != null)
        {
            progressBar.minValue = 0;
            progressBar.maxValue = 1;
            progressBar.value = 0;
        }
        bossSpawn = false;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore.ToString();
        UpdateScoreDisplay();
        // �����Ѷ�������Ϸ����
        int difficulty = PlayerPrefs.GetInt("GameDifficulty", 1); // Ĭ����ͨ�Ѷ�
        switch (difficulty)
        {
            case 0: // ��
                enemySpawnPerSecond = 0.3f;
                totalEnemiesToDefeat = 15;
                break;
            case 1: // ��ͨ
                enemySpawnPerSecond = 0.5f;
                totalEnemiesToDefeat = 20;
                break;
            case 2: // ����
                enemySpawnPerSecond = 0.8f;
                totalEnemiesToDefeat = 30;
                break;
        }
    }

    public void SpawnEnemy()
    {
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        float enemyPadding = enemySpawnPadding;
        if(go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    public void SpawnBoss()
    {
        GameObject go = Instantiate<GameObject>(boss);
        float enemyPadding = enemySpawnPadding;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }
        Vector3 pos = Vector3.zero;
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;
    }

    public void DelayedRestart(float delay)
    {
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        SceneManager.LoadScene("_Scene_0");
    }

    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        if (WEAP_DICT.ContainsKey(wt))
        {
            return WEAP_DICT[wt];
        }
        return new WeaponDefinition();
    }
}
