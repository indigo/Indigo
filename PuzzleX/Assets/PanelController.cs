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
	private int cellWidth;
	private int cellHeight;

	public void Start(){
		Image image = GetComponent<Image>();
		rect = image.GetPixelAdjustedRect();
		width = width == 0 ? 1 : width;
		height = height == 0 ? 1 : height;
		cellWidth = (int)(rect.xMax - rect.xMin) / width;
		cellHeight = (int)(rect.yMax - rect.yMin) / height;
		Debug.Log(rect);
	}

	public void OnPointerDown(PointerEventData ped) {
		mouseDown = true;
		clickPos = transform.position;
		clickMousePos = Input.mousePosition - new Vector3(rect.xMin, rect.yMin,0);
		Debug.Log ("clickPos = " + clickPos + "   clickMousePos = " + clickMousePos);
		Debug.Log ("square = " + (int)clickMousePos.x/cellWidth + " " + (int)clickMousePos.y/cellHeight );
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
