using UnityEngine.SceneManagement;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    public static TransitionController instance;
    private Animator anim;
    private static string scene;

    private void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
    }

    public void TransitionToSceneName(string sceneName)
    {
        scene = sceneName;
        anim.Play("FadeOut");
    }

    public void TransitionEvent()
    {
        SceneManager.LoadScene(scene);
    }
}
