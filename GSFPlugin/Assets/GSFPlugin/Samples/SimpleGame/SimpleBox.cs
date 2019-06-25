using GameSystem.GameCore;
using GameSystem.GameCore.Components;
using GameSystem.GameCore.Network;
using GameSystem.GameCore.SerializableMath;

[Packable(0)]
public class SimpleBox : Component
{
    private static int water_id;
    [PacketMember(0)]
    public int id { get; private set; }
    public Vector3 velocity;
    [PacketMember(1)]
    public Vector3 pos;
    public BoxCollider collider;

    public override void Start()
    {
        id = water_id++;
        pos = transform.position;
        collider = GetComponent<BoxCollider>();
        collider.OnCollisionEvent += Collider_OnCollisionEvent;
    }

    private void Collider_OnCollisionEvent(Collider self, Collider other)
    {
        // display what hit what
        Log($"{self.Name} Hit {other.Name}");
        if (id < other.GetComponent<SimpleBox>().id)
        {
            Destroy(gameObject);
        }
        // end game ...
        EndGame();
    }

    private void EndGame()
    {
        Broadcast(new object[] { -1, "Game is end." }, Reliability.ReliableOrder);
        CloseGame();
    }

    public override void OnDestroy()
    {
        Log($"Destroyed {Name}");
    }
}
