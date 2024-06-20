
using System;
using System.Collections.Generic;
using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using Microsoft.Xna.Framework;
using WarSteel.Entities;
using WarSteel.Scenes;
using Vector3 = System.Numerics.Vector3;


public class PhysicsProcessor : ISceneProcessor
{
    public Simulation Simulation;

    private List<StaticBody> _staticBodies = new();
    private List<DynamicBody> _dynamicBodies = new();

    public PhysicsProcessor()
    {
        BufferPool bufferPool = new();
        SolveDescription solveDescription = new(15, 5);
        Simulation = Simulation.Create(bufferPool, new NarrowPhaseCallbacks(this), new PoseIntegratorCallbacks(), solveDescription);
    }

    public void Draw(Scene scene) {}

    public void Initialize(Scene scene){}

    public void AddBody(RigidBody r)
    {
        r.Build(this);
    }

    public void RemoveDynamicBody(DynamicBody r)
    {
        foreach (var b in _dynamicBodies)
        {
            if (b == r)
            {
                Simulation.Bodies.Remove(b.Handle);
                _dynamicBodies.Remove(b);
                break;
            }
        }

    }

    public void RemoveStaticBody(StaticBody r)
    {
        foreach (var b in _staticBodies)
        {
            if (b == r)
            {
                Simulation.Statics.Remove(r.Handle);
                _staticBodies.Remove(b);
                break;
            }
        }
    }


    public void Update(Scene scene, GameTime gameTime)
    {

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        dt = dt == 0 ? 0.0001f : dt;

        Simulation.Timestep(dt);


        foreach (var r in _dynamicBodies)
        {

            BodyReference body = Simulation.Bodies[r.Handle];
            body.Awake = true;

            body.ApplyLinearImpulse(new Vector3(r.Force.X, r.Force.Y, r.Force.Z));
            body.ApplyAngularImpulse(new Vector3(r.Torque.X, r.Torque.Y, r.Torque.Z));
            body.ApplyLinearImpulse(-body.Velocity.Linear * r.Drag);
            body.ApplyAngularImpulse(-body.Velocity.Angular * r.AngularDrag);

        }
    }

    internal TypedIndex AddShape(Collider collider)
    {
        IShape shape = collider.ColliderShape.GetShape();

        if (shape is Box boxShape)
        {
            return Simulation.Shapes.Add(boxShape);
        }

        if (shape is Sphere sphereShape)
        {
            return Simulation.Shapes.Add(sphereShape);
        }


        if (shape is ConvexHull hullShape)
        {
            return Simulation.Shapes.Add(hullShape);
        }


        else throw new InvalidOperationException("Unsupported shape added!");

    }

    internal void AddStatic(StaticBody body, StaticDescription staticDescription)
    {
        StaticHandle handle = Simulation.Statics.Add(staticDescription);
        body.Handle = handle;
        _staticBodies.Add(body);
    }

    internal void AddDynamic(DynamicBody body, BodyDescription bodyDescription)
    {
        BodyHandle handle = Simulation.Bodies.Add(bodyDescription);
        body.Handle = handle;
        _dynamicBodies.Add(body); ;
    }

    internal DynamicBody FindDynamic(BodyHandle handle)
    {
        foreach (var b in _dynamicBodies)
        {
            if (handle == b.Handle) return b;
        }
        return null;
    }

    internal StaticBody FindStatic(StaticHandle handle)
    {
        foreach (var a in _staticBodies)
        {
            if (handle == a.Handle) return a;
        }
        return null;
    }

    public RigidBody GetRigidBodyFromCollision(CollidableReference r)
    {
        if (r.Mobility == CollidableMobility.Static)
        {
            return FindStatic(r.StaticHandle);
        }
        if (r.Mobility == CollidableMobility.Dynamic)
        {
            return FindDynamic(r.BodyHandle);
        }
        return null;
    }


}


public struct NarrowPhaseCallbacks : INarrowPhaseCallbacks
{

    private PhysicsProcessor _processor;

    public NarrowPhaseCallbacks(PhysicsProcessor processor)
    {
        _processor = processor;
    }

    public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b, ref float speculativeMargin)
    {
        return true;
    }

    public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
    {
        return true;
    }

    public bool ConfigureContactManifold<TManifold>(int workerIndex, CollidablePair pair, ref TManifold manifold, out PairMaterialProperties pairMaterial) where TManifold : unmanaged, IContactManifold<TManifold>
    {
        pairMaterial.FrictionCoefficient = 0.9f;
        pairMaterial.MaximumRecoveryVelocity = 100000f;
        pairMaterial.SpringSettings = new SpringSettings(30, 10);

        RigidBody A = _processor.GetRigidBodyFromCollision(pair.A);
        RigidBody B = _processor.GetRigidBodyFromCollision(pair.B);
        A.Collider.OnCollide(new Collision(B.Entity));
        B.Collider.OnCollide(new Collision(A.Entity));


        return true;
    }

    public bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ref ConvexContactManifold manifold)
    {
        return true;
    }

    public void Dispose()
    {

    }

    public void Initialize(Simulation simulation)
    {

    }
}

public struct PoseIntegratorCallbacks : IPoseIntegratorCallbacks
{

    private Vector3 _gravity = new(0, -1000, 0);

    private float _dragCoeff = 0.2f;

    public PoseIntegratorCallbacks()
    {
    }

    public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;

    public bool AllowSubstepsForUnconstrainedBodies => true;

    public bool IntegrateVelocityForKinematics => true;

    public void Initialize(Simulation simulation)
    {

    }

    public void IntegrateVelocity(Vector<int> bodyIndices, Vector3Wide position, QuaternionWide orientation, BodyInertiaWide localInertia, Vector<int> integrationMask, int workerIndex, Vector<float> dt, ref BodyVelocityWide velocity)
    {
        Vector3Wide gravityWide;
        Vector3Wide dvGrav;
        Vector3Wide.Broadcast(_gravity, out gravityWide);
        Vector3Wide.Scale(gravityWide, dt, out dvGrav);
        Vector3Wide.Add(velocity.Linear, dvGrav, out velocity.Linear);
        Vector3Wide.Scale(velocity.Linear, -Vector.Multiply(_dragCoeff, dt), out Vector3Wide drag);
        Vector3Wide.Add(velocity.Linear, drag, out velocity.Linear);

    }

    public void PrepareForIntegration(float dt)
    {

    }
}




