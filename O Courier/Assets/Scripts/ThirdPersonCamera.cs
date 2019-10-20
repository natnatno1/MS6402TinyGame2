using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {

	public bool lockCursor;
	public float mouseSensitivity = 10;
	public PlayerController target;
	public Vector2 pitchMinMax = new Vector2 (-40, 85);
	public float rotationSmoothTime = .12f;
	Vector3 rotationSmoothVelocity;
	Vector3 currentRotation;
	float yaw;
	float pitch;

	[Header("Offsets")]
	Vector3 defaultOffset;
	public Vector3 normalOffset;
	public Vector3 parkourOffset;

	void Start() {
		if (lockCursor) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	void LateUpdate () {
		yaw += Input.GetAxis ("Mouse X") * mouseSensitivity;
		pitch -= Input.GetAxis ("Mouse Y") * mouseSensitivity;
		pitch = Mathf.Clamp (pitch, pitchMinMax.x, pitchMinMax.y);

		currentRotation = Vector3.SmoothDamp (currentRotation, new Vector3 (pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
		transform.eulerAngles = currentRotation;

		transform.position = target.transform.position - transform.forward * defaultOffset.z + transform.right * defaultOffset.x + transform.up * defaultOffset.y;
		float fov = 55f + target.GetComponent<CharacterController> ().velocity.magnitude;
		fov = Mathf.Clamp (fov, fov, 75f);
		GetComponent<Camera> ().fieldOfView = Mathf.Lerp (GetComponent<Camera> ().fieldOfView, fov, 9 * Time.deltaTime);
	    
		if (target.isJumping || target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Vault") || target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Climb") || target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Slide")) {
			defaultOffset = Vector3.Lerp (defaultOffset, parkourOffset, 8f * Time.deltaTime);
		} else {
			defaultOffset = Vector3.Lerp (defaultOffset, normalOffset, 8f * Time.deltaTime);
		}
	}

}
