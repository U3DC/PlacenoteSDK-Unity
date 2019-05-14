using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projector : MonoBehaviour
{
    /// <summary>
    /// Resulting vertices (UVs)
    /// </summary>
    public Vector2[] ProjectedVertices{ get; private set; }

    private Vector2 min;
    private Vector2 max;

    /// <summary>
    /// Bottom left of the projected vertices bounds
    /// </summary>
    public Vector2 Min => min;
    /// <summary>
    /// Top right of the projected vertices bounds
    /// </summary>
    public Vector2 Max => max;

    /// <summary>
    /// Raw vertices with gameobject transformations
    /// </summary>
    private List<Vector3> vertices;

    private Vector2 aspect_coef;

    /// <summary>
    /// Screen aspect ratio coefficient
    /// </summary>
    public Vector2 AspectCoef => aspect_coef;

    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
        aspect_coef = new Vector2( 1, (float) Screen.height / Screen.width );
    }

    /// <summary>
    /// Prepares vertices after mesh is finalized
    /// </summary>
    public void Prepare()
    {
        vertices = GetComponent<MeshFilter>().sharedMesh.vertices.Select( v => transform.position + transform.rotation * v ).ToList();
        ProjectedVertices = new Vector2[vertices.Count];
    }

    /// <summary>
    /// Recalculates projected vertices and Min/Max
    /// </summary>
    public void Recalculate()
    {
        min = new Vector2( float.MaxValue, float.MaxValue );
        max = new Vector2( float.MinValue, float.MinValue );
        for( int i = 0; i < vertices.Count; i++ )
        {
            ProjectedVertices[i] = camera.WorldToViewportPoint( vertices[i] ) * aspect_coef;
            
            if( ProjectedVertices[i].x < min.x )
                min.x = ProjectedVertices[i].x;
            if( ProjectedVertices[i].y < min.y )
                min.y = ProjectedVertices[i].y;
            
            if( ProjectedVertices[i].x > max.x )
                max.x = ProjectedVertices[i].x;
            if( ProjectedVertices[i].y > max.y )
                max.y = ProjectedVertices[i].y;
        }

        Vector2 dv = Vector2.one / ( max - min );
        
        for( int i = 0; i < vertices.Count; i++ )
        {
            ProjectedVertices[i] -= min;
            ProjectedVertices[i] *= dv;
        }
    }
}