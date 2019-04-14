using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingTextController : MonoBehaviour
{
    private static readonly float lifeDuration = 0.9f;
    private static readonly float fadeDuration = 0.4f;
    private static Vector2 velocity = new Vector2(0, 0.00225f);
    private static readonly float growthFactor = 0.4f;

    private Vector3 position3d = new Vector3(-999999, -999999, -999999);
    private Vector2 position2d;
    private Vector2 positionOffset = Vector2.zero;
    private Camera cam;

    public void AddToCanvas(Canvas canvas) {
        transform.SetParent(canvas.transform, false);
        Invoke("StartFadeOut", lifeDuration - fadeDuration);
        Invoke("RemoveSelf", lifeDuration);
    }

    public void SetPosition3d(Vector3 hitPosition3d, float stackPositionOffset, Camera cam) {
        position3d = hitPosition3d;
        positionOffset += stackPositionOffset * new Vector2(0f, 0.048f);
        SetPositionCam(cam);
    }

    public void SetPosition2d(Vector3 hitPosition2d, Camera cam) {
        position2d = hitPosition2d;
        SetPositionCam(cam);
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

    private void SetPositionCam(Camera cam) {
        this.cam = cam;
        transform.rotation = Quaternion.LookRotation(cam.transform.forward);
        UpdatePosition();
    }

    private void UpdatePosition() {
        positionOffset += velocity * Time.timeScale;

        Vector2 viewportPoint;
        if (position3d != new Vector3(-999999, -999999, -999999)) {
            viewportPoint = (Vector2)cam.WorldToViewportPoint(position3d) + positionOffset;
        } else {
            viewportPoint = position2d + positionOffset;
        }
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchorMin = viewportPoint;
        rectTransform.anchorMax = viewportPoint;

        float scaleIncrease = growthFactor * Time.deltaTime / lifeDuration;
        rectTransform.localScale = rectTransform.localScale + new Vector3(scaleIncrease, scaleIncrease, 0);
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
