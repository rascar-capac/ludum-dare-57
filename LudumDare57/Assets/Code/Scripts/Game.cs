using UnityEngine;

public class Game : MonoBehaviour
{
    private static Game _instance;

    [SerializeField] private Transform _player;
    [SerializeField] private Transform _camera;

    public static Transform Player => _instance._player;
    public static Transform Camera => _instance._camera;

    private void Awake()
    {
        _instance = this;
    }
}
