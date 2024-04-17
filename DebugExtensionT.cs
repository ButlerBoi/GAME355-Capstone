using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugExtension
{
    public static void DrawCircle(Vector3 center, Color color, float radius)
    {
        DrawCircle(center, color, radius, 0.1f);
    }

    public static void DrawCircle(Vector3 center, Color color, float radius, float duration)
    {
        DrawCircle(center, color, radius, duration, false);
    }

    public static void DrawCircle(Vector3 center, Color color, float radius, float duration, bool depthTest)
    {
        Vector3 prevPos = center + new Vector3(radius, 0, 0);
        for (int i = 0; i < 30; i++)
        {
            float angle = (float)(i + 1) / 30.0f * Mathf.PI * 2.0f;
            Vector3 newPos = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Debug.DrawLine(prevPos, newPos, color, duration, depthTest);
            prevPos = newPos;
        }
    }
}