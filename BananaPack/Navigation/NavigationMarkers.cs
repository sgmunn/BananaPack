//   --------------------------------------------------------------------------------------------------------------------
//   <copyright file="NavigationMarkers.cs" company="(c) Greg Munn">
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
using System.Collections.Generic;
using Mono.TextEditor;
using System.Linq;
using MonoDevelop.Ide;
using System.IO;
using BananaTools.Ide;
using MonoDevelop.Ide.Gui;

namespace BananaTools.Navigation
{
	/// <summary>
	/// Manages the stack of navigation markers
	/// </summary>
	public static class NavigationMarkers
	{
		private static readonly List<NavigationMarker> stack = new List<NavigationMarker>();

		public static void DropMarkerAtCaret()
		{
			if (IdeApp.Workbench.ActiveDocument == null)
				return;

			var provider = IdeApp.Workbench.ActiveDocument.GetContent<ITextEditorDataProvider> ();
			if (provider == null)
				return;

			var editor = provider.GetTextEditorData();

			var marker = new Marker(new TextSegment(editor.Caret.Offset, 0));
			marker.IsVisible = true;
			editor.Document.AddMarker(marker);
			editor.Parent.QueueDraw();

			Push(editor.FileName, marker);
		}

		public static bool Any()
		{
			return stack.Count > 0;
		}

		public static void PickupTopMarker()
		{
			var marker = Pop();
			if (marker == null)
			{
				return;
			}

			// force a reset of any navigation loop
			//			ReferenceNavigationLoop.Clear();

			if (File.Exists (marker.FileName)) {
				IdeHelpers.OpenDocument (marker.FileName, true, () => NavigateToMarker (marker));
			}
			else {
				ClearMarkers (marker.FileName, null);
			}
		}

		public static void RemoveMarkersFromDocument (Document document)
		{
			var provider = document.GetContent<ITextEditorDataProvider> ();
			if (provider == null)
				return;

			var editor = provider.GetTextEditorData();
			if (editor != null && editor.Document != null) {
				ClearMarkers (document.FileName, (marker) => editor.Document.RemoveMarker (marker.SegmentMarker));
			}
		}

		private static void Push(string filename, TextSegmentMarker marker)
		{
			stack.Insert(0, new NavigationMarker(filename, marker));
		}

		private static NavigationMarker Peek()
		{
			if (stack.Count > 0)
			{
				return stack[0];
			}

			return null;
		}

		private static NavigationMarker Pop()
		{
			if (stack.Count > 0)
			{
				var marker = stack [0];
				stack.RemoveAt (0);
				return marker;
			}

			return null;
		}

		private static void ClearMarkers (string fileName, Action<NavigationMarker> removeMarker)
		{
			var matching = stack.Where (m => m.FileName == fileName).ToList ();
			foreach (var marker in matching) {
				stack.Remove (marker);
				if (removeMarker != null)
					removeMarker (marker);
			}
		}

		private static void NavigateToMarker(NavigationMarker marker)
		{
			if (IdeApp.Workbench.ActiveDocument == null)
				return;

			var provider = IdeApp.Workbench.ActiveDocument.GetContent<ITextEditorDataProvider> ();
			if (provider == null)
				return;

			var editor = IdeApp.Workbench.ActiveDocument.GetContent<ITextEditorDataProvider>().GetTextEditorData();
			var loc = editor.OffsetToLocation(marker.SegmentMarker.Offset);
			editor.SetCaretTo(loc.Line, loc.Column);

			editor.Document.RemoveMarker(marker.SegmentMarker);
		}
	}
}