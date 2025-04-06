using UnityEngine;

public class KeyMatchManager : MonoBehaviour
{
    [SerializeField] private CrystalsHolder _crystalsHolder;

    public void ValidateMatch(int crystalIndex)
    {
        _crystalsHolder.ActivateCrystal(crystalIndex);
    }
}
