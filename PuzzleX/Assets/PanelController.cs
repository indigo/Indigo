using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PanelController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

	private Vector3 clickMousePos;
	private Vector3 clickPos;
	//public GameObject itemPrefab;
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

    private List<Transform> columns;

	public void Start(){
		Image image = GetComponent<Image>();
        Rect parentRect = transform.parent.GetComponent<Canvas>().pixelRect;
		rect = image.GetPixelAdjustedRect();
        corners = new Vector3[4];
        image.rectTransform.GetWorldCorners(corners);
        worldRect = new Rect(corners[0].x, corners[0].y, rect.width, rect.height);
		width = width == 0 ? 1 : width;
		height = height == 0 ? 1 : height;
		cellWidth = (int)(rect.xMax - rect.xMin) / width;
		cellHeight = (int)(rect.yMax - rect.yMin) / height;

        GameObject colPrefab = transform.GetChild(0).gameObject;
        columns = new List<Transform>();
        columns.Add(colPrefab.transform);
        for (int i = 1; i < width; ++i)
        {
            Transform t = GameObject.Instantiate(colPrefab).transform;
            columns.Add(t);
            t.SetParent(transform);
        }
        colMarker.sizeDelta = new Vector2(cellWidth, rect.height);

    }

    public void OnPointerDown(PointerEventData ped) {
		selected = true;
		clickMousePos = Input.mousePosition;
		currentSelection = (int)(clickMousePos.x- worldRect.x)/cellWidth;
		//Debug.Log ("clickMousePos = " + clickMousePos);
		Debug.Log ("currentSelection => " + currentSelection);
	}

	public void OnDrag(PointerEventData data){
		clickMousePos = Input.mousePosition;
        int savedSelection = (int)((clickMousePos.x - worldRect.x) / cellWidth);
		if (currentSelection != savedSelection) { 
            currentSelection = savedSelection;
            if (savedSelection >= width) currentSelection = width - 1;
            if (savedSelection < 0) currentSelection = 0;
			Debug.Log ("now => " + currentSelection);

            colMarker.position = new Vector3(columns[currentSelection].transform.position.x,0,0);
		}

	}

	public void OnPointerUp(PointerEventData ped) {
		selected = false;
		Debug.Log ("dropping => " + currentSelection);
	}
}
