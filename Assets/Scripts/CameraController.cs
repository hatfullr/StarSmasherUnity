using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera), typeof(PlayerInput)), ExecuteAlways]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 origin;
    [SerializeField, Min(0f)] private float distance;
    [Range(0.0f, 1.0f)] public float mouseSensitivity = 0.5f;
    public bool invertYAxis = false;
    public bool invertXAxis = false;

    private Camera _camera;
    
    [HideInInspector] public Quaternion lookRotation;
    private Vector3 orbitAngles;
    [HideInInspector] public Vector3 lookPosition { get => origin - lookDirection * distance; }
    [HideInInspector] public Vector3 lookDirection { get => lookRotation * Vector3.forward; }

    [HideInInspector] public new Camera camera
    {
        get
        {
            if (_camera == null) _camera = GetComponent<Camera>();
            return _camera;
        }
    }
    
    private PlayerInput _playerInput;

    [HideInInspector] public PlayerInput playerInput
    {
        get
        {
            if (_playerInput == null) _playerInput = GetComponent<PlayerInput>();
            return _playerInput;
        }
    }

    private Vector3 previousWorld;
    private bool doingRotation = false;

	void OnValidate()
	{
		Vector3 position = origin;
		position.z -= distance;
		transform.position = position;
	}

    void Start()
    {
        previousWorld = camera.ScreenToWorldPoint(0.5f * new Vector2(Screen.width, Screen.height));
    }
    
    void LateUpdate()
    {
        if (doingRotation) Rotate();
        transform.LookAt(origin);
    }

    public void MoveCamera(InputAction.CallbackContext context)
    {
        doingRotation = context.performed;
    }
    
    public void Rotate()
    {
        Vector2 dxy = Mouse.current.delta.ReadValue();
        float dx = dxy.y * mouseSensitivity;
        float dy = dxy.x * mouseSensitivity;
        if (!invertYAxis) dy = -dy;
        if (!invertXAxis) dx = -dx;

        orbitAngles = new Vector2(orbitAngles.x + dx, orbitAngles.y + dy);
        orbitAngles.x = Mathf.Clamp(orbitAngles.x, -90f, 90f);

        lookRotation = Quaternion.Euler(orbitAngles);
        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }
    
}
