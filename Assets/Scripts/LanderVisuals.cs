using UnityEngine;

public class LanderVisuals : MonoBehaviour {
	[SerializeField] private ParticleSystem leftThrusterParticlesSystem;
	[SerializeField] private ParticleSystem middleThrusterParticlesSystem;
	[SerializeField] private ParticleSystem rightThrusterParticlesSystem;
	[SerializeField] private GameObject landerExplosionVfx;

	private Lander lander;

	private void Awake() {
		lander = GetComponent<Lander>();
		lander.OnUpForce += Lander_OnUpForce;
		lander.OnLeftForce += Lander_OnLeftForce;
		lander.OnRightForce += Lander_OnRightForce;
		lander.OnBeforeForce += Lander_OnBeforeForce;

		SetEnabledThrusterParticleSystem(leftThrusterParticlesSystem, false);
		SetEnabledThrusterParticleSystem(middleThrusterParticlesSystem, false);
		SetEnabledThrusterParticleSystem(rightThrusterParticlesSystem, false);
	}

	private void Start() {
		lander.OnLanded += Lander_OnLanded;
	}

	private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e) {
		switch (e.landingType) {
			case Lander.LandingType.TooFastLanding:
			case Lander.LandingType.TooSteepAngle:
			case Lander.LandingType.WrongLandingArea:
				Instantiate(landerExplosionVfx, transform.position, Quaternion.identity);
				gameObject.SetActive(false);
				break;
		}
	}

	private void Lander_OnBeforeForce(object sender, System.EventArgs e) {
		SetEnabledThrusterParticleSystem(leftThrusterParticlesSystem, false);
		SetEnabledThrusterParticleSystem(middleThrusterParticlesSystem, false);
		SetEnabledThrusterParticleSystem(rightThrusterParticlesSystem, false);
	}

	private void Lander_OnUpForce(object sender, System.EventArgs e) {
		SetEnabledThrusterParticleSystem(leftThrusterParticlesSystem, true);
		SetEnabledThrusterParticleSystem(middleThrusterParticlesSystem, true);
		SetEnabledThrusterParticleSystem(rightThrusterParticlesSystem, true);
	}

	private void Lander_OnLeftForce(object sender, System.EventArgs e) {
		SetEnabledThrusterParticleSystem(rightThrusterParticlesSystem, true);
	}

	private void Lander_OnRightForce(object sender, System.EventArgs e) {
		SetEnabledThrusterParticleSystem(leftThrusterParticlesSystem, true);
	}


	private void SetEnabledThrusterParticleSystem(ParticleSystem particleSystem, bool enabled) {
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		emission.enabled = enabled;
	}
}