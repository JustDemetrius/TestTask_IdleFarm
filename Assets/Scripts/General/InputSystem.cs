using System;
using GardenLogic;
using UnityEngine;

namespace General
{
    public class InputSystem
    {
        public event Action<PlantSpot> OnPlantSpotClicked;
        
        public void TickUpdate()
        {
            if (Input.GetMouseButtonDown(0)) // in old input system its same as touch
            {
                TryGetSpot();
            }
        }

        private void TryGetSpot()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out PlantSpot spot))
                    OnPlantSpotClicked?.Invoke(spot);
            }
        }
    }
}