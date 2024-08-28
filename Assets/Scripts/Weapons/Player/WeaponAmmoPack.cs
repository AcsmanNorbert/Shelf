using UnityEngine;

public class WeaponAmmoPack : MonoBehaviour
{
    [SerializeField] AmmoType[] ammoPack;

    public AmmoType[] GetAmmoPack => ammoPack;
}