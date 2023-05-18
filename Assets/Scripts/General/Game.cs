using System.Collections;
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

        private int _currentExpAmmount = 0;
        
        public Game(InputSystem inputSystem, SceneLoader sceneLoader)
        {
            _inputSystem = inputSystem;
            _sceneLoader = sceneLoader;

            TestsingGameStart(); // for the test - we will start game from here
        }
        
        private void TestsingGameStart()
        {
            _sceneLoader.Load(MainSceneName, InitializeGame);
        }

        private void InitializeGame()
        {
            _gardenController = Object.Instantiate(Resources.Load<GardenController>(GardenControllerPrefabPath));
            _gameHudController = Object.Instantiate(Resources.Load<GameHudController>(GameHudControllerPrefabPath));
            _playerAvatarController = Object.Instantiate(Resources.Load<PlayerAvatarController>(AvatarPrefabPath));

            _gameHudController.InitController(_gardenController.AwailablePlants);
            _gardenController.Init(_inputSystem, _gameHudController, _playerAvatarController);
            _gardenController.OnPlantGathering += HandleExperienceCollection;
        }

        private void HandleExperienceCollection(int experience)
        {
            _currentExpAmmount += experience;
            _gameHudController.UpdateExperienceStats(_currentExpAmmount.ToString());
        }
    }
}