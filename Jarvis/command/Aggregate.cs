using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jarvis.command
{
    class Aggregate : ICommand
    {
        public Aggregate()
        {
            this.commands = new List<ICommand>();
        }

        public void addCommand(ICommand aCommand)
        {
            this.commands.Add(aCommand);
        }

        private List<ICommand> commands;

        #region Implementation of ICommand

        public bool handle(string aCommand, string[] aArguments)
        {
            bool result = false;
            foreach (ICommand command in this.commands)
            {
                if (command.handle(aCommand, aArguments))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public IEnumerable<string> Commands
        {
            get
            {
                List<string> commandNames =new List<string>();
                foreach (ICommand command in this.commands)
                {
                    commandNames.AddRange(command.Commands);
                }
                return commandNames;
            }
        }

        #endregion
    }
}
