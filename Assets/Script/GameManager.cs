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

    [Header("Temporary Cheat Button")]
    [SerializeField] private Button cheatButton;

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

        // Add cheat button listener
        if (cheatButton != null)
        {
            cheatButton.onClick.AddListener(CheatCompleteLevel);
            cheatButton.gameObject.SetActive(false); // Hide initially
        }

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

        // Show cheat button when game starts
        // if (cheatButton != null)
        // {
        //     cheatButton.gameObject.SetActive(true);
        // }

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

    // NEW CHEAT FUNCTION
    private void CheatCompleteLevel()
    {
        if (!gameStarted)
            return;

        Debug.Log("Cheat activated! Turning off all bulbs...");

        // Turn off all active bulbs
        for (int i = 0; i < bulbButtons.Length; i++)
        {
            if (bulbButtons[i].gameObject.activeInHierarchy)
            {
                Button button = bulbButtons[i];
                Image image = button.GetComponent<Image>();
                TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

                // Force bulb to OFF state
                image.sprite = offBulbImage;
                text.text = BulbState.Off.ToString();
            }
        }

        // Complete the level
        LevelComplete();
    }

    private void LevelComplete()
    {
        gameStarted = false;

        // Hide cheat button when level is complete
        if (cheatButton != null)
        {
            cheatButton.gameObject.SetActive(false);
        }

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
        else if (currentLevel == 7)
            completionMessage = $"Level 8 done! And you only used ${billPrice} of electricity. Just enough to toast a slice of bread... on the sun.";
        else if (currentLevel == 8)
            completionMessage = $"Congratulations! You've now powered a small village for 3 minutes. ${billPrice} well spent!";
        else if (currentLevel == 9)
            completionMessage = $"That puzzle was lit—literally. Hope ${billPrice} was worth the sparks flying in your brain!";
        else if (currentLevel == 10)
            completionMessage = $"You're halfway to lightbulb enlightenment! Unfortunately, your wallet is halfway to empty. ${billPrice}... oof.";
        else if (currentLevel == 11)
            completionMessage = $"Nice! Level 12 complete. The electric grid is now slightly unstable thanks to your ${billPrice} contribution!";
        else if (currentLevel == 12)
            completionMessage = $"Another level down, another ${billPrice} up in smoke. The lights went off, but your energy bill just lit up!";
        else if (currentLevel == 13)
            completionMessage = $"Brilliantly inefficient! You managed to beat this one using only ${billPrice} of pure wattage waste.";
        else if (currentLevel == 14)
            completionMessage = $"Level 15 complete. You're now on your power provider's Christmas card list. ${billPrice} of appreciation!";
        else if (currentLevel == 15)
            completionMessage = $"You blacked out the board again. For ${billPrice}. That's almost enough to charge your phone 700 times!";
        else if (currentLevel == 16)
            completionMessage = $"You're now legally considered a walking circuit breaker. ${billPrice} in light-flipping madness!";
        else if (currentLevel == 17)
            completionMessage = $"That was intense! ${billPrice} for a puzzle? You could've funded a solar panel... or five.";
        else if (currentLevel == 18)
            completionMessage = $"Wow! You beat level 19 and now owe ${billPrice}. Your light switch addiction needs intervention.";
        else if (currentLevel == 19)
            completionMessage = $"All bulbs off! And only ${billPrice}? That's it — you're the reigning Light-Out Lord!";
        else
            completionMessage = $"Final level conquered! Only cost you ${billPrice}... which brings your total energy bill to 'why did I play this game?' dollars!";

        nextLevelText.text = completionMessage;

        if (currentLevel >= bulbLevelData.Levels.Count - 1)
        {
            allLevelsCompleted = true;
            nextLevelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Finish";
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

        // Show cheat button for next level
        // if (cheatButton != null)
        // {
        //     cheatButton.gameObject.SetActive(true);
        // }

        // Load the next level
        currentLevel++;
        LoadLevel(currentLevel);
        gameStarted = true;
    }

    private void ShowGameCompletion()
    {
        gameStarted = false;
        nextLevelButton.gameObject.SetActive(false);

        // Hide cheat button when game is complete
        if (cheatButton != null)
        {
            cheatButton.gameObject.SetActive(false);
        }

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