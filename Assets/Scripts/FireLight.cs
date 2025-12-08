using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FireLight : MonoBehaviour
{
    public Image stove_off;
    public Image stove_on;

    public float fadeDuration = 0.5f; // 0.5 seconds fade

    void Start()
    {
        // Start with ON visible
        SetAlpha(stove_on, 1f);
        SetAlpha(stove_off, 0f);

        StartCoroutine(FireLoop());
    }

    void SetAlpha(Image img, float value)
    {
        Color c = img.color;
        c.a = value;
        img.color = c;
    }

    IEnumerator FireLoop()
    {
        while (true)
        {
            // Stay ON for 2 seconds
            yield return new WaitForSeconds(3f);

            yield return FadeImages(stove_on, stove_off);

            yield return new WaitForSeconds(0.01f);

            yield return FadeImages(stove_off, stove_on);
        }
    }

    IEnumerator FadeImages(Image fromImg, Image toImg)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;

            SetAlpha(fromImg, 1f - alpha);
            SetAlpha(toImg, alpha);

            yield return null;
        }

        // Ensure final state
        SetAlpha(fromImg, 0f);
        SetAlpha(toImg, 1f);
    }
}
