using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour {


	public static InterfaceManager Instance;

	public Text counterText;

	private int _counterValue;
	public int counterValue{
		get{ 
			return _counterValue;
		}
		set{ 
			_counterValue = value;
		}
	}

	// Use this for initialization
	void Awake () {
		Instance = this;
		_counterValue = 0;
		AddCounter (0);
	}
	
	// Update is called once per frame
	void Update () {
	
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
