using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Example of a tile that can be picked up.
// A simple rock that can be thrown by the player and enemies alike
public class LootBoxWeapon : Tile
{
	public GameObject[] maybeWeapons;
	// Sound that's played when we're thrown.
	public AudioClip throwSound;

	// How much force to add when thrown
	public float throwForce = 3000f;

	// How slow we need to be going before we consider ourself "on the ground" again
	public float onGroundThreshold = 0.8f;

	// How much relative velocity we need with a target on a collision to cause damage.
	public float damageThreshold = 14;
	// How much force we apply to a target when we deal damage. 
	public float damageForce = 1000;

	// We keep track of the tile that threw us so we don't collide with it immediately.
	protected Tile _tileThatThrewUs = null;

	// Keep track of whether we're in the air and whether we were JUST thrown
	protected bool _isInAir = false;
	protected float _afterThrowCounter;
	public float afterThrowTime = 0.2f;


	// Like walls, rocks need explosive damage to be hurt.
	public override void takeDamage(Tile tileDamagingUs, int amount, DamageType damageType)
	{
		if (damageType == DamageType.Explosive)
		{
			base.takeDamage(tileDamagingUs, amount, damageType);
		}
	}


	// The idea is that we get thrown when we're used
	public override void useAsItem(Tile tileUsingUs)
	{
		int weaponNum = Random.Range(0, 6);
		
		GameObject newBullet = Instantiate(maybeWeapons[weaponNum]);
		newBullet.transform.parent = tileUsingUs.transform.parent;
		newBullet.transform.position = transform.position;
		newBullet.transform.rotation = transform.rotation;
		newBullet.GetComponent<Tile>().init();


		//Vector2 spawnP = toGridCoord(transform.position);
		//GameObject newBullet = spawnTile(maybeWeapons[weaponNum], tileUsingUs.transform.parent, 0, 0);
		//newBullet.transform.position = transform.position;
		//GameObject newBullet = Instantiate(maybeWeapons[weaponNum]);
		//newBullet.transform.parent = tileUsingUs.transform.parent;

		//Instantiate(maybeWeapons[weaponNum], transform.position, Quaternion.identity);
		Destroy(gameObject);
	}

	protected virtual void Update()
	{



		updateSpriteSorting();
	}

	// When we collide with something in the air, we try to deal damage to it.
	public virtual void OnCollisionEnter2D(Collision2D collision)
	{
		if (_isInAir && collision.gameObject.GetComponent<Tile>() != null)
		{
			float impact = collisionImpactLevel(collision);
			// First, make sure we're going fast enough to do damage
			if (impact <= damageThreshold)
			{
				return;
			}
			Tile otherTile = collision.gameObject.GetComponent<Tile>();
			otherTile.takeDamage(this, 1);
			otherTile.addForce(_body.linearVelocity.normalized * damageForce);
		}
	}



}
