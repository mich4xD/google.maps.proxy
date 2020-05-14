module Google.Maps.Proxy.Controllers.WepApi

open Giraffe
open Google.Maps.Proxy.Controllers.Ping

let webApi: HttpHandler   =
    choose [ 
        GET >=> choose
                  [ 
                    route "/ping" >=> ping
                    setStatusCode 404 >=> text "Not Found" ]
          ]