using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Jarvis.alias;
using Jarvis.command;

namespace Jarvis
{
    class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            this.backingCommandWindowVisible = Visibility.Visible;
            this.backingCommandText = string.Empty;

            this.CommandAlias = new CommandAlias();
            this.CommandAlias.add("save", (aArguments) => { this.save(); return true; });

            this.alias = new Alias();

            this.AggregateCommands = new Aggregate();
            this.AggregateCommands.addCommand(this.CommandAlias);
            this.AggregateCommands.addCommand(this.alias);

            this.commandDispatcher = new CommandDispatcher(this.AggregateCommands);

            this.load();
        }

        #region Public event handler
        public void onCommandKeyUp(KeyEventArgs aKeyEventArgs)
        {
            // This is useful for figuring out the shortcut keys
            //int virtualKeyFromKey = KeyInterop.VirtualKeyFromKey(e.Key);
            if (aKeyEventArgs.Key == Key.Enter)
            {
                this.CommandWindowVisible = Visibility.Hidden;

                string commandToRun = this.getSelectedCommand();

                this.commandDispatcher.handle(commandToRun);
                this.CommandText = string.Empty;
            }
            else if (aKeyEventArgs.Key == Key.Escape)
            {
                this.CommandWindowVisible = Visibility.Hidden;
                this.CommandText = string.Empty;
            }
            else if (aKeyEventArgs.Key == Key.Down)
            {
                this.SelectedMatchIndex = Math.Min(this.SelectedMatchIndex + 1, new List<string>(this.Matches).Count-1);
            }
            else if (aKeyEventArgs.Key == Key.Up)
            {
                this.SelectedMatchIndex = Math.Max(this.SelectedMatchIndex - 1, 0);
            }
        }

        private string getSelectedCommand()
        {
            string commandToRun = this.backingCommandText;
                
            List<string> matches = new List<string>(this.Matches);
            if (matches.Count > 0
                && this.SelectedMatchIndex >= 0
                && this.SelectedMatchIndex < matches.Count)
            {
                commandToRun = matches[this.SelectedMatchIndex];
            }
            return commandToRun;
        }

        #endregion

        #region Public properties for binding
        public IEnumerable<string> Matches
        {
            get
            {
                List<string> result = new List<string>();
                if (!string.IsNullOrEmpty(this.CommandText))
                {
                    IEnumerable<string> commands = this.AggregateCommands.Commands;
                    foreach (string command in commands)
                    {
                        if (command.Contains(this.CommandText))
                        {
                            result.Add(command);
                        }
                    }
                }
                return result;
            }
        }

        public Visibility MatchesVisible
        {
            get 
            { 
                Visibility visibility = Visibility.Collapsed; 
                if (this.Matches.GetEnumerator().MoveNext())
                {
                    visibility = Visibility.Visible;
                }
                return visibility;
            }
        }

        public string CommandText
        {
            get { return this.backingCommandText; }
            set 
            { 
                this.backingCommandText = value;
                this.firePropertyChanged("CommandText");
                this.firePropertyChanged("Matches");
                this.firePropertyChanged("MatchesVisible");
                this.SelectedMatchIndex = 0;
            }
        }

        public int SelectedMatchIndex
        {
            get { return this.backingSelectedMatchIndex; }
            set
            {
                this.backingSelectedMatchIndex = value;
                this.firePropertyChanged("SelectedMatchIndex");
            }
        }

        public Visibility CommandWindowVisible 
        { 
            get { return this.backingCommandWindowVisible; }
            set 
            { 
                this.backingCommandWindowVisible = value;
                this.firePropertyChanged("CommandWindowVisible");
            }
        }
        #endregion

        #region Public properties for extending commands
        public Aggregate AggregateCommands;
        public CommandAlias CommandAlias; 
        #endregion

        #region Loading and saving
        private void load()
        {
            string filePath = generateFilePath();
            if (File.Exists(filePath))
            {
                System.IO.StreamReader file = new System.IO.StreamReader(filePath);
                while (!file.EndOfStream)
                {
                    this.commandDispatcher.handle(file.ReadLine());
                }
                file.Close();
            }
        }

        private void save()
        {
            string filePath = generateFilePath();
            if (File.Exists(filePath))
            {
                File.Copy(filePath, filePath + ".backup", true);
            }
            System.IO.StreamWriter file = new System.IO.StreamWriter(filePath);
            file.WriteLine(this.alias.exportAsCommands());
            file.Close();
        }

        private static string generateFilePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "startupcommands.jarvis");
        }
        #endregion

        #region Backing data
        private Visibility backingCommandWindowVisible;
        private string backingCommandText;
        private int backingSelectedMatchIndex;
        #endregion

        #region Private members
        private readonly Alias alias;
        private readonly CommandDispatcher commandDispatcher;
        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void firePropertyChanged(string aPropertyName)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(aPropertyName));
        }

        #endregion
    }
}
