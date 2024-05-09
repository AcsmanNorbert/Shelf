using UnityEngine;

public class GunAnimation : MonoBehaviour
{
    [SerializeField] Gun gun;
    [SerializeField] Animator animator;

    private void Start()
    {
        if (gun == null)
            gun = GetComponent<Gun>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        gun.OnFireTrigger += OnFireTrigger;
        gun.OnReloadTrigger += OnReloadTrigger;
    }

    void OnFireTrigger()
    {
        animator.SetTrigger("Fire");
    }

    void OnReloadTrigger()
    {
        animator.SetTrigger("Reload");
    }
}
