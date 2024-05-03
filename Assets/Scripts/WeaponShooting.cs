using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class WeaponShooting : MonoBehaviour
{
    [SerializeField] Deck deck;
    [SerializeField] Recoil recoil;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform shootingPoint;
    [SerializeField] LayerMask reverseTargetMask;

    [Space(3)]
    [Header("Data")]
    /// <summary>
    /// Base damage of the gun
    /// </summary>
    [SerializeField] float damage = 1f;
    /// <summary>
    /// Maximum ammo capacity of the weapon
    /// </summary>
    [SerializeField] int magazineSize = 5;
    /// <summary>
    /// Reload time in seconds
    /// </summary>
    [SerializeField] float reloadTime = 1f;
    /// <summary>
    /// Firerate of the weapon in seconds
    /// </summary>
    [SerializeField] float fireRate = 0.1f;

    [Space(3)]
    [Header("Recoil")]
    /// <summary>
    /// Vertical/upwards recoil amount that will be negated
    /// </summary>
    [SerializeField] float recoilX = 1.5f;
    /// <summary>
    /// Maximum horizontal recoil that will be randomized
    /// </summary>
    [SerializeField] float recoilY = 2f;
    /// <summary>
    /// The amount of vertical bounce experienced while shooting
    /// </summary>
    [SerializeField] float snappiness = 3f;
    /// <summary>
    /// The recovery speed after recoil happens
    /// </summary>
    [SerializeField] float returnSpeed = 2f;
    public int GetMagazineSize => magazineSize;

    [Space(3)]
    public UnityEvent OnFireTrigger;
    public UnityEvent OnReloadTrigger;
    public Action<int> OnAmmoChange;

    float fireTimer;
    bool reloading;

    private void Update()
    {
        if (fireTimer > 0) 
        { 
            fireTimer -= Time.deltaTime; 
            return; 
        }

        if (reloading) return;

        CheckWeaponFired();
    }

    private void CheckWeaponFired()
    {
        if (Input.GetMouseButton(0))
        {
            if (deck.MagazineAmmo > 0)
            {
                OnFireTrigger?.Invoke();
                AmmoType ammo = deck.ShootAmmo();

                float recoilX = this.recoilX;
                if (ammo == AmmoType.Crit)
                    recoilX *= 2f;

                recoil.RecoilFire(-recoilX, recoilY, snappiness, returnSpeed);

                //Instantiate(bulletPrefab, shootingPoint.position, transform.rotation);
                RaycastHit hit;
                if (Physics.Raycast(PlayerMovement.GetHeadTransform().position, PlayerMovement.GetHeadTransform().forward, out hit, float.MaxValue, ~reverseTargetMask))
                {
                    if (hit.collider.TryGetComponent(out Health health))
                        health.Damage(damage * GetDamageMultiplyer(ammo));
                }
                fireTimer = fireRate;
            }
            //else
            //    StartCoroutine(Reload());
        }

        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(Reload());
    }

    #region Mult
    const float basicMult = 1f;
    const float elementalMult = 1.25f;
    const float critMult = 2f;
    const float blankMult = 0.8f;

    private float GetDamageMultiplyer(AmmoType ammoType)
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

    private IEnumerator Reload()
    {
        reloading = true;
        deck.EmptyMagazine();
        List<AmmoType> hand = deck.DrawHand(magazineSize);
        OnReloadTrigger?.Invoke();

        yield return new WaitForSeconds(reloadTime);

        deck.LoadMagazine(hand);
        reloading = false;
    }
}
