using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private float scaleX;
	private float scaleY;

	void Start() {
		scaleX = transform.localScale.x;
		scaleY = transform.localScale.y;
	}

	void Update() {
		transform.Translate (Input.acceleration.x, Input.acceleration.y, 0);
		ClampPositionToScreenSize ();
	}

	void LateUpdate() {
		transform.position = new Vector3 (transform.position.x, transform.position.y, 0);
	}

	void ClampPositionToScreenSize() {
		Vector2 pos = Camera.main.WorldToViewportPoint(transform.position);
		pos.x = Mathf.Clamp01(pos.x);
		pos.y = Mathf.Clamp01(pos.y);
		transform.position = Camera.main.ViewportToWorldPoint(pos);
	}

	void OnTriggerEnter2D(Collider2D objCollider) {
		if (objCollider.gameObject.tag == "collectible") {
			GameObject collectible = objCollider.gameObject;
			string collectibleName = collectible.transform.GetComponent<Collectible> ().GetCollectibleName ();
			GameManager.instance.AddIngredient (collectibleName);
			GameObject.Destroy (objCollider.gameObject);
			transform.localScale = new Vector2 (++scaleX, ++scaleY);
		}
	}

}
	