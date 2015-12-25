#region License

/* **********************************************************************************
 * Copyright (c) Roman Ivantsov
 * This source code is subject to terms and conditions of the MIT License
 * for Irony. A copy of the license can be found in the License.txt file
 * at the root of this distribution.
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/

#endregion License

using System;

namespace Irony.Parsing
{
	/// <summary>
	/// A terminal for representing fixed-length lexemes coming up sometimes in programming language
	/// (in Fortran for ex, every line starts with 5-char label, followed by a single continuation char)
	/// It may be also used to create grammar/parser for reading data files with fixed length fields
	/// </summary>
	public class FixedLengthLiteral : DataLiteralBase
	{
		public int Length;

		public FixedLengthLiteral(string name, int length, TypeCode dataType) : base(name, dataType)
		{
			this.Length = length;
		}

		protected override string ReadBody(ParsingContext context, ISourceStream source)
		{
			source.PreviewPosition = source.Location.Position + this.Length;
			var body = source.Text.Substring(source.Location.Position, this.Length);
			return body;
		}
	}
}
