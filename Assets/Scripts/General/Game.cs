using CameraLogic;
using GardenLogic;
using Player;
using UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace General
{
    public class Game
    {
        private const string MainSceneName = "Main";

        private const string AvatarPrefabPath = "Avatar";
        private const string GardenControllerPrefabPath = "GardenController";
        private const string GameHudControllerPrefabPath = "GameHudController";
        
        private readonly InputSystem _inputSystem;
        private readonly SceneLoader _sceneLoader;
        private GardenController _gardenController;
        private GameHudController _gameHudController;
        private PlayerAvatarController _playerAvatarController;
        private CameraController _cameraController;

        private int _currentExpAmmount = 0;
        private int _currentTomatoAmmount = 0;
        private Vector2 _selectedGardenSize;
        
        public Game(InputSystem inputSystem, SceneLoader sceneLoader, Vector2 gardenSize)
        {
            _inputSystem = inputSystem;
            _sceneLoader = sceneLoader;
            _selectedGardenSize = gardenSize;

            _sceneLoader.Load(MainSceneName, InitializeGameScene); // for the test - we will start game from here
        }

        private void InitializeGameScene()
        {
            _gardenController = Object.Instantiate(Resources.Load<GardenController>(GardenControllerPrefabPath));
            _gameHudController = Object.Instantiate(Resources.Load<GameHudController>(GameHudControllerPrefabPath));
            _playerAvatarController = Object.Instantiate(Resources.Load<PlayerAvatarController>(AvatarPrefabPath));
            _cameraController = new CameraController(Camera.main);

            _playerAvatarController.AttachCameraController(_cameraController);
            _gameHudController.InitController(_gardenController.AwailablePlants);
            _gardenController.InitController(_inputSystem, _gameHudController, _playerAvatarController, _selectedGardenSize);
            
            _gardenController.OnPlantFinished += HandleExperienceCollection;
            _gardenController.OnTomatoCollected += HandleTomatoCollection;
        }

        private void DeinitializeGameScene()
        {
            _gardenController = null;
            _gameHudController = null;
            _playerAvatarController = null;
            _cameraController = null;
            
            _gardenController.OnPlantFinished -= HandleExperienceCollection;
            _gardenController.OnTomatoCollected -= HandleTomatoCollection;
        }
        private void HandleExperienceCollection(int experience)
        {
            _currentExpAmmount += experience;
            _gameHudController.UpdateExperienceStats(_currentExpAmmount.ToString());
        }

        private void HandleTomatoCollection(int collected)
        {
            _currentTomatoAmmount += collected;
            _gameHudController.UpdateTomatoStats(_currentTomatoAmmount.ToString());
        }
    }
}