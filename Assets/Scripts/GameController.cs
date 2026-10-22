using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GameController : MonoBehaviour
{
    enum GameState
    {
        Start,
        Game,
        End
    }

    [Header("References")]
    [SerializeField] CursorController cursorController;
    [SerializeField] HeartSpawner heartSpawner;

    [Header("Panels")]
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject endPanel;

    [Header("HUD")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text lifeText;
    [SerializeField] GameObject[] lifeIcons;

    [Header("Rules")]
    [SerializeField] int maxLives = 3;

    GameState currentState;
    int score;
    int missCount;

    void Start()
    {
        EnterStartState();
    }

    void Update()
    {
        if (currentState == GameState.Start)
        {
            if (Keyboard.current != null &&
                Keyboard.current.enterKey.wasPressedThisFrame)
            {
                EnterGameState();
            }
        }
        else if (currentState == GameState.Game)
        {
            // 真正的 miss 条件由 HeartTarget 调用 RegisterMiss()
        }
        else if (currentState == GameState.End)
        {
            if (Keyboard.current != null &&
                Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                EnterStartState();
            }
        }
    }

    void EnterStartState()
    {
        currentState = GameState.Start;

        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        endPanel.SetActive(false);

        cursorController.gameObject.SetActive(false);
        heartSpawner.StopSpawning();
    }

    void EnterGameState()
    {
        currentState = GameState.Game;

        score = 0;
        missCount = 0;
        RefreshHUD();

        startPanel.SetActive(false);
        gamePanel.SetActive(true);
        endPanel.SetActive(false);

        cursorController.gameObject.SetActive(true);
        heartSpawner.StartSpawning();
    }

    void EnterEndState()
    {
        currentState = GameState.End;

        startPanel.SetActive(false);
        gamePanel.SetActive(false);
        endPanel.SetActive(true);

        cursorController.gameObject.SetActive(false);
        heartSpawner.StopSpawning();
    }

    public bool IsPlaying()
    {
        return currentState == GameState.Game;
    }

    public void RegisterScore()
    {
        if (!IsPlaying())
            return;

        score++;
        RefreshHUD();
    }

    public void RegisterMiss()
    {
        if (!IsPlaying())
            return;

        missCount++;
        RefreshHUD();

        if (missCount >= maxLives)
        {
            EnterEndState();
        }
    }

    void RefreshHUD()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        int remainingLives = Mathf.Max(0, maxLives - missCount);

        // Use a common heart symbol instead of the star character.
        // Writing it as a Unicode escape keeps the script encoding-safe.
        if (lifeText != null)
            lifeText.text = new string('\u2665', remainingLives);

        if (lifeIcons != null && lifeIcons.Length > 0)
        {
            for (int i = 0; i < lifeIcons.Length; i++)
            {
                if (lifeIcons[i] != null)
                    lifeIcons[i].SetActive(i < remainingLives);
            }
        }
    }
}
