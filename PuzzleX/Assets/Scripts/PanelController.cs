using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PanelController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

    private Vector3 clickMousePos;
    private Vector3 clickPos;
    //public GameObject itemPrefab;

    // visual references
    public RectTransform colMarker;
    public int width;
    public int height;
    private Rect rect;
    private Rect worldRect;
    private Vector3[] corners;

    private int cellWidth;
    private int cellHeight;

    public int currentSelection;
    private bool selected;
    private List<Tile> tilesInHand;
    private Transform selectedTile;

    private List<Transform> columns;

    public void Start()
    {
        SelectColumn(false);

        Image image = GetComponent<Image>();
        tilesInHand = new List<Tile>();
        // Pop last
        //list[ tilesInHand.Count - 1];
        //list.RemoveAt(tilesInHand.Count - 1);
        // Push First
        //list.Insert(0,item)
        //Push Last
        //list.Add(item);

        // visual setup
        rect = image.GetPixelAdjustedRect();
        corners = new Vector3[4];
        image.rectTransform.GetWorldCorners(corners);
        worldRect = new Rect(corners[0].x, corners[0].y, rect.width, rect.height);
        width = width == 0 ? 1 : width;
        height = height == 0 ? 1 : height;
		cellWidth = cellHeight = (int)(rect.xMax - rect.xMin) / width;
        //cellHeight = (int)(rect.yMax - rect.yMin) / height;
        colMarker.sizeDelta = new Vector2(cellWidth, colMarker.sizeDelta.y);

        // creates columns
        GameObject colPrefab = transform.GetChild(0).gameObject;
        colPrefab.GetComponent<LayoutElement>().preferredWidth = cellWidth;
        columns = new List<Transform>();
        for (int i = 0; i < width; ++i)
        {
            Transform t;
            if (i != 0)
                t = GameObject.Instantiate(colPrefab).transform;
            else
                t = colPrefab.transform;

            columns.Add(t);
            t.SetParent(transform);
        }

        // Populate Test tiles
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
				Tile tile = Tile.CreateTile(cellWidth);
                tile.transform.SetParent(columns[i]);
            }
        }
    }

    public void OnPointerDown(PointerEventData ped)
    {

        clickMousePos = Input.mousePosition;
        currentSelection = (int)(clickMousePos.x - worldRect.x) / cellWidth;

        //exit if column is empty
        if (columns[currentSelection].childCount == 0)
            return;


        SelectColumn(true);
        //Debug.Log ("clickMousePos = " + clickMousePos);

        // check if more than one
        tilesInHand.Clear();
        Transform column = columns[currentSelection].transform;
        int selectedType = column.GetChild(column.childCount-1).GetComponent<Tile>().type;
        for (int i = column.childCount-1; i>=0; i--)
        {
            Tile t = column.GetChild(i).GetComponent<Tile>();
            if (t.type == selectedType)
            {
                tilesInHand.Add(t);
                t.SetSelected(false);
            }
            else {
                break;
            }
        }

        UpdateColMarker(currentSelection);

        Debug.Log("currentSelection => " + currentSelection);
    }

    public void OnDrag(PointerEventData data)
    {
        if (selected == false)
            return;

        clickMousePos = Input.mousePosition;
        int desiredSelection = (int)((clickMousePos.x - worldRect.x) / cellWidth);
        if (currentSelection != desiredSelection)
        {
            UpdateColMarker(desiredSelection);
        }
    }

    private void UpdateColMarker(int desiredSelection)
    {
        Debug.Log(currentSelection + " => " + desiredSelection);
        // clamp to 0 - width
        currentSelection = desiredSelection;
        if (desiredSelection >= width) currentSelection = width - 1;
        if (desiredSelection < 0) currentSelection = 0;

        // should move mor4e
        //selectedTile.SetParent(columns[currentSelection].transform);
        for (int i = 0; i < tilesInHand.Count; i++)
        {
            tilesInHand[i].transform.SetParent(columns[currentSelection]);
        }
        colMarker.position = new Vector3(columns[currentSelection].transform.position.x, colMarker.position.y, 0);
    }

    // only visual
    private void SelectColumn(bool b)
    {
        colMarker.gameObject.SetActive(b);
        selected = b;
    }

    public void OnPointerUp(PointerEventData ped)
    {
        if (selected == false)
            return;

        SelectColumn(false);

        // TODO: Get tiles in current column that match type
        // TODO: Call tile rule book
        Debug.Log("dropping => " + currentSelection);
    }

}