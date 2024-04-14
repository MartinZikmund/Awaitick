#nullable enable

using System.Runtime.CompilerServices;
using EventCountdowns.Core.Services.Navigation;
using EventCountdowns.Core.Infrastructure;
using EventCountdowns.Core.Services.InAppPurchases;
using Windows.UI.ViewManagement;

namespace EventCountdowns.Core.ViewModels;

public abstract class ViewModel : ObservableRecipient
{
	private readonly Dictionary<string, ICommand> _commands = new();

	private INavigationService? _navigationService;
	private bool _isWorking;

	public string Title { get; set; }

	private void OnTitleChanged() =>
		ApplicationView.GetForCurrentView().Title = Title;

	protected INavigationService Navigation => _navigationService ?? (_navigationService = IoC.GetRequiredService<INavigationService>());

	public bool UserPremium => IoC.GetRequiredService<IInAppPurchaseService>().HasUserAnyProduct();

	public bool IsWorking
	{
		get => _isWorking;
		set => SetProperty(ref _isWorking, value);
	}

	public virtual Task LoadAsync(object? parameter) => Task.CompletedTask;

	protected ICommand GetOrCreateCommand(Action action, [CallerMemberName] string commandName = "")
	{
		if (!_commands.TryGetValue(commandName, out var command))
		{
			command = new RelayCommand(action);
			_commands.Add(commandName, command);
		}
		return command;
	}

	protected ICommand GetOrCreateCommand(Action action, Func<bool> canExecute, [CallerMemberName] string commandName = "")
	{
		if (!_commands.TryGetValue(commandName, out var command))
		{
			command = new RelayCommand(action, canExecute);
			_commands.Add(commandName, command);
		}
		return command;
	}

	protected ICommand GetOrCreateCommand<T>(Action<T?> action, [CallerMemberName] string commandName = "")
	{
		if (!_commands.TryGetValue(commandName, out var command))
		{
			command = new RelayCommand<T>(action);
			_commands.Add(commandName, command);
		}
		return command;
	}

	protected ICommand GetOrCreateCommand<T>(Action<T?> action, Predicate<T?> canExecute, [CallerMemberName] string commandName = "")
	{
		if (!_commands.TryGetValue(commandName, out var command))
		{
			command = new RelayCommand<T>(action, canExecute);
			_commands.Add(commandName, command);
		}
		return command;
	}

	protected ICommand GetOrCreateAsyncCommand(Func<Task> action, [CallerMemberName] string commandName = "")
	{
		if (!_commands.TryGetValue(commandName, out var command))
		{
			command = new AsyncRelayCommand(action);
			_commands.Add(commandName, command);
		}
		return command;
	}

	protected ICommand GetOrCreateAsyncCommand(Func<Task> action, Func<bool> canExecute, [CallerMemberName] string commandName = "")
	{
		if (!_commands.TryGetValue(commandName, out var command))
		{
			command = new AsyncRelayCommand(action, canExecute);
			_commands.Add(commandName, command);
		}
		return command;
	}

	protected ICommand GetOrCreateAsyncCommand<T>(Func<T?, Task> action, [CallerMemberName] string commandName = "")
	{
		if (!_commands.TryGetValue(commandName, out var command))
		{
			command = new AsyncRelayCommand<T>(action);
			_commands.Add(commandName, command);
		}
		return command;
	}

	protected ICommand GetOrCreateAsyncCommand<T>(Func<T?, Task> action, Predicate<T?> canExecute, [CallerMemberName] string commandName = "")
	{
		if (!_commands.TryGetValue(commandName, out var command))
		{
			command = new AsyncRelayCommand<T>(action, canExecute);
			_commands.Add(commandName, command);
		}
		return command;
	}
}
