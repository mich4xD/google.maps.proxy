module Google.Maps.Proxy.Controllers.WepApi

open Giraffe
open Google.Maps.Proxy.Controllers.Ping
open Google.Maps.Proxy.Controllers.Directions
open Google.Maps.Proxy.Controllers.AutoComplete

let webApi: HttpHandler   =
    choose [ 
        GET >=> choose
                  [ 
                    routef "/autocomplete/%s" getAutoComplete
                    routef "/directions/origin=%s&destination=%s" getDirections
                    route "/ping" >=> ping
                    setStatusCode 404 >=> text "Not Found" ]
          ]