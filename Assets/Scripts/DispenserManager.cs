using UnityEngine;
using UnityEngine.UI;

public class DispenserManager : MonoBehaviour
{
    [SerializeField] private PanManager panManager; 

    // image fillers 
    [SerializeField] private Image egg_filler;
    [SerializeField] private Image milk_filler;
    [SerializeField] private Image flour_filler;

    // current amount of ingredients
    private int current_eggs = 0;
    private int current_milk = 0;
    private int current_flour = 0;

    private const int MAX_ITEMS = 5; // max items per type of ingredient

    // pan detection
    private bool panInside = false;
    private Coroutine dispensingCoroutine;

    private void Start()
    {
        // Reset filler amount
        if (egg_filler != null) egg_filler.fillAmount = 0.0f;
        if (milk_filler != null) milk_filler.fillAmount = 0.0f;
        if (flour_filler != null) flour_filler.fillAmount = 0.0f;
    }

    private void OnEnable()
    {
        // subscribe to events
        NFC_Simulator.OnEggEvent += AddEgg;
        NFC_Simulator.OnMilkEvent += AddMilk;
        NFC_Simulator.OnFlourEvent += AddFlour;
    }

    private void OnDisable()
    {
        // unsubscribe from events
        NFC_Simulator.OnEggEvent -= AddEgg;
        NFC_Simulator.OnMilkEvent -= AddMilk;
        NFC_Simulator.OnFlourEvent -= AddFlour;
    }

    private void AddEgg()
    {
        if (current_eggs < MAX_ITEMS)
        {
            current_eggs++;
            UpdateFiller(egg_filler, current_eggs);
        }
    }

    private void AddMilk()
    {
        if (current_milk < MAX_ITEMS)
        {
            current_milk++;
            UpdateFiller(milk_filler, current_milk);
        }
    }

    private void AddFlour()
    {
        if (current_flour < MAX_ITEMS)
        {
            current_flour++;
            UpdateFiller(flour_filler, current_flour);
        }
    }

    private void UpdateFiller(Image filler, int currentValue)
    {
        if (filler == null) return;

        filler.fillAmount = (float)currentValue / MAX_ITEMS;
    }

    // if an object enters the box collider
    // check tag of the object. if the tag is "Pan"
    // then if egg, milk and flour >= 1
    // remove 1 egg, 1 milk and 1 flour. 
    // start timer. 
    // every 2 secons, remove 1 egg, 1 milk and 1 flour if there are still enough ingredients
    // it is possible to call AddXXX() at any time while the timer is running. 
    // the moment the pan exits the collider, this behaviour stops. 

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Pan")) return;

        panInside = true;

        // dispense one unit of batter (1x egg, 1x milk, 1x flour)
        TryConsumeOneSet();

        // start corrouting to repeat every 2 seconds
        if (dispensingCoroutine == null)
        {
            dispensingCoroutine = StartCoroutine(DispenseRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Pan")) return;

        panInside = false;

        // stop the behaviour when the pan leaves
        if (dispensingCoroutine != null)
        {
            StopCoroutine(dispensingCoroutine);
            dispensingCoroutine = null;

            panManager.ReleasePancake();
        }
    }

    private System.Collections.IEnumerator DispenseRoutine()
    {
        while (panInside)
        {
            yield return new WaitForSeconds(2f);

            // every 2 seconds, try to consume ingredients if there are enough
            TryConsumeOneSet();
        }

        dispensingCoroutine = null;
    }

    private void TryConsumeOneSet()
    {
        // only consume if we have at least 1 of each and if the pan does not have the maximum amount of batter
        int current_pancake_size = 0;

        if(panManager.spawnedPancake != null)
            current_pancake_size = panManager.spawnedPancake.GetComponent<PancakeData>().batter_units;

        if (current_eggs > 0 && current_milk > 0 && current_flour > 0 && current_pancake_size < panManager.MAX_BATTER_UNITS)
        {
            current_eggs--;
            current_milk--;
            current_flour--;

            UpdateFiller(egg_filler, current_eggs);
            UpdateFiller(milk_filler, current_milk);
            UpdateFiller(flour_filler, current_flour);

            panManager.AddBatter();
        }
    }

}
