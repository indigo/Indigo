using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour {


//	public static InterfaceManager Instance;

	public Text counterText;

	private int _counterValue;
	public int counterValue{
		get{ 
			return _counterValue;
		}
		set{ 
			_counterValue = value;
            counterText.text = "" + _counterValue;
        }
	}

	// Use this for initialization
	void Awake () {
        counterValue = 0;          
    }
	
	public void AddCounter(int addedValue){
		_counterValue += addedValue;
		counterText.text = "" + _counterValue;
	}

	public void ClearCounter(){
		counterText.text = "0";
		_counterValue = 0;
	}
}
