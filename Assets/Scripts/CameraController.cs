using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera), typeof(PlayerInput)), ExecuteAlways]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 origin;
    [SerializeField, Min(0.001f)] private float distance;
    [Range(0.0f, 1.0f)] public float mouseSensitivity = 0.5f;
	[Min(0f)] public float scrollSensitivity = 2f;
    public bool invertYAxis = false;
    public bool invertXAxis = false;

	[Header("References")]
	[SerializeField] private Timer zoomTimer;

    private Camera _camera;
    
    [HideInInspector] public Quaternion lookRotation;
    private Vector3 orbitAngles;
    [HideInInspector] public Vector3 lookPosition { get => origin - lookDirection * distance; }
    [HideInInspector] public Vector3 lookDirection { get => lookRotation * Vector3.forward; }

	private float distanceZoomStart, distanceZoomStop;
	private int previousDirection;

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
		if (!Application.isPlaying)
		{
			Vector3 position = origin;
			position.z -= distance;
			transform.position = position;
		}
	}

    void Start()
    {
        previousWorld = camera.ScreenToWorldPoint(0.5f * new Vector2(Screen.width, Screen.height));
    }
    
    void LateUpdate()
    {
        if (doingRotation) Rotate();
        transform.LookAt(origin);
		Vector3 direction = (transform.position - origin).normalized;
		transform.position = origin + direction * distance;
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

	public void DoZoom()
    {
        // Smoothly zoom from point A to B
        if (zoomTimer.limit > 0f)
        {
            distance = Mathf.SmoothStep(distanceZoomStart, distanceZoomStop, zoomTimer.progress);
        }
    }

	public void Zoom(InputAction.CallbackContext context)
    {
        if (Application.isFocused && context.performed)
        {
            float scrollAmount = -Mouse.current.scroll.ReadValue().y / 120.0f * scrollSensitivity;

            // Makes camera zoom faster when it's further away from the focus
            //scrollAmount *= (distanceFromFocus * 0.3f);

			float distanceZoom = distance + scrollAmount;

            // When we change the directions, just stop the zooming altogether at the current distance
            if (zoomTimer.isActive && previousDirection != (int)Mathf.Sign(scrollAmount)) // Change in directions
                distanceZoom = distance;

            distanceZoomStart = distance;
            distanceZoomStop = distanceZoom;

            previousDirection = (int)Mathf.Sign(scrollAmount);
			
            // Start the zooming
            if (zoomTimer.limit == 0.0f) zoomTimer.onUpdate.Invoke();
            else zoomTimer.Begin();
        }
    }
    
}
