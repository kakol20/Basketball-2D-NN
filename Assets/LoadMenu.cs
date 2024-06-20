using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMenu : MonoBehaviour {
    [SerializeField] private GameObject[] toShow;
    [SerializeField] private Slider loadingBar;

    public void LoadMenuScene() {
        foreach (GameObject item in toShow) {
            item.SetActive(true);
        }

        StartCoroutine(LoadMenuSceneAsync());
    }

    private IEnumerator LoadMenuSceneAsync() {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Menu");

        while (!operation.isDone) {
            // update progress

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            //Debug.Log(progress);

            loadingBar.value = progress;

            yield return null;
        }
    }
}