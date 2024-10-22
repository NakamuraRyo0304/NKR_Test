using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField, HideInspectorForPrefab] private float rotate_speed = 15f;

    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(0, Time.deltaTime * rotate_speed, 0, Space.Self);
    }
}
