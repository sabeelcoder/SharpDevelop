﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class InlineUIElementGenerator : VisualLineElementGenerator, IInlineUIElement
	{
		ITextAnchor anchor;
		UIElement element;
		TextView textView;
		
		public InlineUIElementGenerator(TextView textView, UIElement element, ITextAnchor anchor)
		{
			this.textView = textView;
			this.element = element;
			this.anchor = anchor;
		}
		
		public override int GetFirstInterestedOffset(int startOffset)
		{
			if (anchor.Offset >= startOffset)
				return anchor.Offset;
			
			return -1;
		}
		
		public override VisualLineElement ConstructElement(int offset)
		{
			if (this.anchor.Offset == offset)
				return new InlineObjectElement(0, element);
			
			return null;
		}
		
		public void Remove()
		{
			this.textView.ElementGenerators.Remove(this);
		}
	}
	
	public class AvalonEditEditorUIService : IEditorUIService
	{
		TextView textView;
		
		public AvalonEditEditorUIService(TextView textView)
		{
			this.textView = textView;
		}	
		
		public IInlineUIElement CreateInlineUIElement(ITextAnchor position, UIElement element)
		{
			InlineUIElementGenerator inline = new InlineUIElementGenerator(textView, element, position);
			this.textView.ElementGenerators.Add(inline);
			return inline;
		}
	}
}
