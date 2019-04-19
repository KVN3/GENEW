using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Example
public abstract class Observer : MyMonoBehaviour
{
    public abstract void OnNotify();
}

//
public abstract class Subject : MyMonoBehaviour
{
    private List<IObserver> observers = new List<IObserver>();

    public void Subscribe(IObserver observer)
    {
        observers.Add(observer);
    }

    public void Notify(float score, float charges)
    {
        foreach (IObserver observer in observers)
            observer.OnNotify(score, charges);
    }
}
