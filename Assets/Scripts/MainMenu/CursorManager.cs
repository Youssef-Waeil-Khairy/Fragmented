using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorTexture;

    void Start()
    {
        Vector2 hotspot = new Vector2(5, 5);
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }
}