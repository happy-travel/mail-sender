# Mail Sender

A mail sending library. The current implementation is based on a Sendgrid mail client.


## Usage
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IMailSender, SendGridMailSender>();
        
        ...
    }
}
```


## Automation

New package version is automatically published to [github packages](https://github.com/features/packages) after changes in the master branch.


## Dependencies

The project depends on following packages: 
* `Sendgrid`
