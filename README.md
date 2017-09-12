# GameJam Starter Project
---
- [Advice Doc](https://docs.google.com/a/improbable.io/document/d/1AuNnuTwFS7f6TDnxDm0Ukeqbk82Fi9jw_kTc4gwUYhQ/edit?usp=sharing)
- *GitHub repository*: [https://github.com/alastairdglennie/gamejam-starter-project](https://github.com/alastairdglennie/gamejam-starter-project)

---

## Introduction

This is a SpatialOS starter project for a gamejam.

It contains:

* A Player spawned on client connection as per the [Unity Client Lifecycle Guide](https://spatialos.improbable.io/docs/reference/latest/tutorials/client-lifecycle).
* A Script to visualize and interpolate other players within the game.
* 3 different scripts to control player using either a first person, over the shoulder or third person camera.
* A Cube spawned through a snapshot via an entity template method and an Unity prefab.
* The rest of the features included in the [BlankProject](https://github.com/spatialos/BlankProject).

## Choosing a camera

With the project open in Unity, locate the `Player` prefab in the `EntityPrefabs` folder. The `OtherPlayerMovement` script is responsible for visualizing other players in your game as should remain on the prefab no matter what camera control you want. By default there is also a `ThirdPersonPlayerControls` script which, as you might imagine, gives the user a 3rd person camera to control the player. If you wish to change the camera controls, then first remove the `ThirdPersonPlayerControls` script and add either the `FirstPersonPlayerControls` or `OverTheShoulderPlayerControls` inside the `Gamelogic > Player` folder within Unity. Remember you'll have to also add the refernces back to the newly added MonoBehaviour. Any questions ping @alastair on Slack!

## Running the project

To run the project locally, first build it by running `spatial worker build`, then start the server with `spatial local start`. You can connect a client by opening the Unity project and pressing the play button, or by running `spatial local worker launch UnityClient default`. See the [documentation](https://spatialos.improbable.io/docs/reference/latest/developing/local/run) for more details.

To deploy the project to the cloud, first build it by running `spatial worker build -t=deployment`, then upload the assembly with `spatial cloud upload <assembly name>`, and finally deploy it with `spatial cloud launch <assembly name> <launch configuration file> <deployment name> --snapshot=<snapshot file>`. You can obtain and share links to connect to the deployment from the [console](http://console.improbable.io/projects). See the [documentation](https://spatialos.improbable.io/docs/reference/latest/developing/deploy-an-application) for more details.
