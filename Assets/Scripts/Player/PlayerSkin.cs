using UnityEngine;

public class PlayerSkin : MonoBehaviour
{
    [SerializeField] private AnimatorOverrideController[] skinOverride;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if (DataManager.instance != null)
            anim.runtimeAnimatorController = skinOverride[DataManager.instance.gameData.CurrentSkin];
    }
}
