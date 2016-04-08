using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

    //public GameObject prefab;

    public static Tile CreateTile() {
        Tile t = Resources.Load("Tile") as Tile;
        return t;
    }

}
