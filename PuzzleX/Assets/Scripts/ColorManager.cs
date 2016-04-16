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

    public Color GetColor(int i) {
        Color result = new Color();
        switch (i)
        {
            case 0:
                result = c0;
                break;
            case 1:
                result = c1;
                break;
            case 2:
                result = c2;
                break;
            case 3:
                result = c3;
                break;
            case 4:
                result = c4;
                break;
        }
        return result;
    }


}
