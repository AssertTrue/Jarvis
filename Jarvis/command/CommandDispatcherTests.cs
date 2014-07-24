using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;

namespace Jarvis.command
{
    [TestFixture]
    internal class CommandDispatcherTests
    {
        class CommandStub : ICommand
        {
            #region Implementation of ICommand

            public bool handle(string aCommand, string[] aArguments)
            {
                List<string> expectedTokens = new List<string>();
                expectedTokens.Add(aCommand);
                expectedTokens.AddRange(aArguments);
                this.Tokens = expectedTokens.ToArray();
                return false;
            }

            public IEnumerable<string> Commands
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            public string[] Tokens;
        }

        [Test]
        [TestCase("he is legend", new[] { "he", "is", "legend" })]
        [TestCase("   he   is   legend   ", new[] { "he", "is", "legend" })]
        [TestCase("\"fortune\"", new[] { "fortune" })]
        [TestCase("\"long\" and \"short\"", new[] { "long", "and", "short" })]
        [TestCase("\"long and short\"", new[] { "long and short" })]
        [TestCase("\"  long  \" and \" short \"", new[] { "  long  ", "and", " short " })]
        [TestCase("\"fancy\" mc gubb\"ins", new[] { "fancy", "mc", "gubb\"ins" })]
        [TestCase("fa\"ncy mc gubbins", new[] { "fa\"ncy", "mc", "gubbins" })]
        [TestCase("fa\"ncy mc \"gubbins\"", new[] { "fa\"ncy", "mc", "gubbins" })]
        public void test_handle_VariousQuotedStringCombinations_PassExpectedTokens(string aString, string[] aExpectedTokens)
        {
            CommandStub command = new CommandStub();
            
            CommandDispatcher dispatcher = new CommandDispatcher(command);

            dispatcher.handle(aString);
            
            Assert.AreEqual(aExpectedTokens, command.Tokens);
        }
    }
}
