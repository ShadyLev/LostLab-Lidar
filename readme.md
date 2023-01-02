# Lost Lab
### Project description:
**Lost Lab** is a Unity3D university project presenting a main game mechanic inspired by games like [Gmod LiDAR](https://steamcommunity.com/workshop/filedetails/?id=2813176307) mod or [Scanner Sombre](https://store.steampowered.com/app/475190/Scanner_Sombre/). 

You are tasked with carrying out a mission for a secret government agency. You must retrieve artefacts from a anomaly called the Lost Lab.

All you have is a scanner to detect the environment and a radar to locate the artefacts near by. 

You can download the game demo on itch.io: (comming very soon!)

# Scanner mechanic
### Description
The main point of the scanner mechanic is to display a large amount of points that will be placed on the level geometry. The more points displayed the more accurate the level layout will become. 
<p align="center">
  <img src="./Images/LostLabScreenshot1.png" alt="Enviroment example 1" width="40%" />
  <img src="./Images/LostLabScreenshot2.png" alt="Enviroment example 2" width="40%" />
</p>

## Methods

### Gameobject
The first, and most obvious, method to achieve this effect is using gameobjects. We simply have to instantiate prefabs in the place of a raycast hit. 
The problem with this method is that it is very inefficient as the engine has to spawn and render thousands of gameobjects at once. This results in the FPS dropping very quickly at around 30-50 thousand of spawned points.
Even after enabling GPU Instancing on the said gameobject materials, it hardly helped. This method would not work out.

#### Decals
After gameobjects I quickly tried using decals but it ended with the same results as the previous method.

### Unity Particle System
My next best idea was to utilise Unity's built in particle system which greatly increases the amount of points I can spawn. 
By using the [ParticleSystem.Emit](https://docs.unity3d.com/ScriptReference/ParticleSystem.Emit.html) function I can specify the position, color, spawn count and more of particles. 
<p align="center">
  <img src="./Images/Example1.gif" alt="Unity particle system solution example." width="50%"/>
</p>
Unity's shuriken particle system, however great for small system, breaks down on a larger scale as it is primarily run on the CPU. 
The same as with the Gameobject method, enabling GPU instancing did not solve the optimisation problem.

### VFX Graph
After realising that I will need to utilise the GPU to display the amount of points I will need I discovered VFX Graph. A particle system that uses the GPU to be more optimised. 
[Brackey's video](https://www.youtube.com/watch?v=FvZNVQuLDjI) on VFX Graph introduced me to this Unity package and showed that I could easly display up to 80 million points at one time! Which is perfect as I wanted the ability to view the entire map covered in points. 
The last question remained, how to communicate with VFX Graph my point information like position and color?

#### Texture2D
During my research on VFX Graph I saw a post saying how to send position data to VFX Graph using a Texture2D.
It was easy and very smart. To implement it all I had to do was:
- Create a Visual Effects object (VFX Graph) and set the particle capacity to 16384 (Max texture2D width/height).
- Create a Texture2D with the same width (16384) and RGBAFloat format;
- Store my Vector3 positions in a list and transform that list into a array of Colors.
R = X, G = Y, B = Z, A = alpha of the particle.
- Set the texture pixels to the colors form the array and send it to the VFX Graph.

This way I could display up to 16384 points in my graph. When i would exceed that amount I could simply create a new graph with a new texture.

The only problem with this solution is the lack of customizability. I could only send position data and the alpha color of the particle.

#### Graphics Buffer
After more research I stumbled upon a forum post ini which one of the replies recommended using a graphics buffer to send custom particle data to the VFX Graph.
This was exactly what I was looking for.
So I created my own CustomVFXData buffer like this:
```
    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    struct CustomVFXData
    {
        public Vector3 position;
        public Vector4 color;
        public int useDefaultGradient;
        public float size;
    }
    // List of custom Data points
    private List<CustomVFXData> m_CustomVFXData = new List<CustomVFXData>();
```
Now having this custom data struct and a graphics buffer I was able to follow these points and achieve the result I got:
- Create a new graphics buffer and set its stride to size of CustomVFXData and the buffer size to 10k (can be larger but read time will increase).
- Each time I want to create a new point I create a new CustomVFXData, fill it with my values and add it to the m_CustomVFXData list.
- When I want to display the points I use [GraphicsBuffer.SetData()](https://docs.unity3d.com/ScriptReference/GraphicsBuffer.SetData.html) with my custom data list and reinitialize the VFX Graph.

This way I can sample the graphics buffer in the VFX Graph and use its data to create new particles.
**Important!** It is crucial to not only check if we are accidentaly adding more data to the buffer than its size as that will crash but also release the buffer after not using it.
After filling up the buffer to the set max size I create a new VisualEffects object and release the graphics buffer.

This way I can display millions of points, and assign custom data to them such as different colour based on tags or a default gradient:

https://media.github.falmouth.ac.uk/user/619/files/e96704af-b4f2-44b3-8195-7525e4379e5e
