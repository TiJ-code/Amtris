using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GithubResourcePack_UIElement : MonoBehaviour
{
    private void Awake()
    {
        if (Infrastructure.CheckNetworkConnection())
        {

        }
    }
}

[System.Serializable]
public class GithubResourcePack_Website
{
    public string name;
    public string link;
}

[System.Serializable]
public class GithubResourcePack
{
    public string name;
    public string description;
    public Texture2D icon;

    public string link;
}
