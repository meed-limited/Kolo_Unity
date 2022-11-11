using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrlManager : MonoBehaviour
{
    public void ToSuperUltraUrl()
    {
        Application.OpenURL("https://www.superultra.io/");
    }

    public void ToWithdrawUrl()
    {
        Application.OpenURL("https://kolo.superultra.io/");
    }

    public void ToNewsUrl()
    {
        Application.OpenURL("https://www.superultra.io/blog");
    
    }
}
