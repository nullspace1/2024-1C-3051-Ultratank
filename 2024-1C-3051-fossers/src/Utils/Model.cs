using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarSteel.Utils;

public class ModelUtils
{
    public static float GetHeight(Model model)
{
    float minZ = float.MaxValue;
    float maxZ = float.MinValue;

    foreach (ModelMesh mesh in model.Meshes)
    {
        foreach (ModelMeshPart meshPart in mesh.MeshParts)
        {
            // Get the vertex stride from the VertexDeclaration
            int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;

            // Create a buffer large enough to hold all the vertices in this mesh part
            Vector3[] vertices = new Vector3[meshPart.NumVertices];

            // Get the data from the vertex buffer, starting at the offset of this mesh part's vertices
            VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[meshPart.NumVertices];
            meshPart.VertexBuffer.GetData(meshPart.VertexOffset * vertexStride, vertexData, 0, meshPart.NumVertices, vertexStride);

            // Extract the Z values
            foreach (var vertex in vertexData)
            {
                minZ = Math.Min(minZ, vertex.Position.Z);
                maxZ = Math.Max(maxZ, vertex.Position.Z);
            }
        }
    }

    return Math.Abs(maxZ - minZ);
}

public static float GetWidth(Model model)
{
    float minX = float.MaxValue;
    float maxX = float.MinValue;

    foreach (ModelMesh mesh in model.Meshes)
    {
        foreach (ModelMeshPart meshPart in mesh.MeshParts)
        {
            // Get the vertex stride from the VertexDeclaration
            int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;

            // Create a buffer large enough to hold all the vertices in this mesh part
            Vector3[] vertices = new Vector3[meshPart.NumVertices];

            // Get the data from the vertex buffer, starting at the offset of this mesh part's vertices
            VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[meshPart.NumVertices];
            meshPart.VertexBuffer.GetData(meshPart.VertexOffset * vertexStride, vertexData, 0, meshPart.NumVertices, vertexStride);

            // Extract the X values
            foreach (var vertex in vertexData)
            {
                minX = Math.Min(minX, vertex.Position.X);
                maxX = Math.Max(maxX, vertex.Position.X);
            }
        }
    }

    return Math.Abs(maxX - minX);
}


    
}