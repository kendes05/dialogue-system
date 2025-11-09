using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class TypeTextAnimation : MonoBehaviour
{
    public Action TypeFinished;
    public float typeDelay = 0.05f;
    public TextMeshProUGUI textObject;

    public string fullText;
    Coroutine coroutine;

    public void StartTyping() {
        coroutine = StartCoroutine(TypeText());
    }

    public void Start(){

    }

    public void Skip() {
        StopCoroutine(coroutine);
        textObject.maxVisibleCharacters = textObject.text.Length;
    }

    IEnumerator TypeText(){
        textObject.text = fullText;
        textObject.maxVisibleCharacters = 0;
        for(int i = 0; i<=textObject.text.Length;i++){
            textObject.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typeDelay);
        }
        TypeFinished?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
