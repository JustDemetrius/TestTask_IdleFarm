using System;

namespace GardenLogic
{
    public interface IPlant
    {
        public void PlantThis(PlantScriptableData plantData, Action<PlantScriptableData> onPlantGrowUp = null); //callBack if needed
        public void CollectFinalPlant(Action onPlantDestroyed = null); // onPlantDestroyed if we need to know when plant is destroyed after collection
    }
}