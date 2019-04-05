using System.Linq;
using System;

namespace IngameScript
{
    partial class Program
    {
        public class TerminalMessage
        {
            public string Path;
            public string ControllerName;
            public string ActionName;
            public string Argument;

            public TerminalMessage(string path, string argument)
            {
                Path = path;
                ParsePath();
                Argument = argument;
            }

            public TerminalMessage(string messageString)
            {
                var messageParts = messageString.Split('|');
                if (!messageParts[0].Equals("T") || messageParts.Length < 2)
                    throw new Exception($"{messageParts} it not a valid T message");

                Path = messageParts[1];
                ParsePath();
                
                var agrumentParts = string.Join("", messageParts.Skip(2));
                Argument = agrumentParts;
            }

            private void ParsePath()
            {
                var pathDetails = Path.Split('.');
                ControllerName = pathDetails[0];
                ActionName = pathDetails[1];
            }
        }
    }
}
