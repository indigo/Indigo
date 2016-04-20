using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PanelController : MonoBehaviour{

    // visual references
    public int width;
    public int height;

    private int cellWidth;
    private int cellHeight;

    public List<Tile> tilesInHand;

    private List<Transform> columns;


    public void Start()
    {
        tilesInHand = new List<Tile>();

		//clamp width and height to be > 0
        width = width == 0 ? 1 : width;
        height = height == 0 ? 1 : height;

		cellWidth = (int)(transform.GetComponent<RectTransform>().rect.width/width);
		cellHeight = cellWidth;

		// use the existing column to create all of them
        GameObject colPrefab = transform.GetChild(0).gameObject;
		colPrefab.GetComponent<LayoutElement> ().preferredWidth = cellWidth;
        columns = new List<Transform>();
        for (int i = 0; i < width; ++i)
        {
            Transform t;
			if (i != 0) {
				t = GameObject.Instantiate (colPrefab).transform;
			} else {
				t = colPrefab.transform;
			}

            columns.Add(t);
            t.SetParent(transform,false);
			// set the column number
			t.GetComponent<Column> ().columnNumber = i;
        }


		Restart();
    }

    public void Restart(){
		// Populate Test tiles
		for (int i = 0; i < width; ++i) // column
		{
			for (int t = 0; t < columns [i].childCount; t++) {
				Destroy (columns [i].GetChild (t).gameObject);
			}
			for (int j = 0; j < height; ++j)
			{
				Tile tile = Tile.CreateTile(cellHeight);
				tile.columnNumber = i;
				tile.rowNumber = j;
				tile.transform.SetParent(columns[i],false);
			}
		}	
		InterfaceManager.Instance.ClearCounter ();
	}

	public void OnDrop(Column sourceColumn, Column destColumn){
		//Debug.Log(a
		if (sourceColumn != destColumn) {
			InterfaceManager.Instance.AddCounter (1);
		}
		List<Tile> connectedTiles = GetConnectedTiles (destColumn);
		if (connectedTiles.Count >= 3){
			for (int i = 0; i < connectedTiles.Count; i++) {
				RefreshTiles (connectedTiles [i]);
				Destroy (connectedTiles [i].gameObject);
			}
		}
	}

	public void RefreshTiles (Tile t){
		int c = t.columnNumber;
		for (int i = t.rowNumber ; i< columns[c].childCount ; i++ ){
			columns[c].GetChild(i).GetComponent<Tile>().rowNumber--;
			columns [c].GetChild (i).GetComponent<Tile> ().textUI.text = columns [c].GetChild (i).GetComponent<Tile> ().ToString ();
		}
	}


	// given a Column, returns all connected tiles in all directions but not diagonals
	public List<Tile> GetConnectedTiles(Column a){
		List<Tile> result = new List<Tile> ();
		List<Tile> openList = new List<Tile> ();
		int columnIndex = a.columnNumber;
		Tile currentTile = columns [columnIndex].GetChild (columns [columnIndex].childCount - 1).GetComponent<Tile>();
		// type is type of top of column
		int currentType = currentTile.type;
		// start with openlist = top of column
		openList.Add(currentTile);
		result.Add (currentTile);
		Tile tile;
		for (;openList.Count > 0;){
			// pop a tile from openlist
			currentTile = openList[0];
			openList.RemoveAt (0);
			// check all directions for the same type
			// up
			tile = GetTileAt(currentTile.columnNumber, currentTile.rowNumber - 1);
			if (tile != null && tile.type == currentType && !result.Contains(tile)) {
				// add this to the openList and result
				openList.Add(tile);
				result.Add (tile);
			}
			//left
			tile = GetTileAt(currentTile.columnNumber - 1, currentTile.rowNumber);
			if (tile != null && tile.type == currentType && !result.Contains(tile)) {
				// add this to the openList and result
				openList.Add(tile);
				result.Add (tile);
			}
			//right
			tile = GetTileAt(currentTile.columnNumber + 1, currentTile.rowNumber);
			if (tile != null && tile.type == currentType && !result.Contains(tile)) {
				// add this to the openList and result
				openList.Add(tile);
				result.Add (tile);
			}
			//up
			tile = GetTileAt(currentTile.columnNumber , currentTile.rowNumber + 1);
			if (tile != null && tile.type == currentType && !result.Contains(tile)) {
				// add this to the openList and result
				openList.Add(tile);
				result.Add (tile);
			}

		}
		return result;
	}

	private Tile GetTileAt(int column, int index){
		if (column < 0 || column >= columns.Count || index < 0) {
			Debug.Log ("out : " + column + " index: " + index);
			return null;
		}
		Column c = columns [column].GetComponent<Column>();
		int childCount = c.transform.childCount;
		if (index < childCount) {
			return c.transform.GetChild (index).GetComponent<Tile>();
		} else {
			return null;
		}
	}

}