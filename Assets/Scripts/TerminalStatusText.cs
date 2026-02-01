using UnityEngine;
using TMPro;
using System.Collections;

public class TerminalStatusText : MonoBehaviour
{
    [Header("Reference")]
    public TMP_Text text;

    [Header("Message Rotation")]
    public float minMessageWait = 2.5f;
    public float maxMessageWait = 6.5f;

    [TextArea]
    public string[] messages =
    {
        "SIGNAL: UNSTABLE",
        "CONTAINMENT: ACTIVE",
        "NODE: SECTOR C-07",
        "BIOSCAN: RUNNING",
        "ACCESS: LIMITED",
        "HOST COUNT: ███",
        "FOREIGN MASS DETECTED",
        "THERMAL: NOMINAL",
        "CAM FEED: DEGRADED"
    };

    [Header("Glitch (subtle)")]
    public bool enableGlitch = true;
    public float minGlitchWait = 3.5f;
    public float maxGlitchWait = 12f;
    public float glitchDuration = 0.08f;
    public int minGlitchChars = 1;
    public int maxGlitchChars = 3;

    [Header("Freeze moments (optional)")]
    public bool enableFreeze = true;
    public float minFreezeWait = 10f;
    public float maxFreezeWait = 25f;
    public float freezeDuration = 0.3f;

    private string currentBase;
    private bool frozen;

    const string GLITCH_CHARS = "@#$%&*+-=!?/\\[]{}<>";

    void Awake()
    {
        if (text == null) text = GetComponent<TMP_Text>();
        if (text == null)
            Debug.LogError($"TerminalStatusText on {gameObject.name}: No TMP_Text found.");

        if (messages != null && messages.Length > 0)
        {
            currentBase = messages[0];
            if (text != null) text.text = currentBase;
        }
    }

    void OnEnable()
    {
        if (text == null) return;

        StartCoroutine(MessageLoop());

        if (enableGlitch) StartCoroutine(GlitchLoop());
        if (enableFreeze) StartCoroutine(FreezeLoop());
    }

    IEnumerator MessageLoop()
    {
        while (enabled && text != null)
        {
            yield return new WaitForSecondsRealtime(Random.Range(minMessageWait, maxMessageWait));

            if (frozen) continue;
            if (messages == null || messages.Length == 0) continue;

            currentBase = messages[Random.Range(0, messages.Length)];
            text.text = currentBase;
        }
    }

    IEnumerator GlitchLoop()
    {
        while (enabled && text != null)
        {
            yield return new WaitForSecondsRealtime(Random.Range(minGlitchWait, maxGlitchWait));

            if (frozen) continue;
            if (string.IsNullOrEmpty(currentBase)) continue;

            char[] arr = currentBase.ToCharArray();
            int count = Random.Range(minGlitchChars, maxGlitchChars + 1);

            for (int i = 0; i < count; i++)
            {
                int idx = Random.Range(0, arr.Length);
                arr[idx] = GLITCH_CHARS[Random.Range(0, GLITCH_CHARS.Length)];
            }

            text.text = new string(arr);
            yield return new WaitForSecondsRealtime(glitchDuration);
            text.text = currentBase;
        }
    }

    IEnumerator FreezeLoop()
    {
        while (enabled && text != null)
        {
            yield return new WaitForSecondsRealtime(Random.Range(minFreezeWait, maxFreezeWait));

            frozen = true;
            // optional: add a little indicator during freeze
            // text.text = currentBase + "  [HOLD]";

            yield return new WaitForSecondsRealtime(freezeDuration);

            frozen = false;
            text.text = currentBase;
        }
    }
}
