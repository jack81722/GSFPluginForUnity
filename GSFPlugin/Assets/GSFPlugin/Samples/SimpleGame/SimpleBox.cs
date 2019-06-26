using GameSystem.GameCore;
using GameSystem.GameCore.Components;
using GameSystem.GameCore.Network;
using GameSystem.GameCore.SerializableMath;

[Packable(0)]
public class ServerSimpleBox : Component
{
    [PacketMember(0)]
    public int id;
    public float speed;
    public Vector3 direction;
    public Vector3 velocity { get { return speed * direction; } }
    [PacketMember(1)]
    public Vector3 pos;
    public BoxCollider collider;

    public override void Start()
    {
        pos = transform.position;
        collider = GetComponent<BoxCollider>();
        collider.OnCollisionEvent += Collider_OnCollisionEvent;
    }

    private void Collider_OnCollisionEvent(Collider self, Collider other)
    {
        // display what hit what
        Log($"{self.Name} Hit {other.Name}");
        if (id < other.GetComponent<ServerSimpleBox>().id)
        {
            Destroy(gameObject);
        }
        // end game ...
        //EndGame();
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
