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
        // ���ø�����Ϣ��ʾ
        nameAndIdText.text = "̷�\nѧ��: 2022150219";
        startBtnText.text = "Start Game";

        // ��ʼ���Ѷ�ѡ��
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
        // ������Կ�ʼ��Ϸ�������Ϸ����
        SceneManager.LoadScene("_Scene_0");
    }
}