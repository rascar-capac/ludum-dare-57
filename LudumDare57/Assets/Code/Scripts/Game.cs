using UnityEngine;

public class Game : MonoBehaviour
{
    private static Game _instance;

    [SerializeField] private Transform _player;
    [SerializeField] private Transform _camera;
    [SerializeField] private KeyMatchManager _keyMatchManager;
    [SerializeField] private KeyMatchFeedback _keyMatchFeedback;
    [SerializeField] private KeyToggler _keyToggler;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private Elevator _elevator;

    public static Transform Player => _instance._player;
    public static Transform Camera => _instance._camera;
    public static KeyMatchManager KeyMatchManager => _instance._keyMatchManager;
    public static KeyMatchFeedback KeyMatchFeedback => _instance._keyMatchFeedback;
    public static KeyToggler KeyToggler => _instance._keyToggler;
    public static LevelManager LevelManager => _instance._levelManager;
    public static Elevator Elevator => _instance._elevator;

    private void Awake()
    {
        _instance = this;
    }
}
