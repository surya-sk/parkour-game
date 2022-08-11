using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    public class AddRuntimeGameObjects : MonoBehaviour
    {
        public RuntimeGameObjects Collection;

        private void OnEnable() => Collection?.Item_Add(gameObject);

        private void OnDisable() => Collection?.Item_Remove(gameObject);
    }
}