using UnityEngine;
using UnityEngine.UI;

public class LivesUIManager : MonoBehaviour
{
    private Image[] Hearts;
    private void Start()
    {
        Hearts = GetComponentsInChildren<Image>();
        ManagerGameScript.Singleton.player.OnLostLife.AddListener(RemoveLives);
    }
    
    //Could be extended further if the functionality is needed.
    public void RemoveLives (short livesLeft)
    {
        Hearts[livesLeft].enabled = false;
    }
}
