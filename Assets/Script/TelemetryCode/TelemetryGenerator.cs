using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace CardProject
{
    public class TelemetryGenerator : MonoBehaviour
    {
        public static TelemetryGenerator instance;

        [SerializeField] private CardManager _cardManager;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private GameManager _gameManager;
        
        private int runCount;
        private string columns = "GameTime,MeanFPS,MedianFPS,WorstFPS,CurrentTrick,PlayerScore,Bot2Score,Bot3Score,Bot4Score,MenuOpenCount,PlaySpeedCount,HandNumberCount,HandSizeCount\n";

        private System.DateTime STARTTIME;
        private System.TimeSpan ELAPSEDTIME;
        private double ANCHORTIME;
        private double SETTIME;
        private double FPSTIME;
        private double LOGTIME = 10; // 1 set = 10 seconds

        public float realTimeMEAN;
        public float realTimeMEDIAN;
        public float realTimeWorst;
        
        public int[] frameCountEachSecond = new int[20]; //log every 10 seconds
        private int frameCounter = 0;
        private int frameSumForMean = 0;
        private int tenFrameIndexTail = -1;
        private int tenFrameIndexStart = 0;
        private bool once = false;

        private static string path = "";
        
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            instance = this;
            
        }

        private void Start()
        {
            realTimeWorst = int.MaxValue;
            if (!PlayerPrefs.HasKey("testCount"))
            {
                PlayerPrefs.SetInt("testCount", 0);
            }
            runCount = PlayerPrefs.GetInt("testCount");
            InitCSV();
            ANCHORTIME = 0;

            _cardManager.trickEnded += Log;
        }

        private void Update()
        {
            if (!once)
            {
                STARTTIME = System.DateTime.Now;
                once = true;
            }

            ELAPSEDTIME = System.DateTime.Now - STARTTIME;
            FPSTIME = ELAPSEDTIME.TotalSeconds - ANCHORTIME;

            if (FPSTIME >= 1)
            {
                ANCHORTIME = ELAPSEDTIME.TotalSeconds;

                //frame data
                frameCountEachSecond[tenFrameIndexStart%20] = frameCounter;

                if (tenFrameIndexStart > 9)
                {
                    frameSumForMean += frameCounter;
                    if (tenFrameIndexStart == 10)
                    {
                        frameSumForMean -= frameCountEachSecond[0];
                    }
                    else
                    {
                        frameSumForMean -= frameCountEachSecond[tenFrameIndexTail%20];
                    }
                }
                else
                {
                    frameSumForMean += frameCounter;
                }

                if (tenFrameIndexStart%10 == 0)
                {
                    realTimeWorst = int.MaxValue;
                }
                
                realTimeMEAN = frameSumForMean / 10.0f;
                int half = (tenFrameIndexStart - tenFrameIndexTail) / 2;
                realTimeMEDIAN = frameCountEachSecond[(tenFrameIndexStart - half)%20]; //HOLY MACDONALD

                for (int i = tenFrameIndexTail + 1; i <= tenFrameIndexStart; i++) //o oh, performance??
                {
                    if (realTimeWorst > frameCountEachSecond[(i) % 20])
                    {
                        realTimeWorst = frameCountEachSecond[(i) % 20];
                    }
                }
                
                //frame index handling
                tenFrameIndexStart++;
                if (tenFrameIndexStart > 10)
                {
                    if (tenFrameIndexStart == 11)
                    {
                        tenFrameIndexTail = 0;
                    }
                    tenFrameIndexTail++;
                }
                
                frameCounter = 0;
            }

            frameCounter++;
        }

        public void InitCSV()
        {
            string fileName = "test_" + runCount + ".csv";
            path = Path.Combine(Application.dataPath, fileName);

            if (!File.Exists(path))
            {
                File.WriteAllText(path, columns);
            }
        }

        public void Log()
        {
            LogEvent(ELAPSEDTIME.TotalSeconds, realTimeMEAN, realTimeMEDIAN, realTimeWorst,
                _gameManager.currentTrick, _gameManager.GetPlayerScore(1), _gameManager.GetPlayerScore(2), _gameManager.GetPlayerScore(3), _gameManager.GetPlayerScore(4),
                _gameManager.escCount, _uiManager.playSpeedClickCount, _uiManager.handNumberClickCount, _uiManager.HandSizeClickCount);
        }

        public static void LogEvent(double gametime, float mean, double median, double worst,
            int trickCount, int playerScore, int bot2Score, int bot3Score, int bot4Score,
            int menuOpenCount, float playSpeedCount, int handNumberCount, int handSizeCount)
        {
            string line = $"{gametime},{mean}, {median}, {worst}," +
                          $"{trickCount},{playerScore},{bot2Score},{bot3Score},{bot4Score}," +
                          $"{menuOpenCount},{playSpeedCount},{handNumberCount},{handSizeCount}\n";
            
            File.AppendAllText(path, line + "\n");
        }
        
        public void SaveBeforeExitGame()
        {
            PlayerPrefs.SetInt("testCount", runCount++);
            //GenerateCSV();
        }
    }
}
