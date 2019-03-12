public class DestroyOnTimeout : Bolt.EntityBehaviour<IProjectileState>
{
    public float lifespan;
    private int deathFrame;

    public override void Attached() {
        deathFrame = (int)(BoltNetwork.Frame + lifespan * BoltNetwork.FramesPerSecond);
    }

    public override void SimulateOwner() {
        if (entity.isAttached && BoltNetwork.Frame > deathFrame) {
            BoltNetwork.Destroy(entity);
        }
    }
}
