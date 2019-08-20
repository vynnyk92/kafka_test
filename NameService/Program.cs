﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MessageBroker.Kafka.Lib;

namespace NameService
{
    namespace NameService
    {
        class Program
        {
            private static MessageBus msgBus;
            private static readonly string userHelpMsg = "NameService.\nEnter 'b' to process boy names respectively";
            private static readonly string bTopicNameCmd = "b_name_command";
            private static readonly string bTopicNameResp = "b_name_response";

            private static readonly string[] _boyNames =
            {
            "Arsenii",
            "Igor",
            "Kostya",
            "Ivan",
            "Dmitrii",
        };


            static void Main(string[] args)
            {
                bool canceled = false;

                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true;
                    canceled = true;
                };

                using (msgBus = new MessageBus())
                {
                    Console.WriteLine(userHelpMsg);

                    HandleUserInput(Console.ReadLine());

                    while (!canceled) { }
                }
            }

            private static void HandleUserInput(string userInput)
            {
                switch (userInput)
                {
                    case "b":
                        Task.Run(() => msgBus.SubscribeOnTopic<string>(bTopicNameCmd, (msg) => BoyNameCommandListener(msg), CancellationToken.None));
                        Console.WriteLine("Processing boy names");
                        break;
                    default:
                        Console.WriteLine($"Unknown command. {userHelpMsg}");
                        HandleUserInput(Console.ReadLine());
                        break;
                }
            }

            private static void BoyNameCommandListener(string msg)
            {
                var r = new Random().Next(0, 5);
                var randName = _boyNames[r];

                msgBus.SendMessage(bTopicNameResp, randName);
                Console.WriteLine($"Sending {randName}");
            }

        }
    }
}
