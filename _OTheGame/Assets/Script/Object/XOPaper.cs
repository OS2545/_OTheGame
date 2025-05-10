using UnityEngine;
using System;
public class XOPaper : MonoBehaviour
{
    //----------EventHandler-------------------------------
    public event EventHandler OnGameStarted;
     //---------SerializeField --------------------------------
    [SerializeField]private PolicyPaper policyPaper;
    //---------Variable --------------------------------
    private bool isAnimationPlayed;

     //----------Awake / Start / Update-----------------------
    private void Awake()
    {
        isAnimationPlayed = false;
        
    }

    private void Update()
    {
        if(isAnimationPlayed){
            Vector3 dir = new Vector3(0,0,-0.1f);
            float moveSpeed = 2.5f;
            transform.position += dir * moveSpeed * Time.deltaTime;

            //stop animation
            Vector3 stopPosition = new Vector3(0.001f,0.121f,-0.113f);
            if(transform.position.z <= stopPosition.z){
                isAnimationPlayed = false;
                OnGameStarted?.Invoke(this,EventArgs.Empty);
               
            }
        }
    }

    private void Start()
    {
        policyPaper.OnPolicyPaperAnimationEnd += policyPaper_OnPolicyPaperAnimationEnd;
    }

    private void policyPaper_OnPolicyPaperAnimationEnd(object sender, EventArgs e){
        isAnimationPlayed = true;
        //Debug.Log("XO Animation");
    }
}
