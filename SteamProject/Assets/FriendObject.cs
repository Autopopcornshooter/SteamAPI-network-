using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendObject : MonoBehaviour
{

    public SteamId steamId;

    public void Invite()
    {
        try
        {
            SteamLobbyManager.currentLobby.InviteFriend(steamId);
        }
        catch
        {

        }
    }

   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
