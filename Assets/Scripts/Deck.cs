using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardsData
{
    /// <summary>
    /// The maximum ammo the gun can gold
    /// </summary>
    public int magazineSize;
    /// <summary>
    /// The amount of ammo still left in the magazine
    /// </summary>
    public int ammoCount;
    /// <summary>
    /// The maximum size of your deck
    /// </summary>
    public int deckSize;
    /// <summary>
    /// The remaining amount of cards in your deck
    /// </summary>
    public int cardCount;
}

public class Deck : MonoBehaviour
{
    [SerializeField] WeaponShooting weapon;
    [SerializeField] AmmoType[] fullDeck;
    List<AmmoType> currentDeck;

    int ammoInMagazine;
    int magazineSize;
    public int MagazineAmmo => ammoInMagazine;

    private void Awake()
    {
        magazineSize = weapon.GetMagazineSize;
        ammoInMagazine = magazineSize;
    }

    private void Start()
    {
        ReshuffleDeck();
    }

    public Action<List<AmmoType>, CardsData> OnAmmoChange;

    CardsData CardsData
    {
        get 
        {
            CardsData data = new();
            data.magazineSize = magazineSize;
            data.ammoCount = ammoInMagazine;
            data.deckSize = fullDeck.Count();
            data.cardCount = currentDeck.Count();
            return data;
        }
    }

    public void ReshuffleDeck()
    {
        int deckSize = fullDeck.Length;
        currentDeck = new(deckSize);
        List<AmmoType> shuffle = fullDeck.ToList();

        for (int i = 0; i < deckSize; i++)
        {
            int random = Random.Range(0, deckSize - i);
            AmmoType randomShell = shuffle[random];
            currentDeck.Add(randomShell);
            shuffle.RemoveAt(random);
        }
        OnAmmoChange?.Invoke(currentDeck, CardsData);
    }

    public AmmoType ShootAmmo()
    {
        AmmoType ammo = AmmoType.Blank;
        if (currentDeck.Count > 0)
        {
            ammo = currentDeck[0];
            currentDeck.RemoveAt(0);
        }
        ammoInMagazine--;
        OnAmmoChange?.Invoke(currentDeck, CardsData);
        return ammo;
    }

    public List<AmmoType> DrawHand(int magazineSize)
    {
        if (currentDeck.Count == 0)
            ReshuffleDeck();

        int count = Mathf.Clamp(magazineSize, 0, currentDeck.Count);
        List<AmmoType> newHand = currentDeck.GetRange(0, count);
        for (int i = count; i < magazineSize; i++)
            newHand.Add(AmmoType.Blank);

        return newHand;
    }

    public void EmptyMagazine()
    {
        int count = Mathf.Clamp(ammoInMagazine, 0, currentDeck.Count);
        if (ammoInMagazine != 0)
            currentDeck.RemoveRange(0, count);
        ammoInMagazine = 0;
        OnAmmoChange?.Invoke(currentDeck, CardsData);
    }

    public void LoadMagazine(List<AmmoType> ammo)
    {
        ammoInMagazine = weapon.GetMagazineSize;
        OnAmmoChange?.Invoke(currentDeck, CardsData);
    }
}
