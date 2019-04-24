using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CubemapGenerator : MonoBehaviour
{
    private Texture2D result;
    private Dictionary<Face, bool> fixes;

    public void ResetFixes()
    {
        fixes = new Dictionary<Face, bool>
        {
            {Face.FRONT, false},
            {Face.BACK, false},
            {Face.RIGHT, false},
            {Face.LEFT, false},
            {Face.TOP, false},
            {Face.BOTTOM, false}
        };
    }

    public void Fix( Face face )
    {
        fixes[face] = true;
    }

    public bool IsFixed( Face face )
    {
        return fixes[face];
    }
    
    private Dictionary<Face, Vector2Int> positions = new Dictionary<Face, Vector2Int>
    {
        { Face.FRONT, new Vector2Int( 511, 511 ) },
        { Face.BACK, new Vector2Int( 1535, 511 ) },
        { Face.RIGHT, new Vector2Int( 1023, 511 ) },
        { Face.LEFT, new Vector2Int( 0, 511 ) },
        { Face.TOP, new Vector2Int( 511, 1023 ) },
        { Face.BOTTOM, new Vector2Int( 511, 0 ) }
    };
    
    private void Start()
    {
        result = new Texture2D( 2048, 1536 );
        GetComponent<MeshRenderer>().material.mainTexture = result;
        ResetFixes();
    }

    public void ApplySide( Face face, Texture2D texture )
    {
        if( face == Face.NONE )
            throw new Exception( "Cannot apply texture for None face" );
        
        if( texture.width != 512 || texture.height != 512 )
            throw new Exception( "Resampling not supported" );


        Vector2Int pos = positions[face];
        
        result.SetPixels( pos.x, pos.y, 512, 512, texture.GetPixels() );
        result.Apply();
    }

    public void SaveOnDisk()
    {
        File.WriteAllBytes( Path.Combine( Application.persistentDataPath, "cubemap.png" ), result.EncodeToPNG() );
    }
}
