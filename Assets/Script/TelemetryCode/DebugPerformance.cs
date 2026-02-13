using System;
using TMPro;
using UnityEngine;

namespace CardProject
{
    public class DebugPerformance : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _meshProUGUI;

        private void Update()
        {
            string s = "";
            s += "PERFORMANCE\n last10seconds\n";
            s += "MeanFPS : " + TelemetryGenerator.instance.realTimeMEAN + "\n";
            s += "MedianFPS : " + TelemetryGenerator.instance.realTimeMEDIAN + "\n";
            s += "WorstFPS : " + TelemetryGenerator.instance.realTimeWorst + "\n";
            
            _meshProUGUI.text = s;
        }
    }
}
