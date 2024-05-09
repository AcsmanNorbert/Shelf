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
    [SerializeField] LoadoutManager loadout;
    List<AmmoType> currentDeck = new();
    List<AmmoType> fullDeck = new();

    public Action<List<AmmoType>, CardsData> OnAmmoChange;

    public void LoadFullDeck()
    {
        currentDeck = new();
        foreach (GameObject obj in loadout.GetLoadoutGuns)
        {
            Gun gun = obj.GetComponent<Gun>();
            AmmoType[] pack = gun.GetAmmoPack;
            fullDeck.AddRange(pack);
        }
        ReshuffleDeck();
        LoadMagazine(DrawHand(loadout.GetCurrentGun.GetMagazineSize));
    }

    CardsData CardsData
    {
        get 
        {
            CardsData data = new();
            data.magazineSize = loadout.GetCurrentGun.GetMagazineSize;
            data.ammoCount = loadout.GetCurrentGun.GetAmmoInMagazine;
            data.deckSize = fullDeck.Count();
            data.cardCount = currentDeck.Count();
            return data;
        }
    }

    public void ReshuffleDeck()
    {
        int deckSize = fullDeck.Count;
        currentDeck = new(deckSize);
        List<AmmoType> shuffle = fullDeck.ToList();

        for (int i = 0; i < deckSize; i++)
        {
            int random = Random.Range(0, deckSize - i);
            AmmoType randomAmmo = shuffle[random];
            currentDeck.Add(randomAmmo);
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

    public void EmptyMagazine(int ammoInMagazine)
    {
        int count = Mathf.Clamp(ammoInMagazine, 0, currentDeck.Count);
        if (ammoInMagazine != 0)
            currentDeck.RemoveRange(0, count);
        ammoInMagazine = 0;
        OnAmmoChange?.Invoke(currentDeck, CardsData);
    }

    public void LoadMagazine(List<AmmoType> ammo) => OnAmmoChange?.Invoke(currentDeck, CardsData); 
}
