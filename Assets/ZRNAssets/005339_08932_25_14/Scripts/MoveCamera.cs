using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour
{
	public enum CameraMode { TPP, FPP };
	public CameraMode cameraMode = CameraMode.TPP;

	void Update()
	{
		if (cameraMode == CameraMode.TPP)
		{
			Debug.Log("Switching to TPP mode");
			transform.localPosition = new Vector3(0, 1, -6); // TPP position
			transform.localRotation = Quaternion.Euler(0, 0, 0); // TPP rotation
		}
		else if (cameraMode == CameraMode.FPP)
		{
			Debug.Log("Switching to FPP mode");
			transform.localPosition = new Vector3(0, 0.5f, 0.25f); // FPP position
			transform.localRotation = Quaternion.Euler(5f, 0, 0); // FPP rotation
		}
	}
	
}
