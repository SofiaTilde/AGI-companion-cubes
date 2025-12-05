using UnityEngine;
using System.Collections;

public class DeliveryPortalManager : MonoBehaviour
{
    public bool isCorrect = false;
    public GameObject goodExplosion;
    [SerializeField] private ParticleSystem goodExplosionParticles;
    public GameObject badExplosion;
    [SerializeField] private ParticleSystem badExplosionParticles;

    private void Start()
    {
        goodExplosion = this.gameObject.transform.GetChild(1).gameObject;
        goodExplosionParticles = goodExplosion.GetComponent<ParticleSystem>();
        badExplosion = this.gameObject.transform.GetChild(0).gameObject;
        badExplosionParticles = badExplosion.GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pancake")) {
            if (isCorrect)
            {
                goodExplosion.SetActive(true);
                goodExplosionParticles.Play();
                isCorrect = false;
            } else
            {
                badExplosion.SetActive(true);
                badExplosionParticles.Play();
                isCorrect = true;
            }
            Destroy(other.transform.parent.gameObject);
        }
    }
}
