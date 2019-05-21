using System;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class ScreenTexture : MonoBehaviour
{
    public RenderTexture Render;

    public bool IsRectOutOfScreen( Rect rect )
    {
        return rect.xMin > Render.width || rect.yMin > Render.height ||
               rect.xMax < 0 || rect.yMax < 0;
    }

    public bool IsRectTooSmall( Rect rect )
    {
        return rect.width < 10 || rect.height < 10;
    }

    public bool IsValidRect( Rect rect )
    {
        return !IsRectOutOfScreen( rect ) && !IsRectTooSmall( rect );
    }

    private Rect GetSrcRect( Rect dst )
    {
        float xmin = Mathf.Max( 0, dst.xMin );
        float ymin = Mathf.Max( 0, dst.yMin );

        float xmax = Mathf.Min( dst.xMax, Render.width );
        float ymax = Mathf.Min( dst.yMax, Render.height );
        
        return Rect.MinMaxRect( xmin, ymin, xmax, ymax );
    }

    private Rect GetDstRect( Rect dst )
    {
        Vector2 min = Vector2.zero;
        if( dst.xMin < 0 )
            min.x = -dst.xMin;
        if( dst.yMin < 0 )
            min.y = -dst.yMin;

        Vector2 max = dst.max;

        if( dst.xMax > Render.width )
            max.x = Render.width;
        if( dst.yMax > Render.height )
            max.y = Render.height;

        return Rect.MinMaxRect( min.x, min.y, max.x, max.y );
    }
    
    /// <summary>
    /// Get screen image specified by rect
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public Texture2D GetRect( Rect rect )
    {
        if( !IsValidRect( rect ) )
            return new Texture2D( 1, 1 );

        Rect src_rect = GetSrcRect( rect );
        Rect dst_rect = GetDstRect( rect );
        
        //Create texure with BGRA texture format.
        //Render texture format is RGBA, I don't know why B and R channels swapping during texture copy
        Texture2D result = new Texture2D( (int)rect.width, (int)rect.height, TextureFormat.BGRA32, false );
        
        //Copy pixels from RenderTexture to target texture fast enough
        Graphics.CopyTexture( Render, 0, 0, (int)src_rect.xMin, (int)src_rect.yMin, (int)src_rect.width, (int)src_rect.height, result, 0, 0, (int)dst_rect.xMin, (int)dst_rect.yMin );

        return result;
    }
}
