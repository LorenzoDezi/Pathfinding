using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GraphTextComponent : MonoBehaviour
{
    private TextMeshProUGUI text;
    [SerializeField]
    private GraphGenerator graphGen;
    private Dictionary<GraphGenState, string> stateTextDict = new Dictionary<GraphGenState, string>();

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        graphGen.StateChangedEvent.AddListener((state) => text.SetText(stateTextDict[state]));
        stateTextDict.Add(GraphGenState.StartChosen, "Select the end node!");
        stateTextDict.Add(GraphGenState.EndChosen, "");       
    }
}
