using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GardenLogic
{
    public class PlantSpot : MonoBehaviour
    {
        [Header("Plant UI")] 
        [SerializeField] private Image _timerBg;
        [SerializeField] private TextMeshProUGUI _timerTMP;
        [Space(15)]
        [Header("FinalPlantSpot")]
        [SerializeField] private Transform _plantToGrowSpot;

        public bool IsGrowing { get; private set; }
        public PlantScriptableData AssignedPlantData { get; private set; }

        private GardenController _gardenController;
        private GrowingTimer _growingTimer;
        private Action<PlantScriptableData> _callBack;

        private void Awake()
        {
            IsGrowing = false;
            
            _growingTimer = new GrowingTimer(
                _timerBg,
                _timerTMP,
                FinishGrowing);
        }
        public void PlantThis(PlantScriptableData toPlant, Action<PlantScriptableData> callBack = null)
        {
            if (IsGrowing)
                return;

            _callBack = callBack;
            IsGrowing = true;
            AssignedPlantData = toPlant;

            _growingTimer.StartTimer((int)toPlant.OverAllSecondsToGrow);
        }
        public void CollectFinalPlant(Action onPlantDestroyed = null)
        {
            if (IsGrowing)
                return;
            
            _plantToGrowSpot.DOScale(Vector3.zero, 0.5f)
                .OnComplete(() =>
                {
                    Destroy(_plantToGrowSpot.GetChild(0).gameObject);
                    onPlantDestroyed?.Invoke();
                });
            
            AssignedPlantData = null;
        }

        private void FinishGrowing()
        {
            IsGrowing = false;
            var finalPlant = Instantiate(AssignedPlantData.PlantObject, _plantToGrowSpot);
            finalPlant.transform.localScale = Vector3.zero;
            
            finalPlant.transform.DOScale(Vector3.one, 0.5f)
                .OnComplete(() => _callBack?.Invoke(AssignedPlantData));
            
            _plantToGrowSpot.localScale = Vector3.one * AssignedPlantData.ObjectScaleMultiplier;
        }
    }
}
