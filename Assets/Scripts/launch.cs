using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using Gag2Score;


public class launch : MonoBehaviour
{
    private DictationRecognizer m_DictationRecognizer;

    private int flag = 0;
    private string resultText;

    public GameObject dajare_object = null;

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
