using UnityEngine;

public class EnemyBullet : Bullet 
{
    void Start ()
	{
		_directionVector = Vector2.down;
	}

	protected override void OnTriggerEnter2D(Collider2D other)
	{
        Debug.Log("Hit by " + other.gameObject.tag);

        if (other.gameObject.CompareTag(Config.PLAYER_TAG))
		{
			GameManager.instance.OnPlayerHit();
			
			Destroy(gameObject);
		}
        else if (other.gameObject.CompareTag(Config.SHIELD_TAG))
        {
            Destroy(gameObject);
        }
	}
}
