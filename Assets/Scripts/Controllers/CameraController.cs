using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Define.CameraMode _mode = Define.CameraMode.QuarterView;

    [SerializeField]
    Vector3 _delta = new Vector3(0.0f, 0.5f, -5.0f);

    [SerializeField]
    GameObject _player = null;
    
    private const float MinY = -6.0f;
    private const float MaxY = 6.5f;
    
    public void SetPlayer(GameObject player) { _player = player; }

    void Start()
    {
        
    }

    void LateUpdate()
    {
        if (_mode == Define.CameraMode.QuarterView)
        {
            if (_player == null || !_player.IsValid())
            {
                return;
            }

            Vector3 targetPosition;
            RaycastHit hit;

            if (Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, 1 << (int)Define.Layer.Block))
            {
                float dist = (hit.point - _player.transform.position).magnitude * 0.8f;
                targetPosition = _player.transform.position + _delta.normalized * dist;
            }
            else
            {
                targetPosition = _player.transform.position + _delta;
            }

            // Y-axis boundary clamp
            targetPosition.y = Mathf.Clamp(targetPosition.y, MinY, MaxY);

            // Fix X-axis to the initial X position
            targetPosition.x = 0f;

            // Set camera position
            transform.position = targetPosition;

            // Ensure the camera looks at the player without X-axis and Y-axis rotation
            Vector3 lookDirection = _player.transform.position - transform.position;

            // Fix both X and Y-axis in look direction
            lookDirection.x = 0; // Prevent horizontal rotation due to X-axis movement
            lookDirection.y = 0; // Prevent vertical rotation due to Y-axis movement

            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }


    public void SetQuarterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuarterView;
        _delta = delta;
    }
}
