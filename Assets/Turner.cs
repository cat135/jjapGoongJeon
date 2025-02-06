using UnityEngine;

public class Turner : MonoBehaviour
{
    private float _speed = 50f;
    void Update()
    {
        transform.Rotate(0, 0, _speed * Time.deltaTime);
    }
}
