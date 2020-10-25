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
    public string text = "犬 が 居 ぬ";

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

        StartCoroutine (Connect ());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private IEnumerator Connect(){
        // WWWForm form = new WWWForm();
        // form.AddField("text", "犬 が 居 ぬ");
        string myjson = JsonUtility.ToJson(this);
        byte[] postData = System.Text.Encoding.UTF8.GetBytes (myjson);
        var request = new UnityWebRequest(URL, "POST");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.Send();

        // UnityWebRequest request = UnityWebRequest.Get(URL);
        // yield return request.SendWebRequest();

        // //エラー処理
        // if(request.isNetworkError){
        //     Debug.Log(request.error);
        // }else{
        //     //リクエストが成功した時
        //     if(request.responseCode == 200){
        //         Debug.LogFormat("result: {0}", request.downloadHandler.text);
        //     }
        // }

            // yield return www.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                Debug.LogFormat("result: {0}", request.downloadHandler.text);
            }
    }

    public void pushbutton(){
        Debug.Log("ボタンを押した");
        

        HumorCalculator hc = new HumorCalculator();
        double result = hc.humorScore("布団が吹っ飛んだ");
        Debug.LogFormat("HC: {0}",result);
        return;

        GameObject obj = GameObject.Find("AtomRocket");  
        obj.transform.position += Vector3.up;

        if(flag == 0){
            flag = 1;
            m_DictationRecognizer.Start();
        }
        else{
            m_DictationRecognizer.Stop();
            Debug.LogFormat("result: {0}",resultText);
            dajare();
        }
    }
    public void dajare(){
        //Debug.LogFormat("dajare");
        Text dajare_text = dajare_object.GetComponent<Text>(); 
        dajare_text.text = resultText;
    }
}
