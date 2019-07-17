using GameSystem;
using GameSystem.GameCore;
using GameSystem.GameCore.Components;
using GameSystem.GameCore.Network;
using GameSystem.GameCore.SerializableMath;
using System;
using System.Collections;
using System.Collections.Generic;

public class SimpleBoxManager : Component
{
    private Dictionary<int, ServerSimpleBox> boxes;
    private Vector3 spawnPoint = new Vector3(0, 0, 0);
    private GameObject boxPrefab;

    private List<Bullet> bullets;
    private GameObject bulletPrefab;
    private BulletPool bulletPool;

    public override void Start()
    {
        OnReceiveGamePacket += OnReceiveControlPacket;
        boxPrefab = CreateBoxPrefab();
        boxes = new Dictionary<int, ServerSimpleBox>();

        bulletPrefab = CreateBulletPrefab();
        bulletPool = new BulletPool(bulletPrefab.GetComponent<Bullet>());
        bulletPool.Supple(30);
        bullets = new List<Bullet>();

        HandleJoinRequest();
    }

    

    /// <summary>
    /// Create box prefab
    /// </summary>
    /// <returns>box prefab game object</returns>
    public GameObject CreateBoxPrefab()
    {
        GameObject prefab = CreateGameObject();
        prefab.Name = "Box";
        // add simple box component
        ServerSimpleBox component = prefab.AddComponent<ServerSimpleBox>();
        component.moveSpeed = 3f;
        // add collider component
        BoxCollider collider = prefab.AddComponent<BoxCollider>();
        collider.SetSize(new Vector3(1f));
        prefab.SetActive(false);
        return prefab;
    }

    /// <summary>
    /// Create bullet prefab
    /// </summary>
    /// <returns>bullet prefab game object</returns>
    public GameObject CreateBulletPrefab()
    {
        GameObject prefab = CreateGameObject();
        prefab.Name = "Bullet";
        Bullet component = prefab.AddComponent<Bullet>();
        SphereCollider collider = prefab.AddComponent<SphereCollider>();
        collider.SetSize(0.25f);
        prefab.SetActive(false);
        return prefab;
    }

    private void HandleJoinRequest()
    {
        var joinReqs = Network_GetJoinRequests();
        for (int i = 0; i < joinReqs.Length; i++)
        {
            AcceptPlayer(joinReqs[i]);
        }
    }

    private void HandleExitEvent()
    {
        var exitEvents = Network_GetExitGroupEvents();
        for(int i = 0; i < exitEvents.Length; i++)
        {
            int boxId = exitEvents[i].peer.Id;
            if (boxes.TryGetValue(boxId, out ServerSimpleBox box))
            {
                // destroy player avater
                Destroy(box);
                boxes.Remove(boxId);
            }
        }
        // for each exit join request ...
        // remove box from manager
        // remove all bullet which exited player shot?
    }

    /// <summary>
    /// Accept player
    /// </summary>
    /// <param name="request">join request</param>
    private void AcceptPlayer(JoinGroupRequest request)
    {
        IPeer peer = request.Accept("SimpleGame");
        // check if peer is connected
        if (peer.isConnected)
        {
            GameObject go = Instantiate(boxPrefab);
            go.SetActive(true);
            ServerSimpleBox component = go.GetComponent<ServerSimpleBox>();
            component.id = peer.Id;
            boxes.Add(component.id, component);
        }
    }

    public float[] ToFloatArray(Vector3 v)
    {
        return new float[] { v.x, v.y, v.z };
    }

    public Vector3 ToVector3(float[] floats)
    {
        return new Vector3(floats[0], floats[1], floats[2]);
    }

    /// <summary>
    /// Receive packet handler method
    /// </summary>
    /// <param name="peer">source peer of sending the packet</param>
    /// <param name="packet">packet</param>
    public void OnReceiveControlPacket(IPeer peer, object packet)
    {
        object[] gamePacket = packet as object[];
        if (gamePacket != null)
        {
            int switchCode = (int)gamePacket[0];
            switch (switchCode)
            {
                case SimpleGameMetrics.ClientGameSwitchCode.Move:
                    MoveControl(peer, (float[])gamePacket[1]);
                    break;
                case SimpleGameMetrics.ClientGameSwitchCode.Shoot:
                    ShootControl(peer);
                    break;
            }
        }
    }

    public override void Update()
    {
        float second = (float)DeltaTime.TotalSeconds;
        int posIndex = 0;
        // player existed
        if (boxes.Count > 0)
        {
            BoxInfo[] boxPacket = new BoxInfo[boxes.Count];
            foreach (var box in boxes.Values)
            {
                boxPacket[posIndex] = box.UpdatePosAndRot(second);
                posIndex++;
            }
            
            Array.Sort(boxPacket, (x, y) => x.boxId.CompareTo(y.boxId));
            Network_Broadcast(
                new object[] { SimpleGameMetrics.ServerGameSwitchCode.BoxInfo, boxPacket },
                Reliability.Sequence);


            //if (bullets.Count > 0)
            {
                List<Bullet.BulletInfo> bulletPacket = new List<Bullet.BulletInfo>();
                for (int i = 0; i < bullets.Count; i++)
                {
                    if (bullets[i].UpdateBullet(second))
                    {
                        // if bullet timed out, recycle bullet
                        bulletPool.Recycle(bullets[i]);
                        bullets.RemoveAt(i--);
                    }
                    else
                    {
                        // update bullet info into client
                        bulletPacket.Add(bullets[i].GetInfo());
                    }
                }
                bulletPacket.Sort((x, y) => x.id.CompareTo(y.id));
                Network_Broadcast(
                    new object[] { SimpleGameMetrics.ServerGameSwitchCode.BulletInfo, bulletPacket.ToArray() },
                    Reliability.Sequence);
            }
        }

        HandleJoinRequest();

        HandleExitEvent();
    }

    /// <summary>
    /// Move control handler
    /// </summary>
    private void MoveControl(IPeer peer, float[] direction)
    {   
        Vector3 d = ToVector3(direction);
        if (boxes.TryGetValue(peer.Id, out ServerSimpleBox box))
        {
            box.InputRotation = d.x;
            box.InputMove = d.z;
        }
    }

    private IdentityPool bulletIdPool = new IdentityPool();
    private void ShootControl(IPeer peer)
    {
        if (boxes.TryGetValue(peer.Id, out ServerSimpleBox box))
        {   
            Bullet bullet = bulletPool.Get(bulletIdPool.NewID());
            bullet.direction = box.Direction;
            bullet.transform.position = box.transform.position + box.Direction;
            bullets.Add(bullet);
        }
    }
}



public class Bullet : Component
{
    public int id;
    public Vector3 direction;
    public float speed = 3f;

    public float remainTimer = 0f;
    public float remainTime = 3f;

    public BulletInfo GetInfo()
    {
        return new BulletInfo(id, transform.position);
    }

    public bool UpdateBullet(float deltaTime)
    {
        transform.position += direction * speed * deltaTime;
        //Log(transform.position);
        remainTimer += deltaTime;
        return remainTimer > remainTime;
    }

    public void Reset()
    {
        remainTimer = 0;
    }

    [Serializable]
    public class BulletInfo
    {
        public int id;
        public float[] pos;

        public BulletInfo(int id, Vector3 pos)
        {
            this.id = id;
            this.pos = new float[] { pos.x, pos.y, pos.z };
        }
    }
}

public class BulletPool : TrackableObjectPool<Bullet>
{
    private Bullet bulletPrefab;

    public BulletPool(Bullet prefab)
    {
        bulletPrefab = prefab;
    }

    protected override int Comparison(Bullet x, Bullet y)
    {
        return x.SID.CompareTo(y.SID);
    }

    protected override Bullet Create()
    {
        return GameObject.Instantiate(bulletPrefab.gameObject).GetComponent<Bullet>();
    }

    protected override void SuppleHandler(Bullet item)
    {
        item.gameObject.SetActive(false);
    }

    protected override void GetHandler(Bullet item, object arg)
    {
        item.gameObject.SetActive(true);
        item.id = (int)arg;
    }

    protected override void RecycleHandler(Bullet item)
    {
        item.gameObject.SetActive(false);
        item.Reset();
    }

    protected override void Destroy(Bullet item)
    {
        GameObject.Destroy(item.gameObject);
    }
}
