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

    private FirestoreHandler fish;

    void Start()
    {
	    fish = GameObject.FindWithTag("dataManager")
	                     .GetComponent<FirestoreHandler>();
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

        // Instead of AnimatorController, play the animation using the state name.
        PetAnimation petAnim = currentPet.GetComponent<PetAnimation>();

        // Get the animation state for the current pet and the pet being considered for purchase
        string currentPetStateName = petAnim.animStateNames[PlayerPrefs.GetInt("Pet")];
        string newPetStateName = petAnim.animStateNames[PetIndex];

        // Play the current pet's animation in child(4) (where the current pet is shown)
        buyWindow.transform.GetChild(4).GetComponent<Animator>().Play(currentPetStateName);

        // Play the animation for the pet being considered in child(5)
        buyWindow.transform.GetChild(5).GetComponent<Animator>().Play(newPetStateName);

        // Show or hide the buy button depending on available dollars
        if (PlayerPrefs.GetInt("Dollars") >= PetCost)
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
        
        fish.UpdateStats(PlayerPrefs.GetString("Name"),"PetsBought", 1);
        fish.UpdateStats(PlayerPrefs.GetString("Name"),"MoneySpentOnPets", PetCost);

        // Instead of changing the AnimatorController, directly play the new animation state
        PetAnimation petAnim = currentPet.GetComponent<PetAnimation>();
        string petBuyingStateName = petAnim.animStateNames[PetIndex];
        currentPet.GetComponent<Animator>().Play(petBuyingStateName); // Play the pet's animation based on PetIndex

        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(true);

        setCurrentPetBought();

        buyWindow.SetActive(false);

        GameObject.FindWithTag("PanelHolder").GetComponent<MainScreenNavigation>().setDollarsText();
        PlayerPrefs.Save();
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
