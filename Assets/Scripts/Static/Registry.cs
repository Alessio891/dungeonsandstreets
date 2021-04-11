using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Registry {
	public static GameAssets assets;

	static Registry()
	{		
		assets = Resources.Load<GameAssets> ("Data/GameAssets");
	}
}
