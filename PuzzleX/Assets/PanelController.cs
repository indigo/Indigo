using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PanelController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	private Vector3 clickMousePos;
	private Vector3 clickPos;
	private bool mouseDown;
	public GameObject itemPrefab;
	public Transform ParentPanel;

	public int width;
	public int height;
	private Rect rect;
    private Rect worldRect;
    private Vector3[] corners;

	private int cellWidth;
	private int cellHeight;

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
        Debug.Log(rect);
        Debug.Log(parentRect);
        for (int i = 0; i < 4; i++)
        {
            Debug.Log(corners[i]);
        }
        Debug.Log(worldRect);
    }

    public void OnPointerDown(PointerEventData ped) {
		mouseDown = true;
		//clickPos = transform.position;
		clickMousePos = Input.mousePosition;
		Debug.Log ("clickMousePos = " + clickMousePos);
		Debug.Log ("square = " + (int)(clickMousePos.x- worldRect.x)/cellWidth + " " + (int)(clickMousePos.y- worldRect.y)/cellHeight );
	}

	public void OnPointerUp(PointerEventData ped) {
		/*
		mouseDown = false;
		GameObject newItem = Instantiate(itemPrefab) as GameObject;
		newItem.transform.SetParent(ParentPanel, false);
		newItem.transform.localScale = new Vector3(1,1,1);
		newItem.transform.position = new Vector3(clickMousePos.x, clickMousePos.y, 0); */
	}
}
