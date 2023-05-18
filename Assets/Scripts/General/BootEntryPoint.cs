using System;
using System.Collections.Generic;
using UnityEngine;

namespace General
{
    public class BootEntryPoint : MonoBehaviour
    {
        private SceneLoader _sceneLoader;
        private InputSystem _inputSystem;
        private Game _game;

        private void Awake()
        {
            DontDestroyOnLoad(this);

            _sceneLoader = new SceneLoader();
            _inputSystem = new InputSystem();
            _game = new Game(_inputSystem, _sceneLoader);
        }
        private void Update()
        {
            _inputSystem.TickUpdate();
        }
    }
}
