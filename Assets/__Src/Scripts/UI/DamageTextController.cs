using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageTextController : MonoBehaviour
{
    private static float lifeDuration = 1.3f;
    private static float fadeDuration = 0.3f;
    private static Vector2 velocity = new Vector2(0, 0.0015f);

    private Vector3 position3d;
    private Vector2 positionOffset = Vector2.zero;

    public void AddToCanvas(Canvas canvas) {
        transform.SetParent(canvas.transform, false);
        Invoke("StartFadeOut", lifeDuration - fadeDuration);
        Invoke("RemoveSelf", lifeDuration);
    }

    public void SetPosition(Vector3 hitPosition3d) {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

        position3d = hitPosition3d;
        UpdatePosition();
    }

    public void SetDamage(float damage) {
        GetComponent<TextMeshProUGUI>().text = "-" + damage;
    }

    void Update() {
        UpdatePosition();
    }

    private void UpdatePosition() {
        positionOffset += velocity * Time.timeScale;

        Vector2 viewportPoint = (Vector2)Camera.main.WorldToViewportPoint(position3d) + positionOffset;
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchorMin = viewportPoint;
        rectTransform.anchorMax = viewportPoint;
    }
    
    private void StartFadeOut() {
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        Color startColor = text.color;
        Color endColor = new Color(text.color.r, text.color.g, text.color.b, 0);
        StartCoroutine(UpdateTranslucency(text, startColor, endColor, fadeDuration));
    }

    IEnumerator UpdateTranslucency(TextMeshProUGUI text, Color start, Color end, float duration) {
        for (float t = 0f; t < duration; t += Time.deltaTime) {
            float normalizedTime = t / duration;
            text.color = Color.Lerp(start, end, normalizedTime);
            yield return null;
        }
        text.color = end;
    }

    private void RemoveSelf() {
        Destroy(gameObject);
    }
}
