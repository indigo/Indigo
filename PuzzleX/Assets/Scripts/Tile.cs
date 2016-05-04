using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Tile : MonoBehaviour {
    //public GameObject prefab;
    static int Count = 0;
	BoardController panelControllerFather;

	public Text textUI;
    public Image theImage;

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

	public static Tile CreateTile(int height, int range) {
        GameObject prefab = Resources.Load("Tile") as GameObject;
        Tile t =Instantiate<GameObject>(prefab).GetComponent<Tile>();
        Count++;
		t.textUI.text = "" ;
        t.GetComponent<LayoutElement>().preferredHeight = height;
        t.GetComponent<LayoutElement>().preferredWidth = height;
        t.type = Random.Range(0, range);
        t.SetColor(ColorManager.Instance.GetColor(t.type));
        return t;
    }

    public void Start() {
		panelControllerFather = GetComponentInParent<BoardController> ();
        //textUI.text = this.type;
        RefreshDisplayText();
    }

    public void SetColor(Color c) {
        theImage.color = c;
    }
    public void SetSelected(bool b) {
        // false makes it a bit transparent
        if (theImage != null)
        {
            Color32 savedColor = theImage.color;
            byte alphaValue = 255;
            if (!b) { alphaValue = 180; }
            theImage.color = new Color32(savedColor.r, savedColor.g, savedColor.b, alphaValue);
        }
    }

	public void RefreshDisplayText(){
		displayText = ""+ columnNumber + " " + rowNumber;
		//textUI.text = displayText;
	}

	public IEnumerator FadeOut(){
		iTween.FadeTo(gameObject,iTween.Hash("alpha",0f,"time",.2f, "oncomplete", "FadeOutComplete"));
        yield return new WaitForSeconds(0.1f);
	}

	public void FadeOutComplete(){
		panelControllerFather.DestroyTile (this);
	}

	public override string ToString(){
		return "";//+ columnNumber + " " + rowNumber;
	}
}
