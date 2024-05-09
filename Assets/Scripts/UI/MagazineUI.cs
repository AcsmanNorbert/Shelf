using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MagazineUI : MonoBehaviour
{
    [SerializeField] GameObject container;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] AmmoTypeImages ammoTypeImages;
    List<GameObject> cards = new();

    private void Start()
    {
        InstCards();
        GameObject playerRef = GameManager.i.playerRef;
        Deck magazine = playerRef.GetComponent<Deck>();
        magazine.OnAmmoChange += RefreshAmmoUI;
    }

    [SerializeField] TMP_Text magazineText; 
    [SerializeField] TMP_Text deckText;
    public void RefreshAmmoUI(List<AmmoType> currentDeck, CardsData cardsData)
    {
        magazineText.text = $"{cardsData.ammoCount}/{cardsData.magazineSize}";
        deckText.text = $"{cardsData.cardCount}/{cardsData.deckSize}";
        for (int i = 0; i < cards.Count; i++)
        {
            Image image = cards[i].GetComponent<Image>();
            if (currentDeck.Count > i)
                image.sprite = ammoTypeImages.GetSprite(currentDeck[i]);
            else
                image.sprite = ammoTypeImages.GetSprite(AmmoType.Blank);

            if (cardsData.ammoCount > i)
                image.color = Color.white;
            else
                image.color = Color.gray;
        }
    }

    const float offset = 22;
    public void InstCards()
    {
        Vector3 offsetVector = new(offset, 0, 0.1f);
        const int displayLenght = 15;
        for (int i = 0; i < displayLenght; i++)
        { 
            GameObject newCard = Instantiate(cardPrefab, container.transform.position, Quaternion.identity, container.transform);
            Vector3 offsetPosition = offsetVector * (displayLenght - i - 1);
            newCard.GetComponent<RectTransform>().anchoredPosition = offsetPosition;
            newCard.SetActive(true);
            cards.Add(newCard);
        }
        cards.Reverse();
    }
}
