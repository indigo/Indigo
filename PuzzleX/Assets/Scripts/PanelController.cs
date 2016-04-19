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
            if (i != 0)
                t = GameObject.Instantiate(colPrefab).transform;
            else
                t = colPrefab.transform;

            columns.Add(t);
            t.SetParent(transform,false);
        }


		Restart();
    }

    public void Restart(){
		// Populate Test tiles
		for (int i = 0; i < width; ++i)
		{
			for (int t = 0; t < columns [i].childCount; t++) {
				Destroy (columns [i].GetChild (t).gameObject);
			}
			for (int j = 0; j < height; ++j)
			{
				Tile tile = Tile.CreateTile(cellHeight);
				tile.transform.SetParent(columns[i],false);
			}
		}	
		InterfaceManager.Instance.ClearCounter ();
	}

	public void OnDrop(Column a, Column b){
		//Debug.Log(a
		if (a != b) {
			InterfaceManager.Instance.AddCounter (1);
		}
	}
}