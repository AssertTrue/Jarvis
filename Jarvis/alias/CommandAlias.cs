using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jarvis.command;

namespace Jarvis.alias
{
    class CommandAlias : ICommand
    {
        public CommandAlias()
        {
            this.commands = new Dictionary<string, Func<string[], bool>>();
        }

        public void add(string aCommandAlias, Func<string[], bool> aCommand)
        {
            this.commands[aCommandAlias] = aCommand;
        }

        #region Implementation of ICommand

        public bool handle(string aCommand, string[] aArguments)
        {
            bool result = false;

            if (this.commands.ContainsKey(aCommand))
            {
                result= this.commands[aCommand](aArguments);
            }

            return result;
        }

        public IEnumerable<string> Commands
        {
            get { return this.commands.Keys; }
        }

        #endregion

        private readonly Dictionary<string, Func<string[],bool>> commands;
    }
}
