module Google.Maps.Proxy.Controllers.Directions

open Google.Maps.Proxy.Controllers.GoogleApiHelpers
open Giraffe
open FSharp.Control.Tasks.V2.ContextInsensitive
open FsToolkit.ErrorHandling


let get = tryGet >> AsyncResult.mapError EndpointFailure.RequestFailure

type GoogleResponseContentModel = 
  { Status: string }

type StatusAndOriginalContent = StatusAndOriginalContent of status: string * OriginalContent: string
    

let tryDeserializeStatusAndOriginalContent (content: string): Result<StatusAndOriginalContent, EndpointFailure> =
    TryDeserialize<GoogleResponseContentModel> content
    |> Result.map (fun x -> StatusAndOriginalContent(x.Status, content))
    |> Result.mapError EndpointFailure.DeserializationError


let isContentStatusValid (googleResponseContentStatus: GoogleResponseContentModel): bool =
  googleResponseContentStatus.Status = OkStatus || googleResponseContentStatus.Status = ZeroResultsStatus


let getDirections(origin: string, destination:string): HttpHandler =
  fun next ctx -> 
    let directionsApiPath = "directions/"
    let parameters = [("origin", "place_id:" + origin); ("destination", "place_id:" + destination)]     
    let directionsQuery = composeGoogleApiEndpointGetQuery(directionsApiPath, parameters)

    task {  
      let! response = get directionsQuery
      
      let endpointResult = 
        response
        |> Result.bind tryDeserializeStatusAndOriginalContent
        |> function
           | Ok(StatusAndOriginalContent(OkStatus, content)) -> stringAsJson content
           | Ok(StatusAndOriginalContent(ZeroResultsStatus, content)) -> stringAsJson content
           | Ok(StatusAndOriginalContent(_)) -> setStatusCode 500
           | Error(EndpointFailure.RequestFailure(FailedResponse _)) -> setStatusCode 500
           | Error(EndpointFailure.RequestFailure(Timeout)) -> setStatusCode 408
           | Error(EndpointFailure.RequestFailure(FatalHttpClientFailure _)) -> setStatusCode 500
           | Error(EndpointFailure.DeserializationError(_)) -> setStatusCode 500

      return! endpointResult next ctx
   }
