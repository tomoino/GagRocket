using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using Gag2Score;
using UnityEngine.Networking;
//using System.Linq;

public class launch : MonoBehaviour
{
    public GameObject dajare_object = null;
    public GameObject rocket_object = null;

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
        // data_.Add(Random.Range (0, 50));
        // if (data_.Count > 100) {
        //     data_.RemoveAt(0);
        // }
    }

    private IEnumerator Connect(){
        HumorCalculator hc = new HumorCalculator();
        var (isDajare, kana) = hc.humorScore(resultText);
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
        else{ // 2度目にボタンを押されたとき。音声認識をとめる。
            m_DictationRecognizer.Stop();
            Debug.LogFormat("result: {0}",resultText);
            displayText(resultText); 
            // StartCoroutine (Connect ()); // APIを使ってスコアを計算する

            // ロケット検証用
            score = 0.6;
            speed *= (float)score;
            flag = 2;
        }
    }

    private void displayText(string str){
        Text dajare_text = dajare_object.GetComponent<Text>(); 
        dajare_text.text = str;
    }

    // void OnGUI()
    // {
    //     var area = GUILayoutUtility.GetRect(Screen.width, Screen.height);

    //     // Grid
    //     const int div = 10;
    //     for (int i = 0; i <= div; ++i) {
    //         var lineColor = (i == 0 || i == div) ? Color.white : Color.gray;
    //         var lineWidth = (i == 0 || i == div) ? 2f : 1f;
    //         var x = (area.width  / div) * i;
    //         var y = (area.height / div) * i;
    //         Drawing.DrawLine (
    //             new Vector2(area.x + x, area.y),
    //             new Vector2(area.x + x, area.yMax), lineColor, lineWidth, true);
    //         Drawing.DrawLine (
    //             new Vector2(area.x,    area.y + y),
    //             new Vector2(area.xMax, area.y + y), lineColor, lineWidth, true);
    //     }

    //     // Data
    //     if (data_.Count > 0) {
    //         var max = data_.Max();
    //         var dx  = area.width / data_.Count; 
    //         var dy  = area.height / max;
    //         Vector2 previousPos = new Vector2(area.x, area.yMax); 
    //         for (var i = 0; i < data_.Count; ++i) {
    //             var x = area.x + dx * i;
    //             var y = area.yMax - dy * data_[i];
    //             var currentPos = new Vector2(x, y);
    //             Drawing.DrawLine(previousPos, currentPos, Color.red, 3f, true);
    //             previousPos = currentPos;
    //         }
    //     }
    // }
}
