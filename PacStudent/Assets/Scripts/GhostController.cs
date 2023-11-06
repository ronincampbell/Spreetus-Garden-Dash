using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public bool isScared;
    public bool isRecovering;
    public bool isNormal;
    public bool isDead;
    private GameObject pacStudent;
    private GameObject gameManager;
    private ScoreKeeper scoreKeeper;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        pacStudent = GameObject.FindGameObjectWithTag("Player");
        scoreKeeper = gameManager.GetComponent<ScoreKeeper>();
        NormalState();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Animator>().GetBool("ScaredAnim") != isScared)
        {
            GetComponent<Animator>().SetBool("ScaredAnim", isScared);
        }
        if (GetComponent<Animator>().GetBool("RecoveryAnim") != isRecovering)
        {
            GetComponent<Animator>().SetBool("RecoveryAnim", isRecovering);
        }
        if (GetComponent<Animator>().GetBool("NormalAnim") != isNormal)
        {
            GetComponent<Animator>().SetBool("NormalAnim", isNormal);
        }
        if (GetComponent<Animator>().GetBool("DeadAnim") != isDead)
        {
            GetComponent<Animator>().SetBool("DeadAnim", isDead);
        }
    }

    public void ScaredState()
    {
        isScared = true;
        isRecovering = false;
        isNormal = false;
        isDead = false;

        Debug.Log(this.gameObject.name + " is scared!");
    }

    public void RecoveryState()
    {
        isRecovering = true;
        isScared = false;
        isNormal = false;
        isDead = false;

        Debug.Log(this.gameObject.name + " is recovering!");
    }
    public void NormalState()
    {
        if (!isDead){
            isNormal = true;
            isScared = false;
            isRecovering = false;
            isDead = false;

            Debug.Log(this.gameObject.name + " is normal!");
        }
    }

    public void DeadState()
    {
        isDead = true;
        isNormal = false;
        isScared = false;
        isRecovering = false;

        Debug.Log(this.gameObject.name + " is dead!");
        scoreKeeper.AddScore(300);

        StartCoroutine(DeadTimer());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isNormal) { NormalCollison(); }
        if (isScared) { ScaredCollision(); }
        if (isRecovering) { RecoveringCollision(); }
    }

    private void NormalCollison(){
        pacStudent.GetComponent<PacStudentController>().KillPlayer();
        Debug.Log("Normal collision..");
    }

    private void ScaredCollision(){
        DeadState();
        Debug.Log("Scared collision..");
    }

    private void RecoveringCollision(){
        DeadState();
        Debug.Log("Recovering collision..");
    }

    private IEnumerator DeadTimer()
    {
        yield return new WaitForSeconds(5.0f);
        isDead = false;
        NormalState();
    }
}
