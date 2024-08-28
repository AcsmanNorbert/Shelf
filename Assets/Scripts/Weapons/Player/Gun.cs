using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(WeaponEventAnimationPlayer))]
[RequireComponent(typeof(WeaponAmmoPack))]
[RequireComponent(typeof(WeaponStatHandler))]

public abstract class Gun : Weapon
{
    [SerializeField] GameObject mesh;
    protected Transform headTransform;

    protected LoadoutManager loadoutManager;
    WeaponAmmoPack weaponAmmoPack;

    [Space(3)]
    [Header("Stats")]

    [SerializeField, Min(1)] protected int burstFire = 1;
    int ammoInMagazine;
    public int GetAmmoInMagazine => ammoInMagazine;

    CameraController cameraController;
    WeaponStatHandler statHandler;
    public WeaponStats CurrentStats => statHandler.CurrentStats;

    public AmmoType[] GetAmmoPack => weaponAmmoPack.GetAmmoPack;

    float fireTimer;
    bool reloading;
    bool zoomedIn;

    protected bool activeGun = false;

    [Space(3), Header("Misc")]
    [SerializeField] Sprite scopeCrosshair;
    [SerializeField] float scopeSize;

    public void SetActiveGun(bool setActive) => StartCoroutine(SetActive(setActive, 0));

    protected override void Awake()
    {
        base.Awake();
        loadoutManager = health.GetComponent<LoadoutManager>();
        playerMask = health.GetComponent<PlayerMovement>().PlayerMask;
        cameraController = health.GetComponent<CameraController>();

        weaponAmmoPack = GetComponent<WeaponAmmoPack>();
        statHandler = GetComponent<WeaponStatHandler>();

        headTransform = PlayerMovement.GetHeadTransform();
    }

    void Start()
    {
        ammoInMagazine = CurrentStats.magazineSize;
        List<AmmoType> hand = loadoutManager.deckManager.DrawHand(CurrentStats.magazineSize);
        LoadMagazine(hand);
    }

    #region UPDATE
    void Update()
    {
        UpdateAllways();
        if (activeGun) UpdateIfActive();
        else UpdateIfNotActive();
    }

    private void UpdateAllways()
    {
        if (fireTimer > 0) fireTimer -= Time.deltaTime;
    }

    private void UpdateIfNotActive()
    {
        if (zoomedIn) zoomedIn = cameraController.ZoomOut();
    }

    private void UpdateIfActive()
    {        
    }

    #endregion

    #region SHOOTING
    const float critRecoil = 2f;

    protected abstract void FireShot(AmmoType ammo, Vector3 direction);

    public void OnTriggerPull()
    {
        if (reloading || fireTimer > 0 || !activeGun) return;
        if (ammoInMagazine <= 0) 
        {
            StartCoroutine(ReloadWeapon());
            return; 
        }
        if (burstFire == 1) SingleFire();
        else StartCoroutine(BurstFire());
    }

    protected void SingleFire()
    {
        PlayEffects(WeaponEvent.Fire);
        Vector3 direction = ApplyBulletSpread(headTransform.forward);
        AmmoFired(out AmmoType ammo);
        FireShot(ammo, direction);
        ApplyRecoil(ammo);
    }

    private IEnumerator BurstFire()
    {
        PlayEffectsParent(WeaponEvent.Fire);
        float waitTime = CurrentStats.fireRate * 0.9f / burstFire;
        for (int i = 0; i < burstFire; i++)
        {
            PlayEffectsChild(WeaponEvent.Fire);
            Vector3 direction = SpreadDirection(out Vector3 targetPoint);
            AmmoFired(out AmmoType ammo);
            FireShot(ammo, direction);
            ApplyRecoil(ammo);
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void ZoomIn() => zoomedIn = cameraController.ZoomIn(CurrentStats.zoomFOW, scopeCrosshair, scopeSize, CurrentStats.readyTime);
    public void ZoomOut() => zoomedIn = cameraController.ZoomOut();
    
    protected virtual void AmmoFired(out AmmoType ammo)
    {
        ammoInMagazine--;

        ammo = loadoutManager.deckManager.ShootAmmo();
        OnFireTrigger?.Invoke(ammo);

        fireTimer = CurrentStats.fireRate;
    }

    protected void ApplyRecoil(AmmoType ammo)
    {
        float recoilModifier = CurrentStats.recoilModifier;
        //Debug.Log(recoilModifier);
        float recoilX = CurrentStats.recoilX * recoilModifier;
        float recoilY = CurrentStats.recoilY * recoilModifier;
        float snappiness = CurrentStats.snappiness;
        float returnSpeed = CurrentStats.returnSpeed;
        if (ammo == AmmoType.Crit)
            recoilX *= critRecoil;
        loadoutManager.recoil.RecoilFire(-recoilX, recoilY, snappiness, returnSpeed);
    }

    Vector3 SpreadDirection(out Vector3 targetPoint)
    {
        Vector3 spreadDirection = ApplyBulletSpread(headTransform.forward);

        //targetPoint = Physics.Raycast(headTransform.position, spreadDirection, out RaycastHit hit, maxDisctance, ~playerMask) ? hit.point : headTransform.position + spreadDirection * maxDisctance;
        targetPoint = headTransform.position + spreadDirection * CurrentStats.maximumEffectiveRange;
        targetPoint -= shootingPoint.position;
        Vector3 direction = Vector3.RotateTowards(shootingPoint.forward, targetPoint, 360, 360);
        return direction;
    }

    protected Vector3 ApplyBulletSpread(Vector3 direction)
    {
        float zoomAmount = cameraController.ZoomAmount();

        float horizontalSpread = CurrentStats.horizontalSpread * CurrentStats.spreadModifier * zoomAmount;
        horizontalSpread = Random.Range(-horizontalSpread, horizontalSpread);

        float verticalSpread = CurrentStats.verticalSpread * CurrentStats.spreadModifier * zoomAmount;
        verticalSpread = Random.Range(-verticalSpread, verticalSpread);

        direction += headTransform.right * horizontalSpread;
        direction += headTransform.up * verticalSpread;
        return direction;
    }

    public void Reload() => StartCoroutine(ReloadWeapon());
    IEnumerator ReloadWeapon()
    {
        if (reloading) yield break;

        reloading = true;
        loadoutManager.deckManager.EmptyMagazine(ammoInMagazine);
        ammoInMagazine = 0;
        List<AmmoType> hand = loadoutManager.deckManager.DrawHand(CurrentStats.magazineSize);
        OnReloadTrigger?.Invoke();

        yield return new WaitForSeconds(CurrentStats.reloadSpeed);

        LoadMagazine(hand);
    }

    private void LoadMagazine(List<AmmoType> hand)
    {
        ammoInMagazine = CurrentStats.magazineSize;
        loadoutManager.deckManager.RefreshUI();
        reloading = false;
    }

    #endregion

    #region DAMAGE
    protected float GetDamage(float baseDamage, AmmoType ammo)
    {
        float damage = baseDamage * GetDamageMultiplyer(ammo);
        return damage;
    }

    protected DamageData GetDamageData(float damage, AmmoType ammo)
    {
        DamageData data = new();
        data.damage = damage;
        data.headshotMult = CurrentStats.headshotMult;
        data.type = ammo;
        data.inflicter = health;
        data.weapon = this;

        return data;
    }

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

    #region SWAP

    public float SwapWeapon(bool setActive)
    {
        if (!setActive)
        {
            PlayEffects(WeaponEvent.Rest);
            StartCoroutine(SetActive(false, CurrentStats.readyTime));
        }
        else
        {
            PlayEffects(WeaponEvent.Ready);
            StartCoroutine(SetActive(true, 0));
        }
        return CurrentStats.readyTime;
    }
    private IEnumerator SetActive(bool active, float setMeshActiveTimer)
    {
        activeGun = active;
        yield return new WaitForSeconds(setMeshActiveTimer);
        mesh.SetActive(active);
        loadoutManager.deckManager.RefreshUI();
    }
    #endregion

    #region EVENTS
    public Action<AmmoType> OnFireTrigger;
    public Action OnReloadTrigger;
    #endregion
}
