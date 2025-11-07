using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour {
	public static Lander Instance { get; private set; }

	public event EventHandler OnUpForce;
	public event EventHandler OnRightForce;
	public event EventHandler OnLeftForce;
	public event EventHandler OnBeforeForce;
	public event EventHandler OnCoinPickup;
	public event EventHandler<OnLandedEventArgs> OnLanded;

	public class OnLandedEventArgs : EventArgs {
		public int score;
	}

	private Rigidbody2D landerRigidbody2D;
	private float fuelAmount = 10f;

	private void Awake() {
		Instance = this;
		landerRigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate() {
		OnBeforeForce?.Invoke(this, EventArgs.Empty);

		if (fuelAmount <= 0f) return;

		if (Keyboard.current.upArrowKey.isPressed
		    || Keyboard.current.leftArrowKey.isPressed
		    || Keyboard.current.rightArrowKey.isPressed) {
			ConsumeFuel();
		}

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

		OnLanded?.Invoke(this, new OnLandedEventArgs {
			score = score,
		});
	}

	private void OnTriggerEnter2D(Collider2D collider2D) {
		if (collider2D.gameObject.TryGetComponent(out FuelPickup fuelPickup)) {
			const float addFuelAmount = 10f;
			fuelAmount += addFuelAmount;
			fuelPickup.DestroySelf();
		}

		if (collider2D.gameObject.TryGetComponent(out CoinPickup coinPickup)) {
			OnCoinPickup?.Invoke(this, EventArgs.Empty);
			coinPickup.DestroySelf();
		}
	}

	private void ConsumeFuel() {
		const float fuelConsumptionAmount = 1f;
		fuelAmount -= fuelConsumptionAmount * Time.deltaTime;
	}
}