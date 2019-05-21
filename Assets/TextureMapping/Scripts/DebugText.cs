using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    private static Text self_text;

    void Awake()
    {
        self_text = GetComponent<Text>();
    }

    public static void Log( object data )
    {
        string text = self_text.text;
        string[] splitted = text.Split( '\n' );
        if( splitted.Length > 100 )
            text = string.Join( "\n", splitted.ToList().GetRange( 0, 99 ) );
        self_text.text = $"{data}\n{text}";
    }
}
