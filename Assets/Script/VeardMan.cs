using System.Collections;
using System.Collections.Generic;
using SuperUltra.Container;
using UnityEngine;

public class VeardMan : MonoBehaviour
{
    public TMPro.TextMeshProUGUI _kolText;
    private int _adCount;
    private string _kol;
    [SerializeField] GameObject[] _tokenbar;
    public GameObject _vObject;
    public GameObject _vImage;

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

    

    public void NormalAdsReward()
    {
        _vObject.SetActive(true);
        _vImage.SetActive(true);
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
