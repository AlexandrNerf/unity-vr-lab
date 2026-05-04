using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Builder : MonoBehaviour
{
    [Header("Blocks & Spawn")]
    [SerializeField] private GameObject[] blocks;
    [SerializeField] private Transform blockSpawnpoint;

    [Header("UI Elements")]
    [SerializeField] private GameObject blockMenuPanel;
    [SerializeField] private GameObject difficultyPanel;
    [SerializeField] private GameObject mainScreenPanel;
    [SerializeField] private TextMeshProUGUI schemeText;
    [SerializeField] private Transform schemeContainer;
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject loseText;
    [SerializeField] private Button restartButton;

    [Header("Block UI for Scheme")]
    [SerializeField] private Texture2D[] blockTextures;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;

    private string targetBuilding = "";
    private string currentBuilding = "";
    private int maxBlocks = 0;

    private float timeLeft;
    private bool isTimerRunning = false;

    private float totalBuildTime = 0f;
    private float currentRunTime = 0f;

    void Start()
    {
        blockMenuPanel.SetActive(false);
        winText.SetActive(false);
        loseText.SetActive(false);
        restartButton.gameObject.SetActive(false);

        difficultyPanel.SetActive(true);
        mainScreenPanel.SetActive(true);
        schemeText.text = "";

        RenderScheme(targetBuilding);
    }

    public void StartGame(int difficulty)
    {
        currentRunTime = 0f;
        maxBlocks = difficulty;
        targetBuilding = GenerateBuilding(maxBlocks);
        currentBuilding = "";

        Debug.Log("TARGET: " + targetBuilding);

        difficultyPanel.SetActive(false);
        mainScreenPanel.SetActive(false);
        blockMenuPanel.SetActive(true);

        winText.SetActive(false);
        loseText.SetActive(false);
        restartButton.gameObject.SetActive(false);
        schemeContainer.gameObject.SetActive(true);

        schemeText.text = "Схема: " + targetBuilding;
        RenderScheme(targetBuilding);

        timeLeft = maxBlocks * 10f;
        isTimerRunning = true;
        UpdateTimerUI();
    }

    void Update()
    {
        if (!isTimerRunning) return;

        timeLeft -= Time.deltaTime;
        currentRunTime += Time.deltaTime; // 👈 ВАЖНО

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            isTimerRunning = false;
            OnTimeEnd();
        }

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        int seconds = Mathf.Max(0, Mathf.CeilToInt(timeLeft));
        timerText.text = $"Время: {seconds} сек";
    }

    private void OnTimeEnd()
    {
        EndGame(false);
    }

    private void ShowTotalTime()
    {
        int seconds = Mathf.FloorToInt(totalBuildTime);
        timerText.text = $"Общее время: {seconds} сек";
    }

    private string GenerateBuilding(int length)
    {
        string result = "";
        for (int i = 0; i < length; i++)
        {
            int rand = Random.Range(0, blocks.Length);
            result += rand;
        }
        return result;
    }

    public void SpawnBlock(int blockID)
    {
        blockMenuPanel.SetActive(false);

        GameObject newBlock = Instantiate(blocks[blockID], blockSpawnpoint.position, Quaternion.identity);
        newBlock.transform.SetParent(blockSpawnpoint);

        BlockPhysics bp = newBlock.GetComponent<BlockPhysics>();
        bp.OnPlaced += () =>
        {
            currentBuilding += blockID;
            blockMenuPanel.SetActive(true);

            if (currentBuilding.Length == maxBlocks)
                CheckResult();
        };
    }

    private void CheckResult()
    {
        if(currentBuilding == targetBuilding)
            EndGame(true);
        else
            EndGame(false);
    }

    private void RenderScheme(string building)
    {
        // Удаляем старые картинки
        // foreach (Transform child in schemeContainer)
        //     Destroy(child.gameObject);

        if (string.IsNullOrEmpty(building)) return;

        int numBlocks = building.Length;

        for (int i = 0; i < numBlocks; i++)
        {
            int id = int.Parse(building[i].ToString());

            GameObject go = new GameObject("Block_" + i);
            go.transform.SetParent(schemeContainer, false);

            // изображение
            RawImage ri = go.AddComponent<RawImage>();
            ri.texture = blockTextures[id];
            ri.raycastTarget = false;

            // квадрат 1:1
            AspectRatioFitter arf = go.AddComponent<AspectRatioFitter>();
            arf.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
            arf.aspectRatio = 1f;
        }
    }

    private void EndGame(bool win)
    {
        isTimerRunning = false;
        if (win)
            totalBuildTime += currentRunTime;
        ShowTotalTime();

        blockMenuPanel.SetActive(false);
        mainScreenPanel.SetActive(true);
        schemeContainer.gameObject.SetActive(false);

        winText.SetActive(win);
        loseText.SetActive(!win);

        restartButton.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        currentRunTime = 0f;
        timeLeft = 0f;
        isTimerRunning = false;
        UpdateTimerUI();

        foreach (Transform child in blockSpawnpoint)
            Destroy(child.gameObject);

        currentBuilding = "";
        targetBuilding = "";
        schemeText.text = "";

        blockMenuPanel.SetActive(false);
        winText.SetActive(false);
        loseText.SetActive(false);
        restartButton.gameObject.SetActive(false);

        difficultyPanel.SetActive(true);
        mainScreenPanel.SetActive(true);
    }
}