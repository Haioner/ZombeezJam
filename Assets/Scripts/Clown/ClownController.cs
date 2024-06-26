using UnityEngine;

public class ClownController : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip clownMusic;
    [SerializeField] private FloatNumber floatNumberPrefab;
    [SerializeField] private GameObject interactCanvas;
    [SerializeField] private GameObject weapon;
    private Animator anim;
    private EnemyManager enemyManager;
    private EnemyMovement enemyMovement;
    private EnemyAttack enemyAttack;
    public bool IsNearest { get; set; }
    private bool enemyIsOn;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        enemyManager = GetComponent<EnemyManager>();    
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
    }

    private void Update()
    {
        InteractCanvasState();
    }

    private void InteractCanvasState()
    {
        if (!enemyIsOn)
            interactCanvas.SetActive(IsNearest);
        else
            interactCanvas.SetActive(false);
    }

    public void EnableClownEnemy()
    {
        if (!enemyIsOn)
        {
            enemyIsOn = true;
            MusicManager.instance.ChangeMusic(clownMusic);

            anim.enabled = true;
            enemyManager.enabled = true;
            enemyMovement.enabled = true;
            enemyAttack.enabled = true;

            weapon.SetActive(true);
        }
    }

    public void ClownDead()
    {
        MusicManager.instance.BackToNormalMusic();
    }

    public void Interact()
    {
        if (!enemyIsOn)
        {
            FloatNumber floatNumber = Instantiate(floatNumberPrefab, transform.position, Quaternion.identity);
            floatNumber.InitFloatNumber("Aren't you going to buy anything?...", Color.white);
        }
    }
}
