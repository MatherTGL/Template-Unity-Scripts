using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Boot
{
    public sealed class Bootstrap : MonoBehaviour
    {
        public enum TypeLoadObject
        {
            SuperImportant,
            MediumImportant,
            SimpleImportant,
            UI
        }

        public enum TypeSingleOrLotsOf { Single, LotsOf }

        private List<IBoot> l_bootObject = new List<IBoot>();


        private void Awake() => LoadToList();

        private void Start()
        {
            for (ushort i = 0; i < l_bootObject.Count; i++)
                l_bootObject[i].InitStart();
        }

        private void LoadToList()
        {
            IBoot[] bootObjects = FindObjectsOfType<MonoBehaviour>()
                .OfType<IBoot>().Where(item => ((MonoBehaviour)item).enabled)
                .Distinct().ToArray();

            SortingObjectsList(ref bootObjects);
        }

        private void SortingObjectsList(ref IBoot[] bootObjects)
        {
            foreach (TypeLoadObject typeLoad in Enum.GetValues(typeof(TypeLoadObject)))
                foreach (TypeSingleOrLotsOf singleOrLotsOf in Enum.GetValues(typeof(TypeSingleOrLotsOf)))
                    LoadObjectsToList(typeLoad, singleOrLotsOf, bootObjects);

            Array.Clear(bootObjects, 0, bootObjects.Length);
            StartInitAwake();
        }

        private void LoadObjectsToList(TypeLoadObject typeLoad, TypeSingleOrLotsOf singleOrLotsOf, in IBoot[] bootObjects)
        {
            l_bootObject.AddRange(bootObjects.Where(item => item.GetTypeLoad().typeLoad.Equals(typeLoad) &&
                item.GetTypeLoad().singleOrLotsOf.Equals(singleOrLotsOf)));
        }

        private void StartInitAwake()
        {
            for (ushort i = 0; i < l_bootObject.Count; i++)
                l_bootObject[i].InitAwake();
        }
    }
}