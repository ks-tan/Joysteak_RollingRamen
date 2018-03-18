using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private int _hForcePlayerInput = 20;
	private int _hForceWallCollide = 700;
	private float _scaleX;
	private float _scaleY;
	private float _originalScaleX;
	private float _originalScaleY;
	private Rigidbody2D _rigidbody;

	public static Player instance;

	void Start() {
		if (instance == null) {
			instance = this;
		}
		_scaleX = transform.localScale.x;
		_scaleY = transform.localScale.y;
		_originalScaleX = _scaleX;
		_originalScaleY = _scaleY;
		_rigidbody = GetComponent<Rigidbody2D> ();
	}

	void Update() {
		CheckPlayerInput ();
		CheckWallCollision ();
	}

	void LateUpdate() {
		transform.position = new Vector3 (transform.position.x, transform.position.y, 0);
	}

	void CheckPlayerInput() {
		if (Application.isMobilePlatform) {
			transform.Translate (Input.acceleration.x, Input.acceleration.y, 0);
		} else {
			if (Input.GetKey ("right")) {
				_rigidbody.AddForce (new Vector2 (_hForcePlayerInput, 0));
			} else if (Input.GetKey ("left")) {
				_rigidbody.AddForce (new Vector2 (-_hForcePlayerInput, 0));
			}
		}
	}

	void CheckWallCollision() {
		//Clamp player to edge of the screen
		Vector2 pos = Camera.main.WorldToViewportPoint(transform.position);
		pos.x = Mathf.Clamp01(pos.x);
		pos.y = Mathf.Clamp01(pos.y);
		transform.position = Camera.main.ViewportToWorldPoint(pos);
		//Check if colliding with wall
		bool isLeftWall = pos.x == 0;
		bool isRightWall = pos.x == 1;
		if (isLeftWall) {
			_rigidbody.AddForce (new Vector2 (_hForceWallCollide, 0));
		} else if (isRightWall) {
			_rigidbody.AddForce (new Vector2 (-_hForceWallCollide, 0));
		}
	}

	public void ResetPlayerSize() {
		_scaleX = _originalScaleX;
		_scaleY = _originalScaleY;
		transform.localScale = new Vector2 (_originalScaleX, _originalScaleY);
	}

	void OnTriggerEnter2D(Collider2D objCollider) {
		if (objCollider.gameObject.tag == "collectible") {
			GameObject collectible = objCollider.gameObject;
			string collectibleName = collectible.transform.GetComponent<Collectible> ().GetCollectibleName ();
			GameManager.instance.AddIngredient (collectibleName);
			GameObject.Destroy (objCollider.gameObject);
			transform.localScale = new Vector2 (++_scaleX, ++_scaleY);
		}
	}

}
	