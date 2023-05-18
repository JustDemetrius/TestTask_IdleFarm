using System.Collections.Generic;
using UnityEngine;

namespace GardenLogic
{
    public static class GridGenerator
    {
        private const float PointsOffset = 1.3f;
        public static List<Vector3> GenerateGrid(int width, int height)
        {
            List<Vector3> points = new List<Vector3>();
            
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    var point = new Vector3(x, 0f, z) * PointsOffset;
                    points.Add(point);
                }
            }
            
            return points;
        }
    }
}
