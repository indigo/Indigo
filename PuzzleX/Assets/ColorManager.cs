using UnityEngine;
using System.Collections;

public class ColorManager : MonoBehaviour {

    public static ColorManager Instance;

    public Color c0;
    public Color c1;
    public Color c2;
    public Color c3;
    public Color c4;


    // Use this for initialization
    void Start () {
        Instance = this;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
