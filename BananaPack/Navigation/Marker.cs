//   --------------------------------------------------------------------------------------------------------------------
//   <copyright file="Marker.cs" company="(c) Greg Munn">
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
	public sealed class Marker : TextSegmentMarker
	{
		public Marker (TextSegment textSegment) : base (textSegment)
		{
			this.Color = new Cairo.Color (31 / 255, 149 / 255, 255 / 255);
		}

		public Cairo.Color Color { get; set; }

		public override void Draw (TextEditor editor, Cairo.Context cr, Pango.Layout layout, bool selected, int startOffset, int endOffset, double y, double startXPos, double endXPos)
		{
			int markerStart = Segment.Offset;
			int markerEnd = Segment.EndOffset;
			if (markerEnd < startOffset || markerStart > endOffset) 
				return; 

			this.InternalDraw (markerStart, markerEnd, editor, cr, layout, false, startOffset, endOffset, y, startXPos, endXPos);
		}

		private void InternalDraw (int markerStart, int markerEnd, TextEditor editor, Cairo.Context cr, Pango.Layout layout, bool selected, int startOffset, int endOffset, double y, double startXPos, double endXPos)
		{
			double @from;
			double to;

			if (markerStart < startOffset && endOffset < markerEnd) 
			{
				@from = startXPos;
			} else 
			{ 
				int start = startOffset < markerStart ? markerStart : startOffset;
				int x_pos;

				x_pos = layout.IndexToPos (start - startOffset).X;
				@from = startXPos + (int)(x_pos / Pango.Scale.PangoScale);
			}

			var charWidth = editor.TextViewMargin.CharWidth;

			@from = Math.Max (@from, editor.TextViewMargin.XOffset) - charWidth / 2;

			to = @from + editor.TextViewMargin.CharWidth;

			cr.SetSourceColor (this.Color);

			cr.LineWidth = 1;
			cr.MoveTo (@from, y + editor.LineHeight - 1.0);
			cr.LineTo (to, y + editor.LineHeight - 1.0);

			cr.LineTo (@from + (@to - @from) / 2, y + editor.LineHeight - 6);

			cr.LineTo (@from, y + editor.LineHeight - 1.5);

			cr.Fill();
		}
	}
}