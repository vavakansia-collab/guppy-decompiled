using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using AccountsLibrary;

namespace AccountsManager;

public class ucSettingsPoslegol : UserControl, IComponentConnector
{
	private PoslegolSettings Settings;

	private bool _contentLoaded;

	public ucSettingsPoslegol()
	{
		InitializeComponent();
	}

	private void UserControl_Loaded(object sender, RoutedEventArgs e)
	{
		Settings = base.DataContext as PoslegolSettings;
	}

	private void btnChooseAllSlowSports_Click(object sender, RoutedEventArgs e)
	{
		Settings.SportsParameters.Where((ParametersSport sp) => sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = true;
		});
	}

	private void btnChooseCancelSlowSports_Click(object sender, RoutedEventArgs e)
	{
		Settings.SportsParameters.Where((ParametersSport sp) => sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = false;
		});
	}

	private void btnChooseAllFastSports_Click(object sender, RoutedEventArgs e)
	{
		Settings.SportsParameters.Where((ParametersSport sp) => !sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = true;
		});
	}

	private void btnChooseCancelFastSports_Click(object sender, RoutedEventArgs e)
	{
		Settings.SportsParameters.Where((ParametersSport sp) => !sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = false;
		});
	}

	private void btnChooseAllBC_Click(object sender, RoutedEventArgs e)
	{
		Settings.ParametersCheckBC.ToList().ForEach(delegate(ParameterAccount par)
		{
			par.IsChecked = true;
		});
	}

	private void btnChooseCancelBC_Click(object sender, RoutedEventArgs e)
	{
		Settings.ParametersCheckBC.ToList().ForEach(delegate(ParameterAccount par)
		{
			par.IsChecked = false;
		});
	}

	private void btnChooseAllPoins0_Click(object sender, RoutedEventArgs e)
	{
		bool isChecked = Settings.IssueTypeParameters.First().Points0;
		Settings.IssueTypeParameters.ToList().ForEach(delegate(IssueTypeParameter par)
		{
			par.Points0 = !isChecked;
		});
	}

	private void btnChooseAllPoins1_Click(object sender, RoutedEventArgs e)
	{
		bool isChecked = Settings.IssueTypeParameters.First().Points1;
		Settings.IssueTypeParameters.ToList().ForEach(delegate(IssueTypeParameter par)
		{
			par.Points1 = !isChecked;
		});
	}

	private void btnChooseAllPoins2_Click(object sender, RoutedEventArgs e)
	{
		bool isChecked = Settings.IssueTypeParameters.First().Points2;
		Settings.IssueTypeParameters.ToList().ForEach(delegate(IssueTypeParameter par)
		{
			par.Points2 = !isChecked;
		});
	}

	private void btnChooseAllPoins2Over_Click(object sender, RoutedEventArgs e)
	{
		bool isChecked = Settings.IssueTypeParameters.First().Points2Over;
		Settings.IssueTypeParameters.ToList().ForEach(delegate(IssueTypeParameter par)
		{
			par.Points2Over = !isChecked;
		});
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/AccountsManager;component/_accountsmanager/ucsettingsposlegol.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((ucSettingsPoslegol)target).Loaded += UserControl_Loaded;
			break;
		case 2:
			((Button)target).Click += btnChooseAllSlowSports_Click;
			break;
		case 3:
			((Button)target).Click += btnChooseCancelSlowSports_Click;
			break;
		case 4:
			((Button)target).Click += btnChooseAllFastSports_Click;
			break;
		case 5:
			((Button)target).Click += btnChooseCancelFastSports_Click;
			break;
		case 6:
			((Button)target).Click += btnChooseAllSlowSports_Click;
			break;
		case 7:
			((Button)target).Click += btnChooseCancelSlowSports_Click;
			break;
		case 8:
			((Button)target).Click += btnChooseAllFastSports_Click;
			break;
		case 9:
			((Button)target).Click += btnChooseCancelFastSports_Click;
			break;
		case 10:
			((Button)target).Click += btnChooseAllBC_Click;
			break;
		case 11:
			((Button)target).Click += btnChooseCancelBC_Click;
			break;
		case 12:
			((Button)target).Click += btnChooseAllPoins0_Click;
			break;
		case 13:
			((Button)target).Click += btnChooseAllPoins1_Click;
			break;
		case 14:
			((Button)target).Click += btnChooseAllPoins2_Click;
			break;
		case 15:
			((Button)target).Click += btnChooseAllPoins2Over_Click;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
