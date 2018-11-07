using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BarbaricCode.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BarbaricCode { namespace Networking {
        public static partial class UserHandlers
        {

            // all the flow handlers
            [UserDataHandler((int)FlowMessageType.PLAY)]
            public static void PlayFlowHandler(int nodeID, int connectionID, byte[] buffer, int recieveSize)
            {
                Debug.Log("loading play scene");
                SceneManager.LoadScene("PlayScene");
            }

            [UserDataHandler((int)FlowMessageType.LOAD_FINISH)]
            public static void FinishLoadHandler(int nodeID, int connectionID, byte[] buffer, int recieveSize)
            {
                if (!NetEngine.IsServer)
                {
                    return;
                }

                Debug.Log("Finish loading");
                if (!GameState.loadedNodes.Contains(nodeID) && nodeID != 0)
                {
                    GameState.loadedNodes.Add(nodeID);
                    GameState.players[nodeID].loaded = true;
                }

                foreach (Connection c in NetEngine.Connections.Values)
                {
                    Debug.Log("Connection " + c.nodeID);
                    if (!GameState.loadedNodes.Contains(c.nodeID))
                    {
                        return;
                    }
                }

                Debug.Log("Start Play");
                // spawn person for player
                // should run on server.
                foreach (Connection c in NetEngine.Connections.Values)
                {
                    Debug.Log("Spawning for " + c.nodeID);
                    if (GameState.players[c.nodeID].role == GameRole.GENERAL)
                    {
                        NetEngine.Spawn(1, c.nodeID);
                    }
                    else
                    {
                        NetEngine.Spawn(0, c.nodeID);
                    }

                }
                // spawn for server
                if (GameState.players[0].role == GameRole.GENERAL)
                {
                    NetEngine.Spawn(1, 0);
                }
                else
                {
                    NetEngine.Spawn(0, 0);
                }
            }

            [UserDataHandler((int)FlowMessageType.PLAYER_DIED)]
            public static void PLAYER_DIED(int nodeID, int connectionID, byte[] buffer, int recieveSize)
            {
                if (!NetEngine.IsServer)
                {
                    return;
                }

                // respawn player for that node
                NetEngine.Spawn(0, nodeID);

            }

            [UserDataHandler((int)FlowMessageType.ROLE_CHANGE_GENERAL)]
            public static void RoleChangeGeneralHandler(int nodeID, int connectionID, byte[] buffer, int recieveSize)
            {
                if (!NetEngine.IsServer)
                {
                    return;
                }
                GameState.players[nodeID].role = GameRole.GENERAL;
            }

            [UserDataHandler((int)FlowMessageType.ROLE_CHANGE_SOLDIER)]
            public static void RoleChangeSoldierHandler(int nodeID, int connectionID, byte[] buffer, int recieveSize)
            {
                if (!NetEngine.IsServer)
                {
                    return;
                }
                GameState.players[nodeID].role = GameRole.SOLDIER;
            }

        }
    } }
