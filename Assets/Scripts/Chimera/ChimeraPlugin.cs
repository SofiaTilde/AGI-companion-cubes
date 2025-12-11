using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

public class ChimeraPlugin : MonoBehaviour
{
    [DllImport("main")]
    public static extern void InitPhysics();

    [DllImport("main")]
    public static extern void CleanUpPhysics();

    [DllImport("main")]
    public static extern uint NbVerticesOfDeformable();
    [DllImport("main")]
    public static extern uint NbIndicesOfDeformable();
    [DllImport("main")]
    public static extern uint DeformableSize();

    [DllImport("main")]
    public unsafe static extern void StepAndGetUpdatesMesh(void* vertices, void* indices);

    [DllImport("main")]
    public static extern uint NbVerticesOfPanCollider();

    [DllImport("main")]
    public static extern uint NbIndicesOfPanCollider();

    [DllImport("main")]
    public static extern void SetPanGeometry(IntPtr vertices, IntPtr indices, uint numVertices, uint numIndices);

    private GameObject deformable;

    public Mesh PanColliderMesh;

    void Start()
    {
        SetPanColliderInPlugin();
        InitPhysics();
        _verticesNum = (int)NbVerticesOfDeformable();
        _indicesNum = (int)NbIndicesOfDeformable();
        Debug.Log($"v {_verticesNum} i {_indicesNum}");
        Debug.Log($"panv {NbVerticesOfPanCollider()} pani {NbIndicesOfPanCollider()}");
        deformable = transform.Find("Deformable").gameObject;
        if (deformable == null )
        {
            Debug.LogError("can't find a child called deformable");
        }
    }
    private int _verticesNum = 0;
    private int _indicesNum = 0;

    // Update is called once per frame
    void Update()
    {
        //StepAndFetchMesh();
    }

    private void OnDestroy()
    {
        CleanUpPhysics();
    }

    private void StepAndFetchMesh()
    {
        var dataArray = Mesh.AllocateWritableMeshData(1);
        var data = dataArray[0];

        data.SetVertexBufferParams(
            (int)_verticesNum,
            new VertexAttributeDescriptor(VertexAttribute.Position)
        );

        var verticesArr = data.GetVertexData<Vector3>();

        data.SetIndexBufferParams((int)_indicesNum, IndexFormat.UInt32);
        var indicesArr = data.GetIndexData<uint>();

        unsafe
        {
            var verticesPtr = NativeArrayUnsafeUtility.GetUnsafePtr(verticesArr);
            var indicesPtr = NativeArrayUnsafeUtility.GetUnsafePtr(indicesArr);
            ChimeraPlugin.StepAndGetUpdatesMesh(verticesPtr, indicesPtr);
        }

        data.subMeshCount = 1;
        data.SetSubMesh(0, new SubMeshDescriptor(0, _indicesNum));

        var mesh = new Mesh();
        mesh.name = "Pancake Tet Soup";
        Mesh.ApplyAndDisposeWritableMeshData(dataArray, mesh);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        deformable.GetComponent<MeshFilter>().mesh = mesh;
    }

    private void SetPanColliderInPlugin()
    {
        var vertices = PanColliderMesh.vertices;
        var triangles = PanColliderMesh.triangles;

        var gcVertices = GCHandle.Alloc(vertices, GCHandleType.Pinned);
        var gcTriangles = GCHandle.Alloc(triangles, GCHandleType.Pinned);

        SetPanGeometry(gcVertices.AddrOfPinnedObject(), gcTriangles.AddrOfPinnedObject(), (uint)vertices.Length, (uint)triangles.Length);

        gcVertices.Free();
        gcTriangles.Free();
    }
}
