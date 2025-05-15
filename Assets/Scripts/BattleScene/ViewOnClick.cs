using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewOnClick : MonoBehaviour
{
    [SerializeField] bool isDiscard, isDrawDeck,isConsume;
    public void OnClick()
    {
        if (isDiscard)
        {
            PlayerHand.Instance.ViewDiscard();
        }
        else if (isDrawDeck)
        {
            PlayerHand.Instance.ViewDeck();
        }
        else if (isConsume)
        {
            PlayerHand.Instance.ViewConsume();
        }
        else
        {
            //is just random card
            GetComponent<UnitCard>().SelectedView();
        }
    }
    
}
