using UnityEngine;
using UnityEngine.UI;

public class ScopeController : MonoBehaviour
{
    [SerializeField] RectTransform container;
    [SerializeField] RectTransform crosshair;
    Image scopeImage;
    [SerializeField] RectTransform mask;
    [SerializeField] float scaleSize;
    [SerializeField] CanvasGroup group;
    [SerializeField] float speed = 10;
    [SerializeField] bool useAlphaLerp;

    float targetAlpha;
    float targetSize;
    float readyTime;

    private void OnValidate()
    {
        scopeImage = crosshair.GetComponent<Image>();
        if (TryGetComponent(out CanvasGroup group)) this.group = group;
    }

    private void Start()
    {
        targetAlpha = 0;
        group.alpha = 0;
        targetSize = scaleSize;
        container.localScale = new(scaleSize, scaleSize, scaleSize);
    }

    public void ScopeIn(Sprite sprite, float size, float readyTime)
    {
        this.readyTime = readyTime;
        targetAlpha = 1;
        targetSize = 1;
        if (!useAlphaLerp) group.alpha = 1;

        scopeImage.sprite = sprite;
        crosshair.sizeDelta = new(size, size);
        mask.sizeDelta = new(size - 0.8f, size - 0.8f);
    }

    public void ScopeOut()
    {
        targetAlpha = 0;
        targetSize = scaleSize;
        if (!useAlphaLerp) group.alpha = 0;
    }

    private void Update()
    {
        if (useAlphaLerp)
        {
            float alpha = Mathf.Lerp(group.alpha, targetAlpha, Time.deltaTime * readyTime * speed);
            group.alpha = alpha;
        }

        float size = Mathf.Lerp(container.localScale.x, targetSize, Time.deltaTime * readyTime * speed);
        container.localScale = new(size, size, size);
    }
}