using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tile : MonoBehaviour {
    //public GameObject prefab;
    static int Count = 0;

    public string text;
    public int type;
    public ColorManager cm;

    //private 

    public static Tile CreateTile() {
        GameObject prefab = Resources.Load("Tile") as GameObject;
        Tile t =Instantiate<GameObject>(prefab).GetComponent<Tile>();
        Count++;
        t.text = "" + Count; 
        return t;
    }

    public void Start() {
        Image theImage = GetComponent<Image>();
        theImage.color = ColorManager.Instance.c3;
    }

}
