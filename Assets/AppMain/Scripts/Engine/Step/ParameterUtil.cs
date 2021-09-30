using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FFTAI.X.Trial.Step
{
    public class ParameterUtil
    {
        private static TrialStepState m_StepState = TrialStepState.START_UP;
        // public static TrialStepState StepState = TrialStepState.START_UP;
        public static TrialStepState StepState
        {
            get
            {
                return m_StepState;
            }
        }
        public static string filePath;
        public static string taskName;
        public static int trialNum;

        //每次各task都是一个起点，终点数组
        //P2P与Fitts所使用的起点与终点数组
        public static List<float[]> startPointDic = new List<float[]>();
        public static List<float[]> endPointDic = new List<float[]>();
        //x,y轴阻力方向
        public static List<float[]> Resist = new List<float[]>();
        public static List<float[]> markList = new List<float[]>();
        public static void LoadTaskInfo()
        {
            if (trialNum != 0)
            {
                return;
            }
            m_StepState = TrialStepState.PENDING;

            //当前Block的名称信息
            taskName = "BlockP2P";
            Debug.Log("当前试次为:" + taskName);

            startPointDic = new List<float[]>();
            endPointDic = new List<float[]>();
            Resist = new List<float[]>();
            markList = new List<float[]>();

            List<float> buffer = new List<float>();
            switch (TrialCore.TrialMode)
            {
                default:
                    filePath = System.Environment.CurrentDirectory + $"\\ReachingExp\\Reach - X.csv";
                    break;
            }
            Debug.Log($"trial file path：{filePath}");
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);
            string str;
            bool IsFirst = true;
            while (sr.Peek() >= 0)
            {
                str = sr.ReadLine();

                if (IsFirst)
                {
                    IsFirst = false;
                    continue;
                }

                string[] strSplit = str.Split(',');

                float[] startPoint = new float[2];
                float[] endPoint = new float[2];

                //得到每一个试次的起点与终点信息
                for (int i = 0; i < 4; i++)
                {
                    if (i <= 1)
                        startPoint[i] = float.Parse(strSplit[i]);
                    else
                        endPoint[i % 2] = float.Parse(strSplit[i]);
                }



                switch (TrialCore.TrialMode)
                {
                    default:
                        // 4,5-marker  6,7,8,9,10,11-mkb
                        if (strSplit.Length >= 6)
                        {
                            markList.Add(new float[] { float.Parse(strSplit[4]), float.Parse(strSplit[5]) });
                            // Resist.Add(new float[] { 0.2f, 0.2f, float.Parse(strSplit[6]), float.Parse(strSplit[7]), float.Parse(strSplit[8]), float.Parse(strSplit[9]), float.Parse(strSplit[10]), float.Parse(strSplit[11]) });
                            Resist.Add(new float[] { float.Parse(strSplit[6]), float.Parse(strSplit[7]), float.Parse(strSplit[8]), float.Parse(strSplit[9]) });
                        }
                        break;
                }

                startPointDic.Add(startPoint);
                endPointDic.Add(endPoint);
            }
            sr.Close();
            // Trial information
            trialNum = Resist.Count;
            Debug.Log($"trialNum: {trialNum}");
            m_StepState = TrialStepState.DONE;
        }
        public static void Reset()
        {
            m_StepState = TrialStepState.START_UP;
            taskName = "";
            trialNum = 0;
            startPointDic.Clear();
            endPointDic.Clear();
            Resist.Clear();
        }
    }
}