using UnityEngine;
using UnityEngine.Assertions;

public abstract class LevelSingleton<T> : MyMonoBehaviour where T : LevelSingleton<T> {
	protected static T _Instance;
	
	protected virtual void Awake() {
		Assert.IsFalse(Initialized);

		SetInstance((T)this);

		Assert.IsTrue(Initialized);
	}

	public static T Instance {
		get {
			Assert.IsTrue(Initialized);

			return _Instance;
		}
	}

	public static bool Initialized => _Instance != null;

	public static void SetInstance(T Instance) {
		_Instance = Instance;
	}
}