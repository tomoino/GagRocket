using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;

using UnityEngine.Networking;

public class launch : MonoBehaviour
{
    public GameObject dajare_object = null;
    public GameObject rocket_object = null;
    public GameObject button_object = null;
    private List<float> data_ = new List<float>();

    private DictationRecognizer m_DictationRecognizer;
    private int flag = 0;
    private float speed = 10;
    private string resultText; // Dictationの結果
    private const string URL = "http://localhost:5000/";
    public string text; // APIに渡すテキスト。APIに渡して使用するためpublic
    private double score; // APIの結果。面白さのスコア。

    // Start is called before the first frame update
    void Start()
    {
        m_DictationRecognizer = new DictationRecognizer();

        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.LogFormat("Dictation result: {0}", text);
            resultText = text;
            displayText(resultText);
            buttonText();
        };

        m_DictationRecognizer.DictationComplete += (completionCause) =>
        {
            if (completionCause != DictationCompletionCause.Complete)
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
        };

        m_DictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        };

    }

    // Update is called once per frame
    void Update()
    {
        if (flag == 2) {
            Rigidbody rb = rocket_object.GetComponent<Rigidbody>();  
            Vector3 force = new Vector3 (0.0f,speed,0.0f);
            rb.AddForce (force);
        }

        GameObject obj = GameObject.Find("AtomRocket");
        if (speed <= 5.0 && obj.transform.position.y > 50) {
            Rigidbody rb2 = rocket_object.GetComponent<Rigidbody>();
            Vector3 force2 = new Vector3 (0.0f,-9.8f,0.0f);
            rb2.AddForce (force2);
            flag = 3;
        } 
    }

    private IEnumerator Connect(){
        text = resultText;
        string myjson = JsonUtility.ToJson(this);
        byte[] postData = System.Text.Encoding.UTF8.GetBytes (myjson);
        var request = new UnityWebRequest(URL, "POST");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.Send();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            score = double.Parse(request.downloadHandler.text);
            Debug.LogFormat("Humor Score: {0}", score);
            speed *= (float)score;
            flag = 2;
        }
    }

    public void pushbutton(){
        Debug.Log("ボタンを押した");

        if (flag == 0) { // ボタンを押されてから一度だけ実行。音声認識処理を走らせる。
            flag = 1;
            m_DictationRecognizer.Start(); 
        }
        else if(flag == 1){ // 2度目にボタンを押されたとき。音声認識をとめる。
            m_DictationRecognizer.Stop();
            Debug.LogFormat("result: {0}",resultText);
            StartCoroutine (Connect ()); // APIを使ってスコアを計算する
            buttonText2();

            //ロケット検証用
            score = 0.5;
            speed *= (float)score;
            flag = 2;
        }
        else{
            SceneManager.LoadScene("Title");
        }
    }

    private void displayText(string str){
        Text dajare_text = dajare_object.GetComponent<Text>(); 
        dajare_text.text = str;
    }
    private void buttonText(){
        Text button_text = button_object.GetComponent<Text>(); 
        button_text.text = "発射";
    }

    private void buttonText2(){ 
        Text button_text = button_object.GetComponent<Text>();
        button_text.text = "retry";
    }
}
