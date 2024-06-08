using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WarSteel.Entities;

public class GameObjectGenerator
{
    public static List<GameObject> Generate(Vector3 center, int maxElems, Type Entity, params object[] constructorParams)
    {
        List<GameObject> elems = new();

        Random rand = new();

        for (int i = 0; i < maxElems; i++)
        {
            GameObject elem = (GameObject)Activator.CreateInstance(Entity, constructorParams);
            elem.Transform.Position = center + new Vector3(rand.Next(-10000, 10000), 0, rand.Next(-10000, 10000));
            elems.Add(elem);
        }

        return elems;
    }
}