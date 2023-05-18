using System;
using System.Collections.Generic;
using General;
using Player;
using UI;
using UnityEngine;

namespace GardenLogic
{
    public class GardenController : MonoBehaviour
    {
        private const int MaxDimentionCount = 6;
        
        [Header("Garden dementions")]
        [SerializeField] private uint _gardenWidth = 5;
        [SerializeField] private uint _gardenHeight = 5;
        [Space]
        [Header("All awailable plants to grow")]
        [SerializeField] private List<PlantScriptableData> _awailablePlants;
        [SerializeField] private PlantSpot _plantPrefab;

        public event Action<int> OnPlantGathering;
        public List<PlantScriptableData> AwailablePlants => _awailablePlants;

        private InputSystem _inputSystem;
        private PlantSpot _lastInteractedPlant;
        private GameHudController _hudController;
        private PlayerAvatarController _playerAvatarController;

        private PlantScriptableData _selectedPlant;

        private void Awake()
        {
            // for preventing very large garden :)
            _gardenWidth = _gardenWidth > MaxDimentionCount ? MaxDimentionCount : _gardenWidth;
            _gardenHeight = _gardenHeight > MaxDimentionCount ? MaxDimentionCount : _gardenHeight;
        }

        public void Init(InputSystem inputSystem, GameHudController hudController, PlayerAvatarController avatar)
        {
            _inputSystem = inputSystem;
            _hudController = hudController;
            _playerAvatarController = avatar;
            CreateGarden();
            _inputSystem.OnPlantSpotClicked += TryToInteractWithBed;
        }

        private void CreateGarden()
        {
            var gridPoints = GridGenerator.GenerateGrid(width: (int)_gardenWidth, height: (int)_gardenHeight);
            var gridHolder = GameObject.FindGameObjectWithTag("GridHolder");
            gridHolder.transform.position =
                new Vector3((_gardenWidth * 0.5f) * -1, 0f, (_gardenHeight * 0.5f) * -1);
            
            if (gridHolder == null)
            {
                Debug.LogError("GridHolder not found on scene!!! Check assigned tag.");
                return;
            }

            var gridSpots = new List<PlantSpot>();
            
            for (int i = 0; i < gridPoints.Count; i++)
            {
                var newSpot = Instantiate(_plantPrefab, gridHolder.transform);
                newSpot.transform.localPosition = gridPoints[i];
                gridSpots.Add(newSpot);
            }
        }

        private void TryToInteractWithBed(PlantSpot spot)
        {
            if (_hudController.IsAnyPopUpShowing)
                return;
            
            _lastInteractedPlant = spot;
            
            if (_lastInteractedPlant.IsGrowing || (_lastInteractedPlant.AssignedPlantData != null && !_lastInteractedPlant.AssignedPlantData.IsAbleToCollect))
                return;
            
            if (_lastInteractedPlant.AssignedPlantData == null)
                _hudController.ShowPlantsList(SendAvatarToWork);
            else
            {
                OnPlantGathering?.Invoke(Mathf.FloorToInt(_lastInteractedPlant.AssignedPlantData.OverAllSecondsToGrow * 15));
                _lastInteractedPlant.CollectFinalPlant();
            }
                
        }

        private void SendAvatarToWork(PlantScriptableData chosenPlant)
        {
            _selectedPlant = chosenPlant;
            _playerAvatarController.MoveTo(_lastInteractedPlant, PlantSelectedTarget);
        }
        private void PlantSelectedTarget() =>
            _lastInteractedPlant.PlantThis(_selectedPlant);
            
        private void OnDestroy()
        {
            _inputSystem.OnPlantSpotClicked -= TryToInteractWithBed;
        }
    } 
}