using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutManager : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Transform handTransform;
    [SerializeField] GameObject[] gunPrefabs = new GameObject[3];

    List<GameObject> currentLoadout = new();
    public Gun CurrentGun => currentLoadout[CurrentGunSlot].GetComponent<Gun>();
    public int CurrentGunSlot { get; private set; } = 0;
    public DeckManager deckManager;
    public Recoil recoil;

    [SerializeField] Animator animator;

    bool canSwap = true;

    private void OnValidate()
    {
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
    }

    private void Awake()
    {
        playerInput.OnSwapWeapon += SwapWeapon;
        playerInput.OnWeaponFire += WeaponFire;
        playerInput.OnZoom += Zoom;
        playerInput.OnReload += Reload;
        playerInput.OnNextWeapon += SwapToNextWeapon;
        LoadWeapons();
    }
    const int activeSlot = 0;
    private void LoadWeapons()
    {
        int i = 0;
        foreach (GameObject weapon in gunPrefabs)
        {
            GameObject gunPrefab = Instantiate(weapon, handTransform);
            Gun newGun = gunPrefab.GetComponent<Gun>();

            if (i != activeSlot) newGun.SetActiveGun(false);
            else
            {
                newGun.SetActiveGun(true);
                SetAnimatorLayer(newGun);
            }

            currentLoadout.Add(gunPrefab);
            i++;
        }
    }

    int currentLayerIndex;

    public void SwapWeapon(int loadoutSlot)
    {
        if (!canSwap) return;
        if (CurrentGunSlot == loadoutSlot) return;
        if (loadoutSlot >= currentLoadout.Count) return;
        if (currentLoadout[loadoutSlot] == null) return;

        canSwap = false;
        float restTime = CurrentGun.SwapWeapon(false);
        CurrentGunSlot = loadoutSlot;
        StartCoroutine(SetActiveGun(restTime, loadoutSlot));
    }

    public void SwapToNextWeapon(int direction)
    {
        int slot = CurrentGunSlot + direction;
        if (slot < 0) slot = currentLoadout.Count - 1;
        else if (slot >= currentLoadout.Count) slot = 0;
        SwapWeapon(slot);
    }

    #region CURRENT_WEAPON_ACTIONS
    public void WeaponFire() => CurrentGun.OnTriggerPull();
    public void Reload() => CurrentGun.Reload();
    public void Zoom(bool zoomIn) 
    { 
        if (zoomIn) CurrentGun.ZoomIn(); 
        else CurrentGun.ZoomOut(); 
    }
    private IEnumerator SetActiveGun(float restTime, int loadoutSlot)
    {
        yield return new WaitForSeconds(restTime);

        GameObject currentWeapon = currentLoadout[loadoutSlot];
        Gun currentGun = currentWeapon.GetComponent<Gun>();
        SetAnimatorLayer(currentGun);
        float readyTime = currentGun.SwapWeapon(true);

        yield return new WaitForSeconds(readyTime);

        canSwap = true;
    }
    void SetAnimatorLayer(Gun gun)
    {
        int layerIndex = animator.GetLayerIndex(gun.GetComponent<WeaponEventAnimationPlayer>().AnimationLayer);
        animator.SetLayerWeight(currentLayerIndex, 0);
        animator.SetLayerWeight(layerIndex, 1);
        currentLayerIndex = layerIndex;
    }
    #endregion
}
