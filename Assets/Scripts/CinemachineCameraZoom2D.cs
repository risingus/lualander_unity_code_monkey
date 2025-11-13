using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraZoom2D : MonoBehaviour {
	public static CinemachineCameraZoom2D Instance { get; private set; }
	[SerializeField] private CinemachineCamera cinemaChineCamera;
	private const float NORMAL_ORTHOGRAPHIC_SIZE = 10f;
	private float targetOrthographicSize = 10f;

	private void Awake() {
		Instance = this;
	}

	private void Update() {
		const float zoomSpeed = 2f;
		cinemaChineCamera.Lens.OrthographicSize = Mathf.Lerp(cinemaChineCamera.Lens.OrthographicSize,
			targetOrthographicSize, Time.deltaTime * zoomSpeed);
	}

	public void SetTargetOrthographicSize(float targetOrthographicSize) {
		this.targetOrthographicSize = targetOrthographicSize;
	}

	public void SetNormalOrthographicSize() {
		SetTargetOrthographicSize(NORMAL_ORTHOGRAPHIC_SIZE);
	}
}