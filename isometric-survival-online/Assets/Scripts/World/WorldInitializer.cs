using UnityEngine;
using Zenject;

namespace World
{
    public class WorldInitializer : IInitializable
    {
        public void Initialize()
        {
            Debug.Log("Hello world!");
        }
    }
}