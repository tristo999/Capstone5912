using Mirror;

public class DestroyOnTimeout : NetworkBehaviour
{
    public float lifespan;
    private double deathTime;

    public void Awake() {
        deathTime = NetworkTime.time + lifespan;
    }

    public void FixedUpdate() {
        if (!hasAuthority) return;
        if (NetworkTime.time > deathTime) {
            NetworkServer.Destroy(gameObject);
        }
    }
}
