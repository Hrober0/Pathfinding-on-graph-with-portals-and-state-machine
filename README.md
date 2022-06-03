# Pathfinding on graph with portals and state machine

Pathfinding consists of solutions for setting and organizing paths.
The set points create the graph on which the paths are searched for.

The state machine is used by MPC (guide cube) to define its behavior.
Guide cube controller implements basic logic as switch states and also methods allow moving at the path.
States were projected to easily add new behaviors.
There are 3 basic states:
- Idle
- Go to
- Follow object


Pathfinding video: [youtube.com/hrober/pathfinding](https://www.youtube.com/watch?v=cvg2pc2LpKg)

The whole system was used in-game named Errata.
Errata trailer: [youtube.com/errata-trailer](https://www.youtube.com/watch?v=JyS9zIQbpxQ)


## Interesting parts

Pathfinding main script:<br>
[/github.com/Hrober0/Pathfinding-on-graph-with-portals-and-state-machine/Scripts/Pathfinder.cs](https://github.com/Hrober0/Pathfinding-on-graph-with-portals-and-state-machine/blob/main/Assets/Pathfinding/Scripts/Pathfinder.cs)

State machine controller:<br>
[/github.com/Hrober0/Pathfinding-on-graph-with-portals-and-state-machine/Scripts/GCubeController.cs](https://github.com/Hrober0/Pathfinding-on-graph-with-portals-and-state-machine/blob/main/Assets/CubeStateMachine/Scripts/GCubeController.cs)

Folder with states:<br>
[/github.com/Hrober0/Pathfinding-on-graph-with-portals-and-state-machine/Scripts/States](https://github.com/Hrober0/Pathfinding-on-graph-with-portals-and-state-machine/tree/main/Assets/CubeStateMachine/Scripts/States)


## Used Technologies

#### Unity and C#

#### Additional package:
- NaughtyAttributes (extension for the Unity Inspector):  https://github.com/dbrizov/NaughtyAttributes
