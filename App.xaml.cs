namespace SportEventsApp;

public partial class App : Application
{
	public App()
	{
        InitializeComponent();

#if ANDROID
        Task.Run(async () => await RequestNotificationPermission());
#endif
    }

#if ANDROID
    private async Task RequestNotificationPermission()
    {
        if (DeviceInfo.Version.Major >= 13) 
        {
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            }
        }
    }
#endif

    protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}