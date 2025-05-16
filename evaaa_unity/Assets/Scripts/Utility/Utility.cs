using UnityEngine;

namespace Assets.Scripts.Utility
{
    [System.Serializable]
    public class ThreeDVector
    {
        public float x;
        public float y;
        public float z;

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [System.Serializable]
    public class ColorVector
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public Color ToColor()
        {
            return new Color(r, g, b, a);
        }
    }

    [System.Serializable]
    public class EVRange
    {
        public float min;
        public float max;
    }

    [System.Serializable]
    public class Coefficient
    {
        public float change_0;
        public float change_1;
        public float change_2;
        public float change_3;
        public float change_4;
        public float change_5;
    }

    [System.Serializable]
    public class PositionRange
    {
        public float xMin = 0, xMax = 0, yMin = 0, yMax = 0, zMin = 0, zMax = 0;
    }

    [System.Serializable]
    public class RotationRange
    {
        public float x = 0, y = 0, z = 0;
    }

    [System.Serializable]
    public class ScaleRange
    {
        public float xMin = 1, xMax = 1, yMin = 1, yMax = 1, zMin = 1, zMax = 1;
    }

    public static class OverlapUtility
    {
        public static bool IsOverlapping(Vector3 position, GameObject prefab, Vector3 scale, float boxSizeMultiplier = 1.0f, float padding = 2.0f, string execName = "None")
        {
            Collider prefabCollider = prefab.GetComponent<Collider>();
            if (prefabCollider == null)
            {
                Debug.LogError($"Prefab {prefab.name} does not have a Collider component.");
                return true;
            }

            // Calculate the scaled bounds
            Vector3 extentBound = Vector3.Scale(prefabCollider.bounds.extents, scale) * boxSizeMultiplier;
            extentBound += Vector3.one * padding; // Add padding to all dimensions

            Collider[] colliders = Physics.OverlapBox(position, extentBound, Quaternion.identity);
            foreach (Collider collider in colliders)
            {
                // Ignore specific game objects
                if (collider.gameObject.name == "Court_Floor" ||
                    collider.gameObject.name.Contains(execName) ||
                    collider.gameObject.name.Contains("Court_Wall") ||
                    collider.gameObject.name == "Block" ||
                    collider.gameObject.name == "LandmarkAreaMesh" ||
                    collider.gameObject.tag == "sensor" ||
                    collider.gameObject.tag == "thermalGridCube")
                {
                    continue;
                }

                if (collider.gameObject != prefab)
                {
                    // Debug.Log($"{prefab.name} object's overlap detected: {collider.gameObject.name}");
                    return true;
                }
            }

            return false;
        }
    }

    // Wating specific time funtion
    // public static class Wait
    // {
    //     public static void ForSeconds(float seconds)
    //     {
    //         System.Threading.Thread.Sleep((int)(seconds * 1000));
    //     }
    // }

}