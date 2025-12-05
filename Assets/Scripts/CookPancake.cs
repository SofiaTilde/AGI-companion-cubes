using UnityEngine;

public class CookPancake : MonoBehaviour
{
    [Header("Cooking Times (seconds)")]
    public float timeToCook = 4f;   
    public float timeToBurn = 8f;

    public PanManager pan;
    public OrderingSystem ordering_system;

    private bool pancakeInside = false;
    private float cookTimer = 0f;

    private bool triggered_burnt = false;
    private bool triggered_step2 = false;
    private bool triggered_ps_cooking = false;

    private GameObject current_pancake; // TODO: EL COMPONENT PARA COOK ESTÁ EN EL PARENT DEL PANCAKE, NO EN EL COLLIDER DEL PANCAKE!

    private void Update()
    {
        if (!pancakeInside)
        {
            pan.Stop_PS_cooking();
            return;
        }

        cookTimer += Time.deltaTime;

        // raw -> cooked -> burnt over time
        if (cookTimer >= timeToBurn)
        {
            if (!triggered_burnt)
            {
                triggered_burnt = true;

                pan.Trigger_PS_overcooked();
                current_pancake.GetComponent<PancakeData>().Cook();
            }    
        }
        else if (cookTimer >= timeToCook)
        {
            if (!triggered_step2)
            {
                triggered_step2 = true;

                ordering_system.Start_Step2();
                current_pancake.GetComponent<PancakeData>().Cook();
            }
        }
        else
        {
            if (!triggered_ps_cooking)
            {
                triggered_ps_cooking = true;

                pan.Trigger_PS_cooking();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Pancake"))
            return;

        triggered_burnt = false;
        triggered_step2 = false;
        triggered_ps_cooking = false;

        pancakeInside = true;
        cookTimer = 0f;

        current_pancake = other.transform.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Pancake"))
            return;

        pancakeInside = false;
        cookTimer = 0f;

        current_pancake = null;
    }

}
