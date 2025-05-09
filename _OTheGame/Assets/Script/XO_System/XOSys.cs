using UnityEngine;
using System;
using System.Collections.Generic;

public class XOSys : MonoBehaviour
{
    public static XOSys Instance{get; private set;} //Only one system in run

   //---------SerializeField --------------------------------
    [SerializeField] private List<Material> checkboxMaterial_list; // 0 = Transparent 1 = Cross 2=Circle
    

    //--------Variables--------------------------------------
    private List<XO_Item> XO_Items_List;
    private List<XO_Item> XO_Activate_List;
   
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
        Pre_AddXO_Item_InList(transform, "XO_Item", XO_Items_List);
    }

    private void Pre_AddXO_Item_InList(Transform parent, string tag, List<XO_Item> list){
        //Pre add all XO_Item in the list
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

    private XO_Item GetXO_Item(Vector2 position){
        foreach (XO_Item xo in XO_Items_List){
            if(xo.GetPosition()==position){
                return xo;
            }
        }
        return null;
    }
    public bool CanAddXO(XO_Item xo){
        //Check if possible to add XO_Item
        return AddXO(xo);
    }
    
    private bool AddXO(XO_Item xo){
        if(XO_Activate_List.Contains(xo)) return false; //check if already in the list, return false is so
        //-------------ADD to the List-------------------------------
        XO_Activate_List.Add(xo);
        if(TurnSys.Instance.IsPlayerTurn()) xo.GetComponent<Renderer>().material = checkboxMaterial_list[1]; //Cross Visual if player
        else xo.GetComponent<Renderer>().material = checkboxMaterial_list[2]; //Circle visual if AI

        //------------Check if win--------------------
        //checkIfWin(xo);
        if(XO_Activate_List.Count>=3){
            if(checkIfWin(xo)){
                return true;
            }
        }
        return true;
    }

    private void clearXO(){
         //Clear all checkbox
        foreach (XO_Item xo in XO_Activate_List){
            //Clear checkbox visual
           xo.GetComponent<Renderer>().material = checkboxMaterial_list[0];
        }
        XO_Activate_List.Clear();
    }

    private bool checkIfWin(XO_Item xo){
        Vector2 xo_Position = xo.GetPosition();
        int xo_Position_x = (int)xo_Position.x;
        int xo_Position_y = (int)xo_Position.y;
        int winCount=0;

        for(int y=0;y<3;y++){
            //check row
            Vector2 checkPosition = new Vector2(xo_Position_x,y);
            //Debug.Log(checkPosition);
            winCount = calculateWinCount(winCount,checkPosition,xo);
            //Debug.Log(TurnSys.Instance.IsPlayerTurn()+": "+winCount);
        }

        if(winCount>=3){
            //Debug.Log(TurnSys.Instance.IsPlayerTurn()+": Win");
            return true;
        }
        return false;
    }

    private int calculateWinCount(int currentWinCount,Vector2 checkPosition,XO_Item xo){
        int winCount = currentWinCount;
        foreach(XO_Item xO_Item in XO_Activate_List){
            if(xO_Item.GetPosition()==checkPosition && (xO_Item.IsPlayerXO()==xo.IsPlayerXO())){
                winCount++;
            }
            Debug.Log(xO_Item.GetPosition()+": is player"+xO_Item.IsPlayerXO());
        }

        return winCount;
    }

   




    
}
