using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoralisUnity.Core;
using MoralisUnity;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Web3Api.Models;
using TMPro;
using UnityEngine.SceneManagement;

public class MoralisManager : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] private TMPro.TextMeshProUGUI textMesh;
    //[SerializeField] GameObject _background;
    MoralisUser moralisUser;
    bool isLoggedIn = false;
    private string userAddress;
    private ChainList chainId;
    private int _currentItemsCount;
    [SerializeField] private Transform content;

    /**
    private void Awake()
    {
        Moralis.Start();
        chainId = ChainList.cronos_testnet;
    }
    **/
    private void Start()
    {
        Time.timeScale = 1;
        //ShowName();


    }
    public async void ShowName()
    {
        moralisUser = await Moralis.GetUserAsync();
        isLoggedIn = moralisUser != null;
        Debug.Log($"isLoggedIn = {isLoggedIn}");
        if (!isLoggedIn)
        {
            Debug.Log("Not Loggined!");
            return;
        }
        userAddress = moralisUser.ethAddress;
        textMesh.text = userAddress;
        //_background.SetActive(false);
        //LoadItems(userAddress, "0xe841128435D71364BeadD05E5d71CEF5016f0547", chainId);
    }


}
