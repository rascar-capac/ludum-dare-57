using UnityEngine;

public class KeyMatchFeedback : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    MaterialPropertyBlock _block;

    public void SetMatchScore(float score)
    {
        _block ??= new();
        _renderer.GetPropertyBlock(_block);
        _block.SetFloat("_Match", score);
        _renderer.SetPropertyBlock(_block);
    }
}
