using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Card cardPrefab;
    public Sprite cardBack;
    public Sprite[] level1CardFaces;
    public Sprite[] level2CardFaces;
    public Sprite[] cardFaces;
    public Transform cardHolder;
    public GameObject finalUI;
    public TMP_Text finalText;
    public TMP_Text timerText;
    public TMP_Text scoreText;

    private List<Card> cards = new List<Card>();
    private List<int> cardNumbers = new List<int>();
    private int pairsMatched;
    private int totalPairs;
    private int score;
    private float timer;
    private bool isGameOver;
    private bool isLevelFinished;
    public float maxTime;

    public Card firstCard, secondCard;
    private int currentLevel = 1;
    private int maxLevels = 2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeLevel();
    }

    void Update()
    {
        if (!isGameOver && !isLevelFinished && timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerText();
        }
        else if (timer <= 0)
        {
            GameOver();
        }
    }

    void ClearCards()
    {
        foreach (var card in cards)
        {
            Destroy(card.gameObject);
        }
        cards.Clear();
        cardNumbers.Clear();
    }

    void InitializeLevel()
    {
        pairsMatched = 0;
        score = 0;
        isGameOver = false;
        isLevelFinished = false;
        finalUI.SetActive(false);

        if (currentLevel == 1)
        {
            cardFaces = level1CardFaces;
            totalPairs = cardFaces.Length / 2;
            maxTime = 50f;
        }
        else if (currentLevel == 2)
        {
            cardFaces = level2CardFaces;
            totalPairs = Mathf.Min(cardFaces.Length, (cardFaces.Length / 2) + 4);
            maxTime = 100f;
        }
        
        timer = maxTime;
        UpdateScoreText();
        CreateCards();
    }

    void CreateCards()
    {
        cards.Clear();
        cardNumbers.Clear();

        totalPairs = Mathf.Min(totalPairs, cardFaces.Length);
        List<int> availableImages = new List<int>();
        for (int i = 0; i < cardFaces.Length; i++)
        {
            availableImages.Add(i);
        }
        
        ShuffleList(availableImages);
        
        for (int i = 0; i < totalPairs; i++)
        {
            cardNumbers.Add(availableImages[i]);
            cardNumbers.Add(availableImages[i]);
        }
        
        ShuffleList(cardNumbers);
        
        foreach (int id in cardNumbers)
        {
            Card newCard = Instantiate(cardPrefab, cardHolder);
            newCard.gameManager = this;
            newCard.cardNumber = id;
            cards.Add(newCard);
        }
    }

    void ShuffleList(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    public void CardFlipped(Card flippedCard)
    {
        if (firstCard == null)
        {
            firstCard = flippedCard;
        }
        else if (secondCard == null)
        {
            secondCard = flippedCard;
            CheckMatch();
        }
    }

    void CheckMatch()
    {
        if (firstCard.cardNumber == secondCard.cardNumber)
        {
            pairsMatched++;
            score++;
            UpdateScoreText();
            
            if (pairsMatched == totalPairs)
            {
                NextLevel();
            }
            ResetCards();
        }
        else
        {
            StartCoroutine(FlipBackCards());
        }
    }

    IEnumerator FlipBackCards()
    {
        yield return new WaitForSeconds(1f);
        firstCard.HideCard();
        secondCard.HideCard();
        ResetCards();
    }

    void ResetCards()
    {
        firstCard = null;
        secondCard = null;
    }

    void GameOver()
    {
        isGameOver = true;
        FinalPanel("Game Over");
    }

    void NextLevel()
    {
        ClearCards();
    
        if (currentLevel < maxLevels)
        {
            currentLevel++;
            InitializeLevel();
        }
        else
        {
            LevelFinished();
        }
    }

    void LevelFinished()
    {
        isLevelFinished = true;
        FinalPanel("Game Complete! Final Score: " + score);
    }

    void FinalPanel(string message)
    {
        finalUI.SetActive(true);
        finalText.text = message;
    }

    void RestartGame()
    {
        ClearCards();
		currentLevel = 1;
        InitializeLevel();
    }

    void UpdateTimerText()
    {
        timerText.text = "Time: " + Mathf.Round(timer);
    }
    
    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
}
