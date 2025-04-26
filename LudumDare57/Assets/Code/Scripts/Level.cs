using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private int _metaIndex;
    [SerializeField] private float _height;
    [SerializeField] private IndexDisplay _indexDisplay;

    public float Height => _height;

    public void RefreshInfos(int currentSection)
    {
        int newIndex = -currentSection * 10 - _metaIndex;
        _indexDisplay.RefreshIndex(newIndex);
    }
}
