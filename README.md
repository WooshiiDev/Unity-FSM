
<h1 align="center">  
 Unity Finite State Machine (FSM)
</h1>

<h4 align="center">A powerful lightweight Finite State Machine for Unity, taking advantage of reflection and the editor.</h4>

<p align="center">
 <a href="https://unity3d.com/get-unity/download">
 <img src="https://img.shields.io/badge/unity-2018.4%2B-blue.svg" alt="Unity Download Link">
 <a href="https://github.com/WooshiiDev/HierarchyDecorator/blob/master/LICENSE">
 <img src="https://img.shields.io/badge/License-MIT-brightgreen.svg" alt="License MIT">
</p>
  
<p align="center">
  <a href="#about">About</a> •
  <a href="#usage">Usage</a> •
  <a href="#installation">Installation</a> •
  <a href="#support">Support</a>
</p>

## About
This Unity FSM is extremely lightweight but carried by the power and utility of the Unity Editor. 

Rather than handling states through ScriptableObjects or MonoBehaviours, all states are light classes that are all collected and edited within the FSM Manager. From the FSM Manager, custom default values can be assigned without requiring any form of recompile or edit to the original script.

<p align="center">
 <img src="https://i.imgur.com/z9hMYBU.png" alt="Unity Download Link">
</p>

All public fields that are within the state will be shown and can be edited from the manager. When an instance changes to that state, all set values will be passed over. 

**Use case examples will be soon added to the repository!**

## Usage

Because of it's extremely light system, there are no transition behaviour or conditions you see in other state machines. It is purely a state only system. On each compile, new created states will be recognized by the state manager, so they can also be edited.

### Scene Setup

Running any FSM is extremely simple - all you require is a manager component so that the states can be retrieved:
![image](https://user-images.githubusercontent.com/31889435/118346401-01050d80-b533-11eb-9c60-1a0c232d74fd.png)

And for instances, just add a State Machine to the required game objects. From here you can select the state the instance will start with, and see runtime information on the current state and the previously ran one.
![image](https://user-images.githubusercontent.com/31889435/118346394-f5b1e200-b532-11eb-96cc-a08a93762e14.png)

### State Creation 101

To create a state, simply extend the base state or any custom state made:

```cs
public class ExampleState : State
{

}
```

States will be grouped in the FSM Manager based on their namespace. This decision was chosen as it was the most convenient and structured way to group states based on the structure of the project.

For example, if `MoveState` was in `States.Character.Move`, this would appear in the "Move" category in the Manager:

![image](https://user-images.githubusercontent.com/31889435/118346093-082b1c00-b531-11eb-900b-0d8b85f88b0c.png)

Categories will always take the final term in the namespace.

### State Methods

The following methods are the standards in each state:

```cs
/// <summary>
/// Called when entering the state. Use for getting references or setting up behaviour
/// </summary>
public virtual void OnEnter()
{

}

/// <summary>
/// Called when exiting the state.
/// </summary>
public virtual void OnExit()
{

}

/// <summary>
/// Update tick
/// </summary>
public virtual void Tick(float delta)
{
    age += Time.deltaTime;
}

/// <summary>
/// Fixed update tick
/// </summary>
public virtual void FixedTick(float delta)
{
    fixedAge += Time.fixedDeltaTime;
}

/// <summary>
/// Late update tick
/// </summary>
public virtual void LateTick(float delta)
{

}

/// <summary>
/// Debug update tick
/// </summary>
public virtual void DebugTick(float delta)
{

}
```

`OnEnter` - Will be called once when the state is created.

`OnExit` - Will be called once when the state is exited.


Each update method has a delta parameter that will pass Time.DeltaTime. For FixedUpdate this turns into FixedDeltaTime.

`Tick(float delta)` - Represents Update

`FixedTick(float delta)` - Represents FixedUpdate

`LateTick(float delta)` - Represents LateUpdate

`DebugTick(float delta)` - An additional update method for OnDrawGizmos. Normally used for debugging.

### Switching States

Switching states is done through accessing the FSM MonoBehaviour on the GameObject:

```cs
// Inside a State Class...
public override void Tick(float delta)
{
  // If the state is older than or equal to 10 seconds
  if (age >= 10)
  {
    parent.SetState(new OtherState());
  }
}
```

### Switching State Parents

If for any reason states require a new parent, you can do so through the `SetParent(StateMachine parent)` method.

## Installation
<p align="center">
<a href="https://github.com/WooshiiDev/Unity-FSM/releases/download/v1.0.0/Unity-FSM-v1.0.0.unitypackage">Unity Package</a> •
 <a href="https://github.com/WooshiiDev/Unity-FSM/archive/refs/tags/v1.0.0.zip">Zip</a> •
 <a href="https://github.com/WooshiiDev/Unity-FSM/archive/refs/tags/v1.0.0.tar.gz">Tar</a> 
</p>

You can also clone the repository through git using the following:
```git clone https://github.com/WooshiiDev/Unity-FSM.git```

## Support
Please submit any queries, bugs or issues, to the [Issues](https://github.com/WooshiiDev/Unity-FSM/issues) page on this repository. All feedback and support is massively appreciated as it not only helps me, but allows the projects I work on to be improved.

Reach out to me or see my other work through:

 - Website: https://wooshii.dev/
 - Email: wooshiidev@gmail.com;
