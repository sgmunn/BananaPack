//   --------------------------------------------------------------------------------------------------------------------
//   <copyright file="NavigationMarker.cs" company="(c) Greg Munn">
//     (c) 2014 (c) Greg Munn  
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//     documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//     the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//     to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//     the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//     THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//     CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//     IN THE SOFTWARE.
//   </copyright>
//   --------------------------------------------------------------------------------------------------------------------
using System;
using Mono.TextEditor;

namespace BananaTools.Navigation
{
	public sealed class NavigationMarker
	{
		public NavigationMarker(string filename, TextSegmentMarker marker)
		{
			this.FileName = filename;
			this.SegmentMarker = marker;
		}

		public string FileName { get; private set; }

		public int Offset { get; private set; }

		public TextSegmentMarker SegmentMarker { get; private set; }

		public void RemoveSegmentMarker ()
		{
			if (this.SegmentMarker != null) {
				this.Offset = this.SegmentMarker.Offset;
				this.SegmentMarker = null;
			}
		}

		public void RestoreSegmentMarker ()
		{
			this.SegmentMarker = new TextSegmentMarker (this.Offset, 0);
		}
	}
}