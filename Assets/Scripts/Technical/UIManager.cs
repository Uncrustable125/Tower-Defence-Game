using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panel Settings")]
    public RectTransform panel;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] CanvasGroup gameOverCanvasGroup;
    public Vector2 targetSize = new Vector2(600, 400);
    public float popDuration = 0.3f;    // Very quick for snappy effect
    public float overshootMultiplier = 1.2f; // How much it overshoots

    [Header("Fade Settings")]
    public float textFadeDuration = 1f;
    public float canvasFadeDuration = 1f;

    /// <summary>
    /// Starts the Game Over sequence
    /// </summary>
    public void StartGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // Reset panel to zero size
        panel.sizeDelta = Vector2.zero;

        // Pop panel in with overshoot
        yield return StartCoroutine(PopInPanel(targetSize, popDuration, overshootMultiplier));

        // Fade in text
        yield return StartCoroutine(FadeInText(textFadeDuration));

        // Fade in the canvas group (optional extra elements)
       // yield return StartCoroutine(FadeInCanvasGroup(canvasFadeDuration));
    }

    /// <summary>
    /// Animates the panel popping in with overshoot
    /// </summary>
    private IEnumerator PopInPanel(Vector2 target, float duration, float overshoot)
    {
        Vector2 start = Vector2.zero;
        Vector2 overshootSize = target * overshoot;
        float halfDuration = duration / 2f;
        float elapsed = 0f;

        // Scale up to overshoot
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            panel.sizeDelta = Vector2.Lerp(start, overshootSize, t);
            yield return null;
        }

        // Scale down to final size
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            panel.sizeDelta = Vector2.Lerp(overshootSize, target, t);
            yield return null;
        }

        panel.sizeDelta = target; // Ensure exact final size
    }

    private IEnumerator FadeInText(float duration)
    {
        Color c = gameOverText.color;
        c.a = 0f;
        gameOverText.color = c;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / duration);
            gameOverText.color = c;
            yield return null;
        }

        c.a = 1f;
        gameOverText.color = c;
    }

    private IEnumerator FadeInCanvasGroup(float duration)
    {
        gameOverCanvasGroup.alpha = 0f;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            gameOverCanvasGroup.alpha = Mathf.Clamp01(t / duration);
            yield return null;
        }

        gameOverCanvasGroup.alpha = 1f;
    }
}
