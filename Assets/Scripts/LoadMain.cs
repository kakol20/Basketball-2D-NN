using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMain : MonoBehaviour
{
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TMP_InputField seedInput;
    //[SerializeField] private GameObject[] toHide;
    [SerializeField] private GameObject[] toShow;
    public void LoadMainScene()
    {
        //print(seedInput.text);

        if (seedInput.text.Length > 0)
        {
            StaticManager.Seed = int.Parse(seedInput.text);
        }

        print(StaticManager.Seed);

        for (int i = 0; i < toShow.Length; i++)
        {
            toShow[i].SetActive(true);
        }

        StartCoroutine(LoadMainSceneAsync());

        //for (int i = 0; i < toHide.Length; i++)
        //{
        //    toHide[i].SetActive(false);
        //}
    }

    private IEnumerator LoadMainSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Main");

        while (!operation.isDone)
        {
            // update progress

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            //Debug.Log(progress);

            loadingBar.value = progress;

            yield return null;
        }
    }
}