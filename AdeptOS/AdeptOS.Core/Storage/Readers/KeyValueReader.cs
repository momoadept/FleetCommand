using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    partial class Program
    {
        public class KeyValueReader
        {
            /// <summary>
            /// Example string: {key1:{stringValue1},key2:{subkey1:{blablabla},subkey2:{ololo}}}
            /// </summary>
            /// <param name="values"></param>
            public Dictionary<string, string> Parse(string values)
            {
                var context = new ReaderContext()
                {
                    Data = values.StartsWith("{") ? values.Substring(1, values.Length - 2) : values // trim {}
                };

                var stateMachine = new StateMachine<ReaderHandler, ReaderState, IReaderActions, ReaderContext>(
                    new Dictionary<ReaderState, ReaderHandler>()
                    {
                        {ReaderState.Key, new KeyReader() },
                        {ReaderState.KeySeparator, new KeySeparatorReader() },
                        {ReaderState.Value, new ValueReader() },
                        {ReaderState.EntrySeparator, new EntrySeparatorReader() }
                    },
                    ReaderState.Key,
                    context
                );

                while (stateMachine.CurrentState != ReaderState.Finished)
                    stateMachine.Actions.Next();


                return stateMachine.Context.KeysToValues;
            }

            private enum ReaderState
            {
                Key,
                KeySeparator,
                Value,
                EntrySeparator,
                Finished
            }

            private interface IReaderActions
            {
                void Next();
            }

            private class ReaderContext
            {
                public string Data;
                public int Index = 0;
                public Dictionary<string, string> KeysToValues = new Dictionary<string, string>();
                public string CurrentKey;
            }

            private abstract class ReaderHandler : BaseHandler<ReaderState, ReaderContext>, IReaderActions
            {
                public abstract void Next();

                protected string Data => _stateMachine.Context.Data;
                protected int i => _stateMachine.Context.Index;
                protected void Inc() => _stateMachine.Context.Index++;
            }

            private class KeyReader : ReaderHandler
            {
                private StringBuilder _key = new StringBuilder();

                public override void Enter()
                {
                    _key.Clear();
                }

                public override void Next()
                {
                    if (i >= Data.Length)
                    {
                        _stateMachine.SwitchState(ReaderState.Finished);
                        return;
                    }

                    if (Data[i] == ':' || Data[i] == ' ')
                    {
                        _stateMachine.Context.CurrentKey = _key.ToString();
                        _stateMachine.SwitchState(ReaderState.KeySeparator);
                        return;
                    }

                    _key.Append(Data[i]);
                    Inc();
                }

                public override void Exit()
                {
                }
            }

            private class KeySeparatorReader : ReaderHandler
            {
                public override void Enter()
                {
                }

                public override void Next()
                {
                    if (Data[i] == ':' || Data[i] == ' ')
                        Inc();
                    else
                        _stateMachine.SwitchState(ReaderState.Value);
                }

                public override void Exit()
                {
                }
            }

            private class ValueReader : ReaderHandler
            {
                private StringBuilder _value = new StringBuilder();
                private int _bracketBalance = 0;

                public override void Enter()
                {
                    _value.Clear();
                    _bracketBalance = 0;
                }

                public override void Next()
                {
                    if (Data[i] == '{')
                        _bracketBalance++;

                    if (Data[i] == '}')
                        _bracketBalance--;

                    _value.Append(Data[i]);
                    Inc();

                    if (_bracketBalance == 0)
                    {
                        _stateMachine.Context.KeysToValues.Add(
                            _stateMachine.Context.CurrentKey,
                            _value.ToString(1, _value.Length - 2)
                        );

                        _stateMachine.SwitchState(ReaderState.EntrySeparator);
                    }
                }

                public override void Exit()
                {
                }
            }

            private class EntrySeparatorReader : ReaderHandler
            {
                public override void Enter()
                {
                }

                public override void Next()
                {
                    if (i >= Data.Length)
                    {
                        _stateMachine.SwitchState(ReaderState.Finished);
                        return;
                    }

                    if (Data[i] == ',' || Data[i] == ' ')
                        Inc();
                    else
                    {
                        _stateMachine.SwitchState(ReaderState.Key);
                    }
                }

                public override void Exit()
                {
                }
            }
        }
    }
}
