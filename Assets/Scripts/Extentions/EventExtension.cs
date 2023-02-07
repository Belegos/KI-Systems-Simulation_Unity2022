using UnityEngine;

public static class EventExtension
{
    public static bool IsLeftMouseButtonDown(this Event evt)
    {
        return evt.type == EventType.MouseDown && evt.button == 0;
    }
}
