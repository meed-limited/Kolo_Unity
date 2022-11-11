using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using SimpleJSON;
using BestHTTP.JSON;

namespace SuperUltra.Container
{

    public class LoginManager : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI _address;
        public TMPro.TextMeshProUGUI _kol;
        public GameObject _retryButton;

        private void OnEnable()
        {
            ContainerInterface.OnAutoLogin += UpdateUserData;
            ContainerInterface.OnUpdateFailed += RetryUpdate;
        }

        private void OnDisable()
        {
            ContainerInterface.OnAutoLogin -= UpdateUserData;
            ContainerInterface.OnUpdateFailed -= RetryUpdate;
        }

        void Start()
        {

            Application.targetFrameRate = 60;
            //HidePanel(MessagePopUpUI.instance?.transform);
            if (!CheckInternetConnection())
            {
                return;
            }
            AutoLoginWithToken();
        }

        void UpdateUserData()
        {
            Debug.Log($"KOL Updated! UserKOL: {UserData.KOL} , UserAdsCount :{UserData.AdsCount}");
            _kol.text = UserData.KOL;
            _address.text = UserData.WalletAddress;
        }

        void SwitchRayCastOnOff(Transform transform, bool isOn = true)
        {
            GraphicRaycaster graphicRaycaster = transform.GetComponent<GraphicRaycaster>();
            if (graphicRaycaster == null)
            {
                return;
            }
            graphicRaycaster.enabled = isOn;
        }


        bool CheckInternetConnection()
        {
            if (!NetworkManager.CheckConnection())
            {
                MessagePopUpUI.Show("No Connection", "Retry", () => { CheckInternetConnection(); }, false);
                return false;
            }
            return true;
        }
        /**
        void HidePanel(Transform transform)
        {
            ISlidable slidable = transform.GetComponent<ISlidable>();
            if (slidable != null)
            {
                slidable.SlideOut(0f).OnComplete(
                    () => transform.gameObject.SetActive(false)
                );
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
        }
        **/

        public void RetryUpdate()
        {
            _retryButton.SetActive(true);
        }

        public void AutoLoginWithToken()
        {
            if (PlayerPrefs.HasKey("Token")
                && PlayerPrefs.HasKey("WalletAddress"))
            {
                
                UserData.WalletAddress = PlayerPrefs.GetString("WalletAddress");
                string token = PlayerPrefs.GetString("Token");
                LoadingUI.ShowInstance();
                NetworkManager.AutoLogin(
                    token,
                    (data) =>
                    {
                        if (data.result)
                        {
                            
                            //Do Something
                            LoadingUI.HideInstance();
                            _address.text = UserData.WalletAddress;
                            _kol.text = UserData.KOL.ToString();
                            Debug.Log("AutoLogined");
                            _retryButton.SetActive(false);
                        }
                        else
                        {
                            MessagePopUpUI.Show($"Login fail\n {data.message}");
                            LoadingUI.HideInstance();
                        }
                    }
                );
            }
        }


    }

}