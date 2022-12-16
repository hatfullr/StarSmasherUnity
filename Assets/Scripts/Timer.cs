using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEditor.SceneManagement;

public class Timer : MonoBehaviour
{
    public float limit;
    public bool startOnCreation;
    public bool destroyOnStop;
    public bool runWhileDisabled;

    public UnityEvent onStart = new UnityEvent();
    public UnityEvent onStop = new UnityEvent();
    public UnityEvent onUpdate = new UnityEvent();
    public UnityEvent onLateUpdate = new UnityEvent();

    [Header("Debugging")]
    [ReadOnly] public float time;
    [ReadOnly, SerializeField] private float _progress;
    [ReadOnly] public bool isActive;
    [SerializeField] private bool debug = false;

    private float timeAtBegin;

    private bool runningWhileDisabled;

    [HideInInspector] public float progress
    {
        get
        {
            if (limit == 0f) _progress = 0f;
            else _progress = time / limit;
            return _progress;
        }
    }

    public void Pause() { isActive = false; }
    public void Resume() { isActive = true; }

    void Start()
    {
        if (debug) Debug.Log("Start");
        if (startOnCreation) Begin();
    }
    
    void Update()
    {
        if (debug) Debug.Log("Update: isActive = " + isActive);
        if (isActive)
        {
            time = Mathf.Min(Time.time - timeAtBegin, limit);
            if (time == limit) Stop();
            else onUpdate.Invoke();
        }
    }
    void LateUpdate()
    {
        if (debug) Debug.Log("LateUpdate: isActive = " + isActive);
        if (isActive) onLateUpdate.Invoke();
    }

    void OnDestroy()
    {
        if (debug) Debug.Log("OnDestroy");
    }

    // Continue to run the timer even when the GameObject is inactive in the heirarchy
    void OnDisable()
    {
        if (debug) Debug.Log("OnDisable: isActive, runWhileDisabled, runningWhileDisabled = " + isActive + ", " + runWhileDisabled + ", " + runningWhileDisabled);
        if (isActive && runWhileDisabled && !runningWhileDisabled)
        {
            InvokeRepeating("Update", 0f, 0.00833333333f);
            runningWhileDisabled = true;
        }
    }
    void OnEnable()
    {
        if (debug) Debug.Log("OnEnable: runningWhileDisabled = " + runningWhileDisabled);
        if (runningWhileDisabled) // Let the regular Update step handle increments now
        {
            CancelInvoke("Update");
            runningWhileDisabled = false;
            isActive = true;
        }
    }

    private void Increment(float t = 0.00833333333f)
    {
        if (debug) Debug.Log("Increment: time = " + time);
        time = Mathf.Min(time + t, limit);
        if (time == limit) Stop();
    }

    public void Begin()
    {
        if (debug) Debug.Log("Begin: runWhileDisabled, gameObject.activeInHierarchy = " + runWhileDisabled + ", " + gameObject.activeInHierarchy);
        // Reset the time first if it's already running
        if (isActive || runningWhileDisabled) Reset();

        timeAtBegin = Time.time;
        onStart.Invoke();
        isActive = true;
        

        if (runWhileDisabled && !gameObject.activeInHierarchy)
        {
            InvokeRepeating("Update", 0f, 0.00833333333f);
            runningWhileDisabled = true;
        }
        else runningWhileDisabled = false;
    }

    public void Begin(float offset)
    {
        Begin();
        timeAtBegin -= offset;
    }

    public void Stop()
    {
        if (debug) Debug.Log("Stop: isActive, runningWhileDisabled, destroyOnStop = " + isActive + ", " + runningWhileDisabled + ", " + destroyOnStop);
        if (runningWhileDisabled)
        {
            CancelInvoke("Update");
            runningWhileDisabled = false;
        }

        // Call the update step one last time
        time = limit;
        isActive = false;

        onUpdate.Invoke();
        onStop.Invoke();
        if (destroyOnStop)
        {
#if UNITY_EDITOR
            DestroyImmediate(transform.gameObject);
#else
            Destroy(transform.gameObject);
#endif
        }
    }

    public void Reset()
    {
        if (debug) Debug.Log("Reset: runningWhileDisabled = " + runningWhileDisabled);
        // Reset variables
        isActive = false;
        time = 0.0f;

        if (gameObject != null ? gameObject.activeInHierarchy && runningWhileDisabled : false)
        {
            CancelInvoke("Update");
            runningWhileDisabled = false;
        }
    }


    public void AddOnStart(UnityAction callback) { onStart.AddListener(callback); }
    public void AddOnStop(UnityAction callback) { onStop.AddListener(callback); }
    public void AddOnUpdate(UnityAction callback) { onUpdate.AddListener(callback); }
    public void AddOnLateUpdate(UnityAction callback) { onLateUpdate.AddListener(callback); }
    public void RemoveOnStart(UnityAction callback) { onStart.RemoveListener(callback); }
    public void RemoveOnStop(UnityAction callback) { onStop.RemoveListener(callback); }
    public void RemoveOnUpdate(UnityAction callback) { onUpdate.RemoveListener(callback); }
    public void RemoveOnLateUpdate(UnityAction callback) { onLateUpdate.RemoveListener(callback); }
}

