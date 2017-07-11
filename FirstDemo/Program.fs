namespace FirstDemo

open System
open Elmish
open Elmish.WPF

type Msg =
    | Increment
    | Decrement
    | CauseCrash

type Model = { Count: int }

module State =

    //open Types

    let causeCrash() = 13 / 0 |> ignore

    let init() = { Count = 0 }

    let update (msg: Msg) (model: Model) =
        match msg with
        | Increment -> { model with Count = model.Count + 1 }
        | Decrement -> { model with Count = model.Count - 1 }
        | CauseCrash ->
            // causeCrash() // Uncomment this call if you want an exception here.
            model

module App =

    open System.Windows
    open System.Windows.Threading
    //open Types
    open State

    let view _ _ =
        [ "Increment" |> Binding.cmd (fun _ m -> Increment)
          "Decrement" |> Binding.cmd (fun _ m -> Decrement)
          "CauseCrash" |> Binding.cmd (fun _ m ->
            // causeCrash() // Uncomment this call if you want an exception here.
            CauseCrash)
          "Count" |> Binding.oneWay (fun m -> m.Count) ]

    let elmishErrorHandler (messageToShow: string, ex: exn) =
        Windows.MessageBox.Show(ex.Message) |> ignore
        ()

    let dispatcherUnhandledException (e: DispatcherUnhandledExceptionEventArgs) =
        e.Handled <- true
        Windows.MessageBox.Show(e.Exception.Message) |> ignore
        ()

    let windowLoaded _ =
        let app = Windows.Application.Current
        app.DispatcherUnhandledException.Add dispatcherUnhandledException
        app.ShutdownMode <- ShutdownMode.OnMainWindowClose
        ()

    type MainWindow = FsXaml.XAML<"MainWindow.xaml">

    [<EntryPoint;STAThread>]
    let main argv =
        let window = MainWindow()
        window.Loaded.Add windowLoaded
        Program.mkSimple init update view
        |> fun p -> { p with onError = elmishErrorHandler }
        |> Program.runWindow window
