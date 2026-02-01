using UnityEngine;
using TMPro;
using System.Collections;

public class TitleGlitch : MonoBehaviour
{
    [Header("Reference")]
    public TMP_Text titleText;

    [Header("Base Text")]
    [TextArea] public string baseTitle = "Face Hugger";

    [Header("Timing")]
    public float minWait = 2.5f;
    public float maxWait = 7.5f;

    [Header("Glitch Strength")]
    [Range(1, 10)] public int minCharsToGlitch = 1;
    [Range(1, 10)] public int maxCharsToGlitch = 3;
    public float glitchDurationMin = 0.06f;
    public float glitchDurationMax = 0.14f;

    [Header("Extra flavor")]
    public bool jitterPosition = true;
    public float jitterPixels = 1f; // keep small for pixel art
    public bool flashAlpha = true;
    [Range(0f, 1f)] public float flashToAlpha = 0.65f;

    const string GLITCH_CHARS = "@#$%&*+-=!?/\\[]{}<>";

    RectTransform rect;
    Vector3 basePos;
    Color baseColor;

    void Awake()
    {
        if (titleText == null) titleText = GetComponent<TMP_Text>();
        rect = GetComponent<RectTransform>();

        if (titleText != null)
        {
            if (!string.IsNullOrEmpty(baseTitle))
                titleText.text = baseTitle;

            baseColor = titleText.color;
        }

        if (rect != null) basePos = rect.localPosition;
    }

    void OnEnable()
    {
        if (titleText != null)
            StartCoroutine(GlitchLoop());
    }

    IEnumerator GlitchLoop()
    {
        while (enabled && titleText != null)
        {
            yield return new WaitForSecondsRealtime(Random.Range(minWait, maxWait));

            // pick duration + chars
            float dur = Random.Range(glitchDurationMin, glitchDurationMax);
            int count = Random.Range(minCharsToGlitch, maxCharsToGlitch + 1);

            // build glitched string
            string source = string.IsNullOrEmpty(baseTitle) ? titleText.text : baseTitle;
            char[] arr = source.ToCharArray();

            for (int i = 0; i < count; i++)
            {
                int idx = Random.Range(0, arr.Length);
                // avoid spaces so it stays readable
                if (arr[idx] == ' ') { i--; continue; }

                arr[idx] = GLITCH_CHARS[Random.Range(0, GLITCH_CHARS.Length)];
            }

            // apply glitch
            titleText.text = new string(arr);

            // optional alpha flash
            if (flashAlpha)
            {
                Color c = titleText.color;
                c.a = flashToAlpha;
                titleText.color = c;
            }

            // optional micro-jitter (very tiny)
            if (jitterPosition && rect != null)
            {
                rect.localPosition = basePos + new Vector3(
                    Random.Range(-jitterPixels, jitterPixels),
                    Random.Range(-jitterPixels, jitterPixels),
                    0f
                );
            }

            // hold
            yield return new WaitForSecondsRealtime(dur);

            // restore
            titleText.text = source;
            titleText.color = baseColor;
            if (rect != null) rect.localPosition = basePos;
        }
    }
}
