using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
    //public GameObject prefab;
    static int Count = 0;

    public string text;

    public static Tile CreateTile() {
        GameObject prefab = Resources.Load("Tile") as GameObject;
        Tile t =Instantiate<GameObject>(prefab).GetComponent<Tile>();
        Count++;
        t.text = "" + Count; 
        return t;
    }
}
