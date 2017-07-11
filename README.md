# ElmishWpfDemos

Steps to reproduce FirstDemo

1.   An F# console application for .NET Framework 4.5.2 is created in VS 2017. VS 2015 will also work.

2a.  NuGet prerelease package Elmish.WPF v1.0.0-beta-3 is installed.
2b.  Project Output type is changed from Console Application to Windows Application.
2c.  A new project with name FirstDemoViews of type WPF User Control Library (.NET Framework) Visual C# is added to the solution.
2d.  A new item with name MainWindow of type Window (WPF) is added to the FirstDemoViews project.
2e.  The title of MainWindow is changed to "First Demo".

3a.  Project FirstDemo references project FirstDemoViews.
3b.  Fix: MainWindow property renamed.
3c.  Project FirstDemo references libraries PresentationFramework, PresentationCore, WindowsBase. (Error messages tells you to do this.)
3d.  Borrowing some XAML for MainWindow.xaml from from Elmish.Samples.CounterViews.
3e.  Borrowing some F# for Program.fs from Elmish.Samples.Counter.

Now we have two buttons and a text box that are all bound. Good, we have something to play with.

4.

As a pragmatic programmer, the next thing I am conserned with is this: What happens with unhandled exceptions in various places? In particular, what happens if you make a mistake in the functions init, update or view? Or main or a thread somewhere for that matter? I did some research, and also asked for help on GitHub in project Elmish.WPF.

If an exception is caused in function update, then it is silently ignored by Elmish.WPF. But Elmish.WPF has a handler that we can hook into in order to handle unhandled exceptions in function update. This is very easy, and demonstrated in the source.

If an exception is caused in function view, then the application simply terminates promptly. We need to hook into Application.DispatcherUnhandledException, which turns out not to be that easy. In the source for Program.fs of Elmish.WPF itself, we can see the application object being created, but it is not exposed. In order to get at it, we can use Application.Current. This however is null before the call to Program.RunWindow. The earliest chance we have of getting to it, seems to be in the Loaded event of the main window. In that handler we can hook into the exception handler. In dispatcherUnhandledException we set Handled=true, so that the application will continue to run.

Unhandled exceptions should of course never happen, but we make all sorts of mistakes during development, and so they still happen. When they happen, we should somehow be notified. The normal thing to do is to log them, and notify the user. The program is possibly in an unstable state at that point, so the user may want to restart the application. During development it is an advantage that the application doesn't just blow up in our face, but continue to run as best it can, in order that we may investigate what state the program ended up in.
