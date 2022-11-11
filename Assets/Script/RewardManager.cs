using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperUltra.Container;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;

public class RewardManager : MonoBehaviour
{

    public TMPro.TextMeshProUGUI _kolText;
    private int _adCount;
    private string _kol;
    [SerializeField] GameObject[] _tokenbar;

    private void OnEnable()
    {
        ContainerInterface.OnAutoLogin += UpdateAdsCountBar;
    }

    private void DisEnable()
    {
        ContainerInterface.OnAutoLogin -= UpdateAdsCountBar;
    }

    private void Awake()
    {
        //_adCount = int.Parse(UserData.AdsCount);
        _kol = UserData.KOL;

    }
    private void Start()
    {

        //_tokenText.text = _token.ToString();
        _kolText.text = _kol;
    }

    void UpdateAdsCountBar()
    {
        _adCount = int.Parse(UserData.AdsCount);
        Debug.Log($"_adcount: {_adCount}, Start Create bar");
        for (int i = 0; i < _adCount; i++)
        {
            //Debug.Log($"i +{i}");
            _tokenbar[i].SetActive(true);
        }
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
        /**
        _token = PlayerPrefs.GetInt("token", 0);
        _kol = PlayerPrefs.GetInt("KOL", 0);

        

        }
        **/
        NetworkManager.IncreaseAdsCount();
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

        _kolText.text = _kol;

        _adCount += 1;

        if (_adCount != 0)
            _tokenbar[_adCount - 1].SetActive(true);

        if (_adCount == 10)
        {
            for (int i = 0; i < _adCount; i++)
            {
                _tokenbar[i].SetActive(false);
            }
            _adCount = 0;
            _kol += 1;
        }
    }
}
