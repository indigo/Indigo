﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class BoardController : MonoBehaviour{

    // visual references
    public int width = 1;
    public int heightMatrix;

    public Transform bg;
    private int cellWidth;
    private int cellHeight;

    // 
    public GameObject GameMatrix;

    public GameObject Hud;
    private InterfaceManager interfaceManager;
    // shouldn't be public
    public List<Tile> tilesInHand;
    private List<Transform> columns;

    

    public void Awake() {
        interfaceManager = Hud.GetComponent<InterfaceManager>();
    }

    public void Start()
    {
        tilesInHand = new List<Tile>();

		//clamp width to be > 0

		cellWidth = (int)(GameMatrix.transform.GetComponent<RectTransform>().rect.width/width);
		cellHeight = cellWidth;
        bg.GetComponent<RectTransform>().sizeDelta = new Vector2(0, cellHeight* heightMatrix + 10);
        Debug.Log(bg.GetComponent<RectTransform>().sizeDelta);
        // use the existing column to create all of them
        GameObject colPrefab = GameMatrix.transform.GetChild(0).gameObject;
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
            t.SetParent(GameMatrix.transform,false);
			// set the column number
			t.GetComponent<Column> ().columnNumber = i;
        }

		StartCoroutine( Restart());
    }

	IEnumerator Restart(){
		// Populate Test tiles
		CleanColumns ();
		yield return new WaitForEndOfFrame();
		StartCoroutine( AddNInLineAtRandom (4));
        interfaceManager.ClearCounter ();
	}

	public void OnButtonRestart(){
		StartCoroutine (Restart ());
	}

    // will throw N new blocks spread out randomly in the line, from the bottom
	public IEnumerator AddNInLineAtRandom(int qty){
		List<int> l = new List<int> ();
		List<int> firstInts = new List<int> ();
		for (int k = 0; k < width; k++) {
			firstInts.Add (k);
		}
        for (int k = 0; k < qty; k++)
        {
            int index = UnityEngine.Random.Range(0, width - k);
            l.Add(firstInts[index]);
            firstInts.RemoveAt(index);
        }
        l.Sort();
        for (int j = 0 ; j < qty  ; ++j){
            AddOneInColumn(l[j]);
            yield return new WaitForSeconds(.2f);
        }
	}

    public void AddOneInColumn(int column) {
        Tile tile = Tile.CreateTile(cellHeight);
        tile.rowNumber = columns[column].childCount;

        if (tile.rowNumber > heightMatrix - 1)
        {
            Debug.Log("Death on Add with type : " + tile.type);
            StartCoroutine(OnColorTouchDeath(tile.type));
            return;
        }
            tile.columnNumber = column;
            tile.transform.SetParent(columns[column], false);
            tile.transform.SetAsLastSibling(); // bottom

    }

    // erase all tiles in each column
    public void CleanColumns(){
		for (int i = 0; i < width; ++i) 
		{
			for (int t = 0; t < columns [i].childCount; t++) {
				Destroy (columns [i].GetChild (t).gameObject);
			}
		}
	}

    public IEnumerator OnColorTouchDeath(int type) {
        List<Tile> tilesOfType = getTilesOfType(type);
        // should delete the tiles in hand too
        for (int i = 0; i < tilesOfType.Count; i++) {
            DestroyTile(tilesOfType[i]);
            yield return 0;
        }
        interfaceManager.ClearCounter();
    }

	public IEnumerator OnDrop(Column sourceColumn, Column destColumn){
		if (sourceColumn != destColumn) {
			List<Tile> connectedTiles = GetConnectedTiles (destColumn.transform.GetChild(destColumn.transform.childCount - 1).GetComponent<Tile>());
			if (connectedTiles.Count >= 3){
				for (int i = 0; i < connectedTiles.Count; i++) {
					connectedTiles[i].FadeOut();
                    yield return new WaitForSeconds(.02f);
                }
                interfaceManager.AddCounter(NPremierEntier(connectedTiles.Count));
            }
            if (interfaceManager.counterValue % 1 == 0) {
				StartCoroutine( AddNInLineAtRandom (4));
			}
		}

	}

    public int NPremierEntier(int n) {
        return n * (1 + n) / 2;
    }

	// when you plan to delete a Tile, there is some cleaning to be performed
	public void DestroyTile (Tile t){
		int c = t.columnNumber;
		for (int i = t.rowNumber + 1 ; i < columns[c].childCount ; i++ ){
            Tile upperTile = columns[c].GetChild(i).GetComponent<Tile>();
            upperTile.rowNumber--;
            upperTile.RefreshDisplayText();
        }
		Destroy (t.gameObject);
	}

	// given a Column, returns all connected tiles in all directions but not diagonals
    // should be a tile instead of a column
	public List<Tile> GetConnectedTiles(Tile t){
		List<Tile> result = new List<Tile> ();
		List<Tile> openList = new List<Tile> ();
		int columnIndex = t.columnNumber;
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

    public List<Tile> getTilesOfType(int type) {
        List<Tile> result = new List<Tile>();
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < columns[i].childCount; j++) {
                Tile t = columns[i].GetChild(j).GetComponent<Tile>();
                if (t.type == type) {
                    result.Add(t);
                }
            }
        }
        return result;
    }

	public void OnFadeOutComplete(){
	
	}

	private Tile GetTileAt(int column, int index){
		if (column < 0 || column >= columns.Count || index < 0) {
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