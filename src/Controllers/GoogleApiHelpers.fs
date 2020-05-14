module Google.Maps.Proxy.Controllers.GoogleApiHelpers

open System
open Giraffe
open System.Net.Http
open Newtonsoft.Json
open Google.Maps.Proxy.Controllers.Types
open FsToolkit.ErrorHandling
open Google.Maps.Proxy.Extras.Helpers

let googleApiKey = "GOOGLE_MAPS_API_KEY"
let googleMapsApiPath = "https://maps.googleapis.com/maps/api/"


let composeGoogleApiEndpointGetQuery(pathPostfix: string, parameters: list<string * string>) =
  let parametersString = String.Join ('&', parameters |> List.map(fun (k,v) -> k + "=" + v))
  let googleApiQuery = googleMapsApiPath + pathPostfix + "json?" + parametersString  + "&key=" + googleApiKey
  googleApiQuery


let tryReadContent (resp : HttpResponseMessage): Async<Result<string, exn>> = 
  async {
    try 
      let! response = resp.Content.ReadAsStringAsync() |> Async.AwaitTask
      return response |> Ok      
    with
    | ex -> return Error ex
  }


let TryDeserialize<'t>(content: string): Result<'t, DeserializationError> =
    try
    content 
    |> JsonConvert.DeserializeObject<'t>
    |> Ok
    with 
    | exn -> Error <| DeserializationError.DeserializationError exn


let getContent (response: HttpResponseMessage): Async<Result<string, RequestFailure>> = async {
  let! content = tryReadContent response
  return
    (content
    |> (Result.mapError FatalHttpClientFailure))
}


let tryGet url : Async<Result<string, RequestFailure>> = async {
    try
      let! response = Furl.get url []
      return! response |> getContent
    with
      | :? TimeoutException -> return Error Timeout
      | ex -> return (Error << FatalHttpClientFailure) <| ex
}


let stringAsJson(jsonString: string) : HttpHandler =
  fun (next) (ctx) ->
    ctx.SetHttpHeader "Content-Type" "application/json"
    ctx.SetHttpHeader "Content-Length" jsonString.Length
    ctx.WriteStringAsync(jsonString)

