using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ViewUnwrapper : MonoBehaviour
{
    private Mesh mesh;

    private Projector proj;
    
    private void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        proj = GetComponent<Projector>();
    }

    public void RecalculateUVs()
    {
        mesh.SetUVs( 0, proj.ProjectedVertices.ToList() );
    }
}
