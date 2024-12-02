using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher instance;
    public static MainThreadDispatcher Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("MainThreadDispatcher");
                instance = obj.AddComponent<MainThreadDispatcher>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private static readonly Queue<Action> _executionQueue = new Queue<Action>();
    private static SynchronizationContext unityContext;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        unityContext = SynchronizationContext.Current;
    }
    void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                Action action = _executionQueue.Dequeue();
                if (action != null)
                    action.Invoke();
            }
        }
    }

    public void RunOnMainThread(Action action)
    {
        unityContext.Post(_ => action(), null);
    }

    public void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }
}
