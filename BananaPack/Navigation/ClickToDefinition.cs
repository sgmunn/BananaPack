// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClickToDefinition.cs" company="Greg Munn">
//   (c) Greg Munn 2014
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BananaTools.Navigation
{
    using System;
    using Gdk;
    using Gtk;
    using ICSharpCode.NRefactory.Semantics;
    using ICSharpCode.NRefactory.TypeSystem;
    using Mono.TextEditor;
    using MonoDevelop.Ide;
    using MonoDevelop.Ide.Gui;
    using MonoDevelop.Ide.Gui.Content;

    public sealed class ClickToDefinition
    {
        private static ClickToDefinition instance = new ClickToDefinition();

        private const uint LeftMouseButton = 1;

        private UnderlineTextSegmentMarker marker;

        private Document markedDocument;

        private ClickToDefinition()
        {
        }

        public static ClickToDefinition Instance
        {
            get
            {
                return instance;
            }
        }

        public void AttachToDocument(Document document)
        {
			if (document == null || document.Editor == null || document.Editor.Parent == null)
				return;

            document.Editor.Parent.ButtonReleaseEvent -= this.HandleButtonReleaseEvent;
            document.Editor.Parent.MotionNotifyEvent -= this.HandleMotionNotifyEvent;

            document.Editor.Parent.ButtonReleaseEvent += this.HandleButtonReleaseEvent;
            document.Editor.Parent.MotionNotifyEvent += this.HandleMotionNotifyEvent;
        }

        public void DetachFromDocument(Document document)
        {
			if (document == null || document.Editor == null || document.Editor.Parent == null)
				return;

            if (this.markedDocument == document)
            {
                this.RemoveMarker();
            }

            document.Editor.Parent.ButtonReleaseEvent -= this.HandleButtonReleaseEvent;
            document.Editor.Parent.MotionNotifyEvent -= this.HandleMotionNotifyEvent;
        }
        
        private void HandleMotionNotifyEvent (object sender, MotionNotifyEventArgs args)
        {
            if (!this.CanApplyMarker()) return;

            this.RemoveMarker();

            if ((args.Event.State & ModifierType.Mod1Mask) == ModifierType.Mod1Mask)
            {
                var document = IdeApp.Workbench.ActiveDocument;
                var location = document.Editor.Parent.PointToLocation(args.Event.X - document.Editor.Parent.TextViewMargin.XOffset, args.Event.Y);

                int offset = document.Editor.LocationToOffset(location);
                int start = document.Editor.FindCurrentWordStart(offset);
                int end = document.Editor.FindCurrentWordEnd(offset);

                if (end - start <= 0) return;

                var element = this.GetCurrentElement(document, offset);
                if (element != null)
                {
                    this.PlaceMarker(document, start, end);
                }
                else
                {
                    var variable = this.GetCurrentVariable(document, offset);
                    if (variable != null)
                    {
                        this.PlaceMarker(document, start, end);
                    }
                }
            }
        }

        private void PlaceMarker(Document document, int start, int end)
        {
            this.marker = new UnderlineTextSegmentMarker(new Cairo.Color(0, 0, 0), new TextSegment(start, end - start));
            this.marker.Wave = false;
            this.marker.IsVisible = true;
            this.markedDocument = document;
            document.Editor.Document.AddMarker(marker);
            document.Editor.Parent.QueueDraw();
        }

        private void HandleButtonReleaseEvent (object sender, ButtonReleaseEventArgs args)
        {
            if (!this.CanApplyMarker()) return;

            if (args.Event.Button == LeftMouseButton && ((args.Event.State & ModifierType.Mod1Mask) == ModifierType.Mod1Mask))
            {
                var document = IdeApp.Workbench.ActiveDocument;
                var location = document.Editor.Caret.Location;

                int offset = document.Editor.LocationToOffset(location);
                int start = document.Editor.FindCurrentWordStart(offset);
                int end = document.Editor.FindCurrentWordEnd(offset);

                if (end - start <= 0) return;

                var element = this.GetCurrentElement(document, offset);

                if (element != null)
                {
					NavigationMarkers.DropMarkerAtCaret();
                    this.RemoveMarker();
                    IdeApp.ProjectOperations.JumpToDeclaration(element, true);
                }
                else
                {
                    var variable = this.GetCurrentVariable(document, offset);
                    if (variable != null)
                    {
						NavigationMarkers.DropMarkerAtCaret();
                        this.RemoveMarker();
                        IdeApp.ProjectOperations.JumpToDeclaration(variable);
                    }
                }
            }
        }

        private bool CanApplyMarker()
        {
            if (IdeApp.Workspace == null) return false;
            if (IdeApp.Workbench.ActiveDocument == null) return false;
            if (IdeApp.Workbench.ActiveDocument.ParsedDocument == null) return false;

            return true;
        }

        private INamedElement GetCurrentElement(Document document, int offset)
        {
            DomRegion domRegion;
            var resolveResult = document.GetLanguageItem(offset, out domRegion);
            INamedElement element = null;

            if (resolveResult is TypeResolveResult) 
            {
                element = resolveResult.Type;
            }
            else if (resolveResult is InvocationResolveResult) 
            {
                element = ((InvocationResolveResult)resolveResult).Member;
            }
            else if (resolveResult is MemberResolveResult) 
            {
                element = ((MemberResolveResult)resolveResult).Member;
            }

            return element;
        }
        
        private IVariable GetCurrentVariable(Document document, int offset)
        {
            DomRegion domRegion;
            var resolveResult = document.GetLanguageItem(offset, out domRegion);
            IVariable element = null;

            if (resolveResult is LocalResolveResult) 
            {
                element = ((LocalResolveResult)resolveResult).Variable;
            }
            else if (resolveResult is NamedArgumentResolveResult) 
            {
                element = ((NamedArgumentResolveResult)resolveResult).Parameter;
            }

            return element;
        }

        private void RemoveMarker()
        {
            if (this.marker != null) 
            {
                this.markedDocument.Editor.Document.RemoveMarker(marker);
                this.marker = null;
                this.markedDocument = null;
            }
        }
    }
}