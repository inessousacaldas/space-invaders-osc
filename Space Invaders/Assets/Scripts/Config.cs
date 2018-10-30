using UnityEngine;

public static class Config : object 
{
    public const int BULLETS_UPDATE_FRAME_RATE = 15;
    public const int ENEMIES_UPDATE_FRAME_RATE = 25;
	
	public const int MOTHERSHIP_MIN_POINTS = 50;
	public const int MOTHERSHIP_MAX_POINTS = 150;

	private const int START_BULLET_SPEED = 3;

	public const float EXPLOSION_TIME_OUT = .5f;
	
    public const string PLAYER_TAG = "Player";
    public const string SHIELD_TAG = "Shield";
    public const string ENEMIES_TAG = "Enemy";
	public const string BULLETS_TAG = "Bullet";
		
	public static bool isGamePaused = false;
	public static float bulletsSpeed = START_BULLET_SPEED;
	
	public static Vector2 ScreenLimit 
	{
		get 
		{
			return new Vector2 (Camera.main.orthographicSize * Screen.width / Screen.height, 
								Camera.main.orthographicSize);
		}
	}

	public static Vector2 PLAYERS_START_POS
    {
        get
        {
            return new Vector2(0f, -(ScreenLimit.y - 1));
        }
    }

	public static void ResetBulletsSpeed()
	{
		bulletsSpeed = START_BULLET_SPEED;
	}
}