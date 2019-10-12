using UnityEngine;
using System.Collections;


public class CameraControls : MonoBehaviour {
	public GameObject cameraTarget;
	public GameObject raycastTarget; //could be merged with the cameraTarget, this just helps differentiate them
	public float damping = 5F;
	Vector3 offset;

	private RaycastHit[] hits = null;

	void Start() {
		offset = cameraTarget.transform.position - transform.position;
	}
	
	//calculate if something is between the player and the camera, make it invisible
	private void Update() {

		//debug ray to show where the intersect is
		Debug.DrawRay(this.transform.position, (this.raycastTarget.transform.position - this.transform.position), Color.magenta);

		//needs to be first for things to be shown by default
		Show ();

		//ray from camera to raycast target (the player)
		hits = Physics.RaycastAll(this.transform.position, (this.raycastTarget.transform.position - this.transform.position), Vector3.Distance(this.transform.position, this.raycastTarget.transform.position));

		//needs to be last to be the exception
		Hide ();
		
	}
	//

	//camera should be late update so that it only moves after the target has moved
	void LateUpdate() {
		float currentAngle = transform.eulerAngles.y;
		float desiredAngle = cameraTarget.transform.eulerAngles.y;
		float angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * damping);
		
		Quaternion rotation = Quaternion.Euler(0, angle, 0);
		transform.position = cameraTarget.transform.position - (rotation * offset);
		
		transform.LookAt(cameraTarget.transform);
	}

	void Show() {
		//make sure to show all objects not currently in the way
		if (hits != null) {
			foreach (RaycastHit hit in hits) {
				Renderer r = hit.collider.GetComponent<Renderer>();
				if (r) {
					r.enabled = true;
				}
			}
		}
	}

	void Hide() {
		//if it hits, make it invisible
		foreach (RaycastHit hit in hits) {
			Renderer r = hit.collider.GetComponent<Renderer>();
			if (r) {
				r.enabled = false;
			}
		}
	}
	
}