using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Face
{
    NONE,
    FRONT,
    BACK,
    RIGHT,
    LEFT,
    TOP,
    BOTTOM
}

public class CubeResolver : MonoBehaviour
{
    public Camera RenderCamera;
    
    public Face CurrentFace
    {
        get;
        private set;
    }

    public float CurrentAngle
    {
        get;
        private set;
    }

    public Rect ScreenRect
    {
        get;
        private set;
    }

    private readonly List<int> front_indices = new List<int>{ 6, 7 };
    private readonly List<int> back_indices = new List<int>{ 10, 11 };
    private readonly List<int> right_indices = new List<int>{ 4, 5 };
    private readonly List<int> left_indices = new List<int>{ 8, 9 };
    private readonly List<int> top_indices = new List<int>{ 2, 3 };
    private readonly List<int> bottom_indices = new List<int>{ 0, 1 };

    private MeshCollider collider;
    private Mesh mesh;
    private int[] triangles;

    private void Start()
    {
        collider = GetComponent<MeshCollider>();
        mesh = collider.sharedMesh;
        triangles = mesh.GetTriangles( 0 );
    }
    
    private bool Approximately( Vector3 v1, Vector3 v2 )
    {
        return Mathf.Approximately( v1.x, v2.x ) && 
               Mathf.Approximately( v1.y, v2.y ) &&
               Mathf.Approximately( v1.z, v2.z );
    }

    private Vector2 ToVector2( Vector3 v )
    {
        return new Vector2(v.x, v.y);
    }

    private void Update()
    {
        Ray ray = RenderCamera.ViewportPointToRay( new Vector2( 0.5f, 0.5f ) );
        if( collider.Raycast( ray, out var hit, 10 ) )
        {
            if( front_indices.Contains( hit.triangleIndex ) )
                CurrentFace = Face.FRONT;
            else if( back_indices.Contains( hit.triangleIndex ) )
                CurrentFace = Face.BACK;
            else if( right_indices.Contains( hit.triangleIndex ) )
                CurrentFace = Face.RIGHT;
            else if( left_indices.Contains( hit.triangleIndex ) )
                CurrentFace = Face.LEFT;
            else if( top_indices.Contains( hit.triangleIndex ) )
                CurrentFace = Face.TOP;
            else
                CurrentFace = Face.BOTTOM;
        }
        else
            CurrentFace = Face.NONE;

        if( CurrentFace != Face.NONE )
        {
            CurrentAngle = Vector3.Angle( -hit.normal, RenderCamera.transform.forward );
            List<Vector3> verts = new List<Vector3>();
            int first_triangle_index = hit.triangleIndex;
            int second_triangle_index = hit.triangleIndex % 2 == 0 ? hit.triangleIndex + 1 : hit.triangleIndex - 1;
            
            verts.Add( mesh.vertices[ triangles[first_triangle_index * 3] ] );
            verts.Add( mesh.vertices[ triangles[first_triangle_index * 3 + 1] ] );
            verts.Add( mesh.vertices[ triangles[first_triangle_index * 3 + 2] ] );
            
            if( !verts.Any( v => Approximately( v, mesh.vertices[triangles[second_triangle_index * 3]] ) ) )
                verts.Add( mesh.vertices[ triangles[second_triangle_index * 3] ] );
            if( !verts.Any( v => Approximately( v, mesh.vertices[triangles[second_triangle_index * 3 + 1]] ) ) )
                verts.Add( mesh.vertices[ triangles[second_triangle_index * 3 + 1] ] );
            if( !verts.Any( v => Approximately( v, mesh.vertices[triangles[second_triangle_index * 3 + 2]] ) ) )
                verts.Add( mesh.vertices[ triangles[second_triangle_index * 3 + 2] ] );

            List<Vector2> screen_points = verts.Select( v => ToVector2( RenderCamera.WorldToScreenPoint( transform.position + transform.rotation * v ) ) ).ToList();

            Vector2 max = screen_points.OrderBy( v => v.sqrMagnitude ).Reverse().ToList()[0];
            Vector2 min = screen_points.OrderBy( v => v.sqrMagnitude ).ToList()[0];
            
            ScreenRect = new Rect( min, max - min );
        }
    }
}
