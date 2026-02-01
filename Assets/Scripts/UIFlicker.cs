using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFlicker : MonoBehaviour
{
    [SerializeField] private Graphic target;
    public float minWait = 2f;
    public float maxWait = 6f;

    void Awake()
    {
        if (target == null)
            target = GetComponent<Graphic>();

        if (target == null)
            Debug.LogError($"UIFlicker on {gameObject.name}: No Graphic found. Assign 'target' in the Inspector.");
    }

    void Start()
    {
        if (target != null)
            StartCoroutine(FlickerLoop());
    }

    IEnumerator FlickerLoop()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(Random.Range(minWait, maxWait));

            for (int i = 0; i < Random.Range(1, 4); i++)
            {
                target.enabled = false;
                yield return new WaitForSecondsRealtime(Random.Range(0.03f, 0.08f));

                target.enabled = true;
                yield return new WaitForSecondsRealtime(Random.Range(0.03f, 0.10f));
            }
        }
    }
}
