using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;


public class GoalNetTest : MonoBehaviour
{
    public GoalNet goalNet;
    GameObject Top;
    int numOfObjects;
    int numOfSamples;

    void Start()
    {
        Top = GameObject.Find("Top");
        numOfObjects = Top.transform.childCount;
        numOfSamples = 5;
    }

    public void Update()
    {
        StartCoroutine(Inferance());
    }

    private IEnumerator Inferance()
    {
        for (int i = 0; i < numOfObjects; i++)
        {
            GameObject currGameObject = Top.transform.GetChild(i).gameObject;
            Interaction interaction = currGameObject.GetComponent<Interaction>();
            if (interaction != null)
            {
                for (int j = 0; j < numOfSamples; j++)
                { goalNet.PredictGoal(interaction, null); }
                //Debug.Log("Contacts Added");
               
            }
        }
        yield return new WaitForSeconds(5.0f);

        for (int i = 0; i < numOfObjects; i++)
        {
            GameObject currGameObject = Top.transform.GetChild(i).gameObject;
            Interaction interaction = currGameObject.GetComponent<Interaction>();
            if (interaction != null)
            {
                for (int j = 0; j < numOfSamples; j++)
                { interaction.RemoveContact(); }
                //Debug.Log("Contacts Removed");
            }
        }
        yield return new WaitForSeconds(0.0f);
    }

   



}
