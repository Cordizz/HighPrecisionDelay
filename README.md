# HighPrecisionDelay [![NuGet Version](http://img.shields.io/nuget/v/Cordis.HighPrecisionDelay.svg?style=flat)](https://www.nuget.org/packages/Cordis.HighPrecisionDelay/)

High Resolution Timer for .NET Core. Uses Multimedia Timer on Windows and timerfd on Linux.

Based on https://github.com/HakanL/Haukcode.HighResolutionTimer

Example of usage:

````csharp
WriteLine("Hacking EVE ONLINE...");
HighPrecisionDelay.Wait(1); // wait 1ms
WriteLine("DONE");

using var delay = new HighPrecisionDelay();
while(true)
{
	delay.WaitFor(msDelay: 10);
	if(Random.Shared.Next() > 123456789)
		break;
}
````