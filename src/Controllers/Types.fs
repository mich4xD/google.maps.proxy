[<AutoOpen>]
module Google.Maps.Proxy.Controllers.Types

open System.Net


type Content = Content of string


type RequestFailure =
    | FailedResponse of {| Content : string; StatusCode : HttpStatusCode |}
    | Timeout
    | FatalHttpClientFailure of exn

type DeserializationError = DeserializationError of exn

type EndpointFailure =
    | RequestFailure of RequestFailure
    | DeserializationError of DeserializationError


let [<Literal>] OkStatus = "OK"
let [<Literal>] ZeroResultsStatus = "ZERO_RESULTS"