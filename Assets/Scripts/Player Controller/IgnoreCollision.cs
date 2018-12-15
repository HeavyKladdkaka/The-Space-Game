using UnityEngine;
using System.Collections;

public class IgnoreCollision : MonoBehaviour
{
    public Transform transformToBeIgnored;

    void Start()
    {
        Physics.IgnoreCollision(transformToBeIgnored.GetComponent<Collider>(), GetComponent<Collider>());
    }

    void OnCollisionEnter()
    {
        Application.Quit();
    }
}