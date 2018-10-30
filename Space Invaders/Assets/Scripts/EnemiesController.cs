using UnityEngine;

public class EnemiesController : MonoBehaviour 
{
	private const float START_SHOOTING_INTERVAL = 1.5f;
	private const float MOTHERSHIP_INTERVAL = 4.0f;
	private const float MOTHERSHIP_SPEED_MULT = 1.5f;
    private const float MOTHERSHIP_PERCENTAGE = 60;

	private const int START_SPEED = 20;
	private const int START_NUMBER_LINES = 4;
	private const int START_ENEMIES_PER_LINE = 4;	
	private const int ENEMIES_TYPE_4_LINES_NUM = 1;
	private const int ENEMIES_TYPE_3_LINES_NUM = 1;
	private const int ENEMIES_TYPE_2_LINES_NUM = 2;	
	
	private static Vector2 ENEMIES_SPACE = new Vector2 (0.8f, 0.7f);

    private int _lines = START_NUMBER_LINES;
	private int _enemiesPerLine = START_ENEMIES_PER_LINE;
	private int _frameCounter = 0;
	private int _speed = 15;
	private float _shootingInterval = 2.0f;

    private Vector2 _startPos = new Vector2(0f, 3.0f);

	private float _motherShipWidth;

	private Vector2 _directionVector;
	private Vector2 _motherShipDirectionVector;
	private Vector2 _enemiesRelativeCenter;

	private Bounds _enemiesBounds;

    private GameObject _motherShipObj;

    [SerializeField] private GameObject _motherShipPrefab;
    [SerializeField] private GameObject[] _enemiesPrefabs;

	void Awake () 
	{
		Reset();
	}
	
	void Update () 
	{
        if (Config.isGamePaused)
        {
            return;
        }

        // If no more enemies, level is cleared
        if (transform.childCount == 0 && !_motherShipObj)
            GameManager.instance.EndLevel();

		_frameCounter++;	

		if (_frameCounter % Config.ENEMIES_UPDATE_FRAME_RATE == 0)
		{			
			transform.Translate (_directionVector * _speed * Time.deltaTime);

			if (_motherShipObj)
                _motherShipObj.transform.Translate (_motherShipDirectionVector * _speed * MOTHERSHIP_SPEED_MULT * Time.deltaTime);		

			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).GetComponent<Enemy>().UpdateSprite();
			}
		}
	}

	void FixedUpdate()
	{
        if (Config.isGamePaused)
        {
            return;
        }

        if (_frameCounter % Config.ENEMIES_UPDATE_FRAME_RATE == 0)
		{
            // Check if the enemies have reached the walls
            if (HaveEnemiesReachedHorizontalBounds())
            {
                MoveEnemiesToNextLine();
            }

			if (_motherShipObj) 
			{
                // If the mothership leaves the screen, is eliminated
				if (_motherShipObj.transform.position.x + _motherShipWidth > Config.ScreenLimit.x ||
                    _motherShipObj.transform.position.x - _motherShipWidth < -Config.ScreenLimit.x)
				{
					CancelInvoke ("MotherShipMovement");
                    Destroy (_motherShipObj);
				}
			}
		}		
	}

	public void Reset ()
	{
        // Chooses a random initial direction
		_directionVector = Random.Range (-1.0f, 1.0f) > 0 ? Vector2.right : Vector2.left;

		_frameCounter = 0;
		_speed = START_SPEED;
		_shootingInterval = START_SHOOTING_INTERVAL;

		Config.ResetBulletsSpeed();

		transform.position = Vector2.zero;
	}

	public void CreateEnemies ()
	{
        _startPos.x =  - ( (_enemiesPerLine - 1) * ENEMIES_SPACE.x)/2;

		for (int i = 0; i < _enemiesPerLine * _lines; i++)
		{
			GameObject obj;
            
			if (i < _enemiesPerLine * ENEMIES_TYPE_4_LINES_NUM)
                obj = _enemiesPrefabs[3];
			else if (i < _enemiesPerLine * (ENEMIES_TYPE_4_LINES_NUM + ENEMIES_TYPE_3_LINES_NUM))
                obj = _enemiesPrefabs[2];
			else if (i < _enemiesPerLine * (ENEMIES_TYPE_4_LINES_NUM + ENEMIES_TYPE_3_LINES_NUM + ENEMIES_TYPE_2_LINES_NUM))
                obj = _enemiesPrefabs[1];
			else
                obj = _enemiesPrefabs[0];

			Instantiate (obj, new Vector2 (_startPos.x + i % _enemiesPerLine * ENEMIES_SPACE.x, _startPos.y - i/_enemiesPerLine * ENEMIES_SPACE.y), Quaternion.identity, transform);
		}

		UpdateBounds ();
	}

	public void StartShooting ()
	{
		CancelInvoke ("Shooting");
        InvokeRepeating ("Shooting", _shootingInterval, _shootingInterval);
	}

	private void Shooting ()
	{
		if (transform.childCount == 0) return;
		
		transform.GetChild (Random.Range (0, transform.childCount - 1)).GetComponent<Enemy> ().canShoot = true;
	}

	public void CallMotherShip ()
	{
		CancelInvoke ("MotherShipMovement");
		InvokeRepeating ("MotherShipMovement", MOTHERSHIP_INTERVAL, MOTHERSHIP_INTERVAL);
	}

	private void MotherShipMovement ()
	{
        // If the game is paused, or there is already a mother ship, returns
        if (_motherShipObj || Config.isGamePaused)
        {
            return;
        }

        int perc = Random.Range(0, 100);
		if (perc < MOTHERSHIP_PERCENTAGE)
		{
            _motherShipObj = Instantiate (_motherShipPrefab, Vector2.zero, Quaternion.identity);
            _motherShipWidth = _motherShipObj.GetComponent<SpriteRenderer> ().bounds.extents.x;

			float dirRand = Random.Range (-1.0f, 1.0f);
			_motherShipDirectionVector = dirRand > 0 ? Vector2.right : Vector2.left;

            _motherShipObj.transform.position = new Vector2 (dirRand > 0 ? -Config.ScreenLimit.x + _motherShipWidth : Config.ScreenLimit.x - _motherShipWidth, 
									 			  		 transform.GetChild (0).transform.position.y + ENEMIES_SPACE.y);
		}
	}

	public void PauseEnemies (bool isPaused)
	{
		if (isPaused)
			CancelInvoke ("Shooting");
		else
			InvokeRepeating ("Shooting", _shootingInterval, _shootingInterval);
	}

	public void DestroyAllEnemies ()
	{
		Destroy (_motherShipObj);

        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject[] Bullets = GameObject.FindGameObjectsWithTag (Config.BULLETS_TAG);
		foreach (GameObject bullet in Bullets)
		{
			Destroy (bullet);
		}
	}

	public void UpdateEnemiesNumber(int level)
	{
		_lines = START_NUMBER_LINES + level;
		_enemiesPerLine = START_ENEMIES_PER_LINE + level;
	}

	public void UpdateBounds ()
	{
		_enemiesBounds = new Bounds(transform.position, Vector3.zero);
		
		foreach (Renderer r in GetComponentsInChildren<Renderer>()) 
		{
			_enemiesBounds.Encapsulate(r.bounds);
		}

		_enemiesRelativeCenter = _enemiesBounds.center - transform.position;

    }

	private bool HaveEnemiesReachedHorizontalBounds ()
	{
		if ((_directionVector == Vector2.right && transform.position.x + _enemiesRelativeCenter.x + _enemiesBounds.extents.x + ENEMIES_SPACE.x > Config.ScreenLimit.x) ||
			(_directionVector == Vector2.left && transform.position.x + _enemiesRelativeCenter.x - _enemiesBounds.extents.x - ENEMIES_SPACE.x < - Config.ScreenLimit.x))
			return true;

        return false;
	}

	private bool HaveEnemiesReachedVerticalBounds ()
	{
		if (transform.childCount > 0 && transform.position.y + _enemiesRelativeCenter.y - _enemiesBounds.extents.y - ENEMIES_SPACE.y < Config.PLAYERS_START_POS.y)
			return true;

        return false;
	}

	private void MoveEnemiesToNextLine ()
	{
		transform.position = new Vector2 (transform.position.x, transform.position.y - ENEMIES_SPACE.y);

        // If the enemy lines reached the player, is game over
        if (HaveEnemiesReachedVerticalBounds())
        {
            GameManager.instance.EndGame();
        }
        else
        {
            _directionVector.x *= -1;
            _speed++;
            _shootingInterval -= 0.1f;
            Config.bulletsSpeed += 0.1f;
        }
	}
}