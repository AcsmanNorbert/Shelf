using UnityEngine;

public class AmmoTypeImages: MonoBehaviour
{
    public Sprite basic;
    public Sprite elemental;
    public Sprite crit;
    public Sprite blank;

    public Sprite GetSprite(AmmoType ammoType)
    {
        switch (ammoType)
        {
            case AmmoType.Basic: return basic;
            case AmmoType.Elemental: return elemental;
            case AmmoType.Crit: return crit;
            case AmmoType.Blank: return blank;
            default: return null;
        }
    }
}
