using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace General
{
    public class SceneLoader
    {
        private string _currentScene;

        public SceneLoader()
        {
            _currentScene = SceneManager.GetActiveScene().name;
        }
        
        public async void Load(string sceneName, Action onLoadedCallback = null)
        {
            if (sceneName == _currentScene)
            {
                onLoadedCallback?.Invoke();
                return;
            }
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone) //awaiting when loading scene is done
                await Task.Yield();

            _currentScene = sceneName;
            onLoadedCallback?.Invoke(); //if we needed callback when scene loading is done
        }
    }
}
