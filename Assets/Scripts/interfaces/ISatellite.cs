namespace interfaces
{
    public interface ISatellite
    {
        public IAttractor CurrentAttractor { get; set; }
        
        public void Attach(IAttractor attractor);

        public void Detach();

        public void UpdateOrbitalMovement();
    }
}