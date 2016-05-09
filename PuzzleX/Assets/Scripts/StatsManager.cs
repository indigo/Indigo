using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

// handle stats for the game
public class StatsManager : MonoBehaviour
{

    private Dictionary<string, object> currentGameSession;
    private float gameSessionTimeStart = 0;

    public void Awake() {
        currentGameSession = new Dictionary<string, object>();
    }

    public Dictionary<string, object> GetCurrentGameSessionDictionnary() {
        return currentGameSession;
    }

    public void UpdateIntSessionInfo(string key, int data) {
        if (currentGameSession.ContainsKey(key))
        {
            currentGameSession[key] = (int)currentGameSession[key] + data;
        }
        else
        {
            currentGameSession.Add(key, data);
        }
    }

    public void StartNewGameSession() {
        gameSessionTimeStart = Time.time;
        currentGameSession = new Dictionary<string, object>();
    }

    public void EndCurrentGameSessionData() {
        int sessionTime = (int)(Time.time - gameSessionTimeStart);
        UpdateIntSessionInfo("duration", sessionTime);
        Analytics.CustomEvent("gameOver", GetCurrentGameSessionDictionnary());
    }
}
