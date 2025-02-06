using System;
using UnityEngine;

public class MapData : MonoBehaviour
{
    public float minDeadX = -5f;
    public float maxDeadX = 5f;
    public float minDeadY = -10.5f;
    public float maxDeadY = 10f;
    
    public void Init()
    {
        minDeadX = -5f;
        maxDeadX = 5f;
        minDeadY = -10.5f;
        maxDeadY = 10f;
    }
    
    private static MapData s_instance;

    public static MapData Map
    {
        get
        {
            if (s_instance != null) return s_instance;

            var go = GameObject.Find("@MapData");
            if (go == null)
            {
                go = new GameObject { name = "@MapData" };
                go.transform.position = Vector3.zero;
                go.AddComponent<MapData>();
            }

            s_instance = go.GetComponent<MapData>();
            s_instance.Init();
            return s_instance;
        }
    }

    public bool CheckIfOutOfMap(Vector3 position)
    {
        return position.x < maxDeadX && position.x > minDeadX && position.y > minDeadY &&
               position.y < maxDeadY;
    }
}
