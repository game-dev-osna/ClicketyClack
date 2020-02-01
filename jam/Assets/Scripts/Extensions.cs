using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethodds
{
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T comp = gameObject.GetComponent<T>();

        if (comp == null)     
            comp = gameObject.AddComponent<T>() as T;

        return comp;
    }

    public static T GetOrAddComponent<T>(this Component component) where T : Component
    {
        return component.gameObject.GetOrAddComponent<T>();
    }

    public static float Remap (this float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
