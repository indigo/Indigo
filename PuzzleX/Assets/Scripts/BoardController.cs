using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;


public class BoardController : MonoBehaviour{

    // gameplay things
    public int width = 1;
    public int heightMatrix;
    public int matchQTY = 3;
    public int nbrColor = 4;
    // visual references
    public int spacing = 10;
    public Transform bg;

    private int cellWidth;
    private int cellHeight;

    // 
    public GameObject GameMatrix;

    public GameObject Hud;
    private InterfaceManager interfaceManager;
    private StatsManager statsManager;

    // shouldn't be public
    public List<Tile> tilesInHand;
    private List<Transform> columns;

    // state flags
    private bool movedFlag = false;
    private bool destroyFlag = false;


    public void Awake() {
        interfaceManager = Hud.GetComponent<InterfaceManager>();
        statsManager = GetComponent<StatsManager>() ;
    }

    public void Start()
    {
        tilesInHand = new List<Tile>();

        // set the whole layout
        // size of one square cell
        GameMatrix.GetComponent<RectTransform>().sizeDelta = new Vector2(-2 * spacing, GameMatrix.GetComponent<RectTransform>().sizeDelta.y);
        cellWidth = (int)(GameMatrix.transform.GetComponent<RectTransform>().rect.width/width);
        Vector3[] corners = new Vector3[4];
        cellHeight = cellWidth;

        bg.GetComponent<RectTransform>().sizeDelta = new Vector2(-2*spacing, cellHeight* heightMatrix + spacing);
        //GameMatrix.GetComponent<RectTransform>().sizeDelta = new Vector2(-2 * spacing, cellHeight * heightMatrix + spacing);
        GameMatrix.GetComponent<RectTransform>().sizeDelta = new Vector2(-2 * spacing, cellHeight * heightMatrix + spacing);

        // offsets
        RectOffset offsets = new RectOffset (0, 0, spacing, 0);
		GameMatrix.GetComponent<HorizontalLayoutGroup> ().padding = offsets;
		bg.GetComponent<HorizontalLayoutGroup> ().padding = offsets;

        // test
        GameMatrix.GetComponent<RectTransform>().GetWorldCorners(corners);
//        for (int i = 0; i < 4; i++)
  //      {
            Debug.Log((corners[2].x - corners[0].x)/width + ", " + (corners[2].y - corners[0].y)/heightMatrix);
    //    }


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
        statsManager.StartNewGameSession();
        yield return StartCoroutine( CleanColumns ());
		yield return StartCoroutine( NextTurn ());
        interfaceManager.ClearCounter ();
	}

	public void OnButtonRestart(){
		StartCoroutine (Restart ());
	}

	public IEnumerator NextTurn(){
		for (int i = 0 ; i < width ; i++){
			//if (columns [i].childCount == 0) {
			PushOneInColumn(i);				
			//}
		}
		yield return 0;
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
            yield return StartCoroutine ( AddOneInColumn(l[j]));
            yield return new WaitForSeconds(.1f);
        }
	}

    public IEnumerator AddOneInColumn(int column) {
        Tile tile = Tile.CreateTile(cellHeight, nbrColor);
        tile.rowNumber = columns[column].childCount;

        tile.columnNumber = column;
        tile.transform.SetParent(columns[column], false);
        tile.transform.SetAsLastSibling(); // bottom
		tile.RefreshDisplayText();
		if (tile.rowNumber > heightMatrix - 1)
		{
			Debug.Log("Death on Add with type : " + tile.type);
			yield return StartCoroutine(OnColorTouchDeath(tile.type));
		}

    }

	public void PushOneInColumn(int column) {
		Tile tile = Tile.CreateTile(cellHeight, nbrColor);
		tile.rowNumber = 0;

		for (int i = 0; i < columns [column].childCount; i++) {
			Tile t = columns [column].GetChild (i).GetComponent<Tile> ();
			t.rowNumber += 1;
			t.RefreshDisplayText ();
		}

		tile.columnNumber = column;
		tile.transform.SetParent(columns[column], false);
		tile.transform.SetAsFirstSibling(); // bottom
		tile.RefreshDisplayText();
	}


    // erase all tiles in each column
    public IEnumerator CleanColumns(){
		for (int i = 0; i < width; ++i) 
		{
			for (int t = 0; t < columns [i].childCount; t++) {
				Destroy (columns [i].GetChild (t).gameObject);
			}
		}
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator OnColorTouchDeath(int type) {
        List<Tile> tilesOfType = getTilesOfType(type);
        for (int i = 0; i < tilesOfType.Count; i++) {
			tilesOfType [i].deleteFlag = true;
        }
		yield return DestroySequence();
		for (int col = 0; col < width; col++) {
			for (int row = 0; row < columns [col].childCount; row++) {
				Tile tile = columns [col].GetChild (row).GetComponent<Tile> ();
				tile.dirty = false;
			}
		}
        //Analytics.CustomEvent("gameOver",statsManager.GetCurrentGameSessionDictionnary());
        statsManager.EndCurrentGameSession();
        statsManager.StartNewGameSession();
        interfaceManager.ClearCounter();
    }

	// TBD handle multiple col deaths
	public IEnumerator CheckForDeath(){
		yield return new WaitForEndOfFrame();
		for (int col = 0; col < width; col++) {
			if (columns[col].childCount > heightMatrix ){
				Tile culprit = columns [col].GetChild (columns [col].childCount - 1).GetComponent<Tile> ();
				yield return OnColorTouchDeath(culprit.type);
			}
		}
	}

	public IEnumerator OnDrop(Column sourceColumn, Column destColumn, List<Tile> tiles){
		if (sourceColumn != destColumn) {
			for (int i = 0; i < tiles.Count; i++) {
				tiles [i].moving = true;
				tiles [i].dirty = true;
                movedFlag = true;
			}
            //
            for (; movedFlag == true; Debug.Log ("Loop"))
            {
                yield return StartCoroutine(MovingTiles());
				yield return StartCoroutine (CheckMatches ());
                if (destroyFlag) {
                    yield return StartCoroutine(DestroySequence());
                    //yield return new WaitForSeconds(.3f);
                }
            }
            yield return new WaitForSeconds(.3f);
            yield return NextTurn ();
			yield return StartCoroutine (CheckForDeath ());
        }
    }

    public IEnumerator MovingTiles(){
        yield return new WaitForEndOfFrame();
		for (int col = 0; col < width; col++) {
			for (int row = 0; row < columns [col].childCount; row++) {
				Tile tile = columns [col].GetChild (row).GetComponent<Tile> ();
				if (tile.moving){
					tile.moving = false;
					tile.rowNumber = tile.transform.GetSiblingIndex();
					tile.RefreshDisplayText ();
				}
			}
		}
		movedFlag = false;
	}


	public IEnumerator CheckMatches(){
		yield return new WaitForEndOfFrame ();
		for (int col = 0; col < width; col++) {
			for (int row = 0; row < columns [col].childCount; row++) { // ??
				Debug.Log ("col=" + col + " row=" + row);
				Tile tile = columns [col].GetChild (row).GetComponent<Tile> ();
				Debug.Log (tile.displayText);
				// for every tile
				if (tile.dirty && !tile.deleteFlag){
					Debug.Log ("is dirty and not to delete");
					List<Tile> connectedTiles = GetConnectedTiles (tile);
					// if there is a Match for this tile
					if (connectedTiles.Count >= matchQTY) {
                        int scoreDelta = NPremierEntier(connectedTiles.Count);
                        statsManager.UpdateIntSessionInfo("score", scoreDelta);
                        interfaceManager.AddCounter (NPremierEntier (connectedTiles.Count));
						for (int i = 0; i < connectedTiles.Count; i++) {
							connectedTiles [i].deleteFlag = true;
							connectedTiles [i].dirty = true;
							//yield return new WaitForSeconds (.3f);
							destroyFlag = true;
						}
					} 
					else {
						Debug.Log ("is not part of a match");
						for (int i = 0; i < connectedTiles.Count; i++) {
							Debug.Log ("clean :" + connectedTiles [i].displayText);
							connectedTiles [i].dirty = false;
							//yield return new WaitForSeconds (.3f);
							connectedTiles [i].rowNumber = connectedTiles[i].transform.GetSiblingIndex();
						}
					
					}
				}
			}
		}
        // release
        // add new deletion tag or remove dirty tag
    }

    // destroySequence
    //
    // on all deletion tagged turn alpha off on .5 seconds
    // wait .5 seconds
    // tag all tiles that are impacted by deletion with the moving tag and the dirty tag
    // detroy all tile with deletion tag 
    // wait end of frame
    // start the movingSequence

    public IEnumerator DestroySequence(){
       // Debug.Log("start destroy " + movedFlag + " " + destroyFlag);

        for (int col = 0; col < width; col++) {
			bool dirtyFlag = false;
			for (int row = 0; row < columns [col].childCount; row++) {
				Tile tile = columns [col].GetChild (row).GetComponent<Tile> ();
				if (dirtyFlag && !tile.deleteFlag) {
					tile.dirty = true;
					tile.moving = true;
					movedFlag = true;
					Debug.Log ("something moved");
				}
				if (tile.deleteFlag){
					tile.FadeOut (0.5f);
					dirtyFlag = true;
				}
			}
		}
		yield return new WaitForSeconds (0.5f);

		for (int col = 0; col < width; col++) {
			for (int row = 0; row < columns [col].childCount; row++) {
				Tile tile = columns [col].GetChild (row).GetComponent<Tile> ();
				if (tile.deleteFlag){
					Destroy (tile.gameObject);
				}
			}
		}
        // release
        yield return new WaitForEndOfFrame();
        destroyFlag = false;
        //Debug.Log("end destroy " + movedFlag + " " + destroyFlag);
    }

    public int NPremierEntier(int n) {
        return n * (1 + n) / 2;
    }

	// when you plan to delete a Tile, there is some cleaning to be performed
	/*
	public void DestroyTile (Tile t){
		int c = t.columnNumber;
		for (int i = t.rowNumber + 1 ; i < columns[c].childCount ; i++ ){
            Tile upperTile = columns[c].GetChild(i).GetComponent<Tile>();
            upperTile.rowNumber--;
            upperTile.RefreshDisplayText();
        }
		Destroy (t.gameObject);
	}
*/
	// given a Column, returns all connected tiles in all directions but not diagonals
    // should be a tile instead of a column
	public List<Tile> GetConnectedTiles(Tile t){
		List<Tile> result = new List<Tile> ();
		List<Tile> openList = new List<Tile> ();
		int columnIndex = t.columnNumber;
		//Tile currentTile = columns [columnIndex].GetChild (columns [columnIndex].childCount - 1).GetComponent<Tile>();
		Tile currentTile = t;
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