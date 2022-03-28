using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HFarm.Transiton
{
    public class TransitionManager : MonoBehaviour
    {
        [SceneName]
        public string startSceneName = string.Empty;
        private CanvasGroup fadeCanvasGroup;
        private bool isFade;

        private IEnumerator Start()
        {
            fadeCanvasGroup = FindObjectOfType<CanvasGroup>();
            yield return StartCoroutine(LoadSceneSetActive(startSceneName));
            EventHandler.CallAfterSceneUnloadEvent();
        }

        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
        }

        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }

        private void OnTransitionEvent(string sceneName, Vector3 positionToGo)
        {
            if (!isFade)
                StartCoroutine(Transition(sceneName, positionToGo));
        }

        private IEnumerator Transition(string sceneName, Vector3 targetPos)
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1);
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            
            yield return LoadSceneSetActive(sceneName);
            EventHandler.CallMoveToPosition(targetPos);
            EventHandler.CallAfterSceneUnloadEvent();
            yield return Fade(0);
        }

        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            Scene newcene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            SceneManager.SetActiveScene(newcene);
        }

        private IEnumerator Fade(float targetAlpha)
        {
            isFade = true;
            fadeCanvasGroup.blocksRaycasts = true;

            float speed = Mathf.Abs(targetAlpha - fadeCanvasGroup.alpha) / Settings.fadeDuration;
            while (!Mathf.Approximately(targetAlpha, fadeCanvasGroup.alpha))
            {
                fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
                yield return null;
            }

            fadeCanvasGroup.blocksRaycasts = false;
            isFade = false;
        }
    }

}
