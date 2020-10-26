using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using Gag2Score;

using UnityEngine.Networking;

public class launch : MonoBehaviour
{
    private DictationRecognizer m_DictationRecognizer;

    private int flag = 0;
    private string resultText;

    public GameObject dajare_object = null;
    
    private const string URL = "http://localhost:5000/";
    public string text;
    private double score;

    public GameObject rocket_object = null;

    private float speed = 10;

    // Start is called before the first frame update
    void Start()
    {
        m_DictationRecognizer = new DictationRecognizer();

        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.LogFormat("Dictation result: {0}", text);
            resultText = text;
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
        if(flag == 2){
            Rigidbody rb = rocket_object.GetComponent<Rigidbody>();  
            Vector3 force = new Vector3 (0.0f,speed,0.0f);
            rb.AddForce (force);
        }

        GameObject obj = GameObject.Find("AtomRocket");
        if(speed<=5.0 && obj.transform.position.y>50){
            Rigidbody rb2 = rocket_object.GetComponent<Rigidbody>();
            Vector3 force2 = new Vector3 (0.0f,-9.8f,0.0f);
            rb2.AddForce (force2);
            flag = 3;
        } 
    }

    private IEnumerator Connect(){
        HumorCalculator hc = new HumorCalculator();
        var (isDajare, kana) = hc.humorScore("布団が吹っ飛んだ");
        if (!isDajare)
        {
            score = 0.0;
            yield return null;
        }
        text = kana;
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
        }
    }

    public void pushbutton(){
        Debug.Log("ボタンを押した");

        if(flag == 0){
            flag = 1;
            m_DictationRecognizer.Start();
        }
        else{
            m_DictationRecognizer.Stop();
            Debug.LogFormat("result: {0}",resultText);
            dajare();
            //ダジャレをスコア化（floatで0~1が返ってくる)        
            StartCoroutine (Connect ());
            float score = 0.5f;
            speed *= score;
            flag = 2;
        }
    }
    public void dajare(){
        //Debug.LogFormat("dajare");
        Text dajare_text = dajare_object.GetComponent<Text>(); 
        dajare_text.text = resultText;
    }

    // public void speed(){
    //     Rigidbody rb = rocket_object.GetComponent<Rigidbody>();  
    //     Vector3 force = new Vector3 (0.0f,30.0f,0.0f);
    //     rb.AddForce (force);  
    // }

    //  for文だと処理スピードが速くて加速にならない。
    // public void speed(float score){
    //     Rigidbody rb = rocket_object.GetComponent<Rigidbody>();  
    //     Vector3 force = new Vector3 (0.0f,score*200,0.0f);
    //     for(int i=0;i<score*100;i++){
    //         rb.AddForce (force);
    //     }
    // }
}
