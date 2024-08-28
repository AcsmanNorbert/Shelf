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
        DeckManager magazine = playerRef.GetComponent<DeckManager>();
        magazine.OnAmmoChange += RefreshAmmoUI;
    }

    [SerializeField] TMP_Text magazineText;
    [SerializeField] TMP_Text deckText;
    public void RefreshAmmoUI(List<AmmoType> currentDeck, CardsData cardsData)
    {
        magazineText.text = $"{cardsData.ammoCount}/{cardsData.magazineSize}";
        deckText.text = $"{cardsData.cardCount}";
        for (int i = 0; i < cards.Count; i++)
        {
            Image image = cards[i].GetComponentInChildren<Image>();
            if (currentDeck.Count > i) image.sprite = ammoTypeImages.GetSprite(currentDeck[i]);
            else image.sprite = ammoTypeImages.GetSprite(AmmoType.Blank);

            float greyScale = (8f - i) / 5f;
            greyScale = Mathf.Clamp(greyScale, 0.2f, 1);
            Color greyColor = new(greyScale, greyScale, greyScale);
            if (cardsData.ammoCount > i) image.color = greyColor;
            else image.color = new(0.2f, 0.2f, 0.2f);
        }
    }

    [SerializeField] int displayLenght = 15;
    public void InstCards()
    {
        float rotationOffset = 360 / displayLenght;
        Vector3 rotOffsetVector = new(0, 0, rotationOffset);
        for (int i = 0; i < displayLenght; i++)
        {
            int count = i + 1;
            Vector3 offsetRotation = rotOffsetVector * count;
            GameObject newCard = Instantiate(cardPrefab, container.transform.position, Quaternion.identity, container.transform);
            newCard.SetActive(true);
            newCard.GetComponent<RectTransform>().localEulerAngles = offsetRotation;
            cards.Add(newCard);
        }
        cards.Reverse();
    }
}
