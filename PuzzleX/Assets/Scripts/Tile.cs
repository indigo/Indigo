using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Tile : MonoBehaviour {
    //public GameObject prefab;
    static int Count = 0;

	public Text textUI;

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
	public int columnNumber;
	public int rowNumber;

	public static Tile CreateTile(int height) {
        GameObject prefab = Resources.Load("Tile") as GameObject;
        Tile t =Instantiate<GameObject>(prefab).GetComponent<Tile>();
        Count++;
		t.textUI.text = "" ;
		t.GetComponent<LayoutElement>().preferredHeight = height;
        return t;
    }

    public void Start() {
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
	public override string ToString(){
		return "";//+ columnNumber + " " + rowNumber;
	}
}
