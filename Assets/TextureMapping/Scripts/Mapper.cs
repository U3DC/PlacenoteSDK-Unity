using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using UnityEngine;


/// <summary>
/// Structure to hold mesh bock components
/// </summary>
public class MappingMesh
{
    public readonly MeshResolver resolver;
    public readonly MeshRenderer renderer;
    public readonly ViewUnwrapper unwrapper;
    public readonly Projector projector;

    private bool is_fixed;

    public MappingMesh( MeshFilter filter )
    {
        this.resolver = filter.GetComponent<MeshResolver>();
        this.unwrapper = filter.GetComponent<ViewUnwrapper>();
        this.projector = filter.GetComponent<Projector>();
        this.is_fixed = false;
        renderer = filter.GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Check to see if mapping is fixed for this block
    /// </summary>
    /// <returns></returns>
    public bool IsFixed()
    {
        return is_fixed;
    }

    /// <summary>
    /// Fix mapping for this block
    /// </summary>
    public void Fix()
    {
        is_fixed = true;
    }
}
    

[RequireComponent(typeof( ScreenTexture ))]
public class Mapper : MonoBehaviour
{
    /// <summary>
    /// Stores mesh blocks
    /// </summary>
    private Dictionary<string, MappingMesh> meshes;

    private bool is_working = false;

    private ScreenTexture screen;

    public void StartMapping()
    {
        is_working = true;
        //Initialize mesh blocks
        foreach( MeshFilter mf in GetComponentsInChildren<MeshFilter>() )
        {
            //GetComponentInChildren for some reason returns self component
            string name = mf.name;
            if( name == gameObject.name )
                continue;

            //If there are vertices in block, then destroy it
            if( mf.sharedMesh.vertices.Length == 0 )
            {
                Destroy( mf.gameObject );
                continue;
            }
            
            //name will be the same as texture name
            mf.gameObject.name = Guid.NewGuid().ToString();
            name = mf.gameObject.name;
            
            //Prepare required mesh data
            mf.gameObject.AddComponent<ViewUnwrapper>();
            mf.gameObject.AddComponent<MeshResolver>();
            mf.gameObject.AddComponent<Projector>().Prepare();
            
            meshes.Add( name, new MappingMesh(mf) );
        }
    }

    public void StopMapping()
    {
        is_working = false;
    }
    
    void Start()
    {
        meshes = new Dictionary<string, MappingMesh>();
        screen = GetComponent<ScreenTexture>();
    }

    private void Update()
    {
        if( !is_working )
            return;

        foreach( MappingMesh mesh in meshes.Values )
        {
            //If mesh block is already mapped then do nothing
            if( mesh.IsFixed() )
                continue;
            //Recalculate projected vertices for current frame
            mesh.projector.Recalculate();
            //Get mesh bounds projected on screen
            Rect rect = mesh.resolver.GetScreenRect();
            //If projected bounds is on screen
            if( screen.IsValidRect( rect ) )
            {
                Texture2D tex = screen.GetRect( rect );
                if( tex != null )
                {
                    //Recalculate uvs for this frame
                    mesh.unwrapper.RecalculateUVs();
                    //Destroy texture if exists.Textures will collect
                    //in memory and lead to large amounts of memory consumption
                    if( mesh.renderer.material.mainTexture != null )
                        Destroy( mesh.renderer.material.mainTexture );
                    mesh.renderer.material.mainTexture = tex;
                    //Fix mesh block
                    mesh.Fix();
                }
            }
        }
    }
}