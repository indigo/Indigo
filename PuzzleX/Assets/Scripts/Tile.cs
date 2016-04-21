using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Tile : MonoBehaviour {
    //public GameObject prefab;
    static int Count = 0;
	PanelController panelControllerFather;

	public Text textUI;
	private string _displayText;
	public string displayText{
		get { 
			return _displayText;
		}
		set { 
			_displayText = value;
		}
	}

	// ideally types should not be an int but a TileType type.
	private int _type;
	public int type{
		get{ 
			return _type;
		}
		set{ 
			_type = value;
		}
	}
  //  public ColorManager cm;
	private Image theImage;

    //it s good to know x y for a Tile 
	private int _columnNumber;
	public int columnNumber{
		get{ 
			return _columnNumber;
		}
		set{ 
			_columnNumber = value;
		}
	}
	private int _rowNumber;
	public int rowNumber{
		get{ 
			return _rowNumber;
		}
		set{ 
			_rowNumber = value;
		}
	}

	public static Tile CreateTile(int height) {
        GameObject prefab = Resources.Load("Tile") as GameObject;
        Tile t =Instantiate<GameObject>(prefab).GetComponent<Tile>();
        Count++;
		t.textUI.text = "" ;
		t.GetComponent<LayoutElement>().preferredHeight = height;
        return t;
    }

    public void Start() {
		panelControllerFather = GetComponentInParent<PanelController> ();
        type = Random.Range(1, 5);
		textUI.text = this.ToString();
		theImage = GetComponent<Image>();
        SetColor(ColorManager.Instance.GetColor(type));

    }

    public void SetColor(Color c) {
        theImage.color = c;
    }
    public void SetSelected(bool b) {
        // false makes it a bit transparent
        Color32 savedColor = theImage.color;
        byte alphaValue = 255;
        if (!b) { alphaValue = 180; }
        theImage.color = new Color32(savedColor.r,savedColor.g, savedColor.b,alphaValue);
    }

	public void RefreshDisplayText(){
		displayText = ""+ columnNumber + " " + rowNumber;
		textUI.text = "";//displayText;
	}

	public void FadeOut(){
		Debug.Log ("fade ?");
		iTween.FadeTo(gameObject,iTween.Hash("alpha",0f,"time",.2f, "oncomplete", "FadeOutComplete"));
	}

	public void FadeOutComplete(){
		panelControllerFather.DestroyTile (this);
	}

	public override string ToString(){
		return "";//+ columnNumber + " " + rowNumber;
	}
}
