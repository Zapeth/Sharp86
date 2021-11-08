﻿// Sharp86 - 8086 Emulator
// Copyright © 2017-2021 Topten Software. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may 
// not use this product except in compliance with the License. You may obtain 
// a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
// License for the specific language governing permissions and limitations 
// under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Topten.Sharp86
{
	enum OperatorTypes
	{
		// Binary
		add,
		subtract,
		multiply,
		divide,
		modulus,
		compare,
		compare_lt,
		compare_le,
		compare_gt,
		compare_ge,
		compare_eq,
		compare_ne,
		bitwise_and,
		bitwise_or,
		bitwise_xor,
		logical_and,
		logical_or,
		shl,
		shr,

		// Unary
		negate,
		bitwise_not,
		logical_not,

		na,
	}

	class Operators
	{
		#region Binary Operation Type Map
		// This horrible map tells us what two operands should be converted to for a binary operation.
		static TypeCode[,] type_map = new TypeCode[19, 19]
		{
//		  a:	Empty,				Object,				DBNull,				Boolean,			Char,				SByte,				Byte,				Int16,				UInt16,				Int32,				UInt32,				Int64,				UInt64,				Single,				Double,				Decimal,			DateTime,			Missing				String
// b:
/*Empty*/	{	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty },
/*Object*/	{	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },
/*DBNull*/  {	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },
/*Boolean*/ {	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Boolean,	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },
/*Char*/	{	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Int32,		TypeCode.Int32,		TypeCode.UInt32,	TypeCode.Int32,		TypeCode.UInt32,	TypeCode.Int32,		TypeCode.UInt32,	TypeCode.Int64,		TypeCode.UInt64,	TypeCode.Single,	TypeCode.Double,	TypeCode.Decimal,	TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },

/*SByte*/	{	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Int32,		TypeCode.Int16,		TypeCode.UInt16,	TypeCode.Int16,		TypeCode.UInt16,	TypeCode.Int32,		TypeCode.UInt32,	TypeCode.Int64,		TypeCode.UInt64,	TypeCode.Single,	TypeCode.Double,	TypeCode.Decimal,	TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },
/*Byte*/	{	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.UInt32,	TypeCode.UInt16,	TypeCode.UInt16,	TypeCode.UInt16,	TypeCode.UInt16,	TypeCode.UInt32,	TypeCode.UInt32,	TypeCode.Int64,		TypeCode.UInt64,	TypeCode.Single,	TypeCode.Double,	TypeCode.Decimal,	TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },
/*Int16*/	{	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Int32,		TypeCode.Int16,		TypeCode.UInt16,	TypeCode.Int16,		TypeCode.UInt16,	TypeCode.Int32,		TypeCode.UInt32,	TypeCode.Int64,		TypeCode.UInt64,	TypeCode.Single,	TypeCode.Double,	TypeCode.Decimal,	TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },
/*UInt16*/	{	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.UInt32,	TypeCode.UInt16,	TypeCode.UInt16,	TypeCode.UInt16,	TypeCode.UInt16,	TypeCode.UInt32,	TypeCode.UInt32,	TypeCode.Int64,		TypeCode.UInt64,	TypeCode.Single,	TypeCode.Double,	TypeCode.Decimal,	TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },

/*Int32*/	{	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Int32,		TypeCode.Int32,		TypeCode.UInt32,	TypeCode.Int32,		TypeCode.UInt32,	TypeCode.Int32,		TypeCode.UInt32,	TypeCode.Int64,		TypeCode.UInt64,	TypeCode.Single,	TypeCode.Double,	TypeCode.Decimal,	TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },
/*UInt32*/	{	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.UInt32,	TypeCode.UInt32,	TypeCode.UInt32,	TypeCode.UInt32,	TypeCode.UInt32,	TypeCode.UInt32,	TypeCode.UInt32,	TypeCode.Int64,		TypeCode.UInt64,	TypeCode.Single,	TypeCode.Double,	TypeCode.Decimal,	TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },
/*Int64*/	{	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Int64,		TypeCode.Int64,		TypeCode.UInt64,	TypeCode.Int64,		TypeCode.UInt64,	TypeCode.Int64,		TypeCode.UInt64,	TypeCode.Int64,		TypeCode.Empty,		TypeCode.Single,	TypeCode.Double,	TypeCode.Decimal,	TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },
/*UInt64*/	{	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.UInt64,	TypeCode.UInt64,	TypeCode.UInt64,	TypeCode.UInt64,	TypeCode.UInt64,	TypeCode.UInt64,	TypeCode.UInt64,	TypeCode.Empty,		TypeCode.UInt64,	TypeCode.Single,	TypeCode.Double,	TypeCode.Decimal,	TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },
																																																																																									
/*Single*/	{	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Single,	TypeCode.Single,	TypeCode.Single,	TypeCode.Single,	TypeCode.Single,	TypeCode.Single,	TypeCode.Single,	TypeCode.Single,	TypeCode.Single,	TypeCode.Single,	TypeCode.Double,	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },
/*Double*/	{	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Double,	TypeCode.Double,	TypeCode.Double,	TypeCode.Double,	TypeCode.Double,	TypeCode.Double,	TypeCode.Double,	TypeCode.Double,	TypeCode.Double,	TypeCode.Double,	TypeCode.Double,	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },
																																																																																									
/*Decimal*/	{	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Decimal,	TypeCode.Decimal,	TypeCode.Decimal,	TypeCode.Decimal,	TypeCode.Decimal,	TypeCode.Decimal,	TypeCode.Decimal,	TypeCode.Decimal,	TypeCode.Decimal,	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Decimal,	TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },
																																																																																									
/*DateTime*/{	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },
/*Missing*/	{	TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.Empty,		TypeCode.String },
/*String*/	{	TypeCode.Empty,		TypeCode.String,	TypeCode.String,	TypeCode.String,	TypeCode.String,	TypeCode.String,	TypeCode.String,	TypeCode.String,	TypeCode.String,	TypeCode.String,	TypeCode.String,	TypeCode.String,	TypeCode.String,	TypeCode.String,	TypeCode.String,	TypeCode.String,	TypeCode.String,	TypeCode.String,	TypeCode.String },
		};
		#endregion

		public static bool IsLogicalOp(OperatorTypes op)
		{
			return op == OperatorTypes.logical_and || op == OperatorTypes.logical_or || op == OperatorTypes.logical_not;
		}


		static void unsupported(object a, object b, string op)
		{
			throw new System.InvalidCastException(string.Format("Can't {0} objects of type {1} and {2}", op, a.GetType(), b.GetType()));
		}

		static void unsupported(object a, string op)
		{
			throw new System.InvalidCastException(string.Format("Can't {0} object of type {1}", op, a.GetType()));
		}

		static TypeCode WiderType(object a, object b)
		{
			TypeCode ta = Type.GetTypeCode(a.GetType());
			TypeCode tb = Type.GetTypeCode(b.GetType());
			return type_map[(int)ta, (int)tb];
		}

        public static object add_fp(FarPointer a, object b)
        {
            return new FarPointer(a.Segment, (ushort)Convert.ChangeType(add(a.Offset, b), typeof(ushort)));
        }

		// Add
		public static object add(object a, object b)
		{
            if (a is FarPointer && !(b is FarPointer))
            {
                return add_fp((FarPointer)a, b);
            }

            if (b is FarPointer && !(a is FarPointer))
            {
                return add_fp((FarPointer)b, a);
            }

            switch (WiderType(a, b))
			{
                case TypeCode.Int16: return (short)(Convert.ToInt16(a) + Convert.ToInt16(b));
                case TypeCode.UInt16: return (ushort)(Convert.ToUInt16(a) + Convert.ToUInt16(b));
                case TypeCode.Int32: return Convert.ToInt32(a) + Convert.ToInt32(b);
                case TypeCode.UInt32: return Convert.ToUInt32(a) + Convert.ToUInt32(b);
                case TypeCode.Int64: return Convert.ToInt64(a) + Convert.ToInt64(b);
				case TypeCode.UInt64: return Convert.ToUInt64(a) + Convert.ToUInt64(b);
				case TypeCode.Double: return Convert.ToDouble(a) + Convert.ToDouble(b);
				case TypeCode.Decimal: return Convert.ToDecimal(a) + Convert.ToDecimal(b);
				case TypeCode.String: return Convert.ToString(a) + Convert.ToString(b);
			}
			unsupported(a, b, "add");
			return null;
		}

        public static object sub_fp(FarPointer a, object b)
        {
            return new FarPointer(a.Segment, (ushort)Convert.ChangeType(subtract(a.Offset, b), typeof(ushort)));
        }

        // Subtract
        public static object subtract(object a, object b)
		{
            if (a is FarPointer)
            {
                return sub_fp((FarPointer)a, b);
            }

			switch (WiderType(a, b))
			{
                case TypeCode.Int16: return (short)(Convert.ToInt16(a) - Convert.ToInt16(b));
                case TypeCode.UInt16: return (ushort)(Convert.ToUInt16(a) - Convert.ToUInt16(b));
                case TypeCode.Int32: return Convert.ToInt32(a) - Convert.ToInt32(b);
                case TypeCode.UInt32: return Convert.ToUInt32(a) - Convert.ToUInt32(b);
                case TypeCode.Int64: return Convert.ToInt64(a) - Convert.ToInt64(b);
				case TypeCode.UInt64: return Convert.ToUInt64(a) - Convert.ToUInt64(b);
				case TypeCode.Double: return Convert.ToDouble(a) - Convert.ToDouble(b);
				case TypeCode.Decimal: return Convert.ToDecimal(a) - Convert.ToDecimal(b);
			}

			unsupported(a, b, "subtract");
			return null;
		}

		// Multiply
		public static object multiply(object a, object b)
		{
			switch (WiderType(a, b))
			{
                case TypeCode.Int16: return (short)(Convert.ToInt16(a) * Convert.ToInt16(b));
                case TypeCode.UInt16: return (ushort)(Convert.ToUInt16(a) * Convert.ToUInt16(b));
                case TypeCode.Int32: return Convert.ToInt32(a) * Convert.ToInt32(b);
                case TypeCode.UInt32: return Convert.ToUInt32(a) * Convert.ToUInt32(b);
                case TypeCode.Int64: return Convert.ToInt64(a) * Convert.ToInt64(b);
				case TypeCode.UInt64: return Convert.ToUInt64(a) * Convert.ToUInt64(b);
				case TypeCode.Double: return Convert.ToDouble(a) * Convert.ToDouble(b);
				case TypeCode.Decimal: return Convert.ToDecimal(a) * Convert.ToDecimal(b);
			}

			unsupported(a, b, "multiply");
			return null;
		}

		// Divide
		public static object divide(object a, object b)
		{
			switch (WiderType(a, b))
			{
                case TypeCode.Int16: return (short)(Convert.ToInt16(a) / Convert.ToInt16(b));
                case TypeCode.UInt16: return (ushort)(Convert.ToUInt16(a) / Convert.ToUInt16(b));
                case TypeCode.Int32: return Convert.ToInt32(a) / Convert.ToInt32(b);
                case TypeCode.UInt32: return Convert.ToUInt32(a) / Convert.ToUInt32(b);
                case TypeCode.Int64: return Convert.ToInt64(a) / Convert.ToInt64(b);
				case TypeCode.UInt64: return Convert.ToUInt64(a) / Convert.ToUInt64(b);
				case TypeCode.Double: return Convert.ToDouble(a) / Convert.ToDouble(b);
				case TypeCode.Decimal: return Convert.ToDecimal(a) / Convert.ToDecimal(b);
			}

			unsupported(a, b, "divide");
			return null;
		}

		// Modulus
		public static object modulus(object a, object b)
		{
			switch (WiderType(a, b))
			{
                case TypeCode.Int16: return (short)(Convert.ToInt16(a) % Convert.ToInt16(b));
                case TypeCode.UInt16: return (ushort)(Convert.ToUInt16(a) % Convert.ToUInt16(b));
                case TypeCode.Int32: return Convert.ToInt32(a) % Convert.ToInt32(b);
                case TypeCode.UInt32: return Convert.ToUInt32(a) % Convert.ToUInt32(b);
                case TypeCode.Int64: return Convert.ToInt64(a) % Convert.ToInt64(b);
				case TypeCode.UInt64: return Convert.ToUInt64(a) % Convert.ToUInt64(b);
				case TypeCode.Double: return Convert.ToDouble(a) % Convert.ToDouble(b);
				case TypeCode.Decimal: return Convert.ToDecimal(a) % Convert.ToDecimal(b);
			}

			unsupported(a, b, "modulus");
			return null;
		}

		// Negate
		public static object negate(object a)
		{
			switch (WiderType(a, a))
			{
                case TypeCode.Int16: return -(short)(Convert.ToInt16(a));
                case TypeCode.UInt16: return -(short)(Convert.ToUInt32(a));
                case TypeCode.Int32: return -(int)(Convert.ToInt32(a));
                case TypeCode.UInt32: return -(int)(Convert.ToUInt32(a));
                case TypeCode.Int64: return -Convert.ToInt64(a);
				case TypeCode.Double: return -Convert.ToDouble(a);
				case TypeCode.Decimal: return -Convert.ToDecimal(a);
			}

			unsupported(a, "negate");
			return null;
		}

		// Compare
		public static object compare(object a, object b)
		{
			switch (WiderType(a, b))
			{
                case TypeCode.Int16: return Convert.ToInt16(a).CompareTo(Convert.ToInt16(b));
                case TypeCode.UInt16: return Convert.ToUInt16(a).CompareTo(Convert.ToUInt16(b));
                case TypeCode.Int32: return Convert.ToInt32(a).CompareTo(Convert.ToInt32(b));
                case TypeCode.UInt32: return Convert.ToUInt32(a).CompareTo(Convert.ToUInt32(b));
                case TypeCode.Int64: return Convert.ToInt64(a).CompareTo(Convert.ToInt64(b));
				case TypeCode.UInt64: return Convert.ToUInt64(a).CompareTo(Convert.ToUInt64(b));
				case TypeCode.Double: return Convert.ToDouble(a).CompareTo(Convert.ToDouble(b));
				case TypeCode.Decimal: return Convert.ToDecimal(a).CompareTo(Convert.ToDecimal(b));
				case TypeCode.Boolean: return Convert.ToBoolean(a).CompareTo(Convert.ToBoolean(b));
			}

			unsupported(a, b, "compare");
			return null;
		}

		// Compare LT
		public static object compare_lt(object a, object b)
		{
			return ((int)compare(a, b)) < 0;
		}

		// Compare LE
		public static object compare_le(object a, object b)
		{
			return ((int)compare(a, b)) <= 0;
		}

		// Compare GT
		public static object compare_gt(object a, object b)
		{
			return ((int)compare(a, b)) > 0;
		}

		// Compare GE
		public static object compare_ge(object a, object b)
		{
			return ((int)compare(a, b)) >= 0;
		}

		// Compare EQ
		public static object compare_eq(object a, object b)
		{
			return ((int)compare(a, b)) == 0;
		}

		// Compare NE
		public static object compare_ne(object a, object b)
		{
			return ((int)compare(a, b)) != 0;
		}


		// Bitwise And
		public static object bitwise_and(object a, object b)
		{
			switch (WiderType(a, b))
			{
                case TypeCode.Int16: return (short)(Convert.ToInt16(a) & Convert.ToInt16(b));
                case TypeCode.UInt16: return (ushort)(Convert.ToUInt16(a) & Convert.ToUInt16(b));
                case TypeCode.Int32: return Convert.ToInt32(a) & Convert.ToInt32(b);
                case TypeCode.UInt32: return Convert.ToUInt32(a) & Convert.ToUInt32(b);
                case TypeCode.Int64: return Convert.ToInt64(a) & Convert.ToInt64(b);
				case TypeCode.UInt64: return Convert.ToUInt64(a) & Convert.ToUInt64(b);
			}

			unsupported(a, b, "bitwise and");
			return null;
		}

		// Bitwize Or
		public static object bitwise_or(object a, object b)
		{
			switch (WiderType(a, b))
			{
                case TypeCode.Int16: return (short)(Convert.ToInt16(a) | Convert.ToInt16(b));
                case TypeCode.UInt16: return (ushort)(Convert.ToUInt16(a) | Convert.ToUInt16(b));
                case TypeCode.Int32: return Convert.ToInt32(a) | Convert.ToInt32(b);
                case TypeCode.UInt32: return Convert.ToUInt32(a) | Convert.ToUInt32(b);

                case TypeCode.Int64: return Convert.ToInt64(a) | Convert.ToInt64(b);
				case TypeCode.UInt64: return Convert.ToUInt64(a) | Convert.ToUInt64(b);
			}

			unsupported(a, b, "bitwise or");
			return null;
		}

		// Bitwise Xor
		public static object bitwise_xor(object a, object b)
		{
			switch (WiderType(a, b))
			{
                case TypeCode.Int16: return (short)(Convert.ToInt16(a) ^ Convert.ToInt16(b));
                case TypeCode.UInt16: return (ushort)(Convert.ToUInt16(a) ^ Convert.ToUInt16(b));
                case TypeCode.Int32: return Convert.ToInt32(a) ^ Convert.ToInt32(b);
				case TypeCode.UInt32: return Convert.ToUInt32(a) ^ Convert.ToUInt32(b);
				case TypeCode.Int64: return Convert.ToInt64(a) ^ Convert.ToInt64(b);
				case TypeCode.UInt64: return Convert.ToUInt64(a) ^ Convert.ToUInt64(b);
			}

			unsupported(a, b, "bitwise xor");
			return null;
		}

		// Bitwise Not
		public static object bitwise_not(object a)
		{
			switch (WiderType(a, a))
			{
                case TypeCode.Int16: return (short)(~Convert.ToInt16(a));
                case TypeCode.UInt16: return (ushort)(~Convert.ToUInt16(a));
                case TypeCode.Int32: return ~Convert.ToInt32(a);
                case TypeCode.UInt32: return ~Convert.ToUInt32(a);
                case TypeCode.Int64: return ~Convert.ToInt64(a);
				case TypeCode.UInt64: return ~Convert.ToUInt64(a);
			}

			unsupported(a, "bitwise not");
			return null;
		}

		// Logical And
		public static object logical_and(object a, object b)
		{
			switch (WiderType(a, b))
			{
				case TypeCode.Boolean: return Convert.ToBoolean(a) && Convert.ToBoolean(b);
			}

			unsupported(a, b, "logical and");
			return null;
		}

		// Logical Or
		public static object logical_or(object a, object b)
		{
			switch (WiderType(a, b))
			{
				case TypeCode.Boolean: return Convert.ToBoolean(a) || Convert.ToBoolean(b);
			}

			unsupported(a, b, "logical or");
			return null;
		}

		// Logical Not
		public static object logical_not(object a)
		{
			switch (WiderType(a, a))
			{
				case TypeCode.Boolean: return !Convert.ToBoolean(a);
			}

			unsupported(a, "logical not");
			return null;
		}

		// Shift Left
		public static object shl(object a, object b)
		{
			switch (WiderType(a, a))
			{
                case TypeCode.Int16: return (short)(Convert.ToInt16(a) << Convert.ToInt16(b));
                case TypeCode.UInt16: return (ushort)(Convert.ToUInt16(a) << Convert.ToInt16(b));
                case TypeCode.Int32: return Convert.ToInt32(a) << Convert.ToInt32(b);
                case TypeCode.UInt32: return Convert.ToUInt32(a) << Convert.ToInt32(b);
                case TypeCode.Int64: return Convert.ToInt64(a) << Convert.ToInt32(b);
				case TypeCode.UInt64: return Convert.ToUInt64(a) << Convert.ToInt32(b);
			}

            unsupported(a, b, "shl");
			return null;
		}

		// Shift Right
		public static object shr(object a, object b)
		{
			switch (WiderType(a, a))
			{
                case TypeCode.Int16: return (short)(Convert.ToInt16(a) >> Convert.ToInt16(b));
                case TypeCode.UInt16: return (ushort)(Convert.ToUInt16(a) >> Convert.ToInt16(b));
                case TypeCode.Int32: return Convert.ToInt32(a) >> Convert.ToInt32(b);
				case TypeCode.UInt32: return Convert.ToUInt32(a) >> Convert.ToInt32(b);
				case TypeCode.Int64: return Convert.ToInt64(a) >> Convert.ToInt32(b);
				case TypeCode.UInt64: return Convert.ToUInt64(a) >> Convert.ToInt32(b);
			}

			unsupported(a, b, "shr");
			return null;
		}

		// Arbitrary binary operation
		public static object binary_operator(OperatorTypes op, object a, object b)
		{
			if (op >= OperatorTypes.add && op <= OperatorTypes.shr)
			{
				return binary_operations[(int)op](a, b);
			}
			throw new ArgumentException();
		}

		// Arbitrary unary operation
		public static object unary_operator(OperatorTypes op, object a)
		{
			if (op >= OperatorTypes.negate && op <= OperatorTypes.logical_not)
			{
				return unary_operations[(int)(op - OperatorTypes.negate)](a);
			}
			throw new ArgumentException();
		}

		static Func<object, object, object>[] binary_operations = new Func<object, object, object>[]
		{
			add, 
			subtract, 
			multiply,
			divide,
			modulus,
			compare,
			compare_lt,
			compare_le,
			compare_gt,
			compare_ge,
			compare_eq,
			compare_ne,
			bitwise_and,
			bitwise_or,
			bitwise_xor,
			logical_and,
			logical_or,
			shl,
			shr,
		};

		static Func<object, object>[] unary_operations = new Func<object, object>[]
		{
			negate,
			bitwise_not,
			logical_not,
		};

	}

}
