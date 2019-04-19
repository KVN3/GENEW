using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    void OnNotify(float score, float charges);
}

public interface ISubject
{
    void Subscribe(IObserver observer);
    void Notify(float score, float charges);
}