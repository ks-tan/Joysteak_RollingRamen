using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

	[SerializeField] private float _speed;
	private string _ingredientName;

	void Start() {
		List<string> ingredientList = GameManager.instance.GetIngredientList ();
		int ingredientListCount = ingredientList.Count;
		int ingredientIndex = Random.Range (0, ingredientListCount);
		_ingredientName = ingredientList [ingredientIndex];
		GetComponent<SpriteRenderer> ().sprite = GameManager.instance.GetIngredientSprites () [ingredientIndex];
	}

	public string GetCollectibleName() {
		return _ingredientName;
	}

	void Update() {
		Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
		if(1.0 < pos.y) {
			GameObject.Destroy (this.gameObject);
		} else {
			transform.Translate(Vector3.up * Time.deltaTime * _speed, Space.World);
		}
	}
}
