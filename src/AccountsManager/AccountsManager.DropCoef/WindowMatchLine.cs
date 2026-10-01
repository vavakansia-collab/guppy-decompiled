using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using AccountsManager.DropCoef.ViewModels;

namespace AccountsManager.DropCoef;

public class WindowMatchLine : Window, IComponentConnector
{
	public VMMatchLine VMMatchLine;

	private bool _contentLoaded;

	public WindowMatchLine(MatchLineVM initMatchLineVMSelected)
	{
		InitializeComponent();
		base.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/AccountsManager;component/DropCoef/Resources/dropCoef.ico"));
		VMMatchLine = new VMMatchLine(initMatchLineVMSelected);
		base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		base.DataContext = VMMatchLine;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/AccountsManager;component/dropcoef/windowmatchline.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		_contentLoaded = true;
	}
}
