using System.Collections;
using UnityEngine;

public class LoadoutManager : MonoBehaviour
{
    [SerializeField] GameObject[] gunPrefabs = new GameObject[3];
    public GameObject[] GetLoadoutGuns => gunPrefabs;
    public Gun GetCurrentGun => GetComponentInChildren<Gun>();

    public int currentGun { get; private set; } = -1;
    [SerializeField] Transform handTransform;
    public Deck deck;
    public Recoil recoil;
    public LayerMask reverseTargetMask;

    private void Awake()
    {
        Gun existingGun = GetComponentInChildren<Gun>();
        if (existingGun != null) gunPrefabs[0] = existingGun.gameObject;
        else LoadGun(0);
        deck.LoadFullDeck();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            LoadGun(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            LoadGun(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            LoadGun(2);
    }
    
    private void LoadGun(int loadoutSlot)
    {/*
        if (currentGun == loadoutSlot) return;
        if (gunPrefabs[loadoutSlot] == null) return;

        currentGun = loadoutSlot;
        if (GetComponentInChildren<Gun>() != null)
            Destroy(GetCurrentGun.gameObject);

        GameObject gunPrefab = Instantiate(gunPrefabs[loadoutSlot], handTransform);*/
    }
}
