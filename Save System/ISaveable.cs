using System.Threading.Tasks;
using GameAssets.General;
using Newtonsoft.Json;
using UnityEngine;

namespace GameAssets.Meta.Items.Interfaces
{
    public interface ISaveable<in T> : IPreloadable
    {
        [JsonIgnore] public string SaveId { get; }
        [JsonIgnore] public bool Loaded { get; set; }

        public void Save()
            => SaveSystem.SaveGameData(SaveId, this);

        public async Task AsyncLoad()
        {
            if (string.IsNullOrEmpty(SaveId))
            {
                Debug.LogError("SaveId is empty");
                return;
            }
            if (Loaded) return;

            SaveSystem.ForceReload += ForceReload;
            if (!SaveSystem.HasGameData(SaveId))
            {
                await OnFirstTimeLoad();
                Loaded = true;
                return;
            }

            T loadedItem = SaveSystem.LoadGameData<T>(SaveId);
            OnLoad(loadedItem);
            Loaded = true;
        }

        public void ForceReload()
        {
            SaveSystem.ForceReload -= ForceReload;
            Loaded = false;
            AsyncLoad();
        }

        public void Delete()
            => SaveSystem.DeleteGameData(SaveId);

        void OnLoad(T loadedItem);
        Task OnFirstTimeLoad();
    }
}