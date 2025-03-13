using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Plane = DefaultNamespace.Plane;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    
    [SerializeField] private List<Material> colorsToRandom;
    [SerializeField] private Plane _prefabPlane;
    [SerializeField] private List<Plane> _planes = new List<Plane>();
    [SerializeField] private List<Plane> _teleportDoor = new List<Plane>();
    [SerializeField] private float countDown = 5f;
    [SerializeField] private float disappearTime = 2f;
    [SerializeField] private Canvas _gameOverUI;
    [SerializeField] private PhysicsCharacterController _character;
    [SerializeField] private Transform _startOverPoint;
    private float currentTime = 0;
    private bool isAppearing = false;
    private bool _gameOver = false;

    private void Awake()
    {
        if(Instance != null) Destroy(Instance);
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        #region CSV Reader
        
        string filePath = Path.Combine(Application.streamingAssetsPath, "data.csv");
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] values = line.Split(','); // Split CSV columns
                Plane plane = Instantiate(_prefabPlane);
                int index = Random.Range(0, colorsToRandom.Count);
                plane.SetColor(colorsToRandom[index],index);
                plane.SetData(Convert.ToSingle(values[0]),Convert.ToSingle(values[1]),Convert.ToInt32(values[2]));
                _planes.Add(plane);
                Debug.Log(string.Join(" | ", values)); // Display in Console
            }
        }
        else
        {
            Debug.LogError("CSV file not found at: " + filePath);
        }
        #endregion
        currentTime = countDown;
        StartOver();
    }

    // Update is called once per frame
    void Update()
    {
        if(_gameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in Unity Editor
#endif
        }
        currentTime -= Time.deltaTime; // Decrease time
        //countdownText.text = "Time Left: " + Mathf.CeilToInt(currentTime);
        if (currentTime <= 0)
        {
            if(!isAppearing) Disappear();
            else Appear();
        }
    }

    public void Appear()
    {
        isAppearing = false;
        currentTime = countDown;
        foreach (var plane in _planes)
        {
            plane.gameObject.SetActive(true);
        }
    }
    
    public void Disappear()
    {
        isAppearing = true;
        currentTime = disappearTime;
        int _indexRemain = Random.Range(0, colorsToRandom.Count);
        List<Plane> remaining = _planes.Where(plane => plane.GetIndexColor() != _indexRemain).ToList();
        foreach (var plane in remaining)
        {
            plane.gameObject.SetActive(false);
        }
    }

    public void GameOver()
    {
        _gameOverUI.gameObject.SetActive(true);
        _character.TurnOffControl();
        _gameOver = true;
    }

    public void AddToTeleportList(Plane platform)
    {
        _teleportDoor.Add(platform);
    }
    
    public void Teleport(Collider collider)
    {
        Plane destination = _teleportDoor[Random.Range(0, _teleportDoor.Count)];
        collider.transform.position = destination.transform.position;
    }

    public void StartOver()
    {
        _character.transform.position = _startOverPoint.position;
    }
}
