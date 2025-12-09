using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DeliveryPortalManager : MonoBehaviour
{
    public GameObject goodExplosion;
    [SerializeField] private ParticleSystem goodExplosionParticles;
    public GameObject badExplosion;
    [SerializeField] private ParticleSystem badExplosionParticles;

    public PanManager pan_manager;
    public OrderingSystem ordering_system;

    // audio files
    [SerializeField] private List<AudioClip> audioFiles; // first one is correct delivery, second is wrong delivery

    private AudioSource audioSource;

    private void Start()
    {
        goodExplosion = this.gameObject.transform.GetChild(1).gameObject;
        goodExplosionParticles = goodExplosion.GetComponent<ParticleSystem>();
        badExplosion = this.gameObject.transform.GetChild(0).gameObject;
        badExplosionParticles = badExplosion.GetComponent<ParticleSystem>();

        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pancake")) {

            // check if delivery is correct
            PancakeData this_pancake = other.transform.parent.gameObject.GetComponent<PancakeData>();
            bool isCorrect = CheckOrder(this_pancake);
           
            if (isCorrect)
            {
                goodExplosion.SetActive(true);
                goodExplosionParticles.Play();
                audioSource.PlayOneShot(audioFiles[0], 0.3f);
            } 
            else
            {
                badExplosion.SetActive(true);
                badExplosionParticles.Play();
                audioSource.PlayOneShot(audioFiles[1]);
            }

            // Instead of destroying here, call the pan manager to destroy
            pan_manager.DestroyPancake(other); // this function also resets the order to the next one
        }
    }

    private bool CheckOrder(PancakeData this_pancake)
    {
        bool isCorrect = false;

        if (this_pancake.state == 1) // normal cooking, otherwise, raw or burnt
        {
            Debug.Log("Pancake is cooked");

            // check current order:
            string pancake_type = "hola";
            List<string> toppings_types = null;

            if (ordering_system.queue_pancakes.Count > 0)
            {
                pancake_type = ordering_system.queue_pancakes.Peek();
                toppings_types = ordering_system.queue_toppings.Peek();
            }

            if (pancake_type != "hola" && toppings_types != null)
            {
                // check size of the pancake
                int order_size = 0;
                if (pancake_type == "Medium")
                    order_size = 1;
                else if (pancake_type == "Big")
                    order_size = 2;

                if(this_pancake.batter_units == order_size)
                {
                    // check toppings
                    bool equal_toppings = new HashSet<string>(toppings_types).SetEquals(this_pancake.list_toppings); // returns true if both sets have the same elements, order doesn�t matter.
               
                    if (equal_toppings)
                    {
                        isCorrect = true;
                    }
                    else
                    {
                        isCorrect = false;
                        Debug.Log("Pancake with incorrect toppings.");
                    }
                }
                else
                {
                    isCorrect = false;
                    Debug.Log("Pancake with incorrect size.");
                }

            }
            else
            {
                isCorrect = true; // pancake made without order
            }
        }
        else
        {
            isCorrect = false;
            Debug.Log("Pancake is raw or burnt");
        }

        return isCorrect;
    }
}
