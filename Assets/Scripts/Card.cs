using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int cardNumber;
    public GameManager gameManager;

    private bool isFlipped;

    public Image cardImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isFlipped = false;
        
        cardImage.sprite = GameManager.Instance.cardBack;
    }

    public void FlipCard()
    {
        if (!isFlipped && gameManager.firstCard == null || gameManager.secondCard == null)
        {
            isFlipped = true;
            cardImage.sprite = gameManager.cardFaces[cardNumber];
            gameManager.CardFlipped(this);
        }
    }

    public void HideCard()
    {
        isFlipped = false;
        cardImage.sprite = gameManager.cardBack;
    }
    
}
