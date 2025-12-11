using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

public class ChimeraPancake : MonoBehaviour
{
    public ChimeraPlugin manager;
    public long PhysXPancakeHandler = -1;
    public uint NumVerts = 0;
    public uint NumIndices = 0;

    void Start()
    {

    }

    void Update()
    {
        if (manager == null)
        {
            return;
        }
        FetchMesh();
    }

    public void DestroyPancake()
    {
        manager.DestroyPhysXPancakeByHandler(PhysXPancakeHandler);
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
    }
}
