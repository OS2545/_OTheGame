using UnityEngine;

public class AI_Margin 
{
    //Class for storing XO_item and possible good outcome result
    //Use with AI 
    public double point;
    public XO_Item xO_Item;

    public AI_Margin(double _point, XO_Item _xO_item){
        point = _point;
        xO_Item = _xO_item;
    }

}
