using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Level Data")]
    [SerializeField] private BulbLevelData bulbLevelData;

    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;

    [Header("Buttons")]
    [SerializeField] private Button[] bulbButtons;
    [SerializeField] private Button playButton;
    [SerializeField] private Button nextLevelButton;

    [Header("Bulb Images")]
    [SerializeField] private Sprite onBulbImage;
    [SerializeField] private Sprite offBulbImage;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI billPriceText;
    [SerializeField] private TextMeshProUGUI nextLevelText;

    // Game state variables
    private int currentLevel = 0;
    private int billPrice = 0;
    private float timer;
    private bool gameStarted = false;
    private bool allLevelsCompleted = false;

    private void Start()
    {
        InitializeGame();
    }

    private void Update()
    {
        if (gameStarted)
        {
            timer += Time.deltaTime;
            CalculateBillPrice();
        }
    }

    private void InitializeGame()
    {
        // Validate level data
        if (bulbLevelData == null || bulbLevelData.Levels.Count == 0)
        {
            Debug.LogError("No level data assigned to the GameManager!");
            return;
        }

        // Set up UI and panels
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);
        nextLevelText.gameObject.SetActive(false);

        // Add listeners to buttons
        playButton.onClick.AddListener(StartGame);
        nextLevelButton.onClick.AddListener(LoadNextLevel);

        // Initialize bulb buttons
        for (int i = 0; i < bulbButtons.Length; i++)
        {
            int index = i;
            bulbButtons[i].onClick.AddListener(() => OnBulbClick(index));
        }

      
    }

    private void StartGame()
    {
        billPrice = 0;
        timer = 0;
        gameStarted = true;
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        billPriceText.text = "0";
        
        LoadLevel(currentLevel);
    }

    private void LoadLevel(int levelIndex)
    {
        // Check for level index
        if (levelIndex >= bulbLevelData.Levels.Count)
        {
            allLevelsCompleted = true;
            ShowGameCompletion();
            return;
        }

        var level = bulbLevelData.Levels[levelIndex];
        
        

        for (int i = 0; i < bulbButtons.Length; i++)
        {
            if (i < level.bulbs.Length)
            {
                SetBulbState(i, level.bulbs[i]);
                bulbButtons[i].gameObject.SetActive(true);
            }
            else
            {
                bulbButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void SetBulbState(int index, BulbState state)
    {
        if (index < 0 || index >= bulbButtons.Length)
        {
            Debug.LogWarning($"Tried to set bulb state for invalid index: {index}");
            return;
        }

        Button button = bulbButtons[index];
        Image image = button.GetComponent<Image>();
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

        if (state == BulbState.On)
        {
            image.sprite = onBulbImage;
            text.text = BulbState.On.ToString();
        }
        else
        {
            image.sprite = offBulbImage;
            text.text = BulbState.Off.ToString();
        }
    }

    private void OnBulbClick(int index)
    {
        if (!gameStarted || index < 0 || index >= bulbButtons.Length)
            return;

        // Toggle the clicked bulb
        ToggleBulb(index);

        // Toggle adjacent bulbs
        if (index > 0)
        {
            ToggleBulb(index - 1);
        }

        if (index < bulbButtons.Length - 1)
        {
            ToggleBulb(index + 1);
        }

        CheckWinCondition();
    }

    private void ToggleBulb(int index)
    {
        if (index < 0 || index >= bulbButtons.Length)
            return;

        Button button = bulbButtons[index];
        Image image = button.GetComponent<Image>();
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

        bool isCurrentlyOn = image.sprite == onBulbImage;
        
        // Toggle the state
        image.sprite = isCurrentlyOn ? offBulbImage : onBulbImage;
        text.text = isCurrentlyOn ? BulbState.Off.ToString() : BulbState.On.ToString();
    }

    private BulbState GetBulbState(int index)
    {
        if (index < 0 || index >= bulbButtons.Length)
            return BulbState.Off;
            
        Image image = bulbButtons[index].GetComponent<Image>();
        return image.sprite == onBulbImage ? BulbState.On : BulbState.Off;
    }

    private void CheckWinCondition()
    {
        // Only check active bulbs
        bool allBulbsOff = true;
        
        for (int i = 0; i < bulbButtons.Length; i++)
        {
            if (bulbButtons[i].gameObject.activeInHierarchy && GetBulbState(i) == BulbState.On)
            {
                allBulbsOff = false;
                break;
            }
        }

        if (allBulbsOff)
        {
            LevelComplete();
        }
    }

    private void LevelComplete()
    {
        gameStarted = false;
        
        nextLevelButton.gameObject.SetActive(true);
        nextLevelText.gameObject.SetActive(true);

        // Creative and funny completion messages 
        string completionMessage;
        
        if (currentLevel == 0)
            completionMessage = $"Well done! Your first blackout success! Your wallet is crying though... ${billPrice} for flicking a few switches? Your electricity company loves you!";
        else if (currentLevel == 1)
            completionMessage = $"Shocking work! You managed to rack up ${billPrice} just playing with lights. Who needs food when you can pay power bills?";
        else if (currentLevel == 2)
            completionMessage = $"Lights out! That'll be ${billPrice} please. You could've bought a coffee instead, but hey, this is more... enlightening?";
        else if (currentLevel == 3)
            completionMessage = $"Watt a performance! ${billPrice} worth of energy wasted before solving this. Thomas Edison is rolling in his grave!";
        else if (currentLevel == 4)
            completionMessage = $"Brilliant! You just contributed ${billPrice} to global warming before figuring it out. Mother Earth sends her regards!";
        else if (currentLevel == 5)
            completionMessage = $"Energy guru status: PENDING. Bill status: ${billPrice}. Your power company just named a yacht after you!";
        else if (currentLevel == 6)
            completionMessage = $"Electrifying! You burned through ${billPrice} worth of electricity. Perhaps try solving puzzles by candlelight next time?";
        else 
            completionMessage = $"Final level conquered! Only cost you ${billPrice}... which brings your total energy bill to 'why did I play this game?' dollars!";
            
        nextLevelText.text = completionMessage;
        
        if (currentLevel >= bulbLevelData.Levels.Count - 1)
        {
            allLevelsCompleted = true;
            nextLevelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Finish Game";
        }
    }

    private void LoadNextLevel()
    {
        if (allLevelsCompleted)
        {
            ShowGameCompletion();
            return;
        }
        
        // Reset bill price for next level
        billPrice = 0;
        timer = 0;
        billPriceText.text = "0";
        
        nextLevelButton.gameObject.SetActive(false);
        nextLevelText.gameObject.SetActive(false);
        
        // Load the next level
        currentLevel++;
        LoadLevel(currentLevel);
        gameStarted = true;
    }
    
    private void ShowGameCompletion()
    {
        gameStarted = false;
        nextLevelButton.gameObject.SetActive(false);
        
        // Show completion message
        nextLevelText.gameObject.SetActive(true);
        nextLevelText.text = "Oh you cracked all levels! The game is under construction buddy, wait for more illuminating puzzles!";
        
    }

    private void CalculateBillPrice()
    {
        if (timer >= 1 && gameStarted)
        {
            billPrice++;
            billPriceText.text = billPrice.ToString();
            timer = 0;
        }
    }
}