using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class CubeSideExtractor : MonoBehaviour
{
    public CubeResolver Resolver;
    public CubemapGenerator Generator;
    private RenderTexture rt;
    
    private Vector2Int screen_size = new Vector2Int(1440, 2560);

    private bool is_manual_mode = false;
    
    void Start()
    {
        rt = new RenderTexture( screen_size.x, screen_size.y, 24 );
        RenderTexture.active = rt;
    }

    public void SetMode( bool is_manual )
    {
        is_manual_mode = is_manual;
    }

    private void OnPostRender()
    {
        Face face = Resolver.CurrentFace;
        if( face != Face.NONE && !Generator.IsFixed( face ) )
        {
            Rect screen_rect = Resolver.ScreenRect;
            if( screen_rect.position.x < 0 || screen_rect.position.y < 0 || 
                screen_rect.width < 10 ||
                screen_rect.height < 10 ||
                screen_rect.width >= screen_size.x ||
                screen_rect.height >= screen_size.y )
                return;
            Texture2D result = new Texture2D( Mathf.FloorToInt( screen_rect.width ), Mathf.FloorToInt( screen_rect.height ) );
            result.ReadPixels( screen_rect, 0, 0 );
            result.Apply();
            TextureScale.Bilinear( result, 512, 512 );
            Generator.ApplySide( face, result );
            float angle = Resolver.CurrentAngle;
            if( !is_manual_mode && angle < 10 )
                FixCurrentFace();
        }
    }

    public void FixCurrentFace()
    {
        Generator.Fix( Resolver.CurrentFace );
    }
}
