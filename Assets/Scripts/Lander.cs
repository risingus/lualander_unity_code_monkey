using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour {
	public event EventHandler OnUpForce;
	public event EventHandler OnRightForce;
	public event EventHandler OnLeftForce;
	public event EventHandler OnBeforeForce;
	private Rigidbody2D landerRigidbody2D;

	private void Awake() {
		landerRigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate() {
		OnBeforeForce?.Invoke(this, EventArgs.Empty);
		if (Keyboard.current.upArrowKey.isPressed) {
			const float force = 700f;
			landerRigidbody2D.AddForce(transform.up * (force * Time.deltaTime));
			OnUpForce?.Invoke(this, EventArgs.Empty);
		}

		if (Keyboard.current.leftArrowKey.isPressed) {
			const float turnSpeed = +100f;
			landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
			OnLeftForce?.Invoke(this, EventArgs.Empty);
		}

		if (Keyboard.current.rightArrowKey.isPressed) {
			const float turnSpeed = -100f;
			landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
			OnRightForce?.Invoke(this, EventArgs.Empty);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision2D) {
		if (!collision2D.gameObject.TryGetComponent(out LandingPad landingPad)) {
			Debug.Log("Crashed on the Terrain!");
			return;
		}

		const float softLandingVelocityMagnitude = 4f;
		float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;

		if (relativeVelocityMagnitude > softLandingVelocityMagnitude) {
			Debug.Log("landed too hard!");
			return;
		}

		float dotVector = Vector2.Dot(Vector2.up, transform.up);
		const float minDotVector = .90f;

		if (dotVector < minDotVector) {
			Debug.Log("landed on a too steep angle!");
			return;
		}

		Debug.Log("successful landing");

		const float maxScoreAmountLandingAngle = 100;
		const float scoreDotVectorMultiplier = 10f;
		float landingAngleScore = maxScoreAmountLandingAngle -
		                          Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier * maxScoreAmountLandingAngle;
		const float maxScoreAmountLandingSpeed = 100;
		float landingSpeedScore = (softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmountLandingSpeed;

		Debug.Log("landingAngleScore" + landingAngleScore);
		Debug.Log("landingSpeedScore" + landingSpeedScore);

		int score = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * landingPad.GetScoreMultiplier());

		Debug.Log("multiplier" + score);
	}
}