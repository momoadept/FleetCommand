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

            enum ReaderState
            {
                Key,
                KeySeparator,
                Value,
                EntrySeparator,
                Finished
            }

            interface IReaderActions
            {
                void Next();
            }

            class ReaderContext
            {
                public string Data;
                public int Index;
                public Dictionary<string, string> KeysToValues = new Dictionary<string, string>();
                public string CurrentKey;
            }

            abstract class ReaderHandler : BaseHandler<ReaderState, ReaderContext>, IReaderActions
            {
                public abstract void Next();

                protected string Data => StateMachine.Context.Data;
                protected int I => StateMachine.Context.Index;
                protected void Inc() => StateMachine.Context.Index++;
            }

            class KeyReader : ReaderHandler
            {
                StringBuilder _key = new StringBuilder();

                public override void Enter() => _key.Clear();

                public override void Next()
                {
                    if (I >= Data.Length)
                    {
                        StateMachine.SwitchState(ReaderState.Finished);
                        return;
                    }

                    if (Data[I] == ':' || Data[I] == ' ')
                    {
                        StateMachine.Context.CurrentKey = _key.ToString();
                        StateMachine.SwitchState(ReaderState.KeySeparator);
                        return;
                    }

                    _key.Append(Data[I]);
                    Inc();
                }

                public override void Exit()
                {
                }
            }

            class KeySeparatorReader : ReaderHandler
            {
                public override void Enter()
                {
                }

                public override void Next()
                {
                    if (Data[I] == ':' || Data[I] == ' ')
                        Inc();
                    else
                        StateMachine.SwitchState(ReaderState.Value);
                }

                public override void Exit()
                {
                }
            }

            class ValueReader : ReaderHandler
            {
                StringBuilder _value = new StringBuilder();
                int _bracketBalance;

                public override void Enter()
                {
                    _value.Clear();
                    _bracketBalance = 0;
                }

                public override void Next()
                {
                    if (Data[I] == '{')
                        _bracketBalance++;

                    if (Data[I] == '}')
                        _bracketBalance--;

                    _value.Append(Data[I]);
                    Inc();

                    if (_bracketBalance == 0)
                    {
                        StateMachine.Context.KeysToValues.Add(
                            StateMachine.Context.CurrentKey,
                            _value.ToString(1, _value.Length - 2)
                        );

                        StateMachine.SwitchState(ReaderState.EntrySeparator);
                    }
                }

                public override void Exit()
                {
                }
            }

            class EntrySeparatorReader : ReaderHandler
            {
                public override void Enter()
                {
                }

                public override void Next()
                {
                    if (I >= Data.Length)
                    {
                        StateMachine.SwitchState(ReaderState.Finished);
                        return;
                    }

                    if (Data[I] == ',' || Data[I] == ' ')
                        Inc();
                    else
                    {
                        StateMachine.SwitchState(ReaderState.Key);
                    }
                }

                public override void Exit()
                {
                }
            }
        }
    }
}
