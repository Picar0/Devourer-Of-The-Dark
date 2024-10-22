using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadGameplay : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Image loadingFillImage;

    public void Play()
    {
        HideCursor();
        StartCoroutine(LoadLevel("IntroScene"));
    }

    public void Restart()
    {
        HideCursor();
        StartCoroutine(LoadLevel("Gameplay"));
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    IEnumerator LoadLevel(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        loadingScreen.SetActive(true);
        mainMenu.SetActive(false);

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            loadingFillImage.fillAmount = progress;

            yield return null;
        }
    }
}
