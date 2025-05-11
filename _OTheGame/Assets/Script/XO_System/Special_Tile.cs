using UnityEngine;

public class Special_Tile : MonoBehaviour
{
    [SerializeField]private Material mat;
    [SerializeField]private string tileType;
    [SerializeField]private AudioClip tileTakenAudio;

    public Material GetMat(){
        return mat;
    }
    public string GetSpecialTileType(){
        return tileType;
    }

    public void playAudioClip(){
        SoundVFXManager.Instance.PlaySoundFileClip(tileTakenAudio,transform,1f);
    }
}
