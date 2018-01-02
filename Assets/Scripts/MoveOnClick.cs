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

	public int EnergyPerDistance;
	float fullTraveledDistance;
	int traveledDistance;

	LineRenderer lineRenderer;

	public delegate void StartedMoving (MoveOnClick sender);
	public event StartedMoving OnStartedMoving;
	public event StartedMoving OnFinishedMoving;

	public void MoveToPoint (Vector2 target) {
		start = transform.position;
		this.target = target;
		//this.target = start + (this.target - start) * 0.9f;
		fullTraveledDistance = 0.0f;
		traveledDistance = 0;
		DrawLine (start, target);
		ShowLine ();
		shouldMove = true;
	}

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
				fullTraveledDistance = 0.0f;
				traveledDistance = 0;
				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (hit.collider != null) {							
					if (hit.collider.gameObject.GetComponent<Selectable> () != null) { // If you see that object stops too far - here is why
						Debug.Log (hit.collider.gameObject);
						target = hit.collider.gameObject.transform.position;
						target = start + (target - start) * 0.9f;
					}
				} else {					
					target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				}

				if (Vector2.Distance(firstClick, lastClick) > 0.05f) {
					return;
				}

				DrawLine (start, target);
				ShowLine ();



				GameManager.Instance.InMoveMode = false;
				UIOverlay.Instance.CloseContextButtons (true);
				shouldMove = true;
				//InMoveMode = false;
				TimeLeft = Vector2.Distance(transform.position, target) / Speed;
				if (OnStartedMoving != null) {
					OnStartedMoving (this);
				}
			}
		}

		if (shouldMove /*&& Player.Instance.Energy > 0*/) { // changed from lerp
			TimeLeft = Vector2.Distance(transform.position, target) / Speed;
			float step = Speed * Time.deltaTime;
			fullTraveledDistance += step;
			if ((int)fullTraveledDistance > traveledDistance) {
				int intStep = (int)fullTraveledDistance - traveledDistance;
				traveledDistance += intStep;
				// Player.Instance.Energy -= EnergyPerDistance * intStep;
			}
			transform.position = Vector2.MoveTowards (transform.position, target, step);
			transform.position = new Vector3 (transform.position.x, transform.position.y, initialZ);
			lineRenderer.SetPosition(0, transform.position);
			if (Vector2.Distance (transform.position, target) < 0.0001f) {
				shouldMove = false;
				OnFinishedMoving (this);
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
