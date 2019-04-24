using System.Collections;
using GoogleARCore;
using UnityEngine;

public class MidAirAnchor : MonoBehaviour
{
    private Vector3 init_pos;
    
    private IEnumerator Start()
    {
        init_pos = transform.position;
        yield return new WaitForSeconds( 5 );
        Pose pose = new Pose( init_pos, Quaternion.identity );
        Anchor anchor = Session.CreateAnchor( pose );
        transform.SetParent( anchor.transform );
        transform.localPosition = Vector3.zero;

    }
}
