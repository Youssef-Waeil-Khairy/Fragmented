using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class RoomController : MonoBehaviour
{
    public GameObject Room;
    
    [Header("Rotation")]
    [SerializeField] float rotationSpeed = 30f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.qKey.isPressed)
        {
            Room.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        if (Keyboard.current.eKey.isPressed)
        {
            Room.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
    }
}
