using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    private void Update() {
        if (Keyboard.current.upArrowKey.isPressed) {
            Debug.Log("u´p");
        }
        if (Keyboard.current.leftArrowKey.isPressed) {
            Debug.Log("left");
        }
        if (Keyboard.current.rightArrowKey.isPressed) {
            Debug.Log("right");
        }
    }
}
