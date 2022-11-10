using Steamworks;
using Steamworks.Data;
using Steamworks.ServerList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class SteamLobbyManager : MonoBehaviour
{
    // 로비를 관리하는 변수들, 로비를 생성할때, 조인 했을때, 나갔을때 이벤트 처리가 되어야함
    public static Lobby currentLobby;
    public static bool UserInLobby;
    public UnityEvent OnLobbyCreated;
    public UnityEvent OnLobbyJoined;
    public UnityEvent OnLobbyLeave;

    // 로비에 들어온 친구들 관리를 위한 변수들
    public GameObject InLobbyFriend;
    public Transform content;
    public Dictionary<SteamId, GameObject> inLobby = new Dictionary<SteamId, GameObject>();



    // Start is called before the first frame update
    void Start()
    { 
        DontDestroyOnLoad(this);

        // 스팀의 로비 처리 이벤트에 함수들을 다음과 같이 연동 해야함!
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreatedCallBack;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnChatMessage += OnChatMessage;
        SteamMatchmaking.OnLobbyMemberDisconnected += OnLobbyMemberDisconnected;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberDisconnected;
        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequest;
        SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
    }

    void OnLobbyInvite(Friend friend, Lobby lobby)
    {
        Debug.Log($"{friend.Name} invited you to his lobby.");
    }


    private void OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId id)
    {

    }
 
    private async void OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        Debug.Log($"{friend.Name} joined the lobby");
        GameObject obj = Instantiate(InLobbyFriend, content);
        obj.GetComponentInChildren<Text>().text = friend.Name;
        obj.GetComponentInChildren<RawImage>().texture = await SteamFriendsManager.GetTextureFromSteamIdAsync(friend.Id);
        //GetTextureFromSteamIdAsync로 친구ID의 이미지를 긁어와 obj의 이미지에 적용(비동기 통신)
        inLobby.Add(friend.Id, obj);
    }

    void OnLobbyMemberDisconnected(Lobby lobby, Friend friend)
    {
        Debug.Log($"{friend.Name} left the lobby");
        Debug.Log($"New lobby owner is {currentLobby.Owner}");

        if (inLobby.ContainsKey(friend.Id))
        {
            Destroy(inLobby[friend.Id]);
            inLobby.Remove(friend.Id);
        }
    }

    void OnChatMessage(Lobby lobby, Friend friend, string message)
    {
        Debug.Log($"incoming chat message from {friend.Name} : {message}");
    }

    async void OnGameLobbyJoinRequest(Lobby joinedLobby, SteamId id)
    {
        RoomEnter joinedLobbySuccess = await joinedLobby.Join();
        if (joinedLobbySuccess != RoomEnter.Success)
        {
            Debug.Log("failed to join lobby : " + joinedLobbySuccess);
        }
        else
        {
            currentLobby = joinedLobby;
        }
    }

    void OnLobbyCreatedCallBack(Result result, Lobby lobby)
    {
        if (result != Result.OK)
        {
            Debug.Log("lobby creation result not ok : " + result);
        }
        else
        {
            OnLobbyCreated.Invoke();
            Debug.Log("lobby creation result ok");
        }
    }

    async void OnLobbyEntered(Lobby lobby)
    {
        Debug.Log("Client joined the lobby");
        UserInLobby = true;
        GameObject obj = Instantiate(InLobbyFriend, content);
        obj.GetComponentInChildren<Text>().text = SteamClient.Name;
        obj.GetComponentInChildren<RawImage>().texture = await SteamFriendsManager.GetTextureFromSteamIdAsync(SteamClient.SteamId);
        inLobby.Add(SteamClient.SteamId, obj);

        foreach(var friend in currentLobby.Members)
        {
            if(friend.Id != SteamClient.SteamId)
            {
                GameObject obj2 = Instantiate(InLobbyFriend, content);
                obj2.GetComponentInChildren<Text>().text = friend.Name;
                obj2.GetComponentInChildren<RawImage>().texture = await SteamFriendsManager.GetTextureFromSteamIdAsync(friend.Id);

                inLobby.Add(friend.Id, obj2);
            }
        }

        OnLobbyJoined.Invoke();

    }


    public async void CreateLobbyAsync()
    {
        bool result = await CreateLobby();
        if (!result)
        {
            //Invoke a error message.
        }
    }

    public static async Task<bool> CreateLobby()
    {
        try
        {
            Debug.Log("로비 생성을 시도!");
            var createLobbyOutput = await SteamMatchmaking.CreateLobbyAsync();
            if (!createLobbyOutput.HasValue)
            {
                Debug.Log("로비가 생성이 되었지만 초기화에 실패함!.");
                return false;
            }
            currentLobby = createLobbyOutput.Value;
            currentLobby.SetPublic();
            //currentLobby.SetPrivate();
            currentLobby.SetJoinable(true);
            return true;
        }
        catch (System.Exception exception)
        {
            Debug.Log("Failed to create multiplayer lobby : " + exception);
            return false;
        }
    }

    public void LeaveLobby()
    {
        try
        {
            UserInLobby = false;
            currentLobby.Leave();
            OnLobbyLeave.Invoke();
            foreach (var user in inLobby.Values)
            {
                Destroy(user);
            }
            inLobby.Clear();
            Debug.Log("로비 종료!");
        }
        catch
        {

        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
