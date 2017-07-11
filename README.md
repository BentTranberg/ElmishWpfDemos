# ElmishWpfDemos

The main purpose of this demo repo at this time, is to demonstrate how unhandled exceptions can be caught when you use Elmish.WPF. I was faced with this issue a few days ago, when I wanted to use Elmish.WPF in production.

Unhandled exceptions should of course never happen, but we make all sorts of mistakes during development, and so they still happen. When they happen, we should somehow be notified. The normal thing to do is to log them, and notify the user. The program is possibly in an unstable state at that point, so the user may want to restart the application. During development it is an advantage that the application doesn't just blow up in our face, but continue to run as best it can, in order that we may investigate what state the program ended up in.

Steps to reproduce FirstDemo - or a log of what I did

1.  An F# console application for .NET Framework 4.5.2 is created in VS 2017. VS 2015 will also work.

2.1.  NuGet prerelease package Elmish.WPF v1.0.0-beta-3 is installed.
2.2.  Project Output type is changed from Console Application to Windows Application.
2.3.  A new project with name FirstDemoViews of type WPF User Control Library (.NET Framework) Visual C# is added to the solution.
2.4.  A new item with name MainWindow of type Window (WPF) is added to the FirstDemoViews project.
2.5.  The title of MainWindow is changed to "First Demo".

3.1.  Project FirstDemo references project FirstDemoViews.
3.2.  Fix: MainWindow properly renamed. (Sigh, mistakes ...)
3.3.  Project FirstDemo references libraries PresentationFramework, PresentationCore, WindowsBase. (Error messages tells you to add these.)
3.4.  Borrowing some XAML for MainWindow.xaml from from Elmish.Samples.CounterViews.
3.5.  Borrowing some F# for Program.fs from Elmish.Samples.Counter.

Now we have two buttons and a text box that are all bound. Good, we have something to play with. The "Increment" button and "Decrement" button will change the value of "Count" in the text box.

4.  Handling unhandled exceptions.

As a pragmatic programmer, the next thing I am conserned with is this: What happens with unhandled exceptions in various places? In particular, what happens if you make a mistake in the functions init, update or view? Or main or a thread somewhere for that matter? I did some research, and also asked for help on GitHub in project Elmish.WPF.

If an exception is caused in function update, then it is silently ignored by Elmish.WPF, unless ...

Elmish.WPF has a handler - onError - that we can hook into in order to handle unhandled exceptions in function update. This is very easy, and demonstrated in the source. Exceptions are then no longer silently ignored.

If an exception is caused in function view, then the application simply terminates promptly. That's really bad. To fix this behavior, we need to hook into Application.DispatcherUnhandledException, which turns out not to be that easy. In the source for Program.fs of Elmish.WPF itself, we can see the application object being created, but it is not exposed in any way by Elmish.WPF. Fortunately, in order to get at it, we can use Application.Current. This however is null before the call to Program.RunWindow - naturally. The earliest chance we have of getting to it, seems to be in the Loaded event of the main window. That's good enough, for now at least. In that handler we can hook into the exception handler. Very important: In dispatcherUnhandledException we set Handled=true, so that the application will continue to run.

5.  Using FsXaml with Elmish.WPF

This is demonstrated in the latest sample just added to the Elmish.WPF repo.

I am not sure what the pros/cons of using FsXaml with Elmish.WPF is, but will try this out in FirstDemo now. I know that BAML loads quite a lot faster than XAML, but is it significant? Then there's having one project versus two - which is better? The XAML can now reach the F# source, and perhaps that's useful. My guess is that it's the same designer at work in C# and F# projects, but I wonder if there are still differences. Are there any other pros/cons?

Final notes

I haven't yet checked what happens if an exception occurs in the init function, but I assume it would be the same as the update function.

I haven't demonstrated the use of AppDomain.CurrentDomain.UnhandledException or Threading.Thread.GetDomain().UnhandledException, but I don't think it's relevant here. I do hook up even these in my commercial applications.

If you look up the issue https://github.com/Prolucid/Elmish.WPF/issues/14 you will find an interesting comment from et1975 regarding exception handling in Elmish.WPF.
