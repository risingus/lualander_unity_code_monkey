using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
	[SerializeField] private Button playButton;
	[SerializeField] private Button quiteButton;

	private void Awake() {
		Time.timeScale = 1f;
		playButton.onClick.AddListener(() => {
			GameManager.ResetStaticData();
			SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
		});
		quiteButton.onClick.AddListener(() => { Application.Quit(); });
	}

	private void Start() {
		playButton.Select();
	}
}