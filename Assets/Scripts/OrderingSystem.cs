using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderingSystem : MonoBehaviour
{
    // step 1
    public Image step1_background;
    public TextMeshProUGUI step1_text;
    public GameObject pancake_parent;

    // step 2
    public Image step2_background;
    public TextMeshProUGUI step2_text;
    public GameObject toppings_parent;

    // title
    public TextMeshProUGUI title_text;
    public TextMeshProUGUI orders_remaining_text;

    public Material transparent_mat;
    public Material cooked_mat;

    // private variables
    // pancakes: Big, Medium, Small
    public Queue<string> queue_pancakes = new Queue<string>();
    // toppings: Blueberries, Chocolate, Rose
    public Queue<List<string>> queue_toppings = new Queue<List<string>>(); // this is list of lists, like [blueberries],[blueberries, chocolate]. Each batch of toppings is one order
    private int current_step = 0;

    [Range(0, 100)]
    private int transparency = 30; // percentage of transparency

    private AudioSource audioSource;

    private void Start()
    {
        Start_Step1();
        audioSource = GetComponent<AudioSource>();
    }

    public void Start_Step1()
    {
        if (queue_pancakes.Count == 0)
        {
            // there are no orders
            current_step = 0;

            Reset_Step1();
            Reset_Step2();

            title_text.text = "No orders";
            orders_remaining_text.text = queue_pancakes.Count.ToString() + " remaining";
        }
        else
        {
            // there is at least one order waiting
            Reset_Step2();
            current_step = 1;
            title_text.text = "Current order";

            // set transparency of step1_background to max
            if (step1_background != null)
                SetImageAlpha(step1_background, 1f);

            // set transparency of step1_text to max
            if (step1_text != null)
                SetTextAlpha(step1_text, 1f);

            // check first string in queue_pancakes
            string pancakeName = queue_pancakes.Peek();

            // find child of pancake_parent whose name is the same as the first string in queue_pancakes
            // for that child, toggle on the component WiggleObject
            if (pancake_parent != null)
            {
                foreach (Transform child in pancake_parent.transform)
                {
                    bool isTarget = child.name == pancakeName;

                    if (isTarget)
                    {
                        Renderer rend = GetFirstChildRenderer(child);
                        if (rend != null && cooked_mat != null)
                        {
                            rend.material = cooked_mat;
                        }

                        WiggleObject rot = child.GetComponent<WiggleObject>();
                        if (rot != null)
                        {
                            rot.enabled = true; // toggle on
                        }
                    }
                }
            }
        }
    }

    public void Start_Step2()
    { // this function is called by the fire when the pancake starts cooking
        if (queue_toppings.Count == 0)
        {
            // there are no orders
            current_step = 0;

            Reset_Step1();
            Reset_Step2();
        }
        else
        {
            // there is one order waiting
            Reset_Step1();
            current_step = 2;
            title_text.text = "Current order";

            // set transparency of step2_background to max
            if (step2_background != null)
                SetImageAlpha(step2_background, 1f);

            // set transparency of step2_text to max
            if (step2_text != null)
                SetTextAlpha(step2_text, 1f);

            // check first batch of toppings in the queue
            List<string> toppingsBatch = queue_toppings.Peek();

            // set active all the children of toppings_parent whose name is contained in the first batch of toppings in the queue
            if (toppings_parent != null && toppingsBatch != null)
            {
                foreach (Transform child in toppings_parent.transform)
                {
                    bool shouldBeActive = toppingsBatch.Contains(child.name);
                    child.gameObject.SetActive(shouldBeActive);
                }
            }
        }

    }

    public void CurrentOrderFinished() // this function is called by the deleter to continue to the next order
    {
        // remove the first pancake and the first topping batch in pancake queue and toppings queue. 
        if (queue_pancakes.Count > 0)
            queue_pancakes.Dequeue();

        if (queue_toppings.Count > 0)
            queue_toppings.Dequeue();

        // then call Start_Step1()
        orders_remaining_text.text = queue_pancakes.Count.ToString() + " remaining";
        Reset_Step1();
        Reset_Step2();
        Start_Step1();
    }

    public void AddOrder(string pancake_type, List<string> toppings_list)
    {
        // play sound
        audioSource.Play(); 

        // add pancake type to queue pancakes
        queue_pancakes.Enqueue(pancake_type);

        // add the batch of toppings to the queue_toppings
        if (toppings_list == null)
            toppings_list = new List<string>();

        // store a copy to avoid external modification
        queue_toppings.Enqueue(new List<string>(toppings_list));

        orders_remaining_text.text = queue_pancakes.Count.ToString() + " remaining";

        if (current_step == 0)
            Start_Step1();
    }

    private void Reset_Step1()
    {
        // set transparency of step1_background to transparency
        if (step1_background != null)
            SetImageAlpha(step1_background, TransparencyToAlpha());

        // set transparency of step1_text to transparency
        if (step1_text != null)
            SetTextAlpha(step1_text, TransparencyToAlpha());

        // set the material of all children in pancake_parent to transparent_mat. The material is in child -> first child
        // in each child, toggle off the component WiggleObject
        // set all children in pancake_parent to active false
        if (pancake_parent != null)
        {
            foreach (Transform child in pancake_parent.transform)
            {
                // set material in child -> first child
                Renderer rend = GetFirstChildRenderer(child);
                if (rend != null && transparent_mat != null)
                {
                    rend.material = transparent_mat;
                }

                // toggle off WiggleObject
                WiggleObject rot = child.GetComponent<WiggleObject>();
                if (rot != null)
                {
                    rot.enabled = false;
                }
            }
        }
    }

    private void Reset_Step2()
    {
        // set transparency step2_background to transparency
        if (step2_background != null)
            SetImageAlpha(step2_background, TransparencyToAlpha());

        // set transparency step2_text to transparency
        if (step2_text != null)
            SetTextAlpha(step2_text, TransparencyToAlpha());

        // set all children in toppings_parent to active false
        if (toppings_parent != null)
        {
            foreach (Transform child in toppings_parent.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    // --- Helper methods ---

    private float TransparencyToAlpha()
    {
        return Mathf.Clamp01(transparency / 100f);
    }

    private void SetImageAlpha(Image img, float alpha)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = Mathf.Clamp01(alpha);
        img.color = c;
    }

    private void SetTextAlpha(TextMeshProUGUI tmp, float alpha)
    {
        if (tmp == null) return;
        Color c = tmp.color;
        c.a = Mathf.Clamp01(alpha);
        tmp.color = c;
    }

    private Renderer GetFirstChildRenderer(Transform parent)
    {
        if (parent == null || parent.childCount == 0)
            return null;

        Transform firstChild = parent.GetChild(0);
        return firstChild.GetComponent<Renderer>();
    }
}
