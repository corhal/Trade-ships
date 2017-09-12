using UnityEngine;
using System.Collections;

public class CameraDrag : MonoBehaviour
{
	public float dragSpeed = 2;
	private Vector3 dragOrigin;

	public bool cameraDragging = true;

	public float outerLeft = -50f;
	public float outerRight = 50f;


	void Update() {		
		cameraDragging = true;

		/*if (DragController.ShouldDrag || FloorController.isInFloorMode || Restaurant.instance.IsWindowOpen) {
			cameraDragging = false;
		}*/

		if (cameraDragging) {
			if (Input.GetMouseButtonDown(0)) {
				dragOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition);
				//Debug.Log (dragOrigin);
				return;
			}

			/*if (!Input.GetMouseButton (0)) {
				if(this.transform.position.x > outerRight) {
					transform.position = new Vector3 (outerRight, transform.position.y, transform.position.z);
				}
				if(this.transform.position.x < outerLeft) {
					transform.position = new Vector3 (outerLeft, transform.position.y, transform.position.z);
				}
				return;
			}*/

			if (Input.GetMouseButton(0)) {
				Vector3 mousePos = Camera.main.ScreenToViewportPoint (Input.mousePosition);
				Vector3 pos = mousePos - dragOrigin;
				pos = new Vector3(-pos.x, -pos.y, -10.0f);
				transform.position = pos;
			}

			// Camera.main.ScreenToWorldPoint (newPosition);// + offset;

			/*Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
			Vector3 move = new Vector3(-pos.x * dragSpeed, 0, 0);

			if (move.x > 0f) {
				if(this.transform.position.x < outerRight) {
					transform.Translate(move, Space.World);
				}
			}
			else {
				if(this.transform.position.x > outerLeft) {
					transform.Translate(move, Space.World);
				}
			}*/
		}
	}
}