module Google.Maps.Proxy.Controllers.AutoComplete

open Giraffe
open FSharp.Control.Tasks.V2.ContextInsensitive
open Google.Maps.Proxy.Controllers.GoogleApiHelpers
open FsToolkit.ErrorHandling

[<CLIMutable>]
type Term = 
  {
    Offset: int
    Value: string
  }

[<CLIMutable>]
type MatchedSubstring = 
  {
    Length: int
    Offset: string
  }

[<CLIMutable>]
type Prediction = 
  {
    Description: string
    Distance_meters: int
    Id: string
    Matched_substrings: MatchedSubstring[]
    Place_id: string
    Reference: string
    Terms: Term[]
    Types: string[]
  }

[<CLIMutable>]
type AutoCompleteData =
    { 
      Status: string
      Predictions: Prediction[]
    }

let get = tryGet >> AsyncResult.mapError EndpointFailure.RequestFailure


let tryDeserializeAutoCompleteData (content: string): Result<AutoCompleteData, EndpointFailure> =
    TryDeserialize<AutoCompleteData> content
    |> Result.mapError DeserializationError


let getAutoComplete (locationSearchString: string): HttpHandler =
  fun next ctx -> 
    let autoCompleteApiPath = "place/autocomplete/"
    let parameters = [("input", locationSearchString); ("components", "country:ca")]     
 
    let autoCompleteQuery = composeGoogleApiEndpointGetQuery(autoCompleteApiPath, parameters)
   
    task {  
      let! responseContent = get autoCompleteQuery

      let endpointResult = 
        responseContent
        |> Result.bind tryDeserializeAutoCompleteData
        |> fun autoCompleteData ->
            match autoCompleteData with
             | Ok({ Status = OkStatus }) -> json autoCompleteData
             | Ok({ Status = ZeroResultsStatus }) -> json autoCompleteData
             | Ok(_) -> setStatusCode 500            
             | Error(EndpointFailure.RequestFailure(FailedResponse _)) -> setStatusCode 500
             | Error(EndpointFailure.RequestFailure(Timeout)) -> setStatusCode 408
             | Error(EndpointFailure.RequestFailure(FatalHttpClientFailure _)) -> setStatusCode 500
             | Error(EndpointFailure.DeserializationError(_)) -> setStatusCode 500

      return! endpointResult next ctx 
    }