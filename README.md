# Google.Maps.Proxy

An F# proxy for google maps api that allows you to proxy calls to the Directions and AutoComplete Apis from google.
Built using the Giraffe web framework and functional types that allow more responsible approach for error/exception handling.

## Setup:
- In the command line, cd into the root folder of the project and run `$ .paket/paket.exe install` to install the paket extension for vs code.
- Use the paket restore command to restore the packages locally.(https://fsprojects.github.io/Paket/index.html - for more info)
- Add the gogole api to the code in the GoogleApiHelpers.fs file on line 12:
`let googleApiKey = "GOOGLE_MAPS_API_KEY"`

## Why?
It's a good habbit to not expose external api keys on the publicly exposed client of an application.

For more production ready setup that includes usage of vault, ci/cd pipeline and more,
please check https://github.com/shrideio/shoebox

## Build and test the application

### Windows

Run the `build.bat` script in order to restore, build and test (if you've selected to include tests) the application:

```
> ./build.bat
```

### Linux/macOS

Run the `build.sh` script in order to restore, build and test (if you've selected to include tests) the application:

```
$ ./build.sh
```

## Run the application

After a successful build you can start the web application by executing the following command in your terminal:

```
dotnet run src/google.maps.proxy
```

After the application has started visit [http://localhost:5000](http://localhost:5000) in your preferred browser.
