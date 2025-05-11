using UnityEngine;
using System;
using System.Collections.Generic;

public class Basic_Enemy_AI : MonoBehaviour
{

    public static Basic_Enemy_AI Instance{get; private set;}
    private bool isPlaying;

    //----------Awake / Start / Update-----------------------

    private void Awake()
    {
        if(Instance!=null){ //just in case
            Debug.LogError("THERE ARE MORE THAN ONE MouseWorld CLASS! "+transform+"-"+Instance+" ");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        isPlaying=false;
    }
    private void Start()
    {
        //Subscribe to event handler
        TurnSys.Instance.OnTurnChanged += TurnSys_OnTurnChanged;
    }

    private void Update()
    {
        if(isPlaying){
            if(TryPlaying()){
                isPlaying=false; //END AI Turn
            }
        }
    }

    //-------------------Calculate Move----------------------------
    private bool TryPlaying(){
       List<XO_Item> AllXOList = XOSys.Instance.GetXO_All_List();
       List<XO_Item> ActiveXOList = XOSys.Instance.GetXO_Activate_List();
       List<XO_Item> mockUpActiveXOLIst = new List<XO_Item>();

       foreach(XO_Item xo in ActiveXOList){
        mockUpActiveXOLIst.Add(xo);
       }

       AI_Margin margin = new AI_Margin(-1,ActiveXOList[0]);
        
        //Random the 1st AI tile
        if(ActiveXOList.Count==1){

            XO_Item xO_Item = ActiveXOList[0];

            while(xO_Item == ActiveXOList[0]){
                //Prevent choosing the active tile
                int randomIndex = UnityEngine.Random.Range(0,9);
                xO_Item = AllXOList[randomIndex];
            }

            XOSys.Instance.TryXO(xO_Item);
            return true;
        }

        //Calculate the possible tile outcome
        foreach(XO_Item xO_Item in AllXOList){
            if(ActiveXOList.Contains(xO_Item)) continue; //not calculate margin for taken spot

            xO_Item.SetIsPlayer(false);
            mockUpActiveXOLIst.Add(xO_Item); //mock up active XO tile
            margin=CalculateMargin(margin,xO_Item,mockUpActiveXOLIst); //calculate that xo
            mockUpActiveXOLIst.RemoveAt(mockUpActiveXOLIst.Count-1); //remove mock up
        }

        XOSys.Instance.TryXO(margin.xO_Item);
        return true;
    }

    private AI_Margin CalculateMargin(AI_Margin currentMargin,XO_Item testXO, List<XO_Item> ActiveXOList){
        
        AI_Margin result_Margin = currentMargin;
        int testXO_X = (int)testXO.GetPosition().x;
        int testXO_Y = (int)testXO.GetPosition().y;
        double targetMarginPoint = 0;
        int AmountOfConnectedNeed=3;

        /*if(testXO.GetPosition().x== Mathf.Ceil(XOSys.Instance.GetMaxXSize()/2) &&
        testXO.GetPosition().y== Mathf.Ceil(XOSys.Instance.GetMaxYSize()/2)){
            //Priority middle
            targetMarginPoint+=1;
        }*/

        //Calculate the best possible spot for row, column, diagonals
        targetMarginPoint+=CheckRow(testXO_X,testXO_Y,XOSys.Instance.GetMaxYSize(),testXO,ActiveXOList);
        targetMarginPoint+=CheckCol(testXO_Y,testXO_X,XOSys.Instance.GetMaxXSize(),testXO,ActiveXOList);
        targetMarginPoint+=CheckDiagonalLeft(testXO_X,testXO_Y,AmountOfConnectedNeed,testXO,ActiveXOList);
        targetMarginPoint+=CheckDiagonalRight(testXO_X,testXO_Y,AmountOfConnectedNeed,testXO,ActiveXOList);

        Debug.Log(testXO+" : Margin= "+targetMarginPoint);

        //Assign best Margin/possible outcome
        if(currentMargin.point<targetMarginPoint){
            //if target margin greater means greater effective move
            //Replace best outcome with a new one
            result_Margin = new AI_Margin(targetMarginPoint,testXO);
        }
        return result_Margin;
    }


    //--------------------Look at Row / Col / Diagonal-------------------------
    private double CheckRow(int xo_position_axis_main,int xo_position_axis_check, int maxAxisSize, XO_Item testXo,List<XO_Item> ActiveXOList){
        double point=0;
        int playerHit=0;
        int AIHit=0;
         for(int y=xo_position_axis_check; y<maxAxisSize; y++){
            //Check Right side
            Vector2 checkPosition = new Vector2(xo_position_axis_main,y);
            var result = CalculatePoint_AI(checkPosition,testXo,ActiveXOList);
            point += result.point; 
            playerHit+=result.playerHit;
            AIHit+=result.AIHit;
           
        }

        for(int y=xo_position_axis_check; y>=0; y--){
            //Check Left side
            Vector2 checkPosition = new Vector2(xo_position_axis_main,y);
            var result = CalculatePoint_AI(checkPosition,testXo,ActiveXOList);
           point += result.point;
           playerHit+=result.playerHit;
           AIHit+=result.AIHit;
            
        }

        point += Calculate_Extra_AIPlayerHit(AIHit,playerHit);
        
        return point;
    }

    private double CheckCol(int xo_position_axis_main,int xo_position_axis_check, int maxAxisSize, XO_Item testXo,List<XO_Item> ActiveXOList){
        double point=0;
        int playerHit=0;
        int AIHit=0;
        for(int z=xo_position_axis_check; z<maxAxisSize; z++){
            //Check Top side
            Vector2 checkPosition = new Vector2(z,xo_position_axis_main);
            var result = CalculatePoint_AI(checkPosition,testXo,ActiveXOList);
            point += result.point; 
            playerHit+=result.playerHit;
            AIHit+=result.AIHit;
        }

        for(int z=xo_position_axis_check; z>=0; z--){
            //Check Bottom side
            Vector2 checkPosition = new Vector2(z,xo_position_axis_main);
            var result = CalculatePoint_AI(checkPosition,testXo,ActiveXOList);
            point += result.point; 
            playerHit+=result.playerHit;
            AIHit+=result.AIHit;
        }

        point += Calculate_Extra_AIPlayerHit(AIHit,playerHit);

        return point;
    }

    private double CheckDiagonalLeft(int xo_position_x,int xo_position_y,int AmountOfConnectedNeed,XO_Item testXo,List<XO_Item> ActiveXOList){
        double point =0;
        int playerHit=0;
        int AIHit=0;
       
        //------------------Left diagonal---------------------
        for(int xyi=0;xyi<AmountOfConnectedNeed;xyi++){
            //Check up side
            Vector2 checkPosition = new Vector2(xo_position_x+xyi,xo_position_y-xyi);
            var result = CalculatePoint_AI(checkPosition,testXo,ActiveXOList);
            point += result.point;
            playerHit+=result.playerHit;
            AIHit+=result.AIHit;
            
        }

        for(int xyi=1;xyi<AmountOfConnectedNeed;xyi++){
            //Check down side
            Vector2 checkPosition = new Vector2(xo_position_x-xyi,xo_position_y+xyi);
            var result = CalculatePoint_AI(checkPosition,testXo,ActiveXOList);
            point += result.point + Calculate_Extra_AIPlayerHit(result.AIHit,result.playerHit);
            point += result.point;
            playerHit+=result.playerHit;
            AIHit+=result.AIHit;
            
        }

        point += Calculate_Extra_AIPlayerHit(AIHit,playerHit);
        return point;
    }

    private double CheckDiagonalRight(int xo_position_x,int xo_position_y,int AmountOfConnectedNeed,XO_Item testXo,List<XO_Item> ActiveXOList){
        //------------------Right diagonal---------------------
        double point=0;
        int playerHit=0;
        int AIHit=0;

        for(int xyi=0;xyi<AmountOfConnectedNeed;xyi++){
            //Check up side
             Vector2 checkPosition = new Vector2(xo_position_x+xyi,xo_position_y+xyi);
            var result = CalculatePoint_AI(checkPosition,testXo,ActiveXOList);
            point += result.point + Calculate_Extra_AIPlayerHit(result.AIHit,result.playerHit);
            point += result.point;
            playerHit+=result.playerHit;
            AIHit+=result.AIHit;
        }

        for(int xyi=1;xyi<AmountOfConnectedNeed;xyi++){
            //Check down side
            Vector2 checkPosition = new Vector2(xo_position_x-xyi,xo_position_y-xyi);
            var result = CalculatePoint_AI(checkPosition,testXo,ActiveXOList);
            point += result.point + Calculate_Extra_AIPlayerHit(result.AIHit,result.playerHit);
            point += result.point;
            playerHit+=result.playerHit;
            AIHit+=result.AIHit;
        }
        point += Calculate_Extra_AIPlayerHit(AIHit,playerHit);
        return point;
    }
        

    private (int playerHit,int AIHit,double point) CalculatePoint_AI(Vector2 checkPosition,XO_Item xo,List<XO_Item> XO_Activate_List){
        double point=0;
        int playerHit=0;
        int AIHit=0;
        foreach(XO_Item xO_Item in XO_Activate_List){
            if(xo==xO_Item) continue;

            if((xO_Item.GetPosition()==checkPosition) && (xO_Item.IsPlayerXO()!=xo.IsPlayerXO())){
                //Hit Player's XO
                //encourage block player 
                point-=0.01;
                playerHit++;
                
            }

            if((xO_Item.GetPosition()==checkPosition) && (xO_Item.IsPlayerXO()==xo.IsPlayerXO())){
                //Hit Self's XO
                AIHit++;
                point+=0.01f;
            }
           
        }
        
        return (playerHit,AIHit,point);
    }

    private double Calculate_Extra_AIPlayerHit(int AiHit, int playerHit){
        double point =0;
        if(playerHit>1) point +=0.1; //Encourage to block player
        if(AiHit>1) point+=0.2;
         //Encourage to press connecting XO
        return point;
    }

   

    //----------Subscribe------------------------------------------

    private void TurnSys_OnTurnChanged(object sender, EventArgs e){
        if(TurnSys.Instance.IsBetweenRound()) return; //no action taken in between round
        if(TurnSys.Instance.IsGameEnd()) return; //no action taken when game end
        isPlaying=true;
    }
}
