using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public enum MenuType { Play, Textures, Credits, GameOver, Pause };

    public MenuType type;

    private Vector3 startPos;
    private float posY;


    public void Start()
    {
        startPos = this.transform.position;
        posY = startPos.y;
        this.transform.position = new Vector3(startPos.x, -posY, startPos.z);
    }

    public void OpenMenu()
    {
        if (type == MenuType.Play)
        {
            MoveY(posY, () => LoadGame.instance.LoadGameScene());
        } 
        else
        {
            MoveY(posY);
        }
    }

    public void CloseMenu()
    {
        MoveY(-posY);
    }

    private void MoveY(float y)
    {
        this.transform.LeanMoveY(y, 1.5f)
            .setEaseInOutExpo();
    }

    private void MoveY(float y, Action action)
    {
        this.transform.LeanMoveY(y, 1.5f)
            .setEaseInOutExpo()
            .setOnComplete(action);
    }
}
