using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ClearOutsideFurniture
{
    public class ClearOutsideFurnitureSystem : GenericSystemBase, IModSystem
    {
        private EntityQuery applianceQuery;
        private EntityQuery externalBinQuery;


        protected override void Initialise()
        {
            base.Initialise();
            // Query for appliances marked for destruction
            applianceQuery = GetEntityQuery(new QueryHelper()
                .All(typeof(CAppliance), typeof(CPosition), typeof(CDestroyApplianceAtDay))
            );
            externalBinQuery = GetEntityQuery(new QueryHelper().All(typeof(CApplianceExternalBin)));
        }

        protected override void OnUpdate()
        {
            // Press DELETE key during Planning Phase to delete marked items
            if (!Has<SIsDayTime>() && Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteMarkedFurniture();

            }

            if(Input.GetKeyDown(KeyCode.F9))
            {
                DebugLogBin();
            }
        }

        private void DebugLogBin()
        {
            Unity.Mathematics.float3? binPosition = GetExternalBinPosition();
            if (binPosition.HasValue)
            {
                Debug.Log($"Bin position is {binPosition.Value.ToString()}");
            }
            else
            {
                Debug.Log("BinPosition is null.");
            }
        }

        private void DeleteMarkedFurniture()
        {
            Unity.Mathematics.float3? binPosition = GetExternalBinPosition();

            if (binPosition.HasValue)
                CreateVisibleSquare(binPosition.Value, 5);

            // Get all appliances marked for destruction (with trash icon)
            using (var entities = applianceQuery.ToEntityArray(Allocator.Temp))
            {
                int deletedCount = 0;
                int nearTrashCount = 0;

                foreach (var entity in entities)
                {
                    // Check if the appliance has a position
                    if (!Require(entity, out CPosition position))
                        continue;

                    // Skip if entity is a permanent structure
                    if (Has<CImmovable>(entity))
                        continue;


                    if (IsWithin5x5GridOfBin(position.Position, binPosition.Value))
                    {
                        EntityManager.DestroyEntity(entity);
                        deletedCount++;
                    }
                }

                if (deletedCount > 0)
                {
                    Debug.Log($"[ClearOutsideFurniture] Deleted {deletedCount} appliances with trash icons ({nearTrashCount} near trash bin)");
                } else
                {
                    Debug.Log($"[ClearOutsideFurntirue] Deleted Nothing. Candidates={entities.Length} Deletable");
                }
            }
        }

        private Unity.Mathematics.float3? GetExternalBinPosition()
        {
            using (var entities = externalBinQuery.ToEntityArray(Allocator.Temp))
            {
                if (entities.Length == 0)
                    return null;

                // Get the first external bin (usually only one)
                var binEntity = entities[0];

                if (Require(binEntity, out CPosition position))
                {
                    return position.Position;
                }
            }

            return null;
        }

        private bool IsWithin5x5GridOfBin(Unity.Mathematics.float3 itemPos, Unity.Mathematics.float3 binPos)
        {
            float deltaX = Unity.Mathematics.math.abs(itemPos.x - binPos.x);
            float deltaZ = Unity.Mathematics.math.abs(itemPos.z - binPos.z);

            return deltaX <= 2.5f && deltaZ <= 2.5f;
        }

        private GameObject CreateVisibleSquare(Unity.Mathematics.float3 center, float size, float duration = 2f)
        {
            // Create a GameObject with LineRenderer
            GameObject squareObj = new GameObject("DeletionZoneIndicator");
            LineRenderer lineRenderer = squareObj.AddComponent<LineRenderer>();

            // Configure the line renderer
            lineRenderer.positionCount = 5; // 4 corners + back to start
            lineRenderer.loop = true;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;

            // Calculate corners
            float halfSize = size / 2f;
            float y = center.y + 0.2f; // Height above ground

            Vector3[] positions = new Vector3[5]
            {
                new Vector3(center.x - halfSize, y, center.z + halfSize), // Top-left
                new Vector3(center.x + halfSize, y, center.z + halfSize), // Top-right
                new Vector3(center.x + halfSize, y, center.z - halfSize), // Bottom-right
                new Vector3(center.x - halfSize, y, center.z - halfSize), // Bottom-left
                new Vector3(center.x - halfSize, y, center.z + halfSize)  // Back to start
            };

            lineRenderer.SetPositions(positions);

            // Destroy after duration
            UnityEngine.Object.Destroy(squareObj, duration);

            return squareObj;
        }


    }

}