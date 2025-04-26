using System.Collections.Generic;
using UnityEngine;

public class IndexDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _minusSign;
    [SerializeField] private GameObject _plusSign;
    [SerializeField] private MeshFilter _firstDigit;
    [SerializeField] private MeshFilter _secondDigit;
    [SerializeField] private List<Mesh> _numberMeshes;

    public void RefreshIndex(int newIndex)
    {
        _minusSign.SetActive(newIndex < 0);
        _plusSign.SetActive(newIndex > 0);

        int dozens = Mathf.Abs(newIndex / 10);
        int units = Mathf.Abs(newIndex % 10);

        if (dozens > 0)
        {
            _firstDigit.mesh = _numberMeshes[dozens];
            _secondDigit.gameObject.SetActive(true);
            _secondDigit.mesh = _numberMeshes[units];
        }
        else
        {
            _firstDigit.mesh = _numberMeshes[units];
            _secondDigit.gameObject.SetActive(false);
        }
    }
}
