using GardenLogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlantUiLine : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _plantNameTMP;
        [SerializeField] private TextMeshProUGUI _growTimeTMP;
        [SerializeField] private Button _lineButton;

        public delegate void OnLineClick(PlantScriptableData lineClickedData);
        public event OnLineClick OnLineClicked;
        
        private PlantScriptableData _data;

        public void Init(PlantScriptableData data)
        {
            _data = data;

            _plantNameTMP.text = _data.PlantName;
            _growTimeTMP.text = $"{_data.OverAllSecondsToGrow} sec";
            
            _lineButton.onClick.AddListener(() => OnLineClicked?.Invoke(_data));
        }
    }
}