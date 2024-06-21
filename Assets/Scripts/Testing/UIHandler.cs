using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    private void Awake()
    {
        Infrastructure.CreateFileInfrastructure();
    }
}
