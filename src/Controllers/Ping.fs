module Google.Maps.Proxy.Controllers.Ping
   
open System
open Giraffe


type Ping =
    class
    end
    
let ping: HttpHandler =
    warbler (fun _ ->
        [ ("ts", DateTimeOffset.UtcNow.ToString("o"))
          ("v", typeof<Ping>.Assembly.GetName().Version.ToString()) ]
        |> Map.ofList
        |> json)