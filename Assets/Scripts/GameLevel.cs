using UnityEngine;

public class GameLevel : MonoBehaviour {
	[SerializeField] private int levelNumber;
	[SerializeField] private Transform landerStartPositionTransform;
	[SerializeField] private Transform cameraStartTargetTransform;
	[SerializeField] private float zoomedOutOrthographicSize;

	public int GetLevelNumber() => levelNumber;

	public Vector3 GetLanderStartPosition() => landerStartPositionTransform.position;

	public Transform GetCameraStartTargetTransform() => cameraStartTargetTransform;

	public float GetZoomedOutOrthographicSize() => zoomedOutOrthographicSize;
}