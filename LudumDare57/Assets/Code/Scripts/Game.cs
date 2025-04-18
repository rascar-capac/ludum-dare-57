using UnityEngine;

public class Game : MonoBehaviour
{
    private static Game _instance;

    [SerializeField] private Transform _player;
    [SerializeField] private Transform _camera;
    [SerializeField] private KeyMatchManager _keyMatchManager;
    [SerializeField] private KeyMatchFeedback _keyMatchFeedback;

    public static Transform Player => _instance._player;
    public static Transform Camera => _instance._camera;
    public static KeyMatchManager KeyMatchManager => _instance._keyMatchManager;
    public static KeyMatchFeedback KeyMatchFeedback => _instance._keyMatchFeedback;

    private void Awake()
    {
        _instance = this;
    }
}
