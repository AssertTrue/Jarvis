using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Jarvis.command;

namespace Jarvis.alias
{
    class Alias : ICommand
    {
        public Alias()
        {
            this.commands = new Dictionary<string, string[]>();
        }

        public string exportAsCommands()
        {
            string commandList = string.Empty;
            foreach (var command in this.commands)
            {
                string createCommand = "alias \"" + command.Key + "\"";
                foreach (string argument in command.Value)
                {
                    createCommand += " \"" + argument + "\"";
                }
                commandList += createCommand + Environment.NewLine;
            }
            return commandList;
        }

        #region Implementation of ICommand

        public bool handle(string aCommand, string[] aArguments)
        {
            if (aCommand == "alias")
            {
                List<string> arguments = new List<string>(aArguments);

                switch (aArguments.Count())
                {
                    case 0:
                        break;
                    case 1:
                        if (this.commands.ContainsKey(aArguments[0]))
                        {
                            this.commands.Remove(arguments[0]);
                        }
                        break;
                    default:
                        this.commands[arguments[0]] = arguments.GetRange(1, arguments.Count - 1).ToArray();
                        break;
                }
            }
            else if (this.commands.ContainsKey(aCommand))
            {
                string[] commandArguments = this.commands[aCommand];

                string fileName = commandArguments[0];
                string argumentsAsString = "";

                for (int index = 1; index < commandArguments.Length; ++index)
                {
                    argumentsAsString += commandArguments[index] + " ";
                }
                for (int index = 0; index < aArguments.Length; ++index)
                {
                    argumentsAsString += aArguments[index] + " ";
                }

                System.Diagnostics.Process.Start(fileName, argumentsAsString.Trim());
            }

            return false;
        }

        public IEnumerable<string> Commands
        {
            get { return this.commands.Keys; }
        }

        #endregion

        private readonly Dictionary<string, string[]> commands;
    }
}
