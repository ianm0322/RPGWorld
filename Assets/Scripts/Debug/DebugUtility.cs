using UnityEngine;

public static class DebugUtility
{
    public static void DrawBox(Vector2 point, Vector2 size) => DrawBox(point, size, Color.white);

    public static void DrawBox(Vector2 point, Vector2 size, Color color) => DrawBox(point, size, color, Time.deltaTime);

    public static void DrawBox(Vector2 point, Vector2 size, Color color, float duration)
    {
        size *= 0.5f;
        Debug.DrawLine(new Vector3(point.x - size.x, point.y - size.y), new Vector3(point.x + size.x, point.y - size.y), color, duration);
        Debug.DrawLine(new Vector3(point.x + size.x, point.y - size.y), new Vector3(point.x + size.x, point.y + size.y), color, duration);
        Debug.DrawLine(new Vector3(point.x + size.x, point.y + size.y), new Vector3(point.x - size.x, point.y + size.y), color, duration);
        Debug.DrawLine(new Vector3(point.x - size.x, point.y + size.y), new Vector3(point.x - size.x, point.y - size.y), color, duration);
    }
}
