using UnityEngine;

public class WeaponEventAnimationPlayer : MonoBehaviour, IWeaponEffect
{
    [SerializeField] Animator animator;
    public Animator Animator { get => animator; }

    [SerializeField] protected string animationLayer;
    public string AnimationLayer { get => animationLayer; }
    LoadoutManager loadoutManager;

    private void Start()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        loadoutManager = GameManager.i.loadoutManager;
    }

    public void DoEffect(WeaponEvent weaponEvent)
    {
        Gun currentGun = loadoutManager.CurrentGun;
        switch (weaponEvent)
        {
            case WeaponEvent.Reload: animator?.SetFloat("Speed", 1 / currentGun.CurrentStats.reloadSpeed);
                break;
            case WeaponEvent.Ready: animator?.SetFloat("Speed", 1 / currentGun.CurrentStats.readyTime);
                break;
            case WeaponEvent.Rest: animator?.SetFloat("Speed", 1 / currentGun.CurrentStats.readyTime);
                break;
            default: animator?.SetFloat("Speed", 1);
                break;
        }
        animator?.SetTrigger(weaponEvent.ToString()); 
    }
}
