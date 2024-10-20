//#define YG

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

#if YG
using YG;
#endif

namespace GameAssets.General
{
    public static class SaveSystem
    {
        private static Dictionary<string, string> GameDataSaves = new Dictionary<string, string>();
        private static Dictionary<string, object> OtherDataSave; //для простых типов
        private static bool IsDataLoaded;
        public static event Action ForceReload;
        private static bool pendingReload;

        public static void PendingForceReload()
        {
            pendingReload = true;
            LoadAllData();
            ForceReload.Invoke();
        }

        public static void SaveGameData<T>(string key, T data)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning($"Key is empty");
                return;
            }

            LoadAllData();
            var gameDataSave = JsonConvert.SerializeObject(data);
            Debug.Log($"Saved key: {key} Data: {gameDataSave}");

            if (GameDataSaves.ContainsKey(key))
                GameDataSaves[key] = gameDataSave;
            else
                GameDataSaves.Add(key, gameDataSave);

            SaveAllData();
        }

        public static void SaveOtherData(string key, object data)
        {
            LoadAllData();
            if (OtherDataSave.ContainsKey(key))
            {
                OtherDataSave[key] = data;
            }
            else
            {
                OtherDataSave.Add(key, data);
            }

            SaveAllData();
        }

        public static T LoadGameData<T>(string key)
        {
            LoadAllData();

            if (GameDataSaves.TryGetValue(key, out var save))
            {
                //bug При десереализации создается новый экземпляр объекта, юнити ругается на это, так как SO
                return JsonConvert.DeserializeObject<T>(save);
            }
            else
            {
                throw new UnityException("Save key is don't exist");
            }
        }

        public static object LoadOtherData(string key)
        {
            LoadAllData();

            if (OtherDataSave.TryGetValue(key, out var data))
            {
                return data;
            }
            else
            {
                throw new UnityException("Save key is don't exist");
            }
        }

        public static bool TryLoadOtherData<T>(string key, out T @object)
        {
            LoadAllData();

            if (OtherDataSave.TryGetValue(key, out var data))
            {
                JObject jObj = (JObject)data;
                @object = JsonConvert.DeserializeObject<T>(jObj.ToString());
                return true;
            }
            else
            {
                @object = default;
                Debug.Log(key + " does not exist");
                return false;
            }
        }

        public static void DeleteGameData(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning($"Key is empty");
                return;
            }

            LoadAllData();
            if (GameDataSaves.ContainsKey(key))
            {
                GameDataSaves.Remove(key);
            }

            SaveAllData();
        }

        public static void DeleteAllData()
        {
            GameDataSaves = new Dictionary<string, string>();
            OtherDataSave = new Dictionary<string, object>();
            SaveAllData();
            PendingForceReload();
        }

        public static void DeleteOtherData(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning($"Key is empty");
                return;
            }

            LoadAllData();
            if (OtherDataSave.ContainsKey(key))
            {
                OtherDataSave.Remove(key);
            }

            SaveAllData();
        }

        public static bool HasGameData(string key)
        {
            LoadAllData();

            return GameDataSaves.ContainsKey(key);
        }

        public static bool HasOtherData(string key)
        {
            LoadAllData();

            return OtherDataSave.ContainsKey(key);
        }

#if YG
        private static (string, string) LoadDataYG()
        {
            string gameDataSaves = YandexGame.savesData.GameData;
            string otherDataSaves = YandexGame.savesData.OtherData;
            return (gameDataSaves, otherDataSaves);
        }
        
        private static void SaveYG(string gameDataSaves, string otherDataSaves)
        {
            YandexGame.savesData.GameData = gameDataSaves;
            YandexGame.savesData.OtherData = otherDataSaves;

            YandexGame.SaveProgress();
        }
#endif


        private static (string, string) LoadDataPrefs()
        {
            string gameDataSaves = PlayerPrefs.GetString("gameData");
            string otherDataSaves = PlayerPrefs.GetString("otherData");
            return (gameDataSaves, otherDataSaves);
        }

        private static void SavePrefs(string gameDataSaves, string otherDataSaves)
        {
            PlayerPrefs.SetString("gameData", gameDataSaves);
            PlayerPrefs.SetString("otherData", otherDataSaves);
        }

        private static void LoadAllData()
        {
            bool needReload = !IsDataLoaded || pendingReload;
            if (!needReload) return;

            (string gameDataSaves, string otherDataSaves) saveData = (null, null);
#if YG
            saveData = LoadDataYG();
#else
            saveData = LoadDataPrefs();
#endif
            GameDataSaves = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(saveData.gameDataSaves))
                GameDataSaves = JsonConvert.DeserializeObject<Dictionary<string, string>>(saveData.gameDataSaves);

            OtherDataSave = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(saveData.otherDataSaves))
                OtherDataSave = JsonConvert.DeserializeObject<Dictionary<string, object>>(saveData.otherDataSaves);
            IsDataLoaded = true;
            pendingReload = false;
        }

        private static void SaveAllData()
        {
            string gameDataSaves = JsonConvert.SerializeObject(GameDataSaves);
            string otherDataSaves = JsonConvert.SerializeObject(OtherDataSave);
#if YG
            SaveYG(gameDataSaves, otherDataSaves);
#else
            SavePrefs(gameDataSaves, otherDataSaves);
#endif
        }
    }
}