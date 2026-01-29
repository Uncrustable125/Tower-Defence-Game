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
    public float popDuration = 0.3f;
    public float overshootMultiplier = 1.2f;

    [Header("Button Settings")]
    [SerializeField] Button retryButton;
    [SerializeField] Vector2 retryButtonSize = new Vector2(200, 60);

    [Header("Fade Settings")]
    public float textFadeDuration = 1f;
    public float canvasFadeDuration = 1f;

    [Header("HUD")]
    [SerializeField] TextMeshProUGUI healthText, goldText, stageText;

    public void UpdateLivesAndMoney(int lives, int money)
    {
        healthText.text = lives.ToString();
        goldText.text = money.ToString();
    }

    public void UpdateStage(int stage)
    {
        stageText.text = stage.ToString();
    }
    public void StartGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        gameOverCanvasGroup.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        
            
        panel.sizeDelta = Vector2.zero;
        retryButton.gameObject.SetActive(false);

        yield return StartCoroutine(
            PopInRect(panel, targetSize, popDuration, overshootMultiplier)
        );

        yield return StartCoroutine(FadeInText(textFadeDuration));

        retryButton.gameObject.SetActive(true);
        yield return StartCoroutine(
            PopInRect(
                retryButton.GetComponent<RectTransform>(),
                retryButtonSize,
                0.25f,
                1.15f
            )
        );
    }

    /// <summary>
    /// Generic pop-in animation for any RectTransform
    /// </summary>
    private IEnumerator PopInRect(RectTransform rect, Vector2 targetSize, float duration, float overshoot)
    {
        rect.sizeDelta = Vector2.zero;

        Vector2 overshootSize = targetSize * overshoot;
        float halfDuration = duration / 2f;
        float elapsed = 0f;

        // Grow to overshoot
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            rect.sizeDelta = Vector2.Lerp(Vector2.zero, overshootSize, t);
            yield return null;
        }

        elapsed = 0f;

        // Settle back to final size
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            rect.sizeDelta = Vector2.Lerp(overshootSize, targetSize, t);
            yield return null;
        }

        rect.sizeDelta = targetSize;
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
