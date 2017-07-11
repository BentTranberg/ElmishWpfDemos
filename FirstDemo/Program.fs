namespace FirstDemo

open System
open Elmish
open Elmish.WPF

module Types =

    type Msg =
        | Increment
        | Decrement
    
    type Model = { Count: int }

module State =

    open Types

    let init() = { Count = 0 }

    let update (msg: Msg) (model: Model) =
        match msg with
        | Increment -> { model with Count = model.Count + 1 }
        | Decrement -> { model with Count = model.Count - 1 }

module App =

    open Types
    open State

    let view _ _ =
        [ "Increment" |> Binding.cmd (fun _ m -> Increment)
          "Decrement" |> Binding.cmd (fun _ m -> Decrement)
          "Count" |> Binding.oneWay (fun m -> m.Count) ]

    [<EntryPoint;STAThread>]
    let main argv =
        Program.mkSimple init update view
        |> Program.runWindow (FirstDemoViews.MainWindow())
