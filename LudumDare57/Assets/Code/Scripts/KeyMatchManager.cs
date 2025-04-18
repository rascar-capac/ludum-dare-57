using UnityEngine;

public class KeyMatchManager : MonoBehaviour
{
    [SerializeField] private CrystalsHolder _crystalsHolder;

    public void ValidateMatch(int crystalIndex, KeyMatcher matcher)
    {
        _crystalsHolder.ActivateCrystal(crystalIndex);
        Game.KeyMatchFeedback.PlayMatchFeedback(matcher);
    }
}
