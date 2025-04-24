using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using System.Text.RegularExpressions;

public class DescriptionCreator : MonoBehaviour
{
    TextMeshProUGUI textComp;
    [SerializeField] List<string> keywords = new List<string>();
    [SerializeField] List<string> listy = new List<string>();
    private void Awake()
    {
        textComp = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        keywords = GetSpriteNames();
        listy = textComp.text.Split(" ").ToList();
        textComp.text = CreateDescription(listy);
    }
    
    public string CreateDescription(List<string> textList)
    {
        for (int i=0;i<textList.Count;i++)
        {
            Debug.Log(textList[i]);
            if (keywords.Contains(textList[i]))
            {
                textList[i] = $"<sprite name=\"{textList[i]}\">";
            }
        }
        return  string.Join(" ", textList);
   
    }


    List<string> GetSpriteNames()
    {
        List<string> names = new List<string>();

        if (textComp.spriteAsset != null)
        {
            foreach (var spriteChar in textComp.spriteAsset.spriteCharacterTable)
            {
                names.Add(spriteChar.name);
            }
        }
        else
        {
            Debug.LogWarning("No sprite asset assigned to TMP component!");
        }

        return names;
    }
}
