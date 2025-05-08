using System.Collections;
using UnityEngine;

namespace KBS
{
    public enum SceneType
    {
        None,
        Empty,
        Title,
        Ingame
    }
    public class Main : SingletonBase<Main>
    {
        public SceneBase currentScene;
        private SceneType currentSceneType;


        private void Start()
        {
            StartUp();
        }

        public void Initialize()
        {

        }

        public void StartUp()
        {
            //Start로 시작하는 씬을 타이틀 씬으로 설정
            // ChangeScene(SceneType.Title);
        }

        public void ChangeScene(SceneType sceneType)
        {
            switch (sceneType)
            {
                case SceneType.Title:
                    {
                        ChangeScene<TitleScene>(sceneType);
                    }
                    break;
                case SceneType.Ingame:
                    {
                        ChangeScene<IngameScene>(sceneType);
                    }
                    break;
            }
        }

        private void ChangeScene<T>(SceneType sceneType) where T : SceneBase
        {
            StartCoroutine(InternalChangeScene<T>(sceneType));
        }

        private IEnumerator InternalChangeScene<T>(SceneType sceneType) where T : SceneBase
        {
            if (currentScene != null)
            {
                yield return StartCoroutine(currentScene.OnEnd());
                Destroy(currentScene.gameObject);
            }

            GameObject newSceneInstance = new GameObject(typeof(T).Name);
            newSceneInstance.transform.SetParent(this.transform);
            currentScene = newSceneInstance.AddComponent<T>();
            yield return StartCoroutine(currentScene.OnStart());                 
        }

    
    }
}
