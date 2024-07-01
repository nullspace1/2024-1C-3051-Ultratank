using System;
using Microsoft.Xna.Framework;
using WarSteel.Entities;
using WarSteel.Scenes;

public class Stabilizer : IComponent
{

    private const float TorqueForce = 5000000f;

    private const float SidewaysCancelFactor = 200;

    public void Destroy(GameObject self, Scene scene)
    {

    }

    public void OnStart(GameObject self, Scene scene)
    {

    }

    public void OnUpdate(GameObject self, GameTime gameTime, Scene scene)
    {
        Vector3 dv = self.Transform.Up - Vector3.Up;
        DynamicBody rb = self.GetComponent<DynamicBody>();
        rb.ApplyTorque(Vector3.Cross(dv, self.Transform.Up) * TorqueForce);
        Vector3 forward = self.Transform.Forward;
        Vector3 velocity = rb.Velocity;
        Vector3 sidewaysVelocity = velocity - Vector3.Dot(velocity, forward) * forward;
        sidewaysVelocity.Y = 0;
        rb.ApplyForce(-sidewaysVelocity * SidewaysCancelFactor);
    }
}