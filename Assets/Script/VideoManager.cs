using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public VideoClip[] _videos;
    public GameObject _walletConnectButton;
    public GameObject _walletDisconnectButton;
    public GameObject _vImage;

    private void OnEnable()
    {
        int i = Random.Range(0, _videos.Length);
        gameObject.GetComponent<VideoPlayer>().clip = _videos[i];
        _walletConnectButton.SetActive(false);
        _walletDisconnectButton.SetActive(false);
    }


    public void OnClickClose()
    {
        gameObject.SetActive(false);
        _vImage.SetActive(false);
    }
}
