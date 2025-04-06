using UnityEngine;

public class KeyMatchFeedback : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private FullScreenPassRendererFeature _nearMatchScorePostProcess;

    private MaterialPropertyBlock _keyblock;
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

        _keyblock ??= new();
        _renderer.GetPropertyBlock(_keyblock);
        _keyblock.SetFloat("_NearMatchScore", _currentScore);
        _renderer.SetPropertyBlock(_keyblock);

        _nearMatchScorePostProcess.passMaterial.SetFloat("_NearMatchScore", _currentScore);
    }

    public void PlayMatchFeedback()
    {
        _animator.SetTrigger("match");
    }
}
