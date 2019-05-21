using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using UnityEngine;

/// <summary>
/// Structure for holding mesh bock related components
/// </summary>
public class MappingMesh
{
    public readonly MeshResolver resolver;
    public readonly MeshRenderer renderer;
    public readonly ViewUnwrapper unwrapper;
    public readonly Projector projector;

    public MappingMesh( GameObject go )
    {
        this.resolver = go.GetComponent<MeshResolver>();
        this.unwrapper = go.GetComponent<ViewUnwrapper>();
        this.projector = go.GetComponent<Projector>();
        this.renderer = go.GetComponent<MeshRenderer>();
    }
}

[RequireComponent( typeof( ScreenTexture ) )]
public class Mapper : MonoBehaviour
{
    /// <summary>
    /// Stores mesh blocks
    /// </summary>
    private List<MappingMesh> meshes;

    private ScreenTexture screen;

    public void AddMeshBlock( GameObject mb )
    {
        mb.AddComponent<Projector>();
        mb.AddComponent<ViewUnwrapper>();
        mb.AddComponent<MeshResolver>();

        meshes.Add( new MappingMesh( mb ) );
    }

    void Start()
    {
        meshes = new List<MappingMesh>();
        screen = GetComponent<ScreenTexture>();
        
        StartCoroutine( MapCoroutine() );

        FeaturesVisualizer.OnMeshBlockAdded += AddMeshBlock;
    }

    private IEnumerator MapCoroutine()
    {
        while( true )
        {
            //For all mesh blocks
            foreach( MappingMesh mesh in meshes )
            {
                if( !mesh.renderer.isVisible )
                    continue;
                mesh.projector.Recalculate();
                //Get mesh bounds projected on screen
                Rect rect = mesh.resolver.GetScreenRect();
                //If projected bounds is on screen
                //if( screen.IsValidRect( rect ) )
                //{
                    Texture2D tex = screen.GetRect( rect );
                    if( tex != null )
                    {
                        //Recalculate uvs for this frame
                        mesh.unwrapper.RecalculateUVs();
                        //Destroy texture if exists. Without this textures are collecting
                        //in memory that leads to huge memory consumption
                        if( mesh.renderer.material.mainTexture != null )
                            Destroy( mesh.renderer.material.mainTexture );
                        mesh.renderer.material.mainTexture = tex;
                    }
                //}
            }

            yield return null;
        }
    }
}