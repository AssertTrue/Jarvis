using System.Collections.Generic;

namespace Jarvis.command
{
    interface ICommand
    {
        bool handle(string aCommand, string[] aArguments);
        IEnumerable<string> Commands { get; }
    }
}
