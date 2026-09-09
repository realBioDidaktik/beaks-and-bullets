using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUI : MonoBehaviour
{

	Animator gameOverScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

		gameOverScreen = GameObject.Find("GameOverPanel").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

	public void Retry() {
		gameOverScreen.SetTrigger("FlyOut");
		GameObject.FindWithTag("MainCamera").GetComponent<Player>().SafeScore();
		Invoke("ReloadCurrentScene",1.0f);
	}

	public void QuitToMain() {
		gameOverScreen.SetTrigger("FlyOut");
		GameObject.FindWithTag("MainCamera").GetComponent<Player>().SafeScore();
		Invoke("LoadMainMenuScene",1.0f);
	}

	void ReloadCurrentScene() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	void LoadMainMenuScene() {
		SceneManager.LoadScene(0);
	}
}
