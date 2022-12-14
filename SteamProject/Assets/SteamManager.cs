using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SteamManager : MonoBehaviour
{
    public uint appId;
    public UnityEvent OnSteamFailed;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        try
        {
            Steamworks.SteamClient.Init(appId, true);
            Debug.Log("?????? ??????!");
        }
        catch(System.Exception e)
        {
            Debug.Log(e.Message);
            OnSteamFailed.Invoke();
        }
    }

    private void OnApplicationQuit()
    {
        try
        {
            Steamworks.SteamClient.Shutdown();
        }
        catch
        {

        }
    }
}
