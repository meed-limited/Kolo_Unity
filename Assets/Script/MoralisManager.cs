using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoralisUnity.Core;
using MoralisUnity;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Web3Api.Models;
using TMPro;
using UnityEngine.SceneManagement;
using SuperUltra.Container;

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
        if(PlayerPrefs.GetString("WalletAddress") != null && PlayerPrefs.GetString("Token") != null)
        {
            UserData.WalletAddress = PlayerPrefs.GetString("WalletAddress");
            UserData.Token = PlayerPrefs.GetString("Token");
        }
        Debug.Log(UserData.WalletAddress);
        Debug.Log(UserData.Token);
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
        PlayerPrefs.SetString("WalletAddress", userAddress);
        UserData.WalletAddress = userAddress;
        PlayerPrefs.SetString(Config.KEY_WALLET_ADDRESS, userAddress);

        UserData.ObjectId = moralisUser.objectId;
        textMesh.text = userAddress;

        NetworkManager.GetAuthToken(
                (getAuthResponse) =>
                {
                    if (!getAuthResponse.result)
                    {
                        LoadingUI.HideInstance();
                        MessagePopUpUI.Show($"Register fail\n{getAuthResponse.message}");
                        return;
                    }

                }
                    
            );
        NetworkManager.GetAdsCountandKOL((getAuthResponse) =>
        {
            if (!getAuthResponse.result)
            {
                LoadingUI.HideInstance();
                MessagePopUpUI.Show($"Get Ads Count and KOLs fail\n{getAuthResponse.message}");
                return;
            }
            else
            {
                Debug.Log(getAuthResponse.result);
            }
        });
        //_background.SetActive(false);
        //LoadItems(userAddress, "0xe841128435D71364BeadD05E5d71CEF5016f0547", chainId);
    }


}
