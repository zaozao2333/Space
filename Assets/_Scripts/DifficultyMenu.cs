using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultyMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public TextMeshProUGUI nameAndIdText;

    [Header("Difficulty Settings")]
    public Slider difficultySlider;
    public TextMeshProUGUI difficultyText;
    public Button startBtn;
    public TextMeshProUGUI startBtnText;
    public string[] difficultyNames = { "Easy", "Normal", "Hard" };

    void Start()
    {
        // 设置个人信息显示
        nameAndIdText.text = "谭宇凡\n学号: 2022150219";
        startBtnText.text = "Start Game";

        // 初始化难度选择
        difficultySlider.onValueChanged.AddListener(UpdateDifficultyText);
        startBtn.onClick.AddListener(StartGame);
        UpdateDifficultyText(difficultySlider.value);
    }

    void UpdateDifficultyText(float value)
    {
        int difficultyIndex = Mathf.FloorToInt(value);
        difficultyText.text = "Difficulty: " + difficultyNames[difficultyIndex];
    }

    public void StartGame()
    {
        int selectedDifficulty = Mathf.FloorToInt(difficultySlider.value);
        PlayerPrefs.SetInt("GameDifficulty", selectedDifficulty);
        mainMenuPanel.SetActive(false);
        // 这里可以开始游戏或加载游戏场景
        SceneManager.LoadScene("_Scene_0");
    }
}