using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {
	public static GameInput Instance { get; private set; }
	private InputActions inputActions;

	public event EventHandler OnMenuButtonPressed;

	private void Awake() {
		Instance = this;
		inputActions = new InputActions();
		inputActions.Enable();

		inputActions.Player.Menu.performed += Menu_performed;
	}

	private void Menu_performed(InputAction.CallbackContext obj) {
		OnMenuButtonPressed?.Invoke(this, EventArgs.Empty);
	}


	private void OnDestroy() {
		inputActions.Disable();
	}

	public bool IsUpActionPressed() => inputActions.Player.LanderUp.IsPressed();
	public bool IsRightActionPressed() => inputActions.Player.LanderRight.IsPressed();
	public bool IsLeftActionPressed() => inputActions.Player.LanderLeft.IsPressed();

	public Vector2 GetMovementInputVector2() => inputActions.Player.Movement.ReadValue<Vector2>();
}