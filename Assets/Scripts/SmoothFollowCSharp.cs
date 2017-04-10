using UnityEngine;
using System.Collections;


 
 // Place the script in the Camera-Control group in the component menu
 [AddComponentMenu("Camera-Control/Smooth Follow CSharp")]

public class SmoothFollowCSharp : MonoBehaviour
{
    /*
    This camera smoothes out rotation around the y-axis and height.
    Horizontal Distance to the target is always fixed.

    There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

    For every of those smoothed values we calculate the wanted value and the current value.
    Then we smooth it using the Lerp function.
    Then we apply the smoothed values to the transform's position.
    */

    // The target we are following
    public Transform target;
    // The distance in the x-z plane to the target
    public float distance = 10.0f;
    // the height we want the camera to be above the target
    public float height = 5.0f;
    // How much we 
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;

    [SerializeField] float maxY;
    [SerializeField] float minY;
    [SerializeField] float maxDistance;
    [SerializeField] float minDistance;
    [SerializeField] float camSpeed = 5;

    void LateUpdate()
    {
        // Early out if we don't have a target
        if (!target)
            return;

        // Calculate the current rotation angles
        float wantedRotationAngle = target.eulerAngles.y;
        float wantedHeight = target.position.y + height;
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // Convert the angle into a rotation
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

    }

    public void RotateUp() {
        if (height < maxY) {
            height += Time.deltaTime * camSpeed;
        }
        if (distance > minDistance) {
            distance -= Time.deltaTime * camSpeed;
        }

        if (height > maxY) { height = maxY; }
        if (distance < minDistance) { distance = minDistance; }
    }

    public void RotateDown() {
        if (height > minY) {
            height -= Time.deltaTime * camSpeed;
        }
        if (distance < maxDistance) {
            distance += Time.deltaTime * camSpeed;
        }

        if (height < minY) { height = minY; }
        if (distance > maxDistance) { distance = maxDistance; }
    }
}