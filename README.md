# Wiki-Graph
## Disentangling the web of knowledge
---
### Quick Start

1. Install Unity ( [Download](https://unity3d.com/))
2. Pull project from this repo, open in Unity
3. Open the Assets/Scene folder and click "standard" to use default scene
4. Next build standalone for your system
    1. File -> Build Settings (opens new window)          2. Select your system
    3. Click "Build and Run" (use wiki-graph/Builds folder)
4. Check for errors (note: In linux debug error logs recorded ~/.config/unity3d/DefaultCompany/wiki-graph)
---
### Main items

1. Prefabs: prefabricated templates that are the equivalent of objects in the virtual world. They have components (scripts, meshes, materials) attached to them.
    1. Node, Links : basic types used in drawing the grapha. GameController: Equivalent to void main() method - runs the world. Must point to the Node and Link prefabs so it knows what to instantiate when it runs.
    3. EventSystem: Catches user mouse clicks on in-game objects and invokes appropriate listeners
    4. Main Camera: The camera you view the world through. Has two scripts attached to it which govern how camera moves through world.

2. Plugins - various libraries and internal code that is used for the game. I wrote "Program.cs", which includes basic incarnations of how to connect to wikipedia and drag references to other sites from it (needs further work).

3. Scenes: Folder for different variants of the world, but ours is simple
