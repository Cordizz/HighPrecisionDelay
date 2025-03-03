# HighPrecisionDelay [![NuGet Version](http://img.shields.io/nuget/v/Cordis.HighPrecisionDelay.svg?style=flat)](https://www.nuget.org/packages/Cordis.HighPrecisionDelay/)

High Precision Delay for .NET Core. Uses Multimedia Timer on Windows and timerfd on Linux.

Based on https://github.com/HakanL/Haukcode.HighResolutionTimer

Example of usage:

````csharp
WriteLine("Hacking EVE ONLINE...");
HighPrecisionDelay.Wait(1); // wait 1ms
WriteLine("DONE");

async Task TryWait()
{
	using var delay = new HighPrecisionDelay();
	while(true)
	{
		if(Random.Shared.Next() > 123456789)
			delay.WaitFor(msDelay: 10);
		else
			await delay.WaitForAsync(msDelay: 10);
	}
}

await TryWait();
````
