using System.Collections;
using System.Linq;
using UnityEngine;

public class FVDummy : MonoBehaviour
{
    public Mesh[] SwappingMeshes;
    
    IEnumerator Start()
    {
        //foreach( Transform child in transform )
        //{
        //    child.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        //    child.gameObject.SetActive( false );
        //}

        //yield return new WaitForSeconds( 1 );

        //StartCoroutine( SwapMeshes() );
        
        foreach( Transform child in transform )
        {
            //child.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            child.gameObject.SetActive( true );
            FeaturesVisualizer.OnMeshBlockAdded.Invoke( child.gameObject );
            yield return new WaitForSeconds( 1 );
        }
    }

    IEnumerator SwapMeshes()
    {
        while( true )
        {
            foreach( Transform child in transform )
            {
                Mesh sm = SwappingMeshes[Random.Range( 0, SwappingMeshes.Length )];
                
                child.GetComponent<MeshFilter>().sharedMesh.Clear();
                child.GetComponent<MeshFilter>().sharedMesh.SetVertices( sm.vertices.ToList() );
                child.GetComponent<MeshFilter>().sharedMesh.SetIndices( sm.GetIndices( 0 ), MeshTopology.Triangles, 0 );
                
            }
            yield return new WaitForSeconds( 1 );
        }
    }
}

