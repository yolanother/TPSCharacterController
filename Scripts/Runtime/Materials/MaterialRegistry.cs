using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Footsteps
{
    [CreateAssetMenu(menuName = "TPS Character Controller/Materials/Material Registry")]
    public class MaterialRegistry : ScriptableObject
    {
        [SerializeField] private List<MaterialType> materialTypes;

        private Dictionary<string, MaterialType> namedMaterials;

        public MaterialType this[string typeName]
        {
            get
            {
                if (null == namedMaterials)
                {
                    namedMaterials = new Dictionary<string, MaterialType>();
                    foreach (var type in materialTypes)
                    {
                        namedMaterials[type.name] = type;
                        foreach (var alias in type.materialNameAliases)
                        {
                            namedMaterials[alias] = type;
                        }
                    }
                }

                if (namedMaterials.TryGetValue(typeName, out var value))
                {
                    return value;
                }
                
                Debug.LogWarning("Unregistered material: " + typeName);
                return null;
            }
        }

        public MaterialType this[Terrain terrain, Vector3 coordinates]
        {
            get
            {
                var terrainPosition = GetTerrainCoords(terrain, coordinates);
                return GetTerrainTexture(terrain, terrainPosition);
            }
        }

        private void OnValidate()
        {
            namedMaterials = null;
        }

        private Vector2Int GetTerrainCoords(Terrain terrain, Vector3 coordinate)
        {
            
            Vector3 terrainPosition = coordinate - terrain.transform.position;

            Vector3 mapPosition = new Vector3
            (terrainPosition.x / terrain.terrainData.size.x, 0,
                terrainPosition.z / terrain.terrainData.size.z);

            float xCoord = mapPosition.x * terrain.terrainData.alphamapWidth;
            float zCoord = mapPosition.z * terrain.terrainData.alphamapHeight;

            return new Vector2Int((int) xCoord, (int) zCoord);
        }

        private MaterialType GetTerrainTexture(Terrain terrain, Vector2Int terrainPosition)
        {
            
            float[,,] splatMapData = terrain.terrainData.GetAlphamaps (terrainPosition.x, terrainPosition.y, 1, 1);

            var terrainData = terrain.terrainData;
            var alphaMapWidth = terrainData.alphamapWidth;
            var alphaMapHeight = terrainData.alphamapHeight;
 
            int numTextures = splatMapData.Length / (alphaMapWidth * alphaMapHeight);

            int textureIndex = 0;
            float textureStrength = 0;
            for (int i = 0; i < numTextures; i++)
            {
                if (textureStrength < splatMapData[0, 0, i])
                {
                    textureIndex = i;
                    textureStrength = splatMapData[0, 0, i];
                }
            }
            
            var terrainTexture  = terrainData.splatPrototypes[textureIndex];
            return this[terrainTexture.ToString()];
        }
    }
}
