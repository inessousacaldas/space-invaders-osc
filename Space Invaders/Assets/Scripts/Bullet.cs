using UnityEngine;

public class Bullet : MonoBehaviour 
{
	private int _frameCounter = 0;
	private SpriteRenderer _spriteRenderer;
	protected Vector2 _directionVector;

	void Awake ()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update () 
	{
		if (Config.isGamePaused) return;

		transform.Translate (_directionVector * Config.bulletsSpeed * Time.deltaTime);

        Animate();

    }
	
	void LateUpdate()
	{
		if (Config.isGamePaused) return;
		
		if (transform.position.y >  Config.ScreenLimit.y ||
			transform.position.y < -Config.ScreenLimit.y)
        {
            Destroy(gameObject);
        }
				
	}

    private void Animate()
    {
        if (_frameCounter % Config.BULLETS_UPDATE_FRAME_RATE == 0)
            _spriteRenderer.flipY = !_spriteRenderer.flipY;

        _frameCounter++;
    }

	protected virtual void OnTriggerEnter2D(Collider2D other){}
}