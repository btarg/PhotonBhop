This example show how an outer loop for a game could be structured to work properly with Fusion when loading scenes and setting up and tearing down connections as well as providing basic matchmaking functionality.

More specifically, the example allow players to either create or join sessions with some mock attributes like game mode and map name. It presents a list of sessions to joining users and allow them to configure their avatar before loading the game scene. The example also handles both clients and hosts leaving the session and returning to the intro.

To run the sample, first create a Fusion app Id (https://dashboard.photonengine.com) and paste it into the `App Id Fusion` field in Real Time Settings (reachable from the Fusion menu). Then load the `Launch` scene and press `Play`.

>Prefabs

* App.prefab: This is the main App launcher. It's a singleton and can be dropped into any scene to launch the application from that scene (useful when running in the editor). It has the ability to automatically create a session with some preset values to skip the entire matchmaking process when running from the editor
* Character.prefab: The player avatar - one instance of this prefab is spawned for each player as they enter a map. The character lives only until the map is unloaded.
* Player.prefab: The player session properties - one instance of this prefab is spawned for each player when the session starts. The Player instance survives scene loading and lives until the session is disconnected.
* Session.prefab: The shared session properties - on instance of this prefab is created when the session starts.

>Scenes

* `0.Launch` - The launch scene is only ever used in builds and holds only an instance of the `App` singleton. Configure this instance for builds to ensure you don't accidentally build with a debug (auto connect) configuration of the App.
* `1.Intro` - The intro scene contains the pre-game UI before a connection is established - this is where a topology/client mode and game type is chosen. It also contains the UI for selecting a session to join and for creating a new session. This is where the app will return to if the connection is lost or shut down.
* `2.Staging` - The staging scene is loaded once a network session is established and allow players to configure their avatar and signal to the host that they are ready to play. The app may return here whenever the players need to configure their avatar and indicate that they are ready to play.
* `X.MapY` scenes are actual game maps - each game map instantiates player avatars based on the players configuration from the staging area and tells the host when they're done loading so the game starts at the same time on all clients, even if some are slow to load. The host may move to the next game scene, all clients can disconnect at will.
* `GameOver` - The GameOver scene is essentially just a map where the players don't get an avatar. It could be used to show match results, and just takes players back to the staging area.

>Behaviours

Code in the `GameUI`, `UIComponents` and `Utility` folders are not specific to this example and will not be discussed further.

* `App` The primary entry point for the example. Has methods to create and destroy a game session as well as for keeping track of active players. It implements the main Fusion callbacks.
* `Character` The player in-game avatar - controls basic movement of player characters.
* `Map` The map is simply a network object that exists in actual game scenes and is responsible for spawning the players avatar in that scene.
* `MapLoader` This is the Object Provider implementation for Fusion and controls the scene-load sequence from showing a load screen to collecting a list of loaded Network Objects.
* `Session` Once the first player is connected, a single Session object is spawned and parented to the `App` so that it too will not be destroyed on load. The session controls logic for loading maps and can be access via the App (`App.Instance.Session`).
* `Player` Each player gets a Player object when joining a session and this is also parented to the session game object to keep them alive across scene loads. The Player object has no in-game visual representation, it's just an encapsulation of player information that is shared with all clients.
