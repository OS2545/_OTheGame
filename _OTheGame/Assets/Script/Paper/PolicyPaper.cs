using UnityEngine;
using System;
using System.Collections.Generic;
public class PolicyPaper : MonoBehaviour
{
    //----------EventHandler-------------------------------
    public event EventHandler OnPolicyPaperAnimationEnd;
    //---------SerializeField --------------------------------
    [SerializeField] private Material checkboxMaterial;
    [SerializeField]private List<PolicyPaper_Item> policyPaper_Item_List;

    //---------Variable --------------------------------
    private bool isAnimationStarted;
    private bool isAnimationPlayed;
    private float timer;

    //----------Awake / Start / Update-----------------------
    private void Awake()
    {
        timer=0f;
        isAnimationStarted = false;
        isAnimationPlayed = false;
        
    }

    private void Start()
    {
        policyPaper_Item_List[0].OnPolicyPaperAnimationStarted += policyPaper_Item_List_OnPolicyPaperAnimationStarted;
        policyPaper_Item_List[0].OnCheckBoxInteract += policyPaper_Item_List_OnCheckBoxInteract;
        policyPaper_Item_List[1].OnCheckBoxInteract += policyPaper_Item_List_OnCheckBoxInteract;
    }

    private void Update()
    {

        //-------Paper Slide Animation----------//
        if(isAnimationStarted){
            timer+=1;
            if(timer>=30f){
                isAnimationPlayed = true;
                isAnimationStarted = false;
            }
        }

        if(isAnimationPlayed){
            Vector3 dir = new Vector3(0,0,0.1f);
            float moveSpeed = 2f;
            transform.position += dir * moveSpeed * Time.deltaTime;

            //stop animation
            Vector3 stopPosition = new Vector3(0.001f,0.121f,0.422f);
            if(transform.position.z >= stopPosition.z){
                isAnimationPlayed = false;
                OnPolicyPaperAnimationEnd?.Invoke(this,EventArgs.Empty);
                Destroy(this);
            }
        }
    }

    //--------------------Subscript------------------------------------------------

    private void policyPaper_Item_List_OnPolicyPaperAnimationStarted(object sender, EventArgs e){
        //Listen to animation signal
        isAnimationStarted = true;
    }
     private void policyPaper_Item_List_OnCheckBoxInteract(object sender, EventArgs e){
        //Listen to animation signal
        PolicyPaper_Item policyPaper_Item = sender as PolicyPaper_Item;
        policyPaper_Item.GetComponent<Renderer>().material = checkboxMaterial;
    }
}
