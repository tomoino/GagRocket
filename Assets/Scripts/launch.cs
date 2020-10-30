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
    public GameObject score_object = null;
    private GameObject mainCamera;
    private GameObject subCamera;
    private GameObject Rocket;
    private GameObject particle;
    private Rigidbody rigid;
    private List<float> data_ = new List<float>();

    private DictationRecognizer m_DictationRecognizer;
    private int flag = 0;
    private float speed = 50;
    private string resultText; // Dictationの結果
    private const string URL = "http://localhost:5000/";
    public string text; // APIに渡すテキスト。APIに渡して使用するためpublic
    private double score; // APIの結果。面白さのスコア。

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        subCamera = GameObject.Find("Sub Camera");
        //Rocket = GameObject.Find("AtomRocket");
        Rocket = GameObject.FindGameObjectWithTag("obj");
        rigid = rocket_object.GetComponent<Rigidbody>();
        subCamera.SetActive(false); 

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
        if (flag == 2 && Rocket.transform.position.y < 30){
            Rigidbody rb = rocket_object.GetComponent<Rigidbody>();  
            Vector3 force = new Vector3 (0.0f,5.0f,0.0f);
            rb.AddForce (force);
        }
        if (flag == 2 && Rocket.transform.position.y > 30) {
            Rigidbody rb2 = rocket_object.GetComponent<Rigidbody>();  
            Vector3 force2 = new Vector3 (0.0f,speed,0.0f);
            rb2.AddForce (force2);
        }
        if (speed <= 25.0 && Rocket.transform.position.y > 50) {
            Rigidbody rb3 = rocket_object.GetComponent<Rigidbody>();
            Vector3 force3 = new Vector3 (0.0f,-9.8f,0.0f);
            rb3.AddForce (force3);
            flag = 3;
        }
        if (flag == 3 && rigid.velocity.y < 1.0f) {
            engine2();
        }
        if (flag == 3 && Rocket.transform.position.y<10){
            mainCamera.SetActive(false);
            subCamera.SetActive(true);
        }
        if (Rocket.transform.position.y<0){
            Rigidbody rb4 = rocket_object.GetComponent<Rigidbody>();
            Vector3 force4 = new Vector3 (0.0f,-9.8f,0.0f);
            rb4.AddForce (force4);
            displayText2();
            flag = 4;
        }
        if (Rocket.transform.position.y > 400){
            displayText3();
            flag = 4;
        }
        if (flag == 4){
            displayText4();
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
            engine();

            //ロケット検証用
            score = 0.4;
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
    private void displayText2(){
        Text dajare_text = dajare_object.GetComponent<Text>(); 
        dajare_text.text = "Not Funny";
    }
    private void displayText3(){
        Text dajare_text = dajare_object.GetComponent<Text>(); 
        dajare_text.text = "Funny!!";
    }
    private void displayText4(){
        Text score_text = score_object.GetComponent<Text>(); 
        score_text.text = $"score:{(int)(100*score)}";
    }

    private void engine(){
        particle = GameObject.FindGameObjectWithTag("effect");
        //var obj = Instantiate (particle, transform.position,transform.rotation);
        ParticleSystem p = particle.GetComponent<ParticleSystem>();
        p.Play();
    }
    private void engine2(){
        particle = GameObject.FindGameObjectWithTag("effect");
        ParticleSystem p = particle.GetComponent<ParticleSystem>();
        p.Stop();
    }
}
