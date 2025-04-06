using UnityEngine;

public class KeyMatchFeedback : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Animator _animator;

    private MaterialPropertyBlock _block;
    private KeyMatcher _currentMatcher;
    private float _currentScore;

    public void SetNearMatchScore(float score, KeyMatcher matcher)
    {
        if (matcher == _currentMatcher)
        {
            _currentScore = score;
        }
        else
        {
            if (score > _currentScore)
            {
                _currentMatcher = matcher;
                _currentScore = score;
            }
        }

        _block ??= new();
        _renderer.GetPropertyBlock(_block);
        _block.SetFloat("_NearMatchScore", _currentScore);
        _renderer.SetPropertyBlock(_block);
    }

    public void PlayMatchFeedback()
    {
        _animator.SetTrigger("match");
    }
}
