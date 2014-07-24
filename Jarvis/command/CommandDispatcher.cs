using System.Collections.Generic;

namespace Jarvis.command
{
    class CommandDispatcher
    {
        private readonly ICommand command;

        public CommandDispatcher(ICommand aCommand)
        {
            this.command = aCommand;
        }

        public void handle(string aCommandAsString)
        {
            if (!string.IsNullOrEmpty(aCommandAsString))
            {
                List<string> tokens = new List<string>();

                string buffer = aCommandAsString.Trim();

                while (buffer.Length > 0)
                {
                    int firstSpace = buffer.IndexOf(' ');
                    int closingQuote = buffer.IndexOf('"', 1);
                    if (buffer[0] == '"' && closingQuote > 1)
                    {
                        tokens.Add(buffer.Substring(1, closingQuote-1));
                        buffer = buffer.Substring(closingQuote + 1).Trim();
                    }
                    else if (firstSpace >= 0)
                    {
                        tokens.Add(buffer.Substring(0, firstSpace));
                        buffer = buffer.Substring(firstSpace + 1).Trim();
                    }
                    else
                    {
                        tokens.Add(buffer);
                        buffer = string.Empty;
                    }
                }

                this.command.handle(tokens[0], tokens.GetRange(1, tokens.Count - 1).ToArray());
            }
        }
    }
}
