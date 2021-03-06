﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Column : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler{

	BoardController panelControllerFather;
	Image theImage;

	public static bool isSelected;
	private static int Counter;

	private int _columnNumber;
	public int columnNumber{
		get{ 
			return _columnNumber;
		}
		set { 
			_columnNumber = value;
		}

	}

	private static Column currentColumn;

	public void Start(){
		// get the reference on the Panel controller
		panelControllerFather = GetComponentInParent<BoardController> ();
	 	theImage = GetComponent<Image>();

	}

	public void OnPointerDown(PointerEventData ped)
	{
		// exit if col is empty
		if (transform.childCount == 0)
			return;

		// highlight the column
		SetHighlight(true);
		Column.isSelected = true;
		currentColumn = this;

		// get all Tiles with same type on that column
		List<Tile> tilesInHand = new List<Tile>();
		int selectedType = transform.GetChild(transform.childCount-1).GetComponent<Tile>().type;
		for (int i = transform.childCount-1; i>=0; i--)
		{
			Tile t = transform.GetChild(i).GetComponent<Tile>();
			if (t.type == selectedType)
			{
				tilesInHand.Add(t);
				t.SetSelected(false);
			}
			else {
				break;
			}
		}

		// store the tiles in hand on the panel
		panelControllerFather.tilesInHand = tilesInHand;

	}
	
    // this is a Drop
	public void OnPointerUp(PointerEventData ped)
	{
		// exit if nothing is selected
		if (!Column.isSelected) {
			return;
		}
		Column.isSelected = false;
		SetHighlight (false);
		List<Tile> tilesInHand = panelControllerFather.tilesInHand;
		for (int i = 0; i < tilesInHand.Count; i++)
		{
			tilesInHand[i].SetSelected(true);
		}
		StartCoroutine( panelControllerFather.OnDrop (this, currentColumn, tilesInHand));
	}

	public void OnPointerEnter(PointerEventData ped)
	{
		// exit if nothing is selected
		if (!Column.isSelected) {
			return;
		}
		SetHighlight (true);
		currentColumn = this;
		List<Tile> tilesInHand = panelControllerFather.tilesInHand;
		/*
        if (tilesInHand.Count + transform.childCount > panelControllerFather.heightMatrix ) {
            Debug.Log("Death with type : "+ tilesInHand[0].type + " from OnPointerEnter");
            StartCoroutine(panelControllerFather.OnColorTouchDeath(tilesInHand[0].type));
            return;
        }*/
        for (int i = tilesInHand.Count - 1; i >= 0; i--)
		{
			tilesInHand[i].transform.SetParent(transform);
			tilesInHand [i].columnNumber = columnNumber;
			tilesInHand [i].rowNumber = transform.childCount - 1;
            tilesInHand[i].RefreshDisplayText();
		}
	}
    public void OnPointerExit(PointerEventData ped)
    {
            SetHighlight(false);
	}

	public void SetHighlight(bool b){
		Color32 savedColor = theImage.color;
		byte alphaValue = 0;
		if (b) { alphaValue = 22; }
		theImage.color = new Color32(savedColor.r,savedColor.g, savedColor.b,alphaValue);
	}
}
