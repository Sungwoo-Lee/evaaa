using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPositioner : MonoBehaviour
{
    public RectTransform radialMeterRect;
    public RectTransform happyFaceRect;
    public RectTransform deadFaceRect;
    public float faceOffset = 0f; // Additional offset if needed
    public float faceScaleFactor = 0.2f; // Percentage of radial meter size

    void Update()
    {
        PositionAndScaleFaces();
    }

    void PositionAndScaleFaces()
    {
        if (radialMeterRect == null || happyFaceRect == null || deadFaceRect == null)
            return;

        // Get half the height of the radial meter
        float halfHeight = radialMeterRect.rect.height / 2 + faceOffset;

        // Calculate face size based on radial meter size
        float faceSize = radialMeterRect.rect.width * faceScaleFactor;

        // Set face sizes
        happyFaceRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, faceSize);
        happyFaceRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, faceSize);

        deadFaceRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, faceSize);
        deadFaceRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, faceSize);

        // Position Happy Face at the top (12 o'clock)
        happyFaceRect.anchoredPosition = new Vector2(0, halfHeight);

        // Position Dead Face at the bottom (6 o'clock)
        deadFaceRect.anchoredPosition = new Vector2(0, -halfHeight);
    }
}