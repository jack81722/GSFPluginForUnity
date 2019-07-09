using GameSystem.GameCore;
using GameSystem.GameCore.Components;
using GameSystem.GameCore.Network;
using GameSystem.GameCore.SerializableMath;

[Packable(0)]
public class ServerSimpleBox : Component
{
    [PacketMember(0)]
    public int id;
    public float moveSpeed;
    public float rotateSpeed = 3f;
    public float currentRot;
    public float InputRotation;
    public float InputMove;
    public Vector3 Direction { get {return new Vector3((float)System.Math.Cos(-currentRot), 0, (float)System.Math.Sin(-currentRot)); } }
    public Vector3 velocity { get { return moveSpeed * InputMove * Direction; } }
    [PacketMember(1)]
    public Vector3 pos;
    public BoxCollider collider;

    public override void Start()
    {
        pos = transform.position;
        collider = GetComponent<BoxCollider>();
        collider.OnCollisionEvent += Collider_OnCollisionEvent;
    }

    public BoxInfo UpdatePosAndRot(float deltaTime)
    {   
        currentRot += InputRotation * rotateSpeed * deltaTime;
        Vector3 pos = transform.position;
        pos += velocity * deltaTime;
        transform.position = pos;
        Quaternion q = transform.rotation = Quaternion.Euler(new Vector3(0, currentRot, 0));
        return new BoxInfo(id, pos, q);
    }

    private void Collider_OnCollisionEvent(Collider self, Collider other)
    {
        // display what hit what
        //Log($"{self.Name} Hit {other.Name}");
        //if (id < other.GetComponent<ServerSimpleBox>().id)
        //{
        //    Destroy(gameObject);
        //}
        // end game ...
        //EndGame();
    }

    private void EndGame()
    {
        Network_Broadcast(new object[] { -1, "Game is end." }, Reliability.ReliableOrder);
        CloseGame();
    }

    public override void OnDestroy()
    {
        Log($"Destroyed {Name}");
    }
}
