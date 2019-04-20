using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class UIManager : LevelSingleton<UIManager>, ISubject
{
    private List<IObserver> observers = new List<IObserver>();

    public int playerCount;

    public PlayerShip playerShip;

    public HUD HUDClass;

    protected override void Awake()
    {
        Assert.IsNotNull(HUDClass);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Create HUD
        HUD Hud = Spawn(HUDClass, this, (HUD HUD) => {
            HUD.PlayerShip = playerShip;
            HUD.InGamePanel.GetComponent<UIPanel>().PlayerCount = playerCount;
        });
    }

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
