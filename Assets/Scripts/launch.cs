using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;


public class launch : MonoBehaviour
{
    private DictationRecognizer m_DictationRecognizer;

    private int flag = 0;
    private string resultText;

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
        GameObject obj = GameObject.Find("Sphere");  
        obj.transform.position += Vector3.up;

        if(flag == 0){
            flag = 1;
            m_DictationRecognizer.Start();
        }
        else{
            m_DictationRecognizer.Stop();
            Debug.LogFormat("result: {0}",resultText);
        }
    }
}
