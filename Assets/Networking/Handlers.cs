using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// the partial handler lets you define parts of a class separately
// we use this here to separate core NetEngine functions
// from user specified
namespace BarbaricCode
{
    namespace Networking
    {
        // tells the netengine to iterate through this class to grab handlers. possibly unneded
        // @TODO
        [HandlerClass]
        public static partial class Handlers
        {

            // Segment Handlers

            // in order for your handler to get registered
            // it must have this attribute
            // [SegHandle(type of message)]
            // And it must have this method signature
            // (int nodeID, int connectionID, byte[] buffer, int recieveSize)
            // After that it will be automatically registered at runtime
            [SegHandle(MessageType.GOT_HIT)]
            public static void Hit(int nodeID, int connectionID, byte[] buffer, int recieveSize)
            {
                HIT hit = NetworkSerializer.ByteArrayToStructure<HIT>(buffer);
                NetEngine.NetworkObjects[hit.NetID].gameObject.GetComponent<Damageable>().getHit(hit.Damage);
            }

            // Network Handlers
            // User space custom handlers
            [NetEngineHandler(NetEngineEvent.NewConnection)]
            public static void OnPlayerConnected(int nodeid, int connectionID, byte[] buffer, int recieveSize) {
                MainMenuContextController.instance.RemoveMask();
                MenuContextController.instance.SwitchToNetworkLobby();
            }

            [NetEngineHandler(NetEngineEvent.ConnectionFailed)]
            public static void OnConnectionFailed(int nodeid, int connectionID, byte[] buffer, int recieveSize) {
                MenuContextController.instance.CreateAlert("ConnectionFailed", MainMenuContextController.instance.gameObject);
                MainMenuContextController.instance.RemoveMask();
                NetEngine.CloseSocket();
            }

            [NetEngineHandler(NetEngineEvent.Hosted)]
            public static void OnHosted(int nodeid, int connectionID, byte[] buffer, int recieveSize) {
                MenuContextController.instance.SwitchToNetworkLobby();
            }

            [NetEngineHandler(NetEngineEvent.HostDisconnect)]
            public static void OnHostDisconnect(int nodeid, int connectionID, byte[] buffer, int recieveSize)
            {
                // if i was the host and i disconnected normally should not show anything.

            }

			[NetEngineHandler(NetEngineEvent.Timeout)]
			public static void OnTimeout(int nodeid, int connectionID, byte[] buffer, int recieveSize) {
				/*
				 * If we are the client ->
				 * 		-> Trying to connect in menu
				 * 			-> Send a notify to menu and let them handle it
				 * 		-> We are in game
				 * 			-> Temporary just disconnect and back to main menu
				 * 			-> Try reconnecting, (this is hard)
				 *		-> Lobby
				 *			-> Temporary disconnet & back to main menu
				 * If we are the host ->
				 * 		-> We are in game
				 * 			-> Client disconnects, maybe delete their object for now (temp)
				 * 		-> Lobby
				 * 			-> Disconnect that person
				*/

				if (!NetEngine.IsServer) {
					// client code

					// Assume we are in the menu for now
					MainMenuContextController.instance.RemoveMask();
					MenuContextController.instance.CreateAlert ("Failed to connect: Timeout", MainMenuContextController.instance.gameObject);
				}
			}

			[NetEngineHandler(NetEngineEvent.FlowControl)]
			public static void HandleFlowMessage(int nodeid, int connectionID, byte[] buffer, int recieveSize) {
				FlowMessage fm = NetworkSerializer.ByteArrayToStructure<FlowMessage>(buffer);
                flow nextflow= (flow) fm.Message;
                
                // server issued the message
                GameState.currentFlowStatus = nextflow;
                FlowControl.FlowHandlerMapping[nextflow].Invoke();

			}
        }
    }
}
