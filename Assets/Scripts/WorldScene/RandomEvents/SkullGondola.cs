using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkullGondola : MonoBehaviour
{
    [SerializeField] List<Transform> skullCharmsInShop;
    [SerializeField] TextMeshProUGUI greetingText;
    [SerializeField] TextMeshProUGUI playerSkullAmount;
    private void OnEnable()
    {
        var charms = IDLookupTable.instance.skullCharmList.OrderBy(x => Random.value).Take(Mathf.Min(5,IDLookupTable.instance.skullCharmList.Count)).ToList();
        for (int i =0;i<charms.Count;i++)
        {               
            var charmI = Instantiate(charms[i], skullCharmsInShop[i]);
            IDLookupTable.instance.skullCharmList.Remove(charms[i]);
            var specificTransform = skullCharmsInShop[i];
            charmI.GetComponent<Button>().onClick.AddListener(() => OnBuySkullCharm(charmI,specificTransform ));
            skullCharmsInShop[i].GetComponentInChildren<TextMeshProUGUI>().text = $"{charmI.skullCost} skulls"; 
        }
        playerSkullAmount.text = WorldManager.Instance.skullAmount.ToString();
        //Load up 3 cards and 3 charms, place in their spots based on transform list
        

    }

    public void OnBuySkullCharm(SkullCharm charm, Transform charmInShopTransform)
    {
        if(WorldManager.Instance.skullAmount < charm.skullCost)
        {
            return;
        }
        //check have enough skulls, if so buy charm
        //Remove from given transform

        //add to playerCharms in idlookuptable
        WorldManager.Instance.skullAmount -= charm.skullCost;
        IDLookupTable.instance.charmsInPlayerStorage.Add(charm.CreateCharmSaveData());
        charm.gameObject.SetActive(false);
        Debug.Log(charmInShopTransform.name);
        charmInShopTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        playerSkullAmount.text = WorldManager.Instance.skullAmount.ToString();
        StartCoroutine(OnBuyThanks());
    }

    IEnumerator OnBuyThanks()
    {
        greetingText.text = "More <color=red>SKULLS</color> for the <color=red>SKULL</color> gondola! I am pleased";
        yield return new WaitForSeconds(3f);
        greetingText.text = "";
    }
    public void Continue()
    {
        gameObject.SetActive(false);
    }
}
