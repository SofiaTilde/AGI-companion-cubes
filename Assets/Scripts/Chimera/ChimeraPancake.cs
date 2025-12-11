using UnityEngine;
using UnityEngine.Rendering;

public class ChimeraPancake : MonoBehaviour
{
    public ChimeraPlugin manager;
    public long PhysXPancakeHandler = -1;
    public uint NumVerts = 0;
    public uint NumIndices = 0;
    public bool EnableMeshUpdate = false;

    void Start()
    {

    }

    void Update()
    {
        if (manager && EnableMeshUpdate)
        {
            FetchMesh();
        }
    }

    // immediately destroy chimera pancake.
    public void DestroyPancake()
    {
        DetachPancake();
        NumVerts = 0;
        NumIndices = 0;
        Destroy(gameObject);
    }

    // this will detach the pancake from physx world, but keep the Unity GameObject.
    public void DetachPancake()
    {
        EnableMeshUpdate = false;
        manager.DestroyPhysXPancakeByHandler(PhysXPancakeHandler);
        PhysXPancakeHandler = -1;
        manager = null;
    }

    private void FetchMesh()
    {
        var dataArray = Mesh.AllocateWritableMeshData(1);
        var data = dataArray[0];

        data.SetVertexBufferParams(
            (int)NumVerts,
            new VertexAttributeDescriptor(VertexAttribute.Position)
        );

        var verticesArr = data.GetVertexData<Vector3>();

        data.SetIndexBufferParams((int)NumIndices, IndexFormat.UInt32);
        var indicesArr = data.GetIndexData<uint>();

        manager.FetchPancakeMesh(PhysXPancakeHandler, ref verticesArr, ref indicesArr);

        data.subMeshCount = 1;
        data.SetSubMesh(0, new SubMeshDescriptor(0, (int)NumIndices));

        var mesh = new Mesh();
        mesh.name = "Pancake Tet Soup";
        Mesh.ApplyAndDisposeWritableMeshData(dataArray, mesh);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;

        // update the box collider according to the mesh returned from PhysX world.
        var boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = mesh.bounds.size;
        boxCollider.center = mesh.bounds.center;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Wall"))
        {
            return;
        }
        // crushed with a ceiling, it's labelled with "Wall" LOL.
        DetachPancake();
        // after detaching from PhysX world,
        // the mesh stop updating, should looks like sticks to the ceiling.
        
        // destroy the pancake object after 60secs.
        Destroy(gameObject, 60f);
    }
}
