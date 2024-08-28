public enum WeaponEvent { Fire, Reload, Ready, Rest, Jump }

public interface IWeaponEffect
{
    public void DoEffect(WeaponEvent weaponEvent);
}
