using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    [Header("Player")]
    [SerializeField] GameObject player;
    [SerializeField] Transform playerSpawnPlace;
    [Header("Walls")]
    [SerializeField] GameObject wallPrefab;
    [SerializeField] float wallSpeed;
    [SerializeField] float wallSpawnDelay;
    [SerializeField] Transform wallSpawnPlace;
    [SerializeField] Transform wallSpawnParent;
    [Header("UI")]
    [SerializeField] TextMeshProUGUI textScore;
    [SerializeField] TextMeshProUGUI textCountdown;
    [SerializeField] GameObject panelStart;
    [SerializeField] GameObject panelGameover;
    [Header("Other")]
    [SerializeField] Transform destroyer;
    private GameObject wall;
    private List<GameObject> walls;
    void Awake()
    {
        gameManager = this;
    }
    void Start()
    {
        walls = new List<GameObject>();
        StartCoroutine(SpawnPlayer());
        Invoke("SpawnWalls", 3f); // 3, because the player's spawn coroutine takes 3 seconds.
    }
    
    void Update()
    {
        //Debug.Log(walls.Count);
        // Когда WORLD_EATER уничтожает стену, в списке стен остаётся null. Необходимо найти оптимальный способ чистки.
        // Цикл ниже работает, но слишком ресурсозатратен.
        for (int i = 0; i < walls.Count; i++)
        {
            if (walls[i] == null)
            {
                walls.RemoveAt(i);
                i--;
            }
        }
    }
    void FixedUpdate()
    {
        foreach (GameObject wall in walls)
        {
            if (wall != null)
            {
                wall.transform.position = Vector3.MoveTowards(wall.transform.position,
                    new Vector3(destroyer.position.x, wall.transform.position.y, destroyer.position.z),
                    wallSpeed * Time.fixedDeltaTime);
            }
        }
    }
    private void SpawnWalls()
    {
        wallSpawnPlace.position = new Vector3(wallSpawnPlace.position.x, Random.Range(-3.5f, 3.5f), 0);
        wall = Instantiate(wallPrefab, wallSpawnPlace.position, wallSpawnPlace.rotation, wallSpawnParent);
        walls.Add(wall);
        Invoke("SpawnWalls", wallSpawnDelay);
    }
    private IEnumerator SpawnPlayer()
    {
        player = Instantiate(player, playerSpawnPlace.position, playerSpawnPlace.rotation);
        player.GetComponent<Rigidbody2D>().gravityScale = 0;
        yield return new WaitForSecondsRealtime(1f);
        textCountdown.text = "2";
        yield return new WaitForSecondsRealtime(1f);
        textCountdown.text = "1";
        yield return new WaitForSecondsRealtime(1f);
        textCountdown.text = "Start!";
        yield return new WaitForSecondsRealtime(0.5f);
        panelStart.SetActive(false);
        textScore.text = "0";
        player.GetComponent<Rigidbody2D>().gravityScale = 1;
        yield break;
    }
    public void IncreaseScore()
    {
        textScore.text = (int.Parse(textScore.text) + 1).ToString();
    }
    public void Gameover()
    {
        Destroy(player);
        panelGameover.SetActive(true);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}
