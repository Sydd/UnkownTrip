using UnityEngine;
using UnityEngine.UI;

public class FadeInImage : MonoBehaviour
{
    [Header("Image to Fade")]
    public Image image;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    private void Start()
    {
        if (image == null)
            image = GetComponent<Image>();

        // Start image fully transparent
        Color c = image.color;
        c.a = 0f;
        image.color = c;

        StartCoroutine(FadeIn());
    }

    private System.Collections.IEnumerator FadeIn()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            float alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            Color c = image.color;
            c.a = alpha;
            image.color = c;

            yield return null;
        }

        // Ensure full visibility at the end
        Color finalColor = image.color;
        finalColor.a = 1f;
        image.color = finalColor;
    }
}
