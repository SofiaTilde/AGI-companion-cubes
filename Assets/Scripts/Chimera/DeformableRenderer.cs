using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

public class DeformableRenderer : MonoBehaviour
{
    private int _verticesNum = 0;
    private int _indicesNum = 0;

    // Update is called once per frame
    void Start()
    {

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

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
