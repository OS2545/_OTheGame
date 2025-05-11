using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Animations;

public class XOSys : MonoBehaviour
{
    public static XOSys Instance{get; private set;} //Only one system in run

    //----------Event Handler-----------------------
    public event EventHandler OnFinishRound;
    public event EventHandler OnFamilyChanged;

   //---------SerializeField --------------------------------
    [SerializeField] private List<Material> checkboxMaterial_list; // 0 = Transparent 1 = Cross 2=Circle
    [SerializeField]private List<Special_Tile> special_Tiles_List;
    
    
    //--------Variables--------------------------------------
    private List<XO_Item> XO_Items_List;
    private List<XO_Item> XO_Activate_List;
    private int familyRemaining = 2;
    private int maxXSize = 3;
    private int maxYSize = 3;
   
    //----------Awake / Start / Update-----------------------
    private void Awake()
    {
       if(Instance!=null){ 
            Debug.LogError("THERE ARE MORE THAN ONE XOSys CLASS! "+transform+"-"+Instance+" ");
            Destroy(gameObject);
            return;
        }
         Instance = this;
         XO_Items_List = new List<XO_Item>();
         XO_Activate_List = new List<XO_Item>();
         
    }

    private void Start()
    {
        TurnSys.Instance.OnRoundStarted += TurnSys_OnRoundStarted;
        Pre_AddXO_Item_InList(transform, "XO_Item", XO_Items_List);
        AssignSpecialTile();
       
    }

    private void Pre_AddXO_Item_InList(Transform parent, string tag, List<XO_Item> list){
        //Pre add all XO_Item into the list
         foreach (Transform child in parent)
        {
            if (child.gameObject.tag == tag)
            {
                list.Add(child.gameObject.GetComponent<XO_Item>());
            }
            Pre_AddXO_Item_InList(child, tag, list);
        }

    }

    
    //------------Managing XO---------------------------------------

    public XO_Item GetXOItem_From_AllList(Vector2 position){
        //Find specific XO item via Vector2 (position)
        foreach (XO_Item xo in XO_Items_List){
            if(xo.GetPosition()==position){
                return xo;
            }
        }
        return null;
    }


    private void AssignSpecialTile(){
        int round = TurnSys.Instance.GetRound();

        for(int i=0;i<round+1;i++){
           
            //-------Random index + special Tile--------------------
        
            int random_X_Position = UnityEngine.Random.Range(0, 3); //Random number from 0-2 for x axis position
            int random_y_Position = UnityEngine.Random.Range(0, 3); //Random number from 0-2 for y axis position
            int random_SpecialTile_Index  = UnityEngine.Random.Range(0,4); //random Material

            XO_Item xo = GetXOItem_From_AllList(new Vector2(random_X_Position,random_y_Position));

            if(xo.IsSpecialTile()) continue; //if already special tile change tile

            xo.SetSpecialTile(true,special_Tiles_List[random_SpecialTile_Index]);
            //Debug.Log("xo: "+xo+" ,Tile: "+special_Tiles_List[random_SpecialTile_Index]);       

            xo.GetComponent<Renderer>().material = special_Tiles_List[random_SpecialTile_Index].GetMat();
                   

        }
    }


    //-------------------------Mange XO item-----------------------------------------------------------------
    public bool TryXO(XO_Item xo){
        //Check if possible to add XO_Item
        return AddXO(xo);
    }
    
    private bool AddXO(XO_Item xo){
        if(XO_Activate_List.Contains(xo)) return false; //check if already in the list, return false is so
        if(!CheckSpecialTileCondition(xo)) return false; //check if special tile


        //-------------ADD to the List-------------------------------
        XO_Activate_List.Add(xo);
        if(TurnSys.Instance.IsPlayerTurn()) xo.GetComponent<Renderer>().material = checkboxMaterial_list[1]; //Cross Visual if player
        else xo.GetComponent<Renderer>().material = checkboxMaterial_list[2]; //Circle visual if AI

        //------------Check if win--------------------
        
        if(XO_Activate_List.Count>=3 && XO_Activate_List.Count<=9){
            if(CheckIfWin(xo)){
                OnFinishRound?.Invoke(this,EventArgs.Empty);
                return true;
            }
        }

        TurnSys.Instance.NextTurn(false); //Next turn

         if(XO_Activate_List.Count>=9){
            //Draw = game over
            OnFinishRound?.Invoke(this,EventArgs.Empty);
            return true;
        }
        return true;
    }

    public void ResetXOTiles(){
         //Clear all checkbox
        foreach (XO_Item xo in XO_Items_List){
            //Clear checkbox visual
           xo.GetComponent<Renderer>().material = checkboxMaterial_list[0];
           xo.Reset();
        }
        XO_Activate_List.Clear();
    }

    private bool CheckSpecialTileCondition(XO_Item xo){
        //return true for continute the game and mark XO item
        //return false when reaching Game over
        if(TurnSys.Instance.IsPlayerTurn() && xo.IsSpecialTile()){
            Special_Tile special_Tile = xo.GetSpecialTile();

            if(special_Tile.GetSpecialTileType()=="Part"){
                //In case special tile is body part = GAME OVER
                special_Tile.playAudioClip();
                xo.GetComponent<Renderer>().material = checkboxMaterial_list[1]; //Cross Visual for player
                TurnSys.Instance.NextTurn(true); //Force Enemy win
                OnFinishRound?.Invoke(this,EventArgs.Empty);
                return false;
            }

            if(special_Tile.GetSpecialTileType()=="Family"){
                //if family remaining = 0 = GameOver
                special_Tile.playAudioClip();
                familyRemaining--;
                OnFamilyChanged?.Invoke(this,EventArgs.Empty);
                if(familyRemaining<=0){
                    xo.GetComponent<Renderer>().material = checkboxMaterial_list[1]; //Cross Visual for player
                    TurnSys.Instance.NextTurn(true); //Force Enemy win
                    OnFinishRound?.Invoke(this,EventArgs.Empty);
                    return false;
                }
            }
        }

        return true;
    }

    //--------------------Calculate WIN----------------------------------------------------------

    private bool CheckIfWin(XO_Item xo){
        Vector2 xo_Position = xo.GetPosition();
        int xo_Position_x = (int)xo_Position.x;
        int xo_Position_y = (int)xo_Position.y;
       

        //---------CHECKING ROW-----------------------------//
        int winCount=0;
        winCount += CheckRow(xo_Position_x,xo_Position_y,maxYSize,xo);
        if(winCount>=3){
            //Debug.Log(TurnSys.Instance.IsPlayerTurn()+": Win by row");
            return true;
        }

         //-------------CHECKING COL---------------------------//
        winCount=0;
        winCount += CheckCol(xo_Position_y,xo_Position_x,maxXSize,xo);
        if(winCount>=3){
            //Debug.Log(TurnSys.Instance.IsPlayerTurn()+": Win by col");
            return true;
        }

        //-------------Checking Diagonal-------------------------//
        winCount=0;
        int AmountOfConnectedNeed=3; //
        winCount+=CheckDiagonal(xo_Position_x,xo_Position_y,AmountOfConnectedNeed,xo); 
         if(winCount>=3){
            //Debug.Log(TurnSys.Instance.IsPlayerTurn()+": Win by diagonal");
            return true;
        }
        return false;
        
      
    }

    private int CheckRow(int xo_position_axis_main,int xo_position_axis_check, int maxAxisSize, XO_Item xo){
        int winCount =0;
        for(int y=xo_position_axis_check; y<maxAxisSize; y++){
            //Check Right side
            Vector2 checkPosition = new Vector2(xo_position_axis_main,y);
            var result = CalculateWinCount(checkPosition,xo);
            if(result.hitOpposite) break;
            else winCount += result.winCount;
        }

        for(int y=xo_position_axis_check-1; y>=0; y--){
            //Check Left side
            Vector2 checkPosition = new Vector2(xo_position_axis_main,y);
            var result = CalculateWinCount(checkPosition,xo);
            if(result.hitOpposite) break;
            else winCount += result.winCount;
        }
        //Debug.Log(TurnSys.Instance.IsPlayerTurn()+": "+winCount+" Row");
        return winCount;
    }

    private int CheckCol(int xo_position_axis_main,int xo_position_axis_check, int maxAxisSize, XO_Item xo){
        int winCount =0;
        for(int z=xo_position_axis_check; z<maxAxisSize; z++){
            //Check Top side
            Vector2 checkPosition = new Vector2(z,xo_position_axis_main);
            var result = CalculateWinCount(checkPosition,xo);
            if(result.hitOpposite) break;
            else winCount += result.winCount;
        }

        for(int z=xo_position_axis_check-1; z>=0; z--){
            //Check Bottom side
            Vector2 checkPosition = new Vector2(z,xo_position_axis_main);
            var result = CalculateWinCount(checkPosition,xo);
            if(result.hitOpposite) break;
            else winCount += result.winCount;
        }
        //Debug.Log(TurnSys.Instance.IsPlayerTurn()+": "+winCount+" Col");
        return winCount;
        
        
    }

    private int CheckDiagonal(int xo_position_x,int xo_position_y,int AmountOfConnectedNeed,XO_Item xo){
        int winCount =0;
        //------------------Left diagonal---------------------
        for(int xyi=0;xyi<AmountOfConnectedNeed;xyi++){
            //Check up side
            Vector2 checkPosition = new Vector2(xo_position_x+xyi,xo_position_y-xyi);
            var result = CalculateWinCount(checkPosition,xo);
            if(result.hitOpposite) break;
            else winCount += result.winCount;
        }

        for(int xyi=1;xyi<AmountOfConnectedNeed;xyi++){
            //Check down side
            Vector2 checkPosition = new Vector2(xo_position_x-xyi,xo_position_y+xyi);
            var result = CalculateWinCount(checkPosition,xo);
            if(result.hitOpposite) break;
            else winCount += result.winCount;
        }

        
        if(winCount>=3){
            //Debug.Log(TurnSys.Instance.IsPlayerTurn()+": Win by Left diagonal");
            return winCount;
        }

        //------------------Right diagonal---------------------
        winCount=0;
        for(int xyi=0;xyi<AmountOfConnectedNeed;xyi++){
            //Check up side
             Vector2 checkPosition = new Vector2(xo_position_x+xyi,xo_position_y+xyi);
            var result = CalculateWinCount(checkPosition,xo);
            if(result.hitOpposite) break;
            else winCount += result.winCount;
        }

        for(int xyi=1;xyi<AmountOfConnectedNeed;xyi++){
            //Check down side
             Vector2 checkPosition = new Vector2(xo_position_x-xyi,xo_position_y-xyi);
            var result = CalculateWinCount(checkPosition,xo);
            if(result.hitOpposite) break;
            else winCount += result.winCount;
        }

        if(winCount>=3){
            //Debug.Log(TurnSys.Instance.IsPlayerTurn()+": Win by Right diagonal");
            return winCount;
        }

        return winCount;
    }

    private (int winCount,bool hitOpposite) CalculateWinCount(Vector2 checkPosition,XO_Item xo_Test){
        int winCount = 0;
        bool hit=false;

        foreach(XO_Item xO_Item in XO_Activate_List){
            if(xO_Item.GetPosition()==checkPosition && (xO_Item.IsPlayerXO()!=xo_Test.IsPlayerXO())){
                hit=true;
                continue;
            }

            if(xO_Item.GetPosition()==checkPosition && (xO_Item.IsPlayerXO()==xo_Test.IsPlayerXO())){
                winCount++;
                continue;
            }

            //Debug.Log(xO_Item.GetPosition()+": is player"+xO_Item.IsPlayerXO());
        }

        return (winCount,hit);
    }

    public int GetFamilyRemaining(){
        return familyRemaining;
    }

    public List<XO_Item> GetXO_Activate_List(){
        return XO_Activate_List;
    }

    public List<XO_Item>GetXO_All_List(){
        return XO_Items_List;
    }

    public int GetMaxXSize(){
        return maxXSize;
    }

    public int GetMaxYSize(){
        return maxYSize;
    }
   
    //------------------------Subscribe----------------------------

    private void TurnSys_OnRoundStarted (object sender, EventArgs e){
        ResetXOTiles();
        AssignSpecialTile();
    }



    
}
