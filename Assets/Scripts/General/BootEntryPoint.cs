using UnityEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace General
{
    public class BootEntryPoint : MonoBehaviour
    {
        // for this testTask, we will assign here out garden dimensions, but its shouldn't be here
        [SerializeField] private TMP_InputField _gardenWidth;
        [SerializeField] private TMP_InputField _gardenHeight;
        [SerializeField] private Button _applyButton;
        
        private SceneLoader _sceneLoader;
        private InputSystem _inputSystem;
        private Game _game;

        private void Awake()
        {
            DontDestroyOnLoad(this);

            _sceneLoader = new SceneLoader();
            _inputSystem = new InputSystem();
            _applyButton.onClick.AddListener(StartTheGame);
        }
        
        private void StartTheGame()
        {
            int width = string.IsNullOrEmpty(_gardenWidth.text) ? 6 : int.Parse(_gardenWidth.text);
            int height = string.IsNullOrEmpty(_gardenHeight.text) ? 6 : int.Parse(_gardenHeight.text);
            
            _game = new Game(_inputSystem, _sceneLoader, new Vector2(width, height));
        }
        private void Update()
        {
            _inputSystem.TickUpdate();
        }
    }
}
