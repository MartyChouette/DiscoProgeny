using System.Collections;
using UnityEngine;

public class Splash : Tile
{
    public float damageThreshold = 14;

	public float onGroundThreshold = 1f;

	float destructionTimer = 0f;

	protected float _destroyTimer = 1f;

	bool inCoroutine = false;

    void Update()
    {
        // in case, it doesn't die on collision
		destructionTimer += Time.deltaTime;
		if (destructionTimer >= _destroyTimer) {
			die();
		}
    }


    public virtual void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.GetComponent<Tile>() != null) {
			float impact = collisionImpactLevel(collision);
			if (impact < damageThreshold) {
				return;
			}
			Tile otherTile = collision.gameObject.GetComponent<Tile>();

			Color purpleColor = new Color(0.6709301f, 0.3404236f, 0.8490566f, 1);

			if (otherTile.GetComponent<Dancer>() != null) {
				Dancer otherDancer = otherTile.GetComponent<Dancer>();
				otherDancer.SetIntoxication(true);
				SpriteRenderer otherSpriteRenderer = otherTile.gameObject.GetComponentInChildren<SpriteRenderer>();
				otherSpriteRenderer.color = purpleColor;

				//StartCoroutine(ReturnToNormalColor(otherSpriteRenderer));
			}
			else {
				otherTile.gameObject.GetComponent<SpriteRenderer>().color = purpleColor;
			}
		}
		die();
	}


	// IEnumerator ReturnToNormalColor(SpriteRenderer spriteRenderer) {
	// 	inCoroutine = true;
		
	// 	Color whiteColor = new Color(1, 1, 1, 1);

	// 	yield return new WaitForSeconds(3f);
	// 	spriteRenderer.color = whiteColor;

	// 	inCoroutine = false;
	// }
}
