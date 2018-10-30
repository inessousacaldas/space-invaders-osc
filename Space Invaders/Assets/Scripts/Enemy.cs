using UnityEngine;

public class Enemy : MonoBehaviour 
{
	private bool _isExploding = false;
	private int _currentSprite = 0; //Enemies have two sprites to animate when moving
	private SpriteRenderer _spriteRenderer;

    [SerializeField] private bool _isMotherShip = false;
    [SerializeField] private int _points;

    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private Sprite _explosionSprite;
    [SerializeField] private Bullet _bullet;

    public bool canShoot = false;
	
    void Awake ()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();

        if (_isMotherShip)
        {
            _points = Random.Range(Config.MOTHERSHIP_MIN_POINTS, Config.MOTHERSHIP_MAX_POINTS);
        }

        UpdateSprite();
	}
	
	void Update () 
	{		
		if (canShoot && !_isMotherShip && !_isExploding)
		{
			Instantiate(_bullet.gameObject, new Vector2 (transform.position.x, transform.position.y), Quaternion.identity);
			canShoot = false;
		}
	}

	public void UpdateSprite()
	{
		if (_isExploding) return;

		_spriteRenderer.sprite = _sprites[_currentSprite];

        _currentSprite++;
        _currentSprite = _currentSprite % _sprites.Length;

	}

	public int GetPoints ()
	{
		return _points;
	}

    public void Explode ()
    {
		_isExploding = true;
		_spriteRenderer.sprite = _explosionSprite;
		Destroy(gameObject, Config.EXPLOSION_TIME_OUT);
    }
}