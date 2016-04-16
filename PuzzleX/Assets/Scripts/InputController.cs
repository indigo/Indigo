using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

	public LayerMask inputMask;

	private Camera camera;
	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)){
			if (Physics.Raycast(ray,out hit,1000,inputMask)){
				//Debug.Log(hit.textureCoord);
				if (Input.GetMouseButton(0))
					hit.transform.gameObject.SendMessage("OnButton", hit.textureCoord);
				if (Input.GetMouseButtonDown(0))
					hit.transform.gameObject.SendMessage("OnButtonDown", hit.textureCoord);
				if (Input.GetMouseButtonUp(0))
					hit.transform.gameObject.SendMessage("OnButtonUp", hit.textureCoord);

			}
		}
	}
}
