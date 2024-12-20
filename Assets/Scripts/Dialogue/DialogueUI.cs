using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text_label;

    public void Start(){
        GetComponent<TypewriterEffect>().Run("Hello, World!\nSecond", text_label);
    }

}
