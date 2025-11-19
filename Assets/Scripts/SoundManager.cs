using System;
using UnityEngine;

public class SoundManager : MonoBehaviour {
	public event EventHandler OnSoundVolumeChanged;


	[SerializeField] private AudioClip fuelPickupAudioClip;
	[SerializeField] private AudioClip coinPickupAudioClip;
	[SerializeField] private AudioClip crashAudioClip;
	[SerializeField] private AudioClip landingSuccessAudioClip;
	private static int soundVolume = 6;
	private const int soundVolumeMax = 10;
	public static SoundManager Instance { get; private set; }

	private void Awake() {
		Instance = this;
	}

	private void Start() {
		Lander.Instance.OnFuelPickup += Lander_OnFuelPickup;
		Lander.Instance.OnCoinPickup += Lander_OnCoinPickup;
		Lander.Instance.OnLanded += Lander_OnLanded;
	}

	private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e) {
		switch (e.landingType) {
			case Lander.LandingType.Success:
				AudioSource.PlayClipAtPoint(landingSuccessAudioClip, Camera.main.transform.position,
					GetSoundVolumeNormalized());
				break;
			default:
				AudioSource.PlayClipAtPoint(crashAudioClip, Camera.main.transform.position, GetSoundVolumeNormalized());
				break;
		}
	}

	private void Lander_OnCoinPickup(object sender, EventArgs e) {
		AudioSource.PlayClipAtPoint(coinPickupAudioClip, Camera.main.transform.position, GetSoundVolumeNormalized());
	}

	private void Lander_OnFuelPickup(object sender, EventArgs e) {
		AudioSource.PlayClipAtPoint(fuelPickupAudioClip, Camera.main.transform.position, GetSoundVolumeNormalized());
	}

	public void ChangeSoundVolume() {
		OnSoundVolumeChanged?.Invoke(this, EventArgs.Empty);
		soundVolume = (soundVolume + 1) % soundVolumeMax;
	}

	public int GetSoundVolume() => soundVolume;

	public float GetSoundVolumeNormalized() => ((float)soundVolume) / soundVolumeMax;
}