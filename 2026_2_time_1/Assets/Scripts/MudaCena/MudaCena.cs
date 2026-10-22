using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MudaCena : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    [Header("Transição")]
    [SerializeField] private bool _usarFade = true;
    [SerializeField] private Animator _fadeAnimator;
    [SerializeField] private float _duracaoFade = 0.5f;

    private bool _carregando;

    private void OnMouseDown()
    {
        LoadScene();
    }
    public void LoadScene()
    {
        if (string.IsNullOrEmpty(_sceneName) || _carregando) return;

        if (_usarFade && _fadeAnimator != null)
            StartCoroutine(FadeECarrega());
        else
            SceneManager.LoadScene(_sceneName);
    }
    private IEnumerator FadeECarrega()
    {
        _carregando = true;                    
        _fadeAnimator.SetTrigger("FadeOut");  
        yield return new WaitForSeconds(_duracaoFade);
        SceneManager.LoadScene(_sceneName);
    }
}