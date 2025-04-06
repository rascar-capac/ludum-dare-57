using System.Collections.Generic;
using UnityEngine;

public class CrystalsHolder : MonoBehaviour
{
    [SerializeField] private List<GameObject> _crystalList;

    public void ActivateCrystal(int index)
    {
        _crystalList[index].SetActive(true);
    }

    private void Awake()
    {
        foreach (GameObject crystal in _crystalList)
        {
            crystal.SetActive(false);
        }
    }
}
