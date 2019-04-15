# Unity Visual Effect Graph

The Visual Effect graph is the next-generation entry point for visual effects in Unity. It is targeting modern platforms and relies on Compute Shaders and the High Definition Render Pipeline for maximum performance and visual quality.

Visual Effect Graphs are a new kind of assets that contains one or many components (Mesh, Particle Systems, ...) of a fully customizable, standalone effect. Every graph embeds the behavior, shaders, parameter interface and events of the effect template.

You can use these templates in the scene by referencing them in a Visual Effect component, then adjusting the parameters you have exposed to customize any instance.

https://www.youtube.com/watch?v=BeCs0LJzkiM

## Contexts

Contexts are the logic containers of the systems, each context handles a pass of processing happening at a different stage of the simulation. Here is a summary of all current contexts.

![img](https://github.com/Unity-Technologies/ScriptableRenderPipeline/raw/master/com.unity.visualeffectgraph/Documentation%7E/Images/contexts.png)

### Spawn Context

Spawn contexts are triggered by **SpawnEvent** data types and can be chained to synchronize themselves. **SpawnEvent**s can be considered as messages containing a **spawn order** with a **spawn count** , and a **state payload**.

Spawn Contexts have two inputs : **Start** and **Stop**. These are implicitly bound to the `OnPlay` and `OnStop`  events, which means that the spawning machine will start spawning when  some SpawnEvent hits the start flow input, and shutdown when another  SpawnEvent hits the stop flow Input.

### Initialize Context

### Update Context

In this system, we do not have an upate Context as it is not needed. Data is computed once (Spawn Burst) then reused every frame in the Output Context.

### Output Context

 

## Blocks

### Force

Absolute force and relative force are 2 different things:

- Absolute force is simply the      force applied to the particle: v += f * dt
- Relative force will simulate      drag (linear) in a fluid in motion. The particle velocity will tend      towards the force. To simplify: v += (f - v) * dt

So a force of 0 will make the particle slow down and stop (acting like a linear drag in a motionless fluid). To have no effect a relative force has to be set to the particle velocity not 0. Relative force can be used to simulate wind for instance. 
 
 A drag coefficient will be added to the block (just like in the vector field force). The input should maybe also be renamed from force to velocity in the case of relative force as the force is computed from the difference of velocity.

### Collision

#### DepthBuffer


 Yes we have depth buffer collision and also a block to spawn particles and project them on scene using the depth buffer (Project On Depth in Init IIRC)

 However at the moment, it is not easy to set the zbuffer. In the spaceship demo, we used depth buffer collisions for the sparks but hacked HDRP to insert a custom hook to copy the depth buffer to use it during particle simulation.

 We plan to finish the implementation for 19.1 by adding buffers to the camera type directly (There's a node MainCamera to retrieve the tagged main camera), so getting the depth and color buffers will be automatic.

 For 18.3 I'm afraid you have to provide the buffers manually.

Thomas did it manually for this effect for instance: he renders the mesh in an offscreen render target, then provides the depth and color buffer of the render texture to the VFX Graph to spawn particles for the hologram (using the project on depth block):

##  Attributes

First of all, index and id are two different things:

- id is guaranteed to be unique      per particle (Well except it will cycle on 32bits overflow). Basically      it is incremented every time a particle is spawned
- index is that actual index of      the particle and is used to access particle data in the attribute buffer.

Both Id and index are guaranteed not to change during the particle lifetime. Index can be reused right away by a newly born particle though.Alive particle are stored in a sparse way in the attribute buffer. Meaning alive particles can be spread in the whole buffer with dead particles. There is no compaction of any sort. This is to guarantee particle index remains constant throughout the entire particle life.

### Attribute from Map

This block performs reading a value from a texture based on various indices (particleIndex, indexRelative, Random) or by using regular texture Sampling (2D or 3D)

In this example, we get a random position and color for each particle. The position and the color will be fetched from the same point in the cache as we use a RandomUniformPerParticle sample Mode.

#### sampling modes

A bit of explanation about the sampling modes available in the block SetAttributeFromMap to sample a map from a point cache:

- **Sequential** the sampling is based on the id of the particle (and its looping):      First particle samples first point and so on...
- **Index**      You specify manually the point to sample via the slot "index"
- **IndexRelative** Same as Index except the slot is a float between 0 and 1. The      point index is selected with index * pointCount
- **Random**      The point is selected randomly
- **RandomConstantPerParticle** The point is selected randomly but the      random number is constant per particle (meaning sampling in 2 maps in this      mode gives the same point)
- **Sample2DLod** Sample directly by providing the uv of the texture. More useful      for generic textures than for point caches
- **Sample3DLod** Same as above but for 3D textures

### Attribute Random from Curve

Using the "Set Scale from Curve" with a sample Mode of "RandomUniformPerParticle" will fetch a random position to read the curve, then return the read value. This way, you can perform non-uniform distribution by reading from curves.

### Modifying Attributes in Output Contexts

Output Contexts get the Simulated Data (or Initial Data if there is no update, like this example), as read only every frame, so every modification made to the attributes is only used for rendering.

#### Applying the Blend Mask

Once Computed, the blend mask is used to composite values to the initial values.

## External Data

### Using Point Caches

By Importing a pCache file, it creates a point cache asset that you can reference in a Point Cache operator. By selecting your asset, the operator creates one slot for the point count, then any texture slots for each attribute map. Then, you can use these textures in "Attribute From Map" blocks.

### Signed Distance Fields 

Currently, you can make a 3d texture/Signed Distance Field texture from another regular texture with the Density Volume Tool that comes with HDRP. Window / Rendering / Create Density Volume Texture ????

### Vectorfields

About vector fields: the Box in which the vector field texture is mapped is by default centered at 0 and has a side of 1.
 You can see the vector field box gizmo (and modify it) in the scene by selecting the block (For local vector fields, a Visual Effect component must be attached to the Visual Effect asset being edited).

<https://github.com/Unity-Technologies/VFXToolbox>

<https://twitter.com/peeweekVFX/status/1055934102887575552>

free version of Houdini

video tutorial

https://www.youtube.com/watch?v=z1Am4DIDEzw

https://jangafx.com/software/vectoraygen/

 

I will try to be concise :

- TotalTime & DeltaTime are uniform provided by VisualEffect.

- Age & Lifetime are particles attributes

- Age Over Lifetime
   is an helper of a simple division between age (automatically 
   incremented in Update) and lifetime (particle are killed after lifetime 
   in Update, by default).


​	
​	
​		




- Per Particle Total Time is also a helper, it helps to avoid discretization as explained here : https://twitter.com/peeweekVFX/status/1055599003989987328


​	
​	
​		


(About this, we planned to provide a better way to handle discretization
 & interpolation with parameter of the visual effect graph)



- SpawnTime is an attribute, it is used with custom spawner block "SetSpawnTime"
   which copies the internal total time of a spawner context (which is 
   different from the global total time) into a custom attribute. It's 
   pretty specific and completely experimental...


​	
​	

 