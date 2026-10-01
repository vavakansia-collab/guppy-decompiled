using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using AccountsLibrary;

namespace AccountsManager;

public class WindowUsers : Window, IComponentConnector, IStyleConnector
{
	internal ListBox listBoxUsers;

	private bool _contentLoaded;

	public ObservableCollection<OwnerItem> OwnersItem { get; set; } = new ObservableCollection<OwnerItem>();

	public WindowUsers()
	{
		InitializeComponent();
		foreach (OwnerAccount item in from ow in OwnersAccountBL.OwnersAccount
			orderby ow.IsBlocked, ow.Name
			select ow)
		{
			ViewModelMainWindow.AMSettings.VisibilityOwners.TryGetValue(item.GUID.ToString(), out var value);
			OwnersItem.Add(new OwnerItem
			{
				Owner = item,
				IsVisible = value
			});
		}
		base.DataContext = this;
	}

	private void btnHide_Click(object sender, RoutedEventArgs e)
	{
		base.WindowState = WindowState.Minimized;
	}

	private void btnWindow_Click(object sender, RoutedEventArgs e)
	{
		if (base.WindowState != WindowState.Maximized)
		{
			base.WindowState = WindowState.Maximized;
		}
		else
		{
			base.WindowState = WindowState.Normal;
		}
	}

	private void btnClose_Click(object sender, RoutedEventArgs e)
	{
		Hide();
	}

	private void gridTitle_MouseDown(object sender, MouseButtonEventArgs e)
	{
		DragMove();
	}

	private void btnEdit_Click(object sender, RoutedEventArgs e)
	{
		new WindowUser(((OwnerItem)((Button)sender).DataContext).Owner).Show();
	}

	private void btnAddOwner_Click(object sender, RoutedEventArgs e)
	{
		if (ViewModelMainWindow.instance.MainOwner.AvailabilityCreateAccounts)
		{
			OwnerAccount ownerAccount = new OwnerAccount
			{
				OwnerGuid = ViewModelMainWindow.instance.MainOwner.GUID.ToString()
			};
			OwnersAccountBL.OwnersAccount.Add(ownerAccount);
			OwnersItem.Add(new OwnerItem
			{
				Owner = ownerAccount
			});
			ViewModelMainWindow.AMSettings.VisibilityOwners.Add(ownerAccount.GUID.ToString(), value: true);
			new WindowUser(ownerAccount).Show();
		}
		else
		{
			new WindowInfo("Нет доступа к добавлению пользователей.").Show();
		}
	}

	private void btn_DeleteOwner(object sender, RoutedEventArgs e)
	{
		OwnerItem ownerItem = (OwnerItem)((Button)sender).DataContext;
		OwnerAccount owner = ownerItem.Owner;
		if (WindowAccountsManager.ShowWindowDialog("Подтвердите удаление " + owner.Name))
		{
			OwnersAccountBL.OwnersAccount.Remove(owner);
			OwnersItem.Remove(ownerItem);
		}
	}

	private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ClickCount == 2)
		{
			ViewModelMainWindow.instance.OnSwitchOwner(((OwnerItem)((StackPanel)sender).DataContext).Owner);
			Hide();
		}
	}

	private void checkBoxIsBlocked_Click(object sender, RoutedEventArgs e)
	{
		ViewModelMainWindow.UpdateOwners();
	}

	private async void checkBoxIsVisibile_Click(object sender, RoutedEventArgs e)
	{
		ViewModelMainWindow.instance.BlockUI();
		await Task.Run(async delegate
		{
			ViewModelMainWindow.UpdateOwners();
			await ViewModelMainWindow.instance.LoadAccountsAsync();
			await ViewModelMainWindow.instance.UpdateStatesAsync();
			ViewModelMainWindow.instance.RefreshView();
		});
		ViewModelMainWindow.instance.UnblockUI();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/AccountsManager;component/_accountsmanager/windowusers.xaml", UriKind.Relative);
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
			((Grid)target).MouseDown += gridTitle_MouseDown;
			break;
		case 2:
			((Button)target).Click += btnHide_Click;
			break;
		case 3:
			((Button)target).Click += btnClose_Click;
			break;
		case 4:
			listBoxUsers = (ListBox)target;
			break;
		case 10:
			((Button)target).Click += btnAddOwner_Click;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IStyleConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 5:
			((StackPanel)target).MouseLeftButtonDown += StackPanel_MouseLeftButtonDown;
			break;
		case 6:
			((CheckBox)target).Click += checkBoxIsBlocked_Click;
			break;
		case 7:
			((CheckBox)target).Click += checkBoxIsVisibile_Click;
			break;
		case 8:
			((Button)target).Click += btnEdit_Click;
			break;
		case 9:
			((Button)target).Click += btn_DeleteOwner;
			break;
		}
	}
}
