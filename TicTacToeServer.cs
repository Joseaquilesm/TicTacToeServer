using System;
using System.Collections.Generic;
using DarkRift;
using DarkRift.Server;
using Scripts.Models;
using Scripts.Networking;

namespace TicTacToeServer
{
    public class TicTacToeServer : Plugin
    {
        public override bool ThreadSafe => false;

        public override Version Version => new Version(0,0,1);

        private Player pendingPlayer;
        private Dictionary<ushort, Match> matches;


        public TicTacToeServer(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            ClientManager.ClientConnected += OnClientConnected;
            ClientManager.ClientDisconnected += OnClientDisconnected;
            matches = new Dictionary<ushort, Match>();

        }
        private void OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Logger.Info("Hello " + e.Client.ID);
            e.Client.MessageReceived += OnClientMessageReceived;
        }

       

        private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Logger.Info("Goodbye " + e.Client.ID);
            if(pendingPlayer !=null && pendingPlayer.Client == e.Client)
            {
                pendingPlayer = null;
            }
        }

        private void OnClientMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            switch (e.Tag)
            {
                case (ushort)tags.Tag.SET_NAME:
                    // Registering new player

                    using ( Message message = e.GetMessage())
                    {
                        using (DarkRiftReader reader = message.GetReader())
                        {
                           
                           string name = reader.ReadString();
                            Console.WriteLine("Hello " + name, ConsoleColor.Green);

                            Player newPlayer = new Player(e.Client, name);

                            if (pendingPlayer == null)
                            {
                                // Player is waiting for a match
                                pendingPlayer = newPlayer;
                            }
                            else
                            {
                                // start a new match if there's a player waiting
                                Match match = new Match(pendingPlayer, newPlayer);
                                matches.Add(match.id,match);
                                Console.WriteLine("The match has been added and its ID is: " + match.id);
                                // send a message that match has been found


                                using (DarkRiftWriter writer = DarkRiftWriter.Create())
                                {
                                    writer.Write(match.id);
                                    Console.WriteLine("The match has been wrote and its ID is: " + match.id);
                                    using (Message connectedMessage = Message.Create((ushort) tags.Tag.MATCH_CONNECTED,  writer))
                                    {
                                        //Reliable TCP
                                        //Unreliable UDP
                                        pendingPlayer.Client.SendMessage(connectedMessage, SendMode.Reliable);
                                        newPlayer.Client.SendMessage(connectedMessage, SendMode.Reliable);
                                        Console.WriteLine("The messages have been sent and the match ID is: " + match.id);

                                    }
                                }

                                pendingPlayer = null;
                            }

                        }
                    }

                    break;

                case (ushort)tags.Tag.SPACE_TAKEN:

                    using(Message message = e.GetMessage())
                    {
                        using(DarkRiftReader reader = message.GetReader())
                        {

                            ushort matchId = reader.ReadUInt16();
                            ushort spaceTaken = reader.ReadUInt16();

                            if (matches.ContainsKey(matchId))
                            {
                                Match match = matches[matchId];
                                match.PlayerSpaceClicked(spaceTaken, e.Client);
                            }


                           
                        }
                    }

                    break;
            }
        }
    }
}
