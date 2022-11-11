using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using SimpleJSON;
using BestHTTP.JSON;
using static UnityEngine.Networking.UnityWebRequest;

namespace SuperUltra.Container
{


    public class ResponseData
    {
        public bool result;
        public string message;
    }

    public static class NetworkManager
    {
        static string _token = "";
        static bool _isUserDataRequested = false;
        static Action _onCompleteLoginRequest;
        const float _timeOut = 6f;
        

        public static bool CheckConnection()
        {
            if (
                !Application.internetReachability.Equals(NetworkReachability.ReachableViaLocalAreaNetwork)
                && !Application.internetReachability.Equals(NetworkReachability.ReachableViaCarrierDataNetwork)
            )
            {
                return false;
            }
            return true;
        }

        static string GetDataMessage(JSONNode json)
        {
            if (json != null)
            {
                if (json["message"].IsString && !string.IsNullOrEmpty(json["message"]))
                {
                    return json["message"];
                }

                if (json["error"].IsString && !string.IsNullOrEmpty(json["error"]))
                {
                    return json["error"];
                }
            }
            // Response received (json is null or message empty)
            return "";
        }

        static ResponseData ValidateResponse(HTTPResponse response)
        {
            ResponseData data = new ResponseData() { result = false };
            JSONNode json;
            if (response == null || response.IsSuccess == false)
            {
                data.message = "Server error";
                if (response != null)
                    Debug.Log(response.StatusCode);
                if (response != null && !string.IsNullOrEmpty(response.DataAsText))
                {
                    Debug.Log(response.DataAsText);
                    try
                    {
                        json = JSON.Parse(response.DataAsText);
                        data.message = GetDataMessage(json);
                    }
                    catch (System.Exception e)
                    {
                        throw e;
                    }
                }
                return data;
            }

            json = JSON.Parse(response.DataAsText);
            Debug.Log("ValidateResponse " + json);
            if (json == null || json["success"] == null || json["success"] != true)
            {
                data.message = GetDataMessage(json);
                return data;
            }
            data.message = GetDataMessage(json);
            data.result = true;
            return data;
        }

        /// <summary>
        /// request token from server, then use the token to request
        /// game list, user data and season data
        /// </summary>
        public static void GetAuthToken(Action<ResponseData> callback, string token = "")
        {
            HTTPRequest request = new HTTPRequest(
                new Uri(Config.Domain + "/api/v1/kolohack/users/auth"),
                HTTPMethods.Post,
                (req, res) =>
                {
                    Debug.Log("GetAuthToken response");
                    OnGetAuthTokenRequestFinished(req, res, callback);
                }
            );
            JSONObject json = new JSONObject();
            Debug.Log($"p {UserData.playFabId}\nat {UserData.playFabSessionTicket}");
            json.Add("WalletAddress", UserData.WalletAddress);
            json.Add("ObjectId", UserData.ObjectId);
            //
            request.AddHeader("Content-Type", "application/json");
            request.RawData = Encoding.ASCII.GetBytes(json.ToString());
            request.Timeout = TimeSpan.FromSeconds(_timeOut);
            request.Send();
        }

        static void OnGetAuthTokenRequestFinished(HTTPRequest request, HTTPResponse response, Action<ResponseData> callback)
        {
            ResponseData data = ValidateResponse(response);
            if (data.result)
            {
                JSONNode json = JSON.Parse(response.DataAsText);
                if (json != null && json["data"] != null)
                {
                    _token = json["data"]["token"];
                    PlayerPrefs.SetString("Token", _token);
                    Debug.Log(_token);
                }
            }
            else
            {
                data.message = MessageConst.GetAuthenFailed;
            }
            callback?.Invoke(data);
        }

        public static void IncreaseAdsCount(Action<ResponseData> callback = null)
        {
            // get all th game list from api from Config.domain
            HTTPRequest request = new HTTPRequest(
                new Uri(Config.Domain + "/api/v1/kolohack/users/adscount"),
                HTTPMethods.Post,
                (req, res) =>
                {
                    Debug.Log("IncreaseAdsCount response");
                }
            );
            JSONObject json = new JSONObject();
            json.Add("WalletAddress", UserData.WalletAddress);
            request.AddHeader("Authorization", "Bearer " + _token);
            request.AddHeader("Content-Type", "application/json");
            request.Timeout = TimeSpan.FromSeconds(_timeOut);
            request.RawData = Encoding.ASCII.GetBytes(json.ToString());
            request.Send();
        }

      

        public static void GetAdsCountandKOL(Action<ResponseData> callback, Action<ResponseData> dataRequestCallback = null)
        {
            HTTPRequest request = new HTTPRequest(
                new Uri(Config.Domain + "/api/v1/kolohack/users/adstokencount"),
                HTTPMethods.Post,
                (req, res) =>
                {
                    Debug.Log("GetUserData response");
                    OnAdsTokenCountRequestFinished(req, res, callback, dataRequestCallback);
                }
            );
            JSONObject json = new JSONObject();
            json.Add("WalletAddress", UserData.WalletAddress);
            request.AddHeader("Authorization", "Bearer " + _token);
            request.AddHeader("Content-Type", "application/json");
            request.Timeout = TimeSpan.FromSeconds(_timeOut);
            request.RawData = Encoding.ASCII.GetBytes(json.ToString());
            request.Send();
        }

        static void OnAdsTokenCountRequestFinished(HTTPRequest request, HTTPResponse response, Action<ResponseData> callback, Action<ResponseData> avatarRequestCallback = null)
        {
            ResponseData responseData = ValidateResponse(response);
            Debug.Log("OnAdsTokenCountRequestFinished");
            Debug.Log($"{request.RawData}");
            if (responseData.result)
            {
                JSONNode json = JSON.Parse(response.DataAsText);
                JSONNode data = json["data"];
                if (data != null)
                {
                    
                    Debug.Log("OnUserDataRequestFinished " + response.DataAsText);
                    UserData.AdsCount = data["adsCount"];
                    UserData.KOL = data["tokenCount"];
                    ContainerInterface.UpdateAutoLoginInformation();
                }
            }
            else
            {
                avatarRequestCallback?.Invoke(new ResponseData { result = false });
                
            }

            callback?.Invoke(responseData);
        }

        
        /**
        static void CompleteRequestList(Action<ResponseData> callback, ResponseData data)
        {
            if (!data.result)
            {
                callback?.Invoke(data);
                return;
            }
            Debug.Log($"{_isUserDataRequested} {_isAvatarImageRequested}");
            if (_isUserDataRequested
                && _isAvatarImageRequested
            )
            {
                callback?.Invoke(new ResponseData
                {
                    result = true
                });
            }
        }

        **/

        public static void AutoLogin(string token, Action<ResponseData> callback)
        {
            _token = token;
            GetLoginInfomation(callback);
        }

        public static void LoginRequest(Action<ResponseData> callback)
        {
            if (!CheckConnection())
            {
                callback?.Invoke(new ResponseData { result = false, message = MessageConst.ConnectionFail });
                return;
            }

            if (string.IsNullOrEmpty(UserData.playFabSessionTicket))
            {
                callback?.Invoke(new ResponseData { result = false, message = MessageConst.LoginPlayFabFailure });
                return;
            }

            GetAuthToken(
                (getTokenResponse) =>
                {
                    if (!getTokenResponse.result)
                    {
                        callback?.Invoke(getTokenResponse);
                        return;
                    }
                    GetLoginInfomation(callback);
                }
            );
        }

        static void GetLoginInfomation(Action<ResponseData> callback)
        {
    
            GetAdsCountandKOL((getUserDataResponse) =>
            {
                // Debug.Log("getUserDataResponse");
                _isUserDataRequested = getUserDataResponse.result;
                Debug.Log("Result.... :" + getUserDataResponse.result);
                //CompleteRequestList(callback, getUserDataResponse);
            }, (getAvatarResponse) =>
            {
                // player will proceed the menu whether avatar request is success or not
                // here just make sure we get the response. 
                //_isAvatarImageRequested = true;
                //getAvatarResponse.result = true;
                //CompleteRequestList(callback, getAvatarResponse);
            });
        }

        static JSONArray GetJSONArray(JSONNode node)
        {
            if (node.IsArray)
                return node.AsArray;
            return new JSONArray();
        }

       
     /**
        public static void CreateUser(string playFabId, Action success, Action<ResponseData> failure = null)
        {
            HTTPRequest request = new HTTPRequest(
                new Uri(Config.Domain + "users"),
                HTTPMethods.Post,
                (req, res) => { OnCreateUserRequestFinished(req, res, success, failure); }
            );
            JSONObject json = new JSONObject();
            json.Add("platformId", playFabId);
            request.SetHeader("Authorization", "Bearer " + _token);
            request.AddHeader("Content-Type", "application/json");
            request.Timeout = TimeSpan.FromSeconds(_timeOut);
            request.RawData = Encoding.ASCII.GetBytes(json.ToString());
            request.Send();
        }

        static void OnCreateUserRequestFinished(HTTPRequest request, HTTPResponse response, Action success = null, Action<ResponseData> failure = null)
        {
            ResponseData responseData = ValidateResponse(response);
            if (responseData.result)
            {
                JSONNode json = JSON.Parse(response.DataAsText);
                success?.Invoke();
            }
            else
            {
                failure?.Invoke(responseData);
            }
        }

        
        #region Update User

        public static void UpdateUserWalletAddress(string playFabId, string walletAddress, Action<ResponseData> callback)
        {
            JSONObject json = new JSONObject();
            json.Add("walletAddress", walletAddress);
            json.Add("platformId", playFabId);
            
        }


        public static void UpdateUserProfile(string playFabId, string userName, Texture2D texture2D, Action<ResponseData> callback)
        {
            JSONObject json = new JSONObject();
            string encodedImage = Convert.ToBase64String(texture2D.EncodeToPNG());
            UserData.pendingProfilePic = texture2D;
            json.Add("avatarUrl", encodedImage);
            json.Add("username", userName);
            json.Add("platformId", playFabId);

        }
        **/
        public static void SignOut(Action callback)
        {
            // TODO : May need to call for endpoint to notify the event
            callback?.Invoke();
            UserData.ClearData();
            GameData.ClearData();
            SessionData.ClearData();
        }

        
        //#endregion

    }

}
