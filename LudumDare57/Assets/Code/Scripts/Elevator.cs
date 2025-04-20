using DG.Tweening;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private float _levelHeight;
    [SerializeField] private float _travelTime;
    [SerializeField] private Ease _easeType;

    private void Awake()
    {
        transform.DOLocalMoveY(-_levelHeight, _travelTime).SetEase(_easeType).SetLoops(-1, LoopType.Yoyo);
    }
}
