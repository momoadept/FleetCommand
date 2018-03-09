using System;
using System.Collections.Generic;
using System.Text;
using FC.Core.Core.FakeAsync.Promises;

namespace FC.Core.Core.ComponentModel
{
    public class ActionDescriptor
    {
        protected Dictionary<string, Func<string[], Promise>> Actions = new Dictionary<string, Func<string[], Promise>>();
        protected Dictionary<string, Func<string[], Promise<string>>> Queries = new Dictionary<string, Func<string[], Promise<string>>>();

        public void HasAction(string action, Func<string[], Promise> method)
        {
            Actions.Add(action, method);
        }

        public void HasQuery(string action, Func<string[], Promise<string>> method)
        {
            Queries.Add(action, method);
        }

        public Promise<string> Invoke(string action, params string[] args)
        {
            Promise<string> result;
            if (Queries.ContainsKey(action))
            {
                result = Queries[action](args);
            }
            else if (Actions.ContainsKey(action))
            {
                result = new Promise<string>(promise =>{});

                Actions[action](args)
                    .Then(p => result.Resolve("ok"))
                    .Error(e => result.Reject(e));
            }
            else
            {
                result = new Promise<string>(p => p.Reject(new Exception($"{action}: no action descriptor")));
            }

            return result;
        }
    }
}
