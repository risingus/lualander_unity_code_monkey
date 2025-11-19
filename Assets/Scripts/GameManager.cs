using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager Instance { get; private set; }
	public event EventHandler OnGamePaused;
	public event EventHandler OnGameUnpaused;

	[SerializeField] private List<GameLevel> gameLevelsList;
	[SerializeField] private CinemachineCamera cinemachineCamera;
	private static int levelNumber = 1;
	private static int totalScore;
	private int score;
	private float time;
	private bool isTimerActive;


	private void Awake() {
		Instance = this;
	}

	private void Start() {
		Lander.Instance.OnCoinPickup += Lander_OnCoinPickup;
		Lander.Instance.OnLanded += Lander_OnLanded;
		Lander.Instance.OnStateChanged += Lander_onStateChanged;
		GameInput.Instance.OnMenuButtonPressed += GameInput_OnMenuButtonPressed;
		LoadCurrentLevel();
	}

	private void GameInput_OnMenuButtonPressed(object sender, EventArgs e) {
		PauseUnpauseGame();
	}

	private void Update() {
		if (isTimerActive) {
			time += Time.deltaTime;
		}
	}

	public static void ResetStaticData() {
		levelNumber = 1;
		totalScore = 0;
	}

	private void LoadCurrentLevel() {
		GameLevel gameLevel = GetGameLevel();
		GameLevel spawnedGameLevel = Instantiate(gameLevel, Vector3.zero, Quaternion.identity);
		Lander.Instance.transform.position = spawnedGameLevel.GetLanderStartPosition();
		cinemachineCamera.Target.TrackingTarget = spawnedGameLevel.GetCameraStartTargetTransform();
		CinemachineCameraZoom2D.Instance.SetTargetOrthographicSize(spawnedGameLevel.GetZoomedOutOrthographicSize());
	}

	private GameLevel GetGameLevel() {
		foreach (GameLevel gameLevel in gameLevelsList) {
			if (gameLevel.GetLevelNumber() == levelNumber) {
				return gameLevel;
			}
		}

		return null;
	}

	private void Lander_onStateChanged(object sender, Lander.OnStateChangedEventArgs e) {
		isTimerActive = e.state == Lander.State.Normal;
		if (e.state == Lander.State.Normal) {
			cinemachineCamera.Target.TrackingTarget = Lander.Instance.transform;
			CinemachineCameraZoom2D.Instance.SetNormalOrthographicSize();
		}
	}

	private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e) {
		AddScore(e.score);
	}

	private void Lander_OnCoinPickup(object sender, System.EventArgs e) {
		AddScore(500);
	}

	public void AddScore(int addScoreAmount) {
		score += addScoreAmount;
	}

	public int GetScore() => score;

	public float GetTime() => time;

	public int GetTotalScore() => totalScore;

	public void GoToNextLevel() {
		levelNumber++;
		totalScore += score;

		if (GetGameLevel() == null) {
			// No more levels
			SceneLoader.LoadScene(SceneLoader.Scene.GameOverScene);
			return;
		}

		SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
	}

	public void RetryLevel() => SceneLoader.LoadScene(SceneLoader.Scene.GameScene);

	public int GetLevelNumber() => levelNumber;

	public void PauseGame() {
		Time.timeScale = 0f;
		OnGamePaused?.Invoke(this, EventArgs.Empty);
	}

	public void UnPauseGame() {
		Time.timeScale = 1f;
		OnGameUnpaused?.Invoke(this, EventArgs.Empty);
	}

	public void PauseUnpauseGame() {
		if (Time.timeScale == 1f) {
			PauseGame();
			return;
		}

		UnPauseGame();
	}
}