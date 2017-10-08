using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

	[SerializeField] private float speed;
	private string ingredientName;

	void Start() {
		List<string> ingredientList = GameManager.instance.GetIngredientList ();
		int ingredientListCount = ingredientList.Count;
		ingredientName = ingredientList [Random.Range (0, ingredientListCount)];
	}

	public string GetCollectibleName() {
		return ingredientName;
	}

	void Update() {
		Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
		if(1.0 < pos.y) {
			GameObject.Destroy (this.gameObject);
		} else {
			transform.Translate(Vector3.up * Time.deltaTime * speed, Space.World);
		}
	}
}
