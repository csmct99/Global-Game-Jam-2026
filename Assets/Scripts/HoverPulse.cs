using UnityEngine;
using UnityEngine.EventSystems;

public class HoverPulse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 baseScale;
    private bool hovering;

    void Start() => baseScale = transform.localScale;

    void Update()
    {
        if (!hovering) return;
        float s = 1f + Mathf.Sin(Time.unscaledTime * 8f) * 0.02f;
        transform.localScale = baseScale * s;
    }

    public void OnPointerEnter(PointerEventData eventData) => hovering = true;
    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
        transform.localScale = baseScale;
    }
}
