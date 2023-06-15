using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using NaughtyAttributes;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public static event Action<Hashtable> GameEvent;

    
    // 
    [SerializeField] public WeaponsPack WeaponsPackData;
    [SerializeField] public PlayerStats PlayerData;
    [SerializeField] public Player PlayerScript;

    [SerializeField] public string PlayerTag = "Player";
    [SerializeField] public string EnemyTag = "Enemy";
    [SerializeField] public string StaticTag = "Static";

    [SerializeField] public List<EnemiesStats> Enemies = new List<EnemiesStats>();
    [SerializeField] public List<KeysStats> Keys = new List<KeysStats>();

    [SerializeField] public GameObject PauseMenu;
    [SerializeField] public int SCENE_MAIN = 0;
    [SerializeField] public int SCENE_LEVEL_1 = 1;
    [SerializeField] public int SCENE_LEVEL_2 = 2;

    [SerializeField] public float slowMotionTimeScale = 0.05f;
    [SerializeField] public bool CanShakeCameraAfterHit = false;
    [SerializeField] public float SlowMotionDuration = 0.2f;
    [SerializeField] public float ShakeDuration = 1f;
    [SerializeField] public float ShakeIntensity = 1f;

    private float startTimeScale;
    private float startFixedDeltaTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        startTimeScale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;

        PauseMenu = GameObject.Find("Menus").transform.GetChild(0).gameObject;
    }


    public void ChangePlayerStamina(float staminaRate)
    {
        Debug.Log("Reducing stamina via GameManager");
        Hashtable payload = new Hashtable();
        payload.Add("state", GameState.AffectStamina);
        payload.Add("stamina", staminaRate);
        GameEvent?.Invoke(payload);
    }

    public void AttackPlayer(float damage)
    {
        Hashtable payload = new Hashtable();
        payload.Add("state", GameState.AttackPlayer);
        payload.Add("damage", damage);
        Debug.Log("Applying damage to player");
        GameEvent?.Invoke(payload);
    } 

    

    public void ShakeCamera()
    {
        Hashtable payload = new Hashtable();
        payload.Add("state", GameState.ShakeCamera);
        GameEvent?.Invoke(payload);
    } 


    
    public void StopEnemies(bool isPlayerDead)
    {
        Hashtable payload = new Hashtable();
        payload.Add("state", GameState.StopEnemies);
        payload.Add("dead", isPlayerDead);
        GameEvent?.Invoke(payload);
    }

    public void CreateSlowMotionEffect(float duration, bool shouldShake = false)
    {
        StopCoroutine(SlowMotion(duration, shouldShake));
        StartCoroutine(SlowMotion(duration, shouldShake));
    }



    public Vector2 getTargetPosition(Transform origin, Transform target)
    {
        Vector2 pos = new Vector2(0f, 0f);

        pos.x = origin.position.x - target.position.x;
        pos.y = origin.position.y - target.position.y;

        return pos;
    }


    private IEnumerator SlowMotion(float duration, bool shouldShake)
    {
        StartSlowMotion();
        yield return new WaitForSeconds(duration);
        StopSlowMotion();

        if(shouldShake)
        {
            ShakeCamera();
        }
    }


    private void StartSlowMotion()
    {
        Time.timeScale = slowMotionTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimeScale;
    }

    private void StopSlowMotion()
    {
        Time.timeScale = startTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime;
    }



    public void StartGame()
    {
        SceneManager.LoadScene(SCENE_LEVEL_1);
    }
    public void StartLevel(int level)
    {
        if(level == 1)
        {
            SceneManager.LoadScene(SCENE_LEVEL_1);
        } else if (level == 2)
        {
            SceneManager.LoadScene(SCENE_LEVEL_2);
        }
    }
    public void OpenMainMenu()
    {
        SceneManager.LoadScene(SCENE_MAIN);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        PauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        PauseMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public string[] GetEnemiesDropdown()
    {
        string[] p = new string[Enemies.Count];

        for(int i = 0; i < Enemies.Count; i++)
        {
            p[i] = Enemies[i].Name;
        }

        return p;
    }




    [System.Serializable]
    public class Weapon
    {
        private List<string> List { get { return new List<string>() { "SwordChunky", "SwordGolden", "SwordLava", "SwordMeaty", "SwordOnFire"}; } }

        [Dropdown("List")]
        public string ID;
    }

    [System.Serializable]
    public class Enemy
    {
        private List<string> List { get { return new List<string>() { "AngryBigBoi", "BigBoi", "GreenBi", "MeanSlime", "RedBoi" }; } }

        [Dropdown("List")]
        public string Name;
    }

    [System.Serializable]
    public class Key
    {
        public string ID;

        private List<string> List { get { return new List<string>() { "White", "Green", "Blue", "Yellow", "Purple", "Red" }; } }
        
        [Dropdown("List")]
        public string Color;
    }
}




public enum GameState
{
    AffectStamina,
    AffectHealth,
    AttackPlayer,
    StopEnemies,
    ShakeCamera
}