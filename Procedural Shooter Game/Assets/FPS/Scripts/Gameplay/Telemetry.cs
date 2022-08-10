using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.FPS.Gameplay
{
    public class Telemetry : MonoBehaviour
    {
        public struct DungeonData
        {
            public bool useDDA;
            public int difficultyIDX;
            public StaticDifficultyType selectedStaticDifficulty;
            public float timeSpentInEasyDifficulty;
            public float timeSpentInEasyToMediumDifficulty;
            public float timeSpentInMediumToHardDifficulty;
            public float timeSpentInHardDifficulty;
            public DynamicDifficultyType bestAvaragePlayerPerformance;
            public int killedEnemyAmount;
            public int healthPickedAmount;
            public bool won;
            public float timeSpentInRun;
        }
        
        /*Telemetry form link*/ //Remember to remove the last part of the link *viewform...*
        private const string GoogleFormBaseUrl = "https://docs.google.com/forms/d/e/1FAIpQLScRZOVVlQszOiR1IASeqg3cLMemklxhipFJcWMv4XfJ2Uo0AA/";
        
        /*Telemtry form variables*/

        private const string _gform_id_player = "entry.1125646356";
        private const string _gform_id_run = "entry.1988362384";
        private const string _gform_useDDA = "entry.846266678";
        private const string _gform_difficultyIDX = "entry.1347731439";
        private const string _gform_selectedStaticDifficulty = "entry.919782967";
        private const string _gform_timeSpentInEasyDifficulty = "entry.1513595924";
        private const string _gform_timeSpentInEasyToMediumDifficulty = "entry.1682369591";
        private const string _gform_timeSpentInMediumToHardDifficulty = "entry.1162921065";
        private const string _gform_timeSpentInHardDifficulty = "entry.668470028";
        private const string _gform_bestAvaragePlayerPerformance = "entry.426969479";
        private const string _gform_killedEnemyAmount = "entry.80272591";
        private const string _gform_healthPickedAmount = "entry.986572018";
        private const string _gform_won = "entry.1176019219";
        private const string _gform_timeSpentInRun = "entry.1306073109";
        public static Guid playerID = Guid.NewGuid();
        public static Guid runID;
        
        public static IEnumerator SubmitGoogleForm(DungeonData dungeonData)
        {
            //These lines make sure that you are not going to have any comma/dot problems. 
            //But you also need to make sure that your spreadsheet settings are UK as well.
            CultureInfo ci = CultureInfo.GetCultureInfo("en-GB");
            Thread.CurrentThread.CurrentCulture = ci;

            string urlGoogleFormResponse = GoogleFormBaseUrl + "formResponse";

            WWWForm form = new WWWForm();

            form.AddField(_gform_id_player, GUIDToShortString(playerID));
            form.AddField(_gform_id_run, GUIDToShortString(runID));
            form.AddField(_gform_useDDA, dungeonData.useDDA.ToString());
            form.AddField(_gform_difficultyIDX, dungeonData.difficultyIDX);
            form.AddField(_gform_selectedStaticDifficulty, dungeonData.selectedStaticDifficulty.ToString());
            form.AddField(_gform_timeSpentInEasyDifficulty, dungeonData.timeSpentInEasyDifficulty.ToString("F2"));
            form.AddField(_gform_timeSpentInEasyToMediumDifficulty, dungeonData.timeSpentInEasyToMediumDifficulty.ToString("F2"));
            form.AddField(_gform_timeSpentInMediumToHardDifficulty, dungeonData.timeSpentInMediumToHardDifficulty.ToString("F2"));
            form.AddField(_gform_timeSpentInHardDifficulty, dungeonData.timeSpentInHardDifficulty.ToString("F2"));
            form.AddField(_gform_bestAvaragePlayerPerformance, dungeonData.bestAvaragePlayerPerformance.ToString());
            form.AddField(_gform_killedEnemyAmount, dungeonData.killedEnemyAmount);
            form.AddField(_gform_healthPickedAmount, dungeonData.healthPickedAmount);
            form.AddField(_gform_won, dungeonData.won.ToString());
            form.AddField(_gform_timeSpentInRun, dungeonData.timeSpentInRun.ToString("F2"));

            using (UnityWebRequest www = UnityWebRequest.Post(urlGoogleFormResponse, form))
            {
                yield return www.SendWebRequest(); //Decomment this line to send the data!

                print(www.result != UnityWebRequest.Result.Success ? www.error : "Request sent");

                //You can keep these 2 lines just to be sure that everything is working and there are no errors :)
                yield return null;
          
            }
        }
        
        public static void GenerateNewRunID()
        {
            runID = Guid.NewGuid();
            PlayerPrefs.SetString("runID", runID.ToString());
        }
    
        public static void GenerateNewPlayerID()
        {
            
            playerID = Guid.NewGuid();
            PlayerPrefs.SetString("playerID", playerID.ToString());
        }
        
        public static string GUIDToShortString(Guid guid)
        {
            var base64Guid = Convert.ToBase64String(guid.ToByteArray());

            // Replace URL unfriendly characters with better ones
            base64Guid = base64Guid.Replace('+', '-').Replace('/', '_');

            // Remove the trailing ==
            return base64Guid.Substring(0, base64Guid.Length - 2);
        }
        
    }
}
