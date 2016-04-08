using UnityEngine;
using System.Collections;

public static class TileFactory {
    public static Tile GetTile() {
        Tile t = Tile.CreateTile();
        return t;
    }
}
