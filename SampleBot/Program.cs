﻿using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BattleIA;

namespace SampleBot
{
    class Program
    {
        //private static string serverUrl = "wss://localhost:44367/ia";
        //private static string serverUrl = "wss://10.26.1.182:44367/ia";
        //private static string serverUrl = "wss://ly0500:44367/ia";
        //private static string serverUrl = "ws://ly0500:51973/ia";
        private static string serverUrl = "ws://127.0.0.1:2626/ia";
        private static string botName = "RandomBOT";

        static void Main(string[] args)
        {
            Console.WriteLine("SampleBot");
            DoWork().GetAwaiter().GetResult();
            Console.WriteLine("Bye");
            Console.WriteLine("Press [ENTER] to exit.");
            Console.ReadLine();
        }

        private static MyIA ia = new MyIA();
        private static Bot bot = new Bot();
        private static UInt16 turn = 0;


        static async Task DoWork()
        {

            // 1 - connect to server

            var client = new ClientWebSocket();
            Console.WriteLine($"Connecting to {serverUrl}");
            try
            {
                await client.ConnectAsync(new Uri(serverUrl), CancellationToken.None);
            }
            catch (Exception err)
            {
                Console.WriteLine($"[ERROR] {err.Message}");
                return;
            }

            // 2 - Hello message with our GUID

            Guid guid = Guid.NewGuid();
            var bytes = Encoding.UTF8.GetBytes(guid.ToString());
            Console.WriteLine($"Sending our GUID: {guid}");
            await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);

            // 3 - wait data from server

            bool nameIsSent = false;
            bool isDead = false;

            var buffer = new byte[1024 * 4];
            while (!isDead)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    if (result.Count > 0)
                    {
                        string command = System.Text.Encoding.UTF8.GetString(buffer, 0, 1);
                        switch (command)
                        {
                            case "O": // OK, rien à faire
                                if (result.Count != (int)MessageSize.OK) { Console.WriteLine($"[ERROR] wrong size for 'OK': {result.Count}"); break; }
                                if(!nameIsSent)
                                {
                                    nameIsSent = true;
                                    // sending our name
                                    var bName = Encoding.UTF8.GetBytes("N" + botName);
                                    Console.WriteLine($"Sending our name: {botName}");
                                    await client.SendAsync(new ArraySegment<byte>(bName), WebSocketMessageType.Text, true, CancellationToken.None);
                                    break;
                                }
                                Console.WriteLine("OK, waiting our turn...");
                                break;
                            case "T": // nouveau tour, attend le niveau de détection désiré
                                if (result.Count != (int)MessageSize.Turn) { Console.WriteLine($"[ERROR] wrong size for 'T': {result.Count}"); DebugWriteArray(buffer, result.Count); break; }
                                turn = (UInt16)(buffer[1] + (buffer[2] << 8));
                                bot.Energy = (UInt16)(buffer[3] + (buffer[4] << 8));
                                bot.ShieldLevel = (UInt16)(buffer[5] + (buffer[6] << 8));
                                bot.CloakLevel = (UInt16)(buffer[7] + (buffer[8] << 8));
                                Console.WriteLine($"Turn #{turn} - Energy: {bot.Energy}, Shield: {bot.ShieldLevel}, Cloak: {bot.CloakLevel}");
                                ia.StatusReport(turn, bot.Energy, bot.ShieldLevel, false);
                                if (bot.Energy == 0) break;
                                // must answer with D#
                                var answerD = new byte[2];
                                answerD[0] = System.Text.Encoding.ASCII.GetBytes("D")[0];
                                answerD[1] = ia.GetScanSurface();
                                Console.WriteLine($"Sending Scan: {answerD[1]}");
                                await client.SendAsync(new ArraySegment<byte>(answerD), WebSocketMessageType.Text, true, CancellationToken.None);
                                break;
                            case "C": // nos infos ont changées
                                if (result.Count != (int)MessageSize.Change) { Console.WriteLine($"[ERROR] wrong size for 'C': {result.Count}"); DebugWriteArray(buffer, result.Count); break; }
                                bot.Energy = (UInt16)(buffer[1] + (buffer[2] << 8));
                                bot.ShieldLevel = (UInt16)(buffer[3] + (buffer[4] << 8));
                                bot.CloakLevel = (UInt16)(buffer[5] + (buffer[6] << 8));
                                Console.WriteLine($"Change - Energy: {bot.Energy}, Shield: {bot.ShieldLevel}, Cloak: {bot.CloakLevel}");
                                ia.StatusReport(turn, bot.Energy, bot.ShieldLevel, false);
                                // nothing to reply
                                if (bot.Energy == 0) break;
                                break;
                            case "I": // info sur détection, attend l'action à effectuer
                                byte surface = buffer[1];
                                int all = surface * surface;
                                if (result.Count != (2 + all)) { Console.WriteLine($"[ERROR] wrong size for 'I': {result.Count}"); break; } // I#+data so 2 + surface :)
                                var x = new byte[all];
                                Array.Copy(buffer, 2, x, 0, all);
                                ia.AreaInformation(surface, x);
                                // must answer with action Move / Shield / Cloak / Shoot / None
                                var answerA = ia.GetAction(); // (byte)BotAction.None; // System.Text.Encoding.ASCII.GetBytes("N")[0];
                                Console.WriteLine($"Sending Action: {(BotAction)answerA[0]}");
                                await client.SendAsync(new ArraySegment<byte>(answerA), WebSocketMessageType.Text, true, CancellationToken.None);
                                break;
                            case "D":
                                isDead = true;
                                Console.WriteLine($"We are dead!");
                                await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                                break;
                        }
                    } // if count > 1
                    else
                    {
                        Console.WriteLine("[ERROR] " + Encoding.UTF8.GetString(buffer, 0, result.Count));
                    }
                } // if text
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine($"End with code {result.CloseStatus}: {result.CloseStatusDescription}");
                    await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }
            } // while
            Console.WriteLine("Just dead!");
        } // DoWork

        private static void DebugWriteArray(byte[] data, int length)
        {
            if (length == 0) return;
            Console.Write($"[{data[0]}");
            for (int i = 1; i < length; i++)
            {
                Console.Write($", {data[i]}");
            }
            Console.Write("]");
        }
    }
}