using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using AccountsLibrary;
using CommonLibrary;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace AccountsManager;

public class ViewModelAccountSettings : INotifyPropertyChanged
{
	private AccountSettings accountSettings;

	private OwnerAccount currentOwner;

	private RelayCommand addStrategyCommand;

	private RelayCommand removeStrategyCommand;

	private RelayCommand clearProxy;

	private RelayCommand saveSession;

	private RelayCommand loadSettingsCommand;

	private RelayCommand saveSettingsCommand;

	private RelayCommand setBetConstantCommand;

	private RelayCommand setBalancePercentCommand;

	private RelayCommand setMinBalanceCommand;

	private bool isCheckedPoslegolSettings = true;

	private bool isCheckedForkSettings = true;

	private bool isCheckedValueSettings = true;

	private bool isCheckedValuePlusSettings = true;

	private bool isCheckedValueScoreSettings = true;

	private double sumBetConstantSettings;

	private double balancePercentSettings;

	private double minBalanceSettings;

	public AccountSettings AccountSettings
	{
		get
		{
			return accountSettings;
		}
		set
		{
			accountSettings = value;
			RaisePropertyChanged("AccountSettings");
		}
	}

	public OwnerAccount CurrentOwner
	{
		get
		{
			return currentOwner;
		}
		set
		{
			currentOwner = value;
			RaisePropertyChanged("CurrentOwner");
		}
	}

	public OwnerAccount MainOwner { get; set; }

	public RelayCommand AddStrategyCommand => addStrategyCommand ?? (addStrategyCommand = new RelayCommand(delegate(object obj)
	{
		if (WindowAccountsManager.ShowWindowDialog("Добавить стратегию?"))
		{
			switch (obj.ToString())
			{
			case "1":
				AccountSettings.AddStrategy(TypesStrategy.Poslegoal);
				break;
			case "2":
				AccountSettings.AddStrategy(TypesStrategy.Value);
				break;
			case "3":
				AccountSettings.AddStrategy(TypesStrategy.ValuePlus);
				break;
			case "6":
				AccountSettings.AddStrategy(TypesStrategy.ValueScore);
				break;
			case "8":
				AccountSettings.AddStrategy(TypesStrategy.Fork);
				break;
			}
			AccountSettings.UpdateAccount("");
		}
	}));

	public RelayCommand RemoveStrategyCommand => removeStrategyCommand ?? (removeStrategyCommand = new RelayCommand(delegate(object obj)
	{
		if (WindowAccountsManager.ShowWindowDialog("Подтвердите удаление стратегии."))
		{
			if (obj.GetType() == typeof(PoslegolSettings))
			{
				AccountSettings.PoslegolsSettings.Remove(obj as PoslegolSettings);
			}
			else if (obj.GetType() == typeof(ValueSettings))
			{
				ValueSettings item = obj as ValueSettings;
				AccountSettings.ValuesSettings.Remove(item);
			}
			else if (obj.GetType() == typeof(ForkSettings))
			{
				ForkSettings item2 = obj as ForkSettings;
				AccountSettings.ForksSettings.Remove(item2);
			}
			else if (obj.GetType() == typeof(ValuesPlusSettings))
			{
				ValuesPlusSettings item3 = obj as ValuesPlusSettings;
				AccountSettings.ValuesPlusSettings.Remove(item3);
			}
			else if (obj.GetType() == typeof(ValueScoreSettings))
			{
				ValueScoreSettings item4 = obj as ValueScoreSettings;
				AccountSettings.ValuesScoreSettings.Remove(item4);
			}
			AccountSettings.UpdateAccount("");
		}
	}));

	public RelayCommand ClearProxy => clearProxy ?? (clearProxy = new RelayCommand(delegate
	{
		AccountSettings.Proxy = AccountsLibrary.ProxyBL.ListProxy.SingleOrDefault((Proxy pr) => pr.IsEmpty);
	}));

	public RelayCommand SaveSession => saveSession ?? (saveSession = new RelayCommand(delegate
	{
		ViewModelMainWindow.GetBrowserAccount(AccountSettings)?.SaveCookies();
	}));

	public RelayCommand LoadSettingsCommand => loadSettingsCommand ?? (loadSettingsCommand = new RelayCommand(delegate
	{
		LoadSettings();
	}));

	public RelayCommand SaveSettingsCommand => saveSettingsCommand ?? (saveSettingsCommand = new RelayCommand(delegate
	{
		SaveSettings();
	}));

	public RelayCommand SetBetConstantCommand => setBetConstantCommand ?? (setBetConstantCommand = new RelayCommand(delegate
	{
		SetBetConstant_Click();
	}));

	public RelayCommand SetBalancePercentCommand => setBalancePercentCommand ?? (setBalancePercentCommand = new RelayCommand(delegate
	{
		SetBalancePercent_Click();
	}));

	public RelayCommand SetMinBalanceCommand => setMinBalanceCommand ?? (setMinBalanceCommand = new RelayCommand(delegate
	{
		SetMinBalance_Click();
	}));

	public bool IsCheckedPoslegolSettings
	{
		get
		{
			return isCheckedPoslegolSettings;
		}
		set
		{
			isCheckedPoslegolSettings = value;
			RaisePropertyChanged("IsCheckedPoslegolSettings");
		}
	}

	public bool IsCheckedForkSettings
	{
		get
		{
			return isCheckedForkSettings;
		}
		set
		{
			isCheckedForkSettings = value;
			RaisePropertyChanged("IsCheckedForkSettings");
		}
	}

	public bool IsCheckedValueSettings
	{
		get
		{
			return isCheckedValueSettings;
		}
		set
		{
			isCheckedValueSettings = value;
			RaisePropertyChanged("IsCheckedValueSettings");
		}
	}

	public bool IsCheckedValuePlusSettings
	{
		get
		{
			return isCheckedValuePlusSettings;
		}
		set
		{
			isCheckedValuePlusSettings = value;
			RaisePropertyChanged("IsCheckedValuePlusSettings");
		}
	}

	public bool IsCheckedValueScoreSettings
	{
		get
		{
			return isCheckedValueScoreSettings;
		}
		set
		{
			isCheckedValueScoreSettings = value;
			RaisePropertyChanged("IsCheckedValueScoreSettings");
		}
	}

	public double SumBetConstantSettings
	{
		get
		{
			return sumBetConstantSettings;
		}
		set
		{
			sumBetConstantSettings = value;
			RaisePropertyChanged("SumBetConstantSettings");
		}
	}

	public double BalancePercentSettings
	{
		get
		{
			return balancePercentSettings;
		}
		set
		{
			balancePercentSettings = value;
			RaisePropertyChanged("BalancePercentSettings");
		}
	}

	public double MinBalanceSettings
	{
		get
		{
			return minBalanceSettings;
		}
		set
		{
			minBalanceSettings = value;
			RaisePropertyChanged("MinBalanceSettings");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public ViewModelAccountSettings(AccountSettings initAccountSettings)
	{
		AccountSettings = initAccountSettings;
		CurrentOwner = ViewModelMainWindow.instance.CurrentOwner;
		MainOwner = ViewModelMainWindow.instance.MainOwner;
	}

	public void LoadBrowserCookies()
	{
		_ = Environment.CurrentDirectory + "\\WebCache\\_Chrome\\" + AccountSettings.NameBCAccount + "\\" + AccountSettings.GUID.ToString();
	}

	public void LoadSettings()
	{
		System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
		openFileDialog.Filter = "Strategy Settings documents (.ss)|*.ss|All Files (*.*)|*.*";
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			StrategisDTO? strategisDTO = JsonConvert.DeserializeObject<StrategisDTO>(CommonStaticFunction.DecompressString(File.ReadAllText(openFileDialog.FileName)));
			AccountSettings.PoslegolsSettings.Clear();
			strategisDTO.PoslegolsSettings.ForEach(delegate(PoslegolSettings s)
			{
				AccountSettings.PoslegolsSettings.Add(s);
			});
			AccountSettings.ValuesSettings.Clear();
			strategisDTO.ValuesSettings.ForEach(delegate(ValueSettings s)
			{
				AccountSettings.ValuesSettings.Add(s);
			});
			AccountSettings.ForksSettings.Clear();
			strategisDTO.ForksSettings.ForEach(delegate(ForkSettings s)
			{
				AccountSettings.ForksSettings.Add(s);
			});
			AccountSettings.ValuesPlusSettings.Clear();
			strategisDTO.ValuesPlusSettings.ForEach(delegate(ValuesPlusSettings s)
			{
				AccountSettings.ValuesPlusSettings.Add(s);
			});
			AccountSettings.ValuesScoreSettings.Clear();
			strategisDTO.ValuesScoreSettings.ForEach(delegate(ValueScoreSettings s)
			{
				AccountSettings.ValuesScoreSettings.Add(s);
			});
			AccountSettings.OnSubscribeSettings();
			ChangesBL.AccountsSettingsBL_OnAccountSettingChanged(this.accountSettings);
			AccountSettings accountSettings = AccountSettings;
			accountSettings.Discription = accountSettings.Discription + Environment.NewLine + CommonStaticFunction.GetDateTimeStamp() + " Загружена стратегия из файла " + openFileDialog.SafeFileName;
		}
	}

	public void SaveSettings()
	{
		Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
		saveFileDialog.Filter = "Strategy Settings documents (.ss)|*.ss|All Files (*.*)|*.*";
		saveFileDialog.FileName = "Filename.txt";
		if (saveFileDialog.ShowDialog() == true)
		{
			string text = JsonConvert.SerializeObject(new StrategisDTO
			{
				PoslegolsSettings = accountSettings.PoslegolsSettings.ToList(),
				ValuesSettings = accountSettings.ValuesSettings.ToList(),
				ForksSettings = accountSettings.ForksSettings.ToList(),
				ValuesPlusSettings = accountSettings.ValuesPlusSettings.ToList(),
				ValuesScoreSettings = accountSettings.ValuesScoreSettings.ToList()
			});
			File.WriteAllText(saveFileDialog.FileName, CommonStaticFunction.CompressString(text));
		}
	}

	private void SetBetConstant_Click()
	{
		try
		{
			if (IsCheckedPoslegolSettings)
			{
				AccountSettings.PoslegolsSettings.ToList().ForEach(delegate(PoslegolSettings s)
				{
					s.SumBetConstant = SumBetConstantSettings;
				});
			}
			if (IsCheckedForkSettings)
			{
				AccountSettings.ForksSettings.ToList().ForEach(delegate(ForkSettings s)
				{
					s.SumBetConstant = SumBetConstantSettings;
				});
			}
			if (IsCheckedValueSettings)
			{
				AccountSettings.ValuesSettings.ToList().ForEach(delegate(ValueSettings s)
				{
					s.SumBetConstant = SumBetConstantSettings;
				});
			}
			if (IsCheckedValuePlusSettings)
			{
				AccountSettings.ValuesPlusSettings.ToList().ForEach(delegate(ValuesPlusSettings s)
				{
					s.SumBetConstant = SumBetConstantSettings;
				});
			}
			if (IsCheckedValueScoreSettings)
			{
				AccountSettings.ValuesScoreSettings.ToList().ForEach(delegate(ValueScoreSettings s)
				{
					s.SumBetConstant = SumBetConstantSettings;
				});
			}
			Console.WriteLine($"{CommonStaticFunction.GetDateTimeStamp()} {AccountSettings.NameBCAccount} {AccountSettings.Login} BetConstant {SumBetConstantSettings} set successful");
		}
		catch
		{
		}
	}

	private void SetBalancePercent_Click()
	{
		try
		{
			if (IsCheckedPoslegolSettings)
			{
				AccountSettings.PoslegolsSettings.ToList().ForEach(delegate(PoslegolSettings s)
				{
					s.BalancePercent = BalancePercentSettings;
				});
			}
			if (IsCheckedForkSettings)
			{
				AccountSettings.ForksSettings.ToList().ForEach(delegate(ForkSettings s)
				{
					s.BalancePercent = BalancePercentSettings;
				});
			}
			if (IsCheckedValueSettings)
			{
				AccountSettings.ValuesSettings.ToList().ForEach(delegate(ValueSettings s)
				{
					s.BalancePercent = BalancePercentSettings;
				});
			}
			if (IsCheckedValuePlusSettings)
			{
				AccountSettings.ValuesPlusSettings.ToList().ForEach(delegate(ValuesPlusSettings s)
				{
					s.BalancePercent = BalancePercentSettings;
				});
			}
			if (IsCheckedValueScoreSettings)
			{
				AccountSettings.ValuesScoreSettings.ToList().ForEach(delegate(ValueScoreSettings s)
				{
					s.BalancePercent = BalancePercentSettings;
				});
			}
			Console.WriteLine($"{CommonStaticFunction.GetDateTimeStamp()} {AccountSettings.NameBCAccount} {AccountSettings.Login} BalancePercentSettings {BalancePercentSettings} set successful");
		}
		catch
		{
		}
	}

	private void SetMinBalance_Click()
	{
		try
		{
			if (IsCheckedPoslegolSettings)
			{
				AccountSettings.PoslegolsSettings.ToList().ForEach(delegate(PoslegolSettings s)
				{
					s.MinBalance = MinBalanceSettings;
				});
			}
			if (IsCheckedForkSettings)
			{
				AccountSettings.ForksSettings.ToList().ForEach(delegate(ForkSettings s)
				{
					s.MinBalance = MinBalanceSettings;
				});
			}
			if (IsCheckedValueSettings)
			{
				AccountSettings.ValuesSettings.ToList().ForEach(delegate(ValueSettings s)
				{
					s.MinBalance = MinBalanceSettings;
				});
			}
			if (IsCheckedValuePlusSettings)
			{
				AccountSettings.ValuesPlusSettings.ToList().ForEach(delegate(ValuesPlusSettings s)
				{
					s.MinBalance = MinBalanceSettings;
				});
			}
			if (IsCheckedValueScoreSettings)
			{
				AccountSettings.ValuesScoreSettings.ToList().ForEach(delegate(ValueScoreSettings s)
				{
					s.MinBalance = MinBalanceSettings;
				});
			}
			Console.WriteLine($"{CommonStaticFunction.GetDateTimeStamp()} {AccountSettings.NameBCAccount} {AccountSettings.Login} MinBalanceSettings {MinBalanceSettings} set successful");
		}
		catch
		{
		}
	}
}
