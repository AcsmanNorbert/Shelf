using UnityEngine;

[RequireComponent(typeof(WeaponEventAnimationPlayer))]
public class PlasmaAnimations : MonoBehaviour
{
    Animator animator;
    AudioSource soundPlayer;
    WeaponStatHandler statHandler;
    Gun gun;
    [SerializeField] Upgrade plasmaUpgrade;
    [SerializeField] float spinAmount = 0.5f;
    [SerializeField] float pitchMulty = 0.01f;
    [SerializeField] int pitchStart = 15;
    [SerializeField] float windingSpeed;
    int plasmaStacks;
    float rotationSpeed;

    void Start()
    {
        animator = GetComponent<WeaponEventAnimationPlayer>().Animator;
        soundPlayer = GetComponent<WeaponEventSoundPlayer>().Player;
        basePitch = soundPlayer.pitch;

        statHandler = GetComponent<WeaponStatHandler>();
        statHandler.OnBonusGained += OnStackGained;
        statHandler.OnBonusStackGained += OnStackGained;
        statHandler.OnBonusLost += OnStackGained;
        statHandler.OnBonusStackLost += OnStackGained;

        gun = GetComponent<Gun>();
        gun.OnFireTrigger += OnFireTrigger;
    }

    float basePitch;
    private void OnFireTrigger(AmmoType type)
    {
        int currentMagazine = gun.GetAmmoInMagazine;
        int difference = Mathf.Min(currentMagazine, pitchStart);
        float pitch = (pitchStart * pitchMulty) - (difference * pitchMulty);
        soundPlayer.pitch = basePitch + pitch;
    }

    void OnStackGained(Upgrade upgrade, UpgradeState state)
    {
        if (upgrade != plasmaUpgrade) return;
        plasmaStacks = state.stack;
    }

    private void Update()
    {
        rotationSpeed = Mathf.Lerp(rotationSpeed, plasmaStacks, Time.deltaTime * windingSpeed);
        animator.SetFloat("SpinnerSpeed", 1 + rotationSpeed * spinAmount);
    }
}
