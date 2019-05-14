using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTexture : MonoBehaviour
{
    public RenderTexture Render;

    public bool IsRectOutOfScreen( Rect rect )
    {
        return rect.xMin < 0 || rect.yMin < 0 ||
               rect.xMin + rect.width >= Render.width || rect.yMin + rect.height >= Render.height;
    }

    public bool IsRectTooSmall( Rect rect )
    {
        return rect.width < 10 || rect.height < 10;
    }

    public bool IsValidRect( Rect rect )
    {
        return !IsRectOutOfScreen( rect ) && !IsRectTooSmall( rect );
    }
    
    /// <summary>
    /// Get screen image specified by rect
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public Texture2D GetRect( Rect rect )
    {
        if( !IsValidRect( rect ) )
            return null;
        
        //Create texure in BGRA texture format.
        //Render texture format is RGBA, B and R channels swap during texture copy
        Texture2D result = new Texture2D( (int)rect.width, (int)rect.height, TextureFormat.BGRA32, false );
        //Copy pixels from RenderTexture to target texture
        //Do not use texture.ReadPixels
        Graphics.CopyTexture( Render, 0, 0, (int)rect.xMin, (int)rect.yMin, (int)rect.width, (int)rect.height, result, 0, 0, 0, 0 );

        return result;
    }
}
