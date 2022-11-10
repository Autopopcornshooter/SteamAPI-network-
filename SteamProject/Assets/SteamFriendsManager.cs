using System.Collections;
using System.Collections.Generic;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.UI;

public class SteamFriendsManager : MonoBehaviour
{
    public RawImage pp;
    public Text playerName;

    public Transform friendContent;
    public GameObject friendObj;

    // Start is called before the first frame update
    async void Start()
    {
        if (!SteamClient.IsValid) return;

        playerName.text = SteamClient.Name;

        var img = await SteamFriends.GetLargeAvatarAsync(SteamClient.SteamId);
        pp.texture = GetTextureFromImage(img.Value);
        InitFriends();
    }
    
    public static async System.Threading.Tasks.Task<Texture2D> GetTextureFromSteamIdAsync(SteamId id)
    {
        var img = await SteamFriends.GetLargeAvatarAsync(SteamClient.SteamId);
        Steamworks.Data.Image image = img.Value;
        Texture2D texture = new Texture2D((int)image.Width, (int)image.Height);

        for(int x = 0; x < image.Height; x++)
        {
            for(int y=0; y < image.Width; y++)
            {
                var p = image.GetPixel(x, y);
                texture.SetPixel(x, (int)image.Height - y, new UnityEngine.Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
            }
        }
        texture.Apply();
        return texture;
    }



    public static Texture2D GetTextureFromImage(Steamworks.Data.Image image)
    {
        Texture2D texture = new Texture2D((int)image.Width, (int)image.Height);

        for (int x = 0; x < image.Width; x++)
        {
            for(int y=0; y < image.Height; y++)
            {
                var p = image.GetPixel(x, y);
                texture.SetPixel(x, (int)image.Height - y, new Color(p.r / 255.0f,
                                                                    p.g / 255.0f,
                                                                    p.b / 255.0f,
                                                                    p.a / 255.0f));
            }
            
        }
        texture.Apply();
        return texture;
    }


    public async void InitFriends()
    {
        foreach(var friend in SteamFriends.GetFriends())
        {
            Debug.Log(
                $"{friend.Name}({friend.Id}) | 온라인: {friend.IsOnline} | " +
                $"게임 플레이 여부: {friend.IsPlayingThisGame}"
                );
            GameObject f = Instantiate(friendObj, friendContent);
            f.GetComponentInChildren<Text>().text = friend.Name;
            AssignFriendImage(f, friend.Id);       

        }
    }

    public async void AssignFriendImage(GameObject f, SteamId id)
    {
        var img = await SteamFriends.GetLargeAvatarAsync(id);
        f.GetComponentInChildren<RawImage>().texture = GetTextureFromImage(img.Value);

    }
}
