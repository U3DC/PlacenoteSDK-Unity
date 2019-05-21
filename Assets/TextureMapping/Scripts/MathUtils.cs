using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Ext
{
    public static Vector2 Clamp01( this Vector2 v )
    {
        v.x = Mathf.Clamp01( v.x );
        v.y = Mathf.Clamp01( v.y );
        return v;
    }
}
