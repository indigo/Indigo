using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour {


	public static InterfaceManager Instance;

	public Text counterText;

	private int counterValue;

	// Use this for initialization
	void Start () {
		Instance = this;
		counterValue = 0;
		AddCounter (0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddCounter(int addedValue){
		counterValue += addedValue;
		counterText.text = "" + counterValue;
	}


}
