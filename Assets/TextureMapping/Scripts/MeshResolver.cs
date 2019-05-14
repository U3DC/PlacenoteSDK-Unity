using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshResolver : MonoBehaviour
{
    private Projector proj;
    private Vector2 screen_size;

    private void Start()
    {
        proj = GetComponent<Projector>();
        screen_size = new Vector2( Screen.width, Screen.height );
    }

    /// <summary>
    /// Gets projected vertices rect in screen space
    /// </summary>
    /// <returns></returns>
    public Rect GetScreenRect()
    {
        Vector2 min = proj.Min / proj.AspectCoef * screen_size;
        Vector2 max = proj.Max / proj.AspectCoef * screen_size;

        return new Rect( min, max - min );
    }
}
