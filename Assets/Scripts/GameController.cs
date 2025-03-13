using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Plane = DefaultNamespace.Plane;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    
    [SerializeField] private List<Material> colorsToRandom;
    [SerializeField] private List<Color> colors;
    [SerializeField] private Plane _prefabPlane;
    [SerializeField] private List<Plane> _planes = new List<Plane>();
    [SerializeField] private List<Plane> _teleportDoor = new List<Plane>();
    [SerializeField] private float countDown = 5f;
    [SerializeField] private float disappearTime = 2f;
    [SerializeField] private float coolDownTeleport = 5f;
    [SerializeField] private Text _gameOverUI;
    [SerializeField] private Text _countdown;
    [SerializeField] private Image _colorNext;
    [SerializeField] private PhysicsCharacterController _character;
    [SerializeField] private Transform _startOverPoint;
    private float currentTime = 0;
    private float countDownTeleport = 0f;
    private bool isAppearing = false;
    private bool _gameOver = false;
    private bool _canTeleport = true;
    private int _indexToDisapper;

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
                plane.SetData(Convert.ToSingle(values[0]),Convert.ToSingle(values[1]),Convert.ToInt32(values[2]));
                plane.SetColor(colorsToRandom[index],index);
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
        _indexToDisapper = Random.Range(0, colorsToRandom.Count);
        _colorNext.color = colors[_indexToDisapper];
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
        _countdown.text = Mathf.CeilToInt(currentTime).ToString();

        if (!_canTeleport)
        {
            countDownTeleport -= Time.deltaTime;

            if (countDownTeleport <= 0)
            {
                _canTeleport = true;
                countDownTeleport = coolDownTeleport;
            }
        }
        
        if (currentTime <= 0)
        {
            if(!isAppearing) Disappear();
            else Appear();
        }
    }

    public void Appear()
    {
        isAppearing = false;
        _indexToDisapper = Random.Range(0, colorsToRandom.Count);
        _colorNext.color = colors[_indexToDisapper];
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
        List<Plane> remaining = _planes.Where(plane => plane.GetIndexColor() != _indexToDisapper).ToList();
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
    
    public void Teleport()
    {
        if (!_canTeleport) return;
        int index = Random.Range(0, _teleportDoor.Count);
        Plane destination = _teleportDoor[index];
        Debug.Log("Teleport to " + index);
        _canTeleport = false;
        _character.transform.position = destination.transform.position + Vector3.up * 0.3f;
    }

    public void StartOver()
    {
        _canTeleport = true;
        countDownTeleport = coolDownTeleport;
        _character.transform.position = _startOverPoint.position;
    }
}
