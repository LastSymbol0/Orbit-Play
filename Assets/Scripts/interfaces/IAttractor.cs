using System.Collections.Generic;
using models;
using UnityEngine;

namespace interfaces
{
    public interface IAttractor
    {
        IEnumerable<ISatellite> Satellites { get; set; }

        public Ellipse GetOrbit();
        
        public GameObject GetGameObject();
    }
}