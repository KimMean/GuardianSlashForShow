using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProbabilityTuple : MonoBehaviour
{
    [SerializeField] Text itemName;
    [SerializeField] Text woodProbability;
    [SerializeField] Text silverProbability;
    [SerializeField] Text goldProbability;

    public void SetTuple(string name, string wood, string silver, string gold)
    {
        itemName.text = name;
        woodProbability.text = wood;
        silverProbability.text = silver;
        goldProbability.text = gold;
    }
}
