namespace TextAnalyzerSystem.Utils

open System
open System.Threading.Tasks
open System.Windows.Forms

// This lets you run heavy operations off the UI thread safely.
module AsyncUI =
    let runAsync (work: unit -> 'a) (callback: 'a -> unit) =
        Task.Run(fun () ->
            let result = work()
            result
        ).ContinueWith(fun (t:Task<'a>) ->
            if not t.IsFaulted then callback t.Result
        , TaskScheduler.FromCurrentSynchronizationContext())
        |> ignore
