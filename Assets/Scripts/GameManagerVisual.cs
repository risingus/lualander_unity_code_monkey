using UnityEngine;
using Unity.Cinemachine;

public class GameManagerVisual : MonoBehaviour {
	[SerializeField] private CinemachineImpulseSource crashCinemachineInpulseSource;

	void Start() {
		Lander.Instance.OnLanded += Lander_OnLanded;
	}

	private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e) {
		switch (e.landingType) {
			case Lander.LandingType.TooFastLanding:
			case Lander.LandingType.TooSteepAngle:
			case Lander.LandingType.WrongLandingArea:
				crashCinemachineInpulseSource.GenerateImpulse(50f);
				break;
		}
	}
}