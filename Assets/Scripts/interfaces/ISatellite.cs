using UnityEngine;

namespace interfaces
{
    public interface ISatellite
    {
        public GameObject GameObject { get; }
        public IAttractor CurrentAttractor { get; set; }
        
        // call only from attractor script!
        public void Attach(IAttractor attractor);

        // call only from attractor script!
        public void Detach();

        public void UpdateOrbitalMovement();
    }
}