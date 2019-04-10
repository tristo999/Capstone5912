using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingTextController : MonoBehaviour
{
    private static float lifeDuration = 1.3f;
    private static float fadeDuration = 0.3f;
    private static Vector2 velocity = new Vector2(0, 0.002f);

    private Vector3 position3d;
    private Vector2 positionOffset = Vector2.zero;
    private Camera cam;

    public void AddToCanvas(Canvas canvas) {
        transform.SetParent(canvas.transform, false);
        Invoke("StartFadeOut", lifeDuration - fadeDuration);
        Invoke("RemoveSelf", lifeDuration);
    }

    public void SetPosition3d(Vector3 hitPosition3d, Camera cam) {
        this.cam = cam;
        position3d = hitPosition3d;

        transform.rotation = Quaternion.LookRotation(cam.transform.forward);
        UpdatePosition();
    }

    public void SetColor(Color color) {
        GetComponent<TextMeshProUGUI>().color = color;
    }

    public void SetText(string text) {
        GetComponent<TextMeshProUGUI>().text = text;
    }

    void Update() {
        UpdatePosition();
    }

    private void UpdatePosition() {
        positionOffset += velocity * Time.timeScale;

        Vector2 viewportPoint = (Vector2)cam.WorldToViewportPoint(position3d) + positionOffset;
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
