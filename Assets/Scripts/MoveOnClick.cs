using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnClick : MonoBehaviour {

	public bool InMoveMode = false;
	bool shouldMove = false;

	public float TimeLeft;
	public float Speed = 1.0F;
	Vector2 firstClick;
	Vector2 lastClick;
	Vector2 start;
	Vector2 target;
	float initialZ;

	LineRenderer lineRenderer;

	public delegate void StartedMoving (MoveOnClick sender);
	public event StartedMoving OnStartedMoving;

	void Awake () {
		lineRenderer = GetComponentInChildren<LineRenderer> ();
		lineRenderer.gameObject.SetActive (false);
	}

	void Start () {
		initialZ = transform.position.z;
	}

	void Update () {	
		if (!Utility.IsPointerOverUIObject()) {
			if (Input.GetMouseButtonDown (0) && InMoveMode) {	
				firstClick = Input.mousePosition;
			}
			if (Input.GetMouseButtonUp(0) && InMoveMode) {		
				lastClick = Input.mousePosition;
				start = transform.position;
				target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				if (Vector2.Distance(firstClick, lastClick) > 0.05f) {
					return;
				}

				DrawLine (start, target);
				ShowLine ();

				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (hit.collider != null) {					
					if (hit.collider.gameObject.GetComponent<Port> () != null) { // If you see that object stops too far - here is why
						target = hit.collider.gameObject.transform.position;
						target = start + (target - start) * 0.9f;
					}
				}

				GameManager.Instance.InMoveMode = false;
				GameManager.Instance.CloseContextButtons (true);
				shouldMove = true;
				InMoveMode = false;
				TimeLeft = Vector2.Distance(transform.position, target) / Speed;
				OnStartedMoving (this);
			}
		}

		if (shouldMove) { // changed from lerp
			TimeLeft = Vector2.Distance(transform.position, target) / Speed;
			float step = Speed * Time.deltaTime;
			transform.position = Vector2.MoveTowards (transform.position, target, step);
			transform.position = new Vector3 (transform.position.x, transform.position.y, initialZ);
			lineRenderer.SetPosition(0, transform.position);
			if (Vector2.Distance (transform.position, target) < 0.0001f) {
				shouldMove = false;
				lineRenderer.gameObject.SetActive (false);
			}
		}
	}

	void DrawLine (Vector3 startPosition, Vector3 endPoisiton) {
		lineRenderer.positionCount = 2;	
		lineRenderer.SetPosition(0, startPosition);
		lineRenderer.SetPosition(1, endPoisiton);
	}

	void ShowLine () {
		lineRenderer.gameObject.SetActive (true);
	}

	void HideLine () {
		lineRenderer.gameObject.SetActive (false);
	}
}
