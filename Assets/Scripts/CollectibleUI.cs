using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleUI : MonoBehaviour {

	public string collectibleName;
	public int collectibleCount;

	public void UpdateCollectibleCountText () {
		transform.Find ("Text").GetComponent<Text> ().text = "x" + collectibleCount;
	}

}
