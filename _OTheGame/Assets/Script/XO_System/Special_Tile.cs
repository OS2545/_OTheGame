using UnityEngine;

public class Special_Tile : MonoBehaviour
{
    [SerializeField]private Material mat;
    [SerializeField]private string tileType;

    public Material GetMat(){
        return mat;
    }
    public string GetSpecialTileType(){
        return tileType;
    }
}
