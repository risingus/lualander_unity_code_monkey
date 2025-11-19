using System;
using UnityEngine;

public class MusicManager : MonoBehaviour {
	public event EventHandler OnMusicVolumeChanged;
	private const int musicVolumeMax = 10;
	private AudioSource musicAudioSource;
	private static float musicTime;
	private static int musicVolume = 4;
	public static MusicManager Instance { get; private set; }

	private void Start() {
		musicAudioSource.volume = GetMusicVolumeNormalized();
	}

	private void Awake() {
		musicAudioSource = GetComponent<AudioSource>();
		musicAudioSource.time = musicTime;
		Instance = this;
	}

	private void Update() {
		musicTime = musicAudioSource.time;
	}

	public void ChangeMusicVolume() {
		OnMusicVolumeChanged?.Invoke(this, EventArgs.Empty);
		musicAudioSource.volume = GetMusicVolumeNormalized();
		musicVolume = (musicVolume + 1) % musicVolumeMax;
	}

	public int GetMusicVolume() => musicVolume;

	public float GetMusicVolumeNormalized() => ((float)musicVolume) / musicVolumeMax;
}