using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    protected LoadoutManager loadoutManager;
    [SerializeField] Transform shootingPoint;

    [Space(3)]
    [Header("Data")]
    /// <summary>
    /// Base damage of the gun
    /// </summary>
    [SerializeField] protected float damage = 1f;
    /// <summary>
    /// Maximum ammo capacity of the weapon
    /// </summary>
    [SerializeField] protected int magazineSize = 5;
    /// <summary>
    /// Reload time in seconds
    /// </summary>
    [SerializeField] protected float reloadTime = 1f;
    /// <summary>
    /// Firerate of the weapon in seconds
    /// </summary>
    [SerializeField] protected float fireRate = 0.1f;

    [Space(3)]
    [Header("Recoil")]
    /// <summary>
    /// Vertical/upwards recoil amount that will be negated
    /// </summary>
    [SerializeField] protected float recoilX = 1.5f;
    /// <summary>
    /// Maximum horizontal recoil that will be randomized
    /// </summary>
    [SerializeField] protected float recoilY = 2f;
    /// <summary>
    /// The amount of vertical bounce experienced while shooting
    /// </summary>
    [SerializeField] protected float snappiness = 3f;
    /// <summary>
    /// The recovery speed after recoil happens
    /// </summary>
    [SerializeField] protected float returnSpeed = 2f;

    [Space(3)]
    [Header("Events")]
    public Action OnFireTrigger;
    public Action OnReloadTrigger;
    public Action<int> OnAmmoChange;

    float fireTimer;
    bool reloading;

    void Awake()
    {
        loadoutManager = GetComponentInParent<LoadoutManager>();
    }

    void Start()
    {
        ammoInMagazine = magazineSize; 
        List<AmmoType> hand = loadoutManager.deck.DrawHand(magazineSize);
        LoadMagazine(hand);
    }

    void Update()
    {
        if (fireTimer > 0) fireTimer -= Time.deltaTime;

        if (reloading || fireTimer > 0) return;

        if (Input.GetMouseButton(0))
            if (ammoInMagazine > 0)
                OnTriggerPull();

        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(Reload());
    }

    int ammoInMagazine;
    public int GetAmmoInMagazine => ammoInMagazine;

    protected virtual void OnTriggerPull()
    {
        ammoInMagazine--;
        OnFireTrigger?.Invoke();

        AmmoType ammo = loadoutManager.deck.ShootAmmo();

        float recoilX = this.recoilX;
        if (ammo == AmmoType.Crit)
            recoilX *= 2f;
        loadoutManager.recoil.RecoilFire(-recoilX, recoilY, snappiness, returnSpeed);

        fireTimer = fireRate;
        OnGunFired(ammo);
    }

    protected abstract void OnGunFired(AmmoType ammo);

    IEnumerator Reload()
    {
        if (fireTimer > 0) yield break;

        reloading = true;
        loadoutManager.deck.EmptyMagazine(ammoInMagazine);
        ammoInMagazine = 0;
        List<AmmoType> hand = loadoutManager.deck.DrawHand(magazineSize);
        OnReloadTrigger?.Invoke();

        yield return new WaitForSeconds(reloadTime);

        LoadMagazine(hand);
    }

    private void LoadMagazine(List<AmmoType> hand)
    {
        ammoInMagazine = magazineSize;
        loadoutManager.deck.LoadMagazine(hand);
        reloading = false;
    }

    #region MULT
    float basicMult = 1f;
    float elementalMult = 1.25f;
    float critMult = 2f;
    float blankMult = 0.8f;

    protected float SetBasicMult { set => basicMult = value; }
    protected float SetElementalMult { set => elementalMult = value; }
    protected float SetCritMult { set => critMult = value; }
    protected float SetBlankMult { set => blankMult = value; }

    protected float GetDamageMultiplyer(AmmoType ammoType)
    {
        switch (ammoType)
        {
            case AmmoType.Basic: return basicMult;
            case AmmoType.Elemental: return elementalMult;
            case AmmoType.Crit: return critMult;
            case AmmoType.Blank: return blankMult;
            default: return 1f;
        }
    }
    #endregion

    #region PACK
    [SerializeField] AmmoType[] ammoPack;
    public AmmoType[] GetAmmoPack => ammoPack;
    public int GetMagazineSize => magazineSize;
    #endregion
}