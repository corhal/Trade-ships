using UnityEngine;
using System.Collections;

public class CameraDrag : MonoBehaviour
{	
	public GameObject MainUI;
	Vector3 dragOrigin;

	public bool cameraDragging = true;

	public float HorizontalBorder;
	public float VerticalBorder;

	public int cameraCurrentZoom = 5;
	public int cameraZoomMax = 20;
	public int cameraZoomMin = 5;

	void Start () {
		cameraCurrentZoom = (int)Camera.main.orthographicSize;
	}

	void LateUpdate() {	
		if (Input.GetMouseButtonDown(0) && Utility.IsPointerOverUIObject ()) {
			cameraDragging = false;
			return;
		}
		if (Input.GetMouseButtonDown(0)) {			
			dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			cameraDragging = true;
			GameManager.Instance.CameraDragged = true;
			return;
		}
		if (cameraDragging) {
			if (Input.GetMouseButton (0)) {
				Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition) - Camera.main.transform.position;
				Vector3 pos = dragOrigin - mousePos;
				transform.position = pos;
			}

			if (transform.position.x > HorizontalBorder) {
				transform.position = new Vector3 (HorizontalBorder, transform.position.y, transform.position.z);
			}
			if (transform.position.x < -HorizontalBorder) {
				transform.position = new Vector3 (-HorizontalBorder, transform.position.y, transform.position.z);
			}
			if (transform.position.y > VerticalBorder) {
				transform.position = new Vector3 (transform.position.x, VerticalBorder, transform.position.z);
			}
			if (transform.position.y < -VerticalBorder) {
				transform.position = new Vector3 (transform.position.x, -VerticalBorder, transform.position.z);
			}

			MainUI.transform.position = new Vector3 (transform.position.x, transform.position.y, MainUI.transform.position.z);
		}

		if (Input.GetMouseButtonUp (0)) {
			Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);// - Camera.main.transform.position;
			Vector3 pos = dragOrigin - mousePos;
			Debug.Log ("calling from cameraDrag");
			if (Mathf.Abs (pos.x) > 0.01f || Mathf.Abs (pos.y) > 0.01f || Mathf.Abs (pos.z) > 0.01f) {
				GameManager.Instance.CameraDragged = true;
			} else {
				GameManager.Instance.CameraDragged = false;			
			}
		}

		if (Input.GetAxis("Mouse ScrollWheel") < 0) { // back
			if (cameraCurrentZoom < cameraZoomMax) {
				cameraCurrentZoom += 1;
				Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize + 1); // 0.004f
				MainUI.GetComponent<RectTransform>().localScale = new Vector3 (MainUI.transform.lossyScale.x + 0.004f, MainUI.transform.lossyScale.y + 0.004f, 1.0f); 
			} 
		}

		if (Input.GetAxis("Mouse ScrollWheel") > 0) { // forward
			if (cameraCurrentZoom > cameraZoomMin) {
				cameraCurrentZoom -= 1;
				Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize - 1);
				MainUI.GetComponent<RectTransform>().localScale = new Vector3 (MainUI.transform.lossyScale.x - 0.004f, MainUI.transform.lossyScale.y - 0.004f, 1.0f);
			}   
		}
	}
}