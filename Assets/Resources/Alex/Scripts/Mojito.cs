using UnityEngine;

public class Mojito : Tile
{
	public AudioSource audioSource;
	
	public GameObject splashPrefab;

	public GameObject drinkTopObj;

	public float shootForce = 1000f;

	public float cooldownTime = 0.1f;

	protected float _cooldownTimer;

	protected void aim() {
		_sprite.transform.localPosition = new Vector3(1f, 0, 0);
		float aimAngle = Mathf.Atan2(_tileHoldingUs.aimDirection.y, _tileHoldingUs.aimDirection.x)*Mathf.Rad2Deg;
		transform.localRotation = Quaternion.Euler(0, 0, aimAngle);
		if (_tileHoldingUs.aimDirection.x < 0) {
			_sprite.flipY = true;
			drinkTopObj.transform.localPosition = new Vector3(drinkTopObj.transform.localPosition.x, -Mathf.Abs(drinkTopObj.transform.localPosition.y), drinkTopObj.transform.localPosition.z);
		}
		else {
			_sprite.flipY = false;
			drinkTopObj.transform.localPosition = new Vector3(drinkTopObj.transform.localPosition.x, Mathf.Abs(drinkTopObj.transform.localPosition.y), drinkTopObj.transform.localPosition.z);
		}
	}

	protected virtual void Update() {
		if (_cooldownTimer > 0) {
			_cooldownTimer -= Time.deltaTime;
		}

		if (_tileHoldingUs != null) {
			// If we're held, rotate and aim the drink.
			aim();
		}
		else {
			// Otherwise, move the drink back to the normal position. 
			_sprite.transform.localPosition = Vector3.zero;
			transform.rotation = Quaternion.identity;
		}
		updateSpriteSorting();
	}

	public override void useAsItem(Tile tileUsingUs) {
		if (_cooldownTimer > 0) {
			return;
		}

		// First, make sure we're aimed properly (to avoid shooting ourselves by accident)
		aim();
		
		// Get starting point of drink
		Vector2 drinkTopPos = drinkTopObj.transform.position;

		// Spawn the splash obj
		GameObject newSplash = Instantiate(splashPrefab);
		newSplash.transform.parent = tileUsingUs.transform.parent;
		newSplash.transform.position = drinkTopObj.transform.position;
		newSplash.transform.rotation = transform.rotation;

		audioSource.Play();

		newSplash.GetComponent<Tile>().init();
		newSplash.GetComponent<Tile>().addForce(tileUsingUs.aimDirection.normalized*shootForce);

		_cooldownTimer = cooldownTime;
	}
}
