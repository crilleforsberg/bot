using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Modules
{
    public partial class ScriptManager
    {
        public class VariableCollection
        {
            public VariableCollection(ScriptManager parent)
            {
                this.Parent = parent;
                this.Variables = new List<Variable>();
            }

            public ScriptManager Parent { get; private set; }
            private List<Variable> Variables { get; set; }

            public object GetValue(string name)
            {
                foreach (var variable in this.Variables.ToArray())
                {
                    if (variable.Key == name) return variable.Value;
                }
                return null;
            }
            public void SetValue(string key, object value)
            {
                foreach (var variable in this.Variables.ToArray())
                {
                    if (variable.Key != key) continue;
                    variable.Value = value;
                    return;
                }
                this.Variables.Add(new Variable(key, value));
            }
            public bool RemoveValue(string key)
            {
                foreach (var variable in this.Variables.ToArray())
                {
                    if (variable.Key != key) continue;
                    return this.Variables.Remove(variable);
                }
                return false;
            }

            public class Variable
            {
                public Variable(string key, object value)
                {
                    this.Key = key;
                    this.Value = value;
                }

                public string Key { get; set; }
                public object Value { get; set; }
            }
        }
    }
}
