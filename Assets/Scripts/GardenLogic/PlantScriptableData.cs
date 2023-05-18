using UnityEngine;

namespace GardenLogic
{
    [CreateAssetMenu(fileName = "NewPlant", menuName = "Plants/Create new plant data")]
    public class PlantScriptableData : ScriptableObject
    {
        [SerializeField] private string _plantName;
        [SerializeField] private uint _overAllSecondsToGrow;
        [Space]
        [SerializeField] private float _objectScaleMultiplier = 1;
        [SerializeField] private GameObject _plantObject;
        [Space]
        [SerializeField] private bool _isAbleToCollect = true;
        [SerializeField] private uint _collectAmmount;

        public string PlantName => _plantName;
        public uint OverAllSecondsToGrow => _overAllSecondsToGrow;
        public GameObject PlantObject => _plantObject;
        public bool IsAbleToCollect => _isAbleToCollect;
        public int CollectAmmount => (int)_collectAmmount;
        public float ObjectScaleMultiplier => _objectScaleMultiplier;

    }
}