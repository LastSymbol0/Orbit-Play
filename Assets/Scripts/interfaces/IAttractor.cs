using System.Collections.Generic;
using models;
using UnityEngine;

namespace interfaces
{
    public interface IAttractor
    {
        IList<ISatellite> Satellites { get; }

        public Ellipse GetOrbit();
        
        public GameObject GetGameObject();
    }
}