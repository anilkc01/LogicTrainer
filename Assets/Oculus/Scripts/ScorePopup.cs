using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    public static ScorePopup Instance;
    public TextMeshPro text;
    public float displayTime = 0.7f;

    private Coroutine activeCoroutine;

    void Awake()
    {
        Instance = this;
        if (text == null) text = GetComponent<TextMeshPro>();
        text.text = "";
    }

    public void Show(string label, bool isPositive)
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(ShowRoutine(label, isPositive));
    }

    System.Collections.IEnumerator ShowRoutine(string label, bool isPositive)
    {
        text.color = isPositive ? Color.green : Color.red;
        text.text = label;

        yield return new WaitForSeconds(displayTime);

        text.text = "";
    }
}