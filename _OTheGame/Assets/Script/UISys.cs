using UnityEngine;
using TMPro;
using System;
using UnityEditor.Animations;
public class UISys : MonoBehaviour
{
    public static UISys Instance{get; private set;}

    //------Event Handler

    public event EventHandler OnUINextRound;

    //----SerializeField
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI familyText;
    [SerializeField] private GameObject GameOverVFXUIContainer;
    [SerializeField] private Animator animator;

    //-----Variable
    private bool UIisActivate;
    private bool isGameOver;
    private float freezeTimer;
    private float AnimationNTime;
  
    private void Awake(){
        if(Instance!=null){ //just in case
            Debug.LogError("THERE ARE MORE THAN ONE UISys CLASS! "+transform+"-"+Instance+" ");
            Destroy(gameObject);
            return;
        }
         Instance = this;

         freezeTimer = 30f;
         UIisActivate = false;
         isGameOver = false;
         
        
    }

    
    private void Start()
    {
        TurnSys.Instance.OnRoundFinish += TurnSys_OnRoundChanged;
        XOSys.Instance.OnFamilyChanged += XOSys_OnFamilyChanged;
       
    }

     private void Update()
    {
         if(UIisActivate){
            //Run In between round animatoin
            freezeTimer-=.1f;
            if(freezeTimer<=0){
                UIisActivate = false;
                SetNextRound();
            }
        }

        if(isGameOver){
            //Run Game Over animation and Exit the game
            AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
             AnimationNTime = animStateInfo.normalizedTime;
             if(AnimationNTime>1.0f) {
                Application.Quit();
             }
        }
    }

    private void SetNextRound(){
        OnUINextRound?.Invoke(this,EventArgs.Empty);
        freezeTimer = 30f;
        
        if(!TurnSys.Instance.IsPlayerTurn()){
            //Fail the game
            GameOverVFXUIContainer.gameObject.SetActive(true);
            isGameOver = true;
            return;
        }

        UICanvas.gameObject.SetActive(false);

       
    }

    //-----------Subscribe

    
    private void TurnSys_OnRoundChanged(object sender, EventArgs e){
        int roundNumber = TurnSys.Instance.GetRound();

        if(!TurnSys.Instance.IsPlayerTurn()){
            //AI WIN
           roundText.text = "You broke policy";
        }

        if(TurnSys.Instance.IsPlayerTurn() && roundNumber<TurnSys.Instance.GetMaxRound()){
            //On to Next Round
            roundText.text = roundNumber.ToString()+"\nwin";
        }

        if(TurnSys.Instance.IsPlayerTurn() && roundNumber>=TurnSys.Instance.GetMaxRound()){
            //On to Next Round
            roundText.text = "COLLECT YOUR\nREWARD";
        }

        //Animation
        UICanvas.gameObject.SetActive(true);
        UIisActivate = true;

    }

    private void XOSys_OnFamilyChanged(object sender, EventArgs e){
        familyText.text = "Family remaining: "+XOSys.Instance.GetFamilyRemaining();
    }
}
