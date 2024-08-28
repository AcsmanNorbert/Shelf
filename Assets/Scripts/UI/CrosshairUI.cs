using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    [Header("Recoil")]
    [SerializeField] GameObject crosshair;
    LoadoutManager loadout;
    RectTransform rect;
    CameraController cameraController;
    Recoil recoil;

    Vector2 baseSize;

    [SerializeField, Min(1)] float sizeScalar;
    [SerializeField] float recoilScalar;
    [SerializeField] float spreadScalar;

    [Space(3), Header("Hit")]
    [SerializeField] CanvasGroup hitAlpha;
    [SerializeField] CanvasGroup weakPointAlpha;
    [SerializeField, Range(0, 1)] float maxVisibility = 1;
    [SerializeField] float hitSpeed = 1;
    float hitVisibility;
    float weakVisibility;

    [Space(3), Header("Kill")]
    [SerializeField] Image killImage;
    [SerializeField] CanvasGroup killAlpha;
    [SerializeField] Sprite killSprite;
    [SerializeField] Sprite weakPointSprite;
    [SerializeField] float killSizeIncrease = 1.1f;
    float killSize;
    float killVisibility;

    private void Start()
    {
        GameObject player = GameManager.i.playerRef;
        loadout = player.GetComponent<LoadoutManager>();
        cameraController = player.GetComponent<CameraController>();

        rect = crosshair.GetComponent<RectTransform>();

        recoil = cameraController.Recoil;
        baseSize = rect.sizeDelta;

        Health playerHealth = player.GetComponent<Health>();
        playerHealth.OnInflictingDamage += OnInflictingDamage;
        playerHealth.OnKill += OnKill;
        hitAlpha.alpha = 0;
        weakPointAlpha.alpha = 0;
        killAlpha.alpha = 0;
    }

    void Update()
    {
        UpdateCrosshairUI();
        UpdateHitUI();
        UpdateKillUI();
    }

    void UpdateCrosshairUI()
    {
        Vector2 newSize = baseSize * sizeScalar;
        WeaponStats stats = loadout.CurrentGun.CurrentStats;

        Vector2 spreadVector = new(stats.horizontalSpread, stats.verticalSpread);
        float spreadAmount = stats.spreadModifier * spreadScalar;

        float recoilAmount = Mathf.Abs(recoil.RecoilAmount()) * recoilScalar;

        float zoomAmount = cameraController.ZoomAmount();

        newSize += spreadVector * spreadAmount * zoomAmount + newSize * recoilAmount;

        rect.sizeDelta = newSize;
    }

    void UpdateHitUI()
    {
        float hitAlphaColor = Mathf.Lerp(hitAlpha.alpha, 0, Time.deltaTime * hitSpeed);
        float weakAlphaColor = Mathf.Lerp(weakPointAlpha.alpha, 0, Time.deltaTime * hitSpeed);
        hitAlpha.alpha = hitAlphaColor;
        weakPointAlpha.alpha = weakAlphaColor;
    }

    void UpdateKillUI()
    {
        float killAlphaColor = Mathf.Lerp(killAlpha.alpha, 0, Time.deltaTime * hitSpeed);
        killAlpha.alpha = killAlphaColor;
        killSize = Mathf.Lerp(killSize, 1, Time.deltaTime * hitSpeed);
        killImage.rectTransform.localScale = new(killSize, killSize, killSize);
    }

    void OnInflictingDamage(HitData hitData)
    {
        if (!hitData.weakPointHit)
        {
            hitVisibility = maxVisibility;
            hitAlpha.alpha = hitVisibility;
        }
        else
        {
            weakVisibility = maxVisibility;
            weakPointAlpha.alpha = weakVisibility;
        }
    }

    void OnKill(HitData hitData)
    {
        killVisibility = maxVisibility;
        killAlpha.alpha = killVisibility;
        killImage.sprite = !hitData.weakPointHit ? killSprite : weakPointSprite;
        killSize = killSizeIncrease;
        killImage.rectTransform.localScale = new(killSize, killSize, killSize);
    }
}
