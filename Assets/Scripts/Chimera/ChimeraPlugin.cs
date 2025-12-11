using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.XR.CoreUtils;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct MeshDimension
{
    public uint numVerts;
    public uint numIndices;
}

[StructLayout(LayoutKind.Sequential)]
public struct BoundingBox
{
    public Vector3 position;
    public Vector3 size;
}

public class ChimeraPlugin : MonoBehaviour
{
    [DllImport("main")]
    private static extern void InitPhysics();
    [DllImport("main")]
    private static extern void StepPhysics(uint substep);

    [DllImport("main")]
    private static extern void CleanUpPhysics();

    [DllImport("main")]
    private static extern uint NbVerticesOfDeformable();
    [DllImport("main")]
    private static extern uint NbIndicesOfDeformable();

    [DllImport("main")]
    private unsafe static extern void StepAndGetUpdatesMesh(void* vertices, void* indices);

    [DllImport("main")]
    private static extern void FetchPanCollider(IntPtr halfExtents, IntPtr globalPose);

    [DllImport("main")]
    private static extern void SetPanColliderTransform(Vector3 pos, Quaternion rot);

    [DllImport("main")]
    private static extern long CreatePancakeDeformable(Vector3 pos, float radius);
    [DllImport("main")]
    private static extern void DestroyPancakeDeformable(long handler);
    [DllImport("main")]
    private static extern void GetPancakeMeshDimension(long handler, ref MeshDimension dim);
    [DllImport("main")]
    private static extern unsafe void GetPancakeMesh(long handler, void* vertices, void* indices);
    [DllImport("main")]
    private static extern uint NbEnvironmentColliders();
    [DllImport("main")]
    private static extern void FetchEnvironmentCollider(uint index, ref BoundingBox aabb);
    [DllImport("main")]
    private static extern void SetStaticBox(BoundingBox box);

    public GameObject DebugCube = null;
    public uint substep = 2;
    public GameObject PanController;
    public Material RawPancakeMaterial = null;
    public Material CookedPancakeMaterial = null;
    public Material BurntPancakeMaterial = null;
    public GameObject RoomRoot = null;

    private bool isGenesis = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitPhysics();
        if (RoomRoot)
        {
            // transfer the entire room's box collider into PhysX world as PxRigidStatic.
            SetEnvironmentIntoPhysX();
        }
        isGenesis = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Get Controller transform and set PhysX rigid body accordingly.
        ProcessControllerTransform();
        // Drive the PhysX world.
        StepPhysics(substep);

        if (DebugCube != null)
        {
            // for debugging, the collider of the pan.
            TransformDebugCube();
        }
    }

    private void ProcessControllerTransform()
    {
        if (!isGenesis)
        {
            return;
        }
        if (PanController == null)
        {
            return;
        }
        PanController.transform.GetPositionAndRotation(out var position, out var rotation);
        SetPanColliderTransform(position, rotation);
    }

    private void TransformDebugCube()
    {
        if (!isGenesis)
        {
            return;
        }
        if (DebugCube == null)
        {
            return;
        }

        var halfExtents = new float[3];
        var transform = new float[7];
        var gcHalfExtents = GCHandle.Alloc(halfExtents, GCHandleType.Pinned);
        var gcTransform = GCHandle.Alloc(transform, GCHandleType.Pinned);

        FetchPanCollider(gcHalfExtents.AddrOfPinnedObject(), gcTransform.AddrOfPinnedObject());

        gcHalfExtents.Free();
        gcTransform.Free();

        var scale = new Vector3(halfExtents[0], halfExtents[1], halfExtents[2]);
        scale.Scale(new Vector3(2.0f, 2.0f, 2.0f));
        var pos = new Vector3(transform[4], transform[5], transform[6]);
        var rot = new Quaternion(transform[0], transform[1], transform[2], transform[3]);

        DebugCube.transform.localScale = scale;
        DebugCube.transform.position = pos;
        DebugCube.transform.rotation = rot;
    }

    public GameObject CreatePancakeAt(Vector3 position, float size)
    {
        if (!isGenesis)
        {
            return null;
        }
        var handle = CreatePancakeDeformable(position, size);
        var pancake = new GameObject($"pancake{handle}", typeof(MeshFilter), typeof(MeshRenderer), typeof(ChimeraPancake), typeof(PancakeData), typeof(BoxCollider));
        gameObject.tag = "Pancake";
        var pancakeComp = pancake.GetComponent<ChimeraPancake>();
        pancakeComp.manager = this;
        pancakeComp.PhysXPancakeHandler = handle;

        MeshDimension dim = new();
        GetPancakeMeshDimension(handle, ref dim);
        Debug.Log($"pancake {handle} verts {dim.numVerts} and indices {dim.numIndices}");
        pancakeComp.NumVerts = dim.numVerts;
        pancakeComp.NumIndices = dim.numIndices;
        pancakeComp.EnableMeshUpdate = true;

        // pancake mounts on chimera manager initially.
        pancake.transform.parent = transform;
        // pass material for pancakes.
        if (RawPancakeMaterial && CookedPancakeMaterial && BurntPancakeMaterial)
        {
            // the pancake should be spawned with a raw material
            var pancakeRender = pancake.GetComponent<MeshRenderer>();
            pancakeRender.material = RawPancakeMaterial;
            var pancakeData = pancake.GetComponent<PancakeData>();
            pancakeData.pancakeRaw = RawPancakeMaterial;
            pancakeData.pancakeCooked = CookedPancakeMaterial;
            pancakeData.pancakeBurnt = BurntPancakeMaterial;
        }

        // box collider, for trigging event with multiple manager.
        var pancakeTriggerCollider = pancake.GetComponent<BoxCollider>();
        pancakeTriggerCollider.isTrigger = true;

        return pancake;
    }

    public GameObject CreatePancakeAt()
    {
        if (PanController == null)
        {
            return null;
        }

        PanController.transform.GetPositionAndRotation(out var position, out var _);
        return CreatePancakeAt(position + new Vector3(0.0f, 0.05f, 0.0f), 0.05f);
    }

    public void DestroyPhysXPancakeByHandler(long handler)
    {
        if (!isGenesis)
        {
            return;
        }
        if (handler < 0)
        {
            return;
        }
        DestroyPancakeDeformable(handler);
    }

    public void FetchPancakeMesh(long handler, ref NativeArray<Vector3> verts, ref NativeArray<uint> indices)
    {
        if (!isGenesis)
        {
            return;
        }
        unsafe
        {
            var verticesPtr = NativeArrayUnsafeUtility.GetUnsafePtr(verts);
            var indicesPtr = NativeArrayUnsafeUtility.GetUnsafePtr(indices);
            GetPancakeMesh(handler, verticesPtr, indicesPtr);
        }
    }

    private void SetEnvironmentIntoPhysX()
    {
        if (!isGenesis)
        {
            return;
        }
        var colliders = RoomRoot.GetComponents<BoxCollider>();
        foreach (var c in colliders)
        {
            if (c.isTrigger)
            {
                // skip trigger only colliders.
                continue;
            }
            var box = new BoundingBox
            {
                position = c.center,
                size = c.size,
            };
            SetStaticBox(box);
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (!isGenesis)
        {
            return;
        }
        var numEnvironmentObjs = NbEnvironmentColliders();
        for (uint i = 0; i < numEnvironmentObjs; i++)
        {
            BoundingBox box = new();
            FetchEnvironmentCollider(i, ref box);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(box.position, box.size);
        }
    }

    private void OnDestroy()
    {
        CleanUpPhysics();
        isGenesis = false;
    }
}
