using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameOver : MonoBehaviour {
    void Start() {
        this.gameObject.GetComponent<Text>().text = "SCORE: " + Snake.score;
        Score score = new Score(Snake.score);
        Debug.Log(JsonUtility.ToJson(score));
        //StartCoroutine(PostScore(JsonUtility.ToJson(score)));
    }

    public void Restart() {
        Snake.score = 0;
        SceneManager.LoadScene("Main Game");
    }

    IEnumerator PostScore(string json) {
        var uwr = new UnityWebRequest("http://localhost:3000/update_score", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError) {
            Debug.Log("Error While Sending: " + uwr.error);
        }
    }

    class Score {
        public int score;
        public string game;

        public Score(int sc) {
            score = sc;
            game = "Snake";
        }
    }
}
