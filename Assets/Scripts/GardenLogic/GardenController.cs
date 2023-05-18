using System;
using System.Collections.Generic;
using CameraLogic;
using General;
using Player;
using UI;
using Unity.AI.Navigation;
using UnityEngine;

namespace GardenLogic
{
    public class GardenController : MonoBehaviour
    {
        private const int MaxSizeValue = 6;
        
        [Header("All awailable plants to grow")]
        [SerializeField] private List<PlantScriptableData> _awailablePlants;
        [SerializeField] private PlantSpot _plantPrefab;

        [Space] [Header("NavMeshSurface ref")] [SerializeField]
        private NavMeshSurface _navMeshSurface;

        public event Action<int> OnPlantFinished;
        public event Action<int> OnTomatoCollected;
        public List<PlantScriptableData> AwailablePlants => _awailablePlants;

        private int _gardenWidth = 6;
        private int _gardenHeight = 6;
        
        private InputSystem _inputSystem;
        private PlantSpot _lastInteractedPlant;
        private GameHudController _hudController;
        private PlayerAvatarController _playerAvatarController;

        private PlantScriptableData _selectedPlant;
        private bool IsAbleToPlant = true;

        public void InitController(InputSystem inputSystem, GameHudController hudController, PlayerAvatarController avatar, Vector2 gardenSize)
        {
            _inputSystem = inputSystem;
            _hudController = hudController;
            _playerAvatarController = avatar;
            
            _gardenWidth = gardenSize.x > MaxSizeValue ? MaxSizeValue : (int)gardenSize.x;
            _gardenHeight = gardenSize.y > MaxSizeValue ? MaxSizeValue : (int)gardenSize.y;
            
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
            
            _navMeshSurface.BuildNavMesh(); // baking navmesh map
        }

        private void TryToInteractWithBed(PlantSpot spot)
        {
            if (_hudController.IsAnyPopUpShowing || !IsAbleToPlant)
                return;
            
            _lastInteractedPlant = spot;
            
            if (_lastInteractedPlant.IsGrowing || (_lastInteractedPlant.AssignedPlantData != null && !_lastInteractedPlant.AssignedPlantData.IsAbleToCollect))
                return;
            
            if (_lastInteractedPlant.AssignedPlantData == null)
                _hudController.ShowPlantsList(SendAvatarToWork);
            else
                _playerAvatarController.MoveTo(_lastInteractedPlant, RigisterPlantDestroying);
                
        }

        private void RegisterPlantComplition(PlantScriptableData finishedPlantData)
        {
            OnPlantFinished?.Invoke(Mathf.FloorToInt(finishedPlantData.OverAllSecondsToGrow * 15));
            UpdateNavMeshMap();
        }
        private void RigisterPlantDestroying()
        {
            if (_lastInteractedPlant.AssignedPlantData.PlantName == "Tomato")
                OnTomatoCollected?.Invoke(3);
            
            _lastInteractedPlant.CollectFinalPlant(UpdateNavMeshMap);
        }
        
        private void UpdateNavMeshMap() =>
            _navMeshSurface.UpdateNavMesh(_navMeshSurface.navMeshData); // rebaking navmesh map
        
        private void SendAvatarToWork(PlantScriptableData chosenPlant)
        {
            IsAbleToPlant = false;
            _selectedPlant = chosenPlant;
            _playerAvatarController.MoveTo(_lastInteractedPlant, PlantSelectedTarget);
        }

        private void PlantSelectedTarget()
        {
            _lastInteractedPlant.PlantThis(_selectedPlant, RegisterPlantComplition);
            IsAbleToPlant = true;
        }
        private void OnDestroy()
        {
            _inputSystem.OnPlantSpotClicked -= TryToInteractWithBed;
        }
    } 
}