using TMPro;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    static public SceneData Singleton;

    [SerializeField] public TextMeshProUGUI TextLobbyPlayerCount;

    void Awake()
    {
        if (Singleton == null)
            Singleton = this;
        else
            Destroy(gameObject);
    }
    
    public void SetTextLobbyPlayerCount(int count)
    {
        TextLobbyPlayerCount.text = $"Lobby Player Count: {count}"; ;
    }
}
