﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PetaJson;

namespace Sharp86
{
    public interface IBreakPointMemRead
    {
        void ReadByte(ushort seg, ushort offset, byte value);
    }

    public interface IBreakPointMemWrite
    {
        void WriteByte(ushort seg, ushort offset, byte oldValue, byte newValue);
    }

    public abstract class BreakPoint : IJsonWriting, ISymbolScope
    {
        public BreakPoint()
        {
            Enabled = true;
        }

        public abstract bool ShouldBreak(DebuggerCore debugger);

        public bool CheckMatchConditions(DebuggerCore debugger)
        {
            if (_matchConditionExpression == null)
                return true;

            var condResult = _matchConditionExpression.Eval(debugger.ExpressionContext, this);
            return (bool)Convert.ChangeType(condResult, typeof(bool));
        }

        public bool CheckBreakConditions(DebuggerCore debugger)
        {
            if (_breakConditionExpression == null)
                return true;

            var condResult = _breakConditionExpression.Eval(debugger.ExpressionContext, this);
            return (bool)Convert.ChangeType(condResult, typeof(bool));
        }

        [Json("number")]
        public int Number
        {
            get;
            set;
        }
        
        [Json("enabled")]
        public bool Enabled
        {
            get;
            set;
        }

        [Json("matchCondition")]
        public string MatchConditionText
        {
            get { return _matchConditionExpression == null ? null : _matchConditionExpression.OriginalExpression; }
            set { _matchConditionExpression = value == null ? null : new Expression(value); }
        }

        [Json("breakCondition")]
        public string BreakConditionText
        {
            get { return _breakConditionExpression == null ? null : _breakConditionExpression.OriginalExpression; }
            set { _breakConditionExpression = value == null ? null : new Expression(value); }
        }

        public virtual void CopyState(BreakPoint other)
        {
            Number = other.Number;
            Enabled = other.Enabled;
            TripCount = other.TripCount;
            MatchConditionExpression = other.MatchConditionExpression;
            BreakConditionExpression = other.BreakConditionExpression;
        }

        public abstract string EditString
        {
            get;
        }

        // Condition expression
        Expression _matchConditionExpression;
        public Expression MatchConditionExpression
        {
            get { return _matchConditionExpression; }
            set { _matchConditionExpression = value; }
        }

        Expression _breakConditionExpression;
        public Expression BreakConditionExpression
        {
            get { return _breakConditionExpression; }
            set { _breakConditionExpression = value; }
        }

        // Number of times this breakpoint has been tripped
        public int TripCount;

        public string ToString(string content)
        {
            var sb = new StringBuilder(string.Format("#{0}{1} {2}",
                Number,
                Enabled ? "" : " [Disabled]",
                content));

            if (TripCount > 0)
                sb.AppendFormat("\n  tripped {0} time{1}", TripCount, TripCount == 1 ? "" : "s");
            if (_matchConditionExpression != null)
                sb.AppendFormat("\n  match if {0}", _matchConditionExpression.OriginalExpression);
            if (_breakConditionExpression != null)
                sb.AppendFormat("\n  break if {0}", _breakConditionExpression.OriginalExpression);

            return sb.ToString();
        }

        void IJsonWriting.OnJsonWriting(IJsonWriter w)
        {
            w.WriteKey("type");
            w.WriteStringLiteral(_typeToName[this.GetType()]);
        }

        static Dictionary<string, Type> _nameToType = new Dictionary<string, Type>();
        static Dictionary<Type, string> _typeToName = new Dictionary<Type, string>();
        public static void RegisterBreakPointType(string name, Type t)
        {
            _nameToType.Add(name, t);
            _typeToName.Add(t, name);
        }

        public static void RegisterJson()
        {
            RegisterBreakPointType("code", typeof(CodeBreakPoint));
            RegisterBreakPointType("mem", typeof(MemoryChangeBreakPoint));
            RegisterBreakPointType("cputime", typeof(CpuTimeBreakPoint));
            RegisterBreakPointType("expr", typeof(ExpressionBreakPoint));
            RegisterBreakPointType("memw", typeof(MemoryWriteBreakPoint));
            RegisterBreakPointType("memr", typeof(MemoryReadBreakPoint));
            RegisterBreakPointType("int", typeof(InterruptBreakPoint));

            Json.RegisterTypeFactory(typeof(BreakPoint), (reader, key) =>
            {
                if (key != "type")
                    return null;

                return reader.ReadLiteral(literal =>
                {
                    if (!_nameToType.ContainsKey((string)literal))
                        throw new InvalidDataException(string.Format("Unknown break point kind: {0}", literal));
                    return (BreakPoint)Activator.CreateInstance(_nameToType[(string)literal]);
                });
            });
        }

        public virtual Symbol ResolveSymbol(string name)
        {
            if (string.Compare(name, "TripCount", true)==0)
                return new CallbackSymbol(() => TripCount);
            return null;
        }
    }


}