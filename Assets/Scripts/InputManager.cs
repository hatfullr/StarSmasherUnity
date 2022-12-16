using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset _actions;

    public InputActionAsset actions
    {
        get => _actions;
        set => _actions = value;
    }

    private InputAction quitInputAction { get; set; }
    private InputAction interactInputAction { get; set; }

    private void OnEnable()
    {
        Setup();
        quitInputAction?.Enable();
        interactInputAction?.Enable();
        
    }

    private void Setup()
    {

        interactInputAction = actions.FindAction("Interact");
        if (interactInputAction != null)
        {
            interactInputAction.started += OnInteract;
            interactInputAction.performed += OnInteract;
            interactInputAction.canceled += OnInteract;
        }
        else
        {
            Debug.LogError("Missing Interact Binding");
        }

        quitInputAction = actions.FindAction("Application Quit");
        if (quitInputAction != null)
        {
            quitInputAction.started += OnAppQuit;
            quitInputAction.performed += OnAppQuit;
            quitInputAction.canceled += OnAppQuit;
        }
        else
        {
            Debug.LogError("Missing Application Quit Binding");
        }
    }

    protected virtual void OnAppQuit(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            Debug.Log("Quit");
            Application.Quit();
        }

        else if (context.canceled)
            Debug.Log("Application Quit Cancelled");
    }

    protected virtual void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            Debug.Log("Interact");

        }

        else if (context.canceled)
            Debug.Log("Application Quit Cancelled");
    }
}
