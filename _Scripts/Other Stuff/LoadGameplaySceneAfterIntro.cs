using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadGameplaySceneAfterIntro : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingFillImage;
    private void OnEnable()
    {
        StartCoroutine(LoadLevel("Gameplay"));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        loadingScreen.SetActive(true);
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            loadingFillImage.fillAmount = progress;

            yield return null;
        }
    }
}
