using UnityEngine;
using System.Collections;

public class ReceiveComands : MonoBehaviour {
    
   	public OSC osc;

    private static ReceiveComands instance = null;

    public bool rightArrow = false;
    public bool leftArrow = false;
    public bool jump = false;
    public bool fire = false;
    public bool left = false;
    public bool right = false;
    public bool shield = false;

    //OSC 
    [SerializeField]
    private float _coolDown = 2f;
    private float _timer;
    private int _move = 0;

    public static ReceiveComands Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }


    // Use this for initialization
    void Start () {

        _timer = Time.time;
        osc.SetAddressHandler("/jump", MessageHandler);
        osc.SetAddressHandler("/action", MessageHandler);
        osc.SetAddressHandler("/stop", MessageHandler);
        osc.SetAddressHandler("/left", MessageHandler);
        osc.SetAddressHandler("/right", MessageHandler);
        osc.SetAddressHandler("/piano", MessageHandler);
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Space))
            fire = true;

        if (Input.GetKey(KeyCode.C))
        {
            jump = true;

        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            jump = false;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            left = true;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            right = true;
        }

        if (Input.GetKeyDown(KeyCode.X)) //Activates shield
        {
            shield = true;
        }

        if (Input.GetKeyUp(KeyCode.X)) //Deactivates shield
        {
            shield = false;
        }

        
        if(_timer < Time.time)
        {
            if(_move < 0)
            {
                left = true;
            } 
            else if(_move > 0)
            {
                right = true;
            }
        }
        

    }

    public bool Fire()
    {
        if (fire)
        {
            fire = false;
            return true;
        }

        return false;
    }

    public bool Shield()
    {
        return shield;
    }

    public bool Left()
    {
        if (left) {
            left = false;
            return true;
        }

        return false;
    }

    public bool Right()
    {
        if (right)
        {
            right = false;
            return true;
        }

        return false;
    }

    public bool Jump()
    {
        return jump;
    }

    void MessageHandler(OscMessage message)
    {
        string message_str = message.GetString(0);
        Debug.Log("Entrei: " + message.GetString(0));

        switch (message_str)
        {
            case "action":
                fire = true;
                break;

            case "jump":
                jump = true;
                break;

            case "stop":
                jump = false;
                break;

            case "left":
                left = true;
                _timer += Time.time + _coolDown;
                _move = -1;
                break;

            case "right":
                right = true;
                _timer += Time.time + _coolDown;
                _move = 1;
                break;

            case "piano":
                shield = true;
                break;

            default:
                Debug.Log("Unknown message: " + message);
                break;
        }

    }

}
