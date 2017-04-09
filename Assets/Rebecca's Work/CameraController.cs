using UnityEngine;
using System.Collections;

// TODO:
// - Completely fucks up if target doesn't start at Vector3.zero
// - Have it more dynamic (e.g. camera zooms out when moving character moves towards it)

[DisallowMultipleComponent]
public class CameraController : MonoBehaviour
{
	[SerializeField] GameObject cameraTarget;
	public float rotateSpeed;
	float rotate = 0;
	public float offsetDistance;
	public float offsetHeight;
	public float smoothing;
	Vector3 offset;
	Vector3 lastPosition;

	void Start()
	{
		// cameraTarget = GameObject.FindGameObjectWithTag("Player");
		lastPosition = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y + offsetHeight, cameraTarget.transform.position.z - offsetDistance);
		offset = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y + offsetHeight, cameraTarget.transform.position.z - offsetDistance);
        this.transform.rotation = cameraTarget.transform.rotation;
	}

	void Update()
	{
		//offset = Quaternion.AngleAxis(rotate * rotateSpeed, Vector3.up) * offset;
		transform.position = cameraTarget.transform.localPosition + offset; 
		transform.position = new Vector3(Mathf.Lerp(lastPosition.x, cameraTarget.transform.position.x + offset.x, smoothing * Time.deltaTime), 
		Mathf.Lerp(lastPosition.y, cameraTarget.transform.position.y + offset.y, smoothing * Time.deltaTime), 
		Mathf.Lerp(lastPosition.z, cameraTarget.transform.position.z + offset.z, smoothing * Time.deltaTime));

        Debug.Log("Position: " + transform.position);
        Debug.Log("Player Position: " + cameraTarget.transform.position);
        Debug.Log("Offset: " + offset);

		transform.LookAt(cameraTarget.transform.position + new Vector3(0, offsetHeight - 1, offsetDistance));
	}

	public void RotateLeft () {

		rotate = -1;
	}

	public void RotateRight () {

		rotate = 1;
	}

	public void RotateNone () {

		rotate = 0;
	}

	void LateUpdate()
	{
		lastPosition = transform.localPosition;
	}
}