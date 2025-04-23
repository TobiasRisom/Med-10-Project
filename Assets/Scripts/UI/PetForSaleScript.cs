using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PetForSaleScript : MonoBehaviour
{
	public int PetIndex;
	public int PetCost;
	public GameObject currentPet;

	[SerializeField]
	private GameObject buyWindow;
    void Start()
    {
	    buyWindow = GameObject.FindWithTag("PetShopPanel")
	                          .transform.GetChild(12).gameObject;

	    currentPet = GameObject.FindWithTag("Pet");
    }

    public void BuyWindow()
    {
	    buyWindow.SetActive(true);

	    buyWindow.transform.GetChild(1)
	             .GetComponent<TextMeshProUGUI>()
	             .text = "Vil du have at " + PlayerPrefs.GetString("PetName") + " skal udvikle sig?";
	    
	    buyWindow.transform.GetChild(2)
	             .GetComponent<TextMeshProUGUI>()
	             .text = "(Koster: " + PetCost + " point)";
	    
	    buyWindow.transform.GetChild(4)
	             .GetComponent<Animator>()
	             .runtimeAnimatorController = currentPet.GetComponent<PetAnimation>()
	                                                    .animCon[PlayerPrefs.GetInt("Pet")];

	    buyWindow.transform.GetChild(5)
	             .GetComponent<Animator>()
	             .runtimeAnimatorController = currentPet.GetComponent<PetAnimation>()
	                                                    .animCon[PetIndex];
	    if (PlayerPrefs.GetInt("Dollars") > PetCost)
	    {
		    buyWindow.transform.GetChild(6)
		             .gameObject.SetActive(true);
		    buyWindow.transform.GetChild(7)
		             .gameObject.SetActive(false);

		    buyWindow.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(buyPet);
	    }
	    else
	    {
		    buyWindow.transform.GetChild(6)
		             .gameObject.SetActive(false);
		    buyWindow.transform.GetChild(7)
		             .gameObject.SetActive(true);
	    }
	    buyWindow.transform.GetChild(8).GetComponent<Button>().onClick.AddListener(close);
    }
    
    private void removeListeners()
    {
	    buyWindow.transform.GetChild(6)
	             .GetComponent<Button>()
	             .onClick.RemoveAllListeners();
	    buyWindow.transform.GetChild(8).GetComponent<Button>().onClick.RemoveAllListeners();
    }
    
    public void close()
    {
	    removeListeners();
	    buyWindow.SetActive(false);
    }

    public void buyPet()
    {
	    removeListeners();
	    PlayerPrefs.SetInt("Pet", PetIndex);

	    int purchase = PlayerPrefs.GetInt("Dollars");
	    PlayerPrefs.SetInt("Dollars", purchase - PetCost);
	    
	    currentPet.GetComponent<Animator>()
	              .runtimeAnimatorController = currentPet.GetComponent<PetAnimation>().animCon[PlayerPrefs.GetInt("Pet")];
	    
	    transform.GetChild(0).gameObject.SetActive(false);
	    transform.GetChild(1).gameObject.SetActive(false);
	    transform.GetChild(2).gameObject.SetActive(false);
	    transform.GetChild(3).gameObject.SetActive(true);
	    
	    setCurrentPetBought();
	    
	    buyWindow.SetActive(false);
	    
	    GameObject.FindWithTag("PanelHolder").GetComponent<MainScreenNavigation>().setDollarsText();
    }

    private void setCurrentPetBought()
    {
	    GameObject[] petsForSale = GameObject.FindGameObjectsWithTag("PetForSale");

	    foreach (GameObject pet in petsForSale)
	    {
		    if (pet.GetComponent<PetForSaleScript>()
		           .PetIndex == PlayerPrefs.GetInt("Pet"))
		    {
			    pet.transform.GetChild(0).gameObject.SetActive(false);
			    pet.transform.GetChild(1).gameObject.SetActive(false);
			    pet.transform.GetChild(2).gameObject.SetActive(false);
			    pet.transform.GetChild(3).gameObject.SetActive(true);
		    }
		    else
		    {
			    pet.transform.GetChild(0).gameObject.SetActive(true);
			    pet.transform.GetChild(1).gameObject.SetActive(true);
			    pet.transform.GetChild(2).gameObject.SetActive(true);
			    pet.transform.GetChild(3).gameObject.SetActive(false); 
		    }
	    }
    }
}
