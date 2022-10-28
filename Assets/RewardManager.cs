using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperUltra.Container;
using UnityEngine.UI;
using TMPro;

public class RewardManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI _tokenText;
    public TMPro.TextMeshProUGUI _kolText;
    private int _token;
    private int _kol;

    private void Start()
    {
        _token = PlayerPrefs.GetInt("token", 0);
        _kol = PlayerPrefs.GetInt("KOL", 0);
    }

    public void RequestLifeAds()
    {
        ContainerInterface.RequestRewardedAds(
            RequestLifeRewardedAdCallback
        );
    }

    void RequestLifeRewardedAdCallback(bool result)
    {
        Debug.Log("RequestLifeRewardedAdCallback " + result);
        if (result)
        {
            //AddLife();
        }
    }

    public void RequestExtraTimeAds()
    {
        ContainerInterface.RequestRewardedAds(
            RequestExtraTimeRewardedAdCallback
        );
    }

    void RequestExtraTimeRewardedAdCallback(bool result)
    {
        Debug.Log("RequestExtraTimeRewardedAdCallback " + result);
        if (result)
        {
            //AddTime();
        }
    }

    public void RequestNormalAds()
    {
        ContainerInterface.RequestRewardedAds(
            RequestNormalRewardedAdCallback
        );
    }

    void RequestNormalRewardedAdCallback(bool result)
    {
        Debug.Log("RequestNormalRewardedAdCallback " + result);
        if (result)
        {
            NormalAdsReward();
        }
    }

    void NormalAdsReward()
    {
        Debug.Log("Ads Checked");
        gameObject.GetComponent<Button>().interactable = false;
        _token = PlayerPrefs.GetInt("token", 0);
        _kol = PlayerPrefs.GetInt("KOL", 0);

        _token += 1;
        if(_token == 10)
        {
            _token = 0;
            _kol += 1;
        }
        _tokenText.text = _token.ToString();
        _kolText.text = _kol.ToString();
        PlayerPrefs.SetInt("token", _token);
        PlayerPrefs.SetInt("KOL", _kol);
    }
}
