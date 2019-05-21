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
        proj = GetComponent<Projector>();
    }

    public void RecalculateUVs()
    {
        GetComponent<MeshFilter>().sharedMesh.SetUVs( 0, proj.ProjectedVertices.ToList() );
    }
}
