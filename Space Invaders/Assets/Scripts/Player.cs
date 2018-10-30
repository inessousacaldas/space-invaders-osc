using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	private const float EXPLOSION_TIME = 0.5f;
	private const float EXPLOSION_FRAME_RATE = 5;

    private bool _isExploding = false;
	private int _speed = 5;
	private int _frameCounter = 0;
	private SpriteRenderer _spriteRenderer;

	[SerializeField] private Sprite _sprite;
    [SerializeField] private Sprite _explosionSprite;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject _shield;

    void Awake ()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void Update () 
	{
		if (Config.isGamePaused) return;

		if (_isExploding)
		{
			if (_frameCounter % EXPLOSION_FRAME_RATE == 0)
            {
                _spriteRenderer.flipX = !_spriteRenderer.flipX;
            }	
			_frameCounter++;
			return;
		}

        if (ReceiveComands.Instance.Right() && transform.position.x + _spriteRenderer.bounds.size.x < Config.ScreenLimit.x)
        {
            transform.Translate(Vector2.right * _speed * Time.deltaTime);
        }
        else if (ReceiveComands.Instance.Left() && transform.position.x - _spriteRenderer.bounds.size.x > -Config.ScreenLimit.x)
        {
           transform.Translate(Vector2.left * _speed * Time.deltaTime);
        }

        if (ReceiveComands.Instance.Jump() && transform.position.y + _spriteRenderer.bounds.size.y < Config.ScreenLimit.y)
        {
            transform.Translate(Vector2.up * _speed * Time.deltaTime);
        } 
        else if(transform.position.y - _spriteRenderer.bounds.size.y > -Config.ScreenLimit.y)
        {
            transform.Translate(-Vector2.up * _speed * Time.deltaTime);
        }
        
        if (ReceiveComands.Instance.Fire())
        {
            Instantiate(_bullet.gameObject, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        }
        if (ReceiveComands.Instance.Shield())
        {
            _shield.SetActive(true);
        }
        else
        {
            _shield.SetActive(false);
        }
	}	
	
	public void Explode ()
    {
        _isExploding = true;
        _spriteRenderer.sprite = _explosionSprite;

        Invoke("ResetPlayer", EXPLOSION_TIME);
    }

    private void ResetPlayer()
    {
        _isExploding = false;
        _spriteRenderer.sprite = _sprite;
        transform.position = Config.PLAYERS_START_POS;
    }
}