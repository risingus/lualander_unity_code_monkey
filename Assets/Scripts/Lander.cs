using System;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Lander : MonoBehaviour {
	private const float GRAVITY_NORMAL = 0.7f;

	public static Lander Instance { get; private set; }

	public event EventHandler OnUpForce;
	public event EventHandler OnRightForce;
	public event EventHandler OnLeftForce;
	public event EventHandler OnBeforeForce;
	public event EventHandler OnCoinPickup;
	public event EventHandler OnFuelPickup;
	public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

	public class OnStateChangedEventArgs : EventArgs {
		public State state;
	}

	public event EventHandler<OnLandedEventArgs> OnLanded;

	public class OnLandedEventArgs : EventArgs {
		public LandingType landingType;
		public int score;
		public float dotVector;
		public float landingSpeed;
		public float scoreMultiplier;
	}

	public enum LandingType {
		Success,
		WrongLandingArea,
		TooSteepAngle,
		TooFastLanding,
	}

	public enum State {
		WaitingToStart,
		Normal,
		GameOver,
	}

	private Rigidbody2D landerRigidbody2D;
	private float fuelAmount;
	private const float fuelAmountMax = 10f;
	private State state;

	private void Awake() {
		Instance = this;
		fuelAmount = fuelAmountMax;
		state = State.WaitingToStart;
		landerRigidbody2D = GetComponent<Rigidbody2D>();
		landerRigidbody2D.gravityScale = 0f;
	}

	private void FixedUpdate() {
		OnBeforeForce?.Invoke(this, EventArgs.Empty);

		switch (state) {
			default:
			case State.WaitingToStart:
				if (GameInput.Instance.IsUpActionPressed() ||
				    GameInput.Instance.IsRightActionPressed() ||
				    GameInput.Instance.IsLeftActionPressed() ||
				    GameInput.Instance.GetMovementInputVector2() != Vector2.zero) {
					landerRigidbody2D.gravityScale = GRAVITY_NORMAL;
					SetState(State.Normal);
				}

				break;
			case State.Normal:
				if (fuelAmount <= 0f) return;

				if (GameInput.Instance.IsUpActionPressed()
				    || GameInput.Instance.IsRightActionPressed()
				    || GameInput.Instance.IsLeftActionPressed() ||
				    GameInput.Instance.GetMovementInputVector2() != Vector2.zero) {
					ConsumeFuel();
				}

				const float gamePadDeadzone = 0.4f;
				if (GameInput.Instance.IsUpActionPressed() ||
				    GameInput.Instance.GetMovementInputVector2().y > gamePadDeadzone) {
					const float force = 700f;
					landerRigidbody2D.AddForce(transform.up * (force * Time.deltaTime));
					OnUpForce?.Invoke(this, EventArgs.Empty);
				}

				if (GameInput.Instance.IsLeftActionPressed() ||
				    GameInput.Instance.GetMovementInputVector2().x < -gamePadDeadzone) {
					const float turnSpeed = +100f;
					landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
					OnLeftForce?.Invoke(this, EventArgs.Empty);
				}

				if (GameInput.Instance.IsRightActionPressed() ||
				    GameInput.Instance.GetMovementInputVector2().x > gamePadDeadzone) {
					const float turnSpeed = -100f;
					landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
					OnRightForce?.Invoke(this, EventArgs.Empty);
				}

				break;
			case State.GameOver:
				landerRigidbody2D.gravityScale = 0f;
				break;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision2D) {
		if (!collision2D.gameObject.TryGetComponent(out LandingPad landingPad)) {
			OnLanded?.Invoke(this, new OnLandedEventArgs {
				score = 0,
				landingType = LandingType.WrongLandingArea,
				dotVector = 0f,
				landingSpeed = 0f,
				scoreMultiplier = 0
			});
			SetState(State.GameOver);
			return;
		}

		const float softLandingVelocityMagnitude = 4f;
		float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;

		if (relativeVelocityMagnitude > softLandingVelocityMagnitude) {
			OnLanded?.Invoke(this, new OnLandedEventArgs {
				score = 0,
				landingType = LandingType.TooFastLanding,
				dotVector = 0f,
				landingSpeed = relativeVelocityMagnitude,
				scoreMultiplier = 0
			});
			SetState(State.GameOver);
			return;
		}

		float dotVector = Vector2.Dot(Vector2.up, transform.up);
		const float minDotVector = .90f;

		if (dotVector < minDotVector) {
			OnLanded?.Invoke(this, new OnLandedEventArgs {
				score = 0,
				landingType = LandingType.TooSteepAngle,
				dotVector = dotVector,
				landingSpeed = relativeVelocityMagnitude,
				scoreMultiplier = 0
			});
			SetState(State.GameOver);
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
			landingType = LandingType.Success,
			dotVector = dotVector,
			landingSpeed = relativeVelocityMagnitude,
			scoreMultiplier = landingPad.GetScoreMultiplier()
		});
		SetState(State.GameOver);
	}

	private void OnTriggerEnter2D(Collider2D incomingCollider) {
		if (incomingCollider.gameObject.TryGetComponent(out FuelPickup fuelPickup)) {
			const float addFuelAmount = 10f;
			fuelAmount += addFuelAmount;
			if (fuelAmount > fuelAmountMax) {
				fuelAmount = fuelAmountMax;
			}

			OnFuelPickup?.Invoke(this, EventArgs.Empty);
			fuelPickup.DestroySelf();
		}

		if (incomingCollider.gameObject.TryGetComponent(out CoinPickup coinPickup)) {
			OnCoinPickup?.Invoke(this, EventArgs.Empty);
			coinPickup.DestroySelf();
		}
	}

	private void SetState(State newState) {
		this.state = newState;
		OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
			state = newState
		});
	}

	private void ConsumeFuel() {
		const float fuelConsumptionAmount = 1f;
		fuelAmount -= fuelConsumptionAmount * Time.deltaTime;
	}

	public float GetFuel() {
		return fuelAmount;
	}

	public float GetFuelAmountNormalized() => fuelAmount / fuelAmountMax;

	public float GetSpeedX() => landerRigidbody2D.linearVelocityX;

	public float GetSpeedY() => landerRigidbody2D.linearVelocityY;
}