using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace AccountsManager.DropCoef;

public class WindowDropCoef : Window, IComponentConnector
{
	internal DataGrid dataGridMathces;

	private bool _contentLoaded;

	public WindowDropCoef()
	{
		InitializeComponent();
		base.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/AccountsManager;component/DropCoef/Resources/dropCoef.ico"));
	}

	private void Window_Closing(object sender, CancelEventArgs e)
	{
		Hide();
		e.Cancel = true;
	}

	private void SearchAccountsByText_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Return)
		{
			VMDropCoef.instance.OnSearchTextChanged();
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocater = new Uri("/AccountsManager;component/dropcoef/windowdropcoef.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocater);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((WindowDropCoef)target).Closing += Window_Closing;
			break;
		case 2:
			((TextBox)target).KeyDown += SearchAccountsByText_KeyDown;
			break;
		case 3:
			dataGridMathces = (DataGrid)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
