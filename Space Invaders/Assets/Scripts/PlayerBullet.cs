using UnityEngine;

public class PlayerBullet : Bullet 
{
	void Start()
	{
		_directionVector = Vector2.up;
	}

	protected override void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.CompareTag(Config.ENEMIES_TAG))
		{
			GameManager.instance.OnEnemyHit(other.gameObject.GetComponent<Enemy>().GetPoints());
			other.gameObject.GetComponent<Enemy>().Explode();
			
			Destroy(gameObject);
		}
	}
}