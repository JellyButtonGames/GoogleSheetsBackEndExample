# Google Sheets Back-End Example
a Short example of how to use Google Sheets as a back-end tool for a Unity3D project

## How To Use?
1. Clone the project using command line (or your favorite Git UI)
> git clone git@github.com:simongri/GoogleSheetsBackEndExample.git

1. Load the project using Unity3D (we've developed the example with Unity3d 4.6.2p2, but tested with Unity3D 5.0.2 as well)
1. Run the project and watch the cube move from side to side
1. On the _Cube_ game object there's a _Cube Mover_ script in which you can turn the _Load From Google_ property on and off to see the effects of the Google Sheet
> You can see the Google Sheet the project is using [here](https://docs.google.com/spreadsheets/d/1lW-uec71bgVSEwJCiRjxjYD4E3PDs70MKbErxqWent0)

## How To Change The Parameters?
1. Since the Google Sheet noted above is read-only, you have to make a copy of the document on your Google Drive
1. Open your copy of the Google Sheets doc
1. Note that the document's URL will look something like this:
`https://docs.google.com/spreadsheets/d/<unique file identifier>/edit#gid=<unique sheet identifier>`
1. Open the _GoogleSheetsLoader_ class in your editor
1. In _GoogleSheetsLoader_ replace:
  1. _GoogleDriveFileGuid_ to your _unique file identifier_
  1. _GoogleDriveSheetGuid_ to your _unique sheet identifier_
1. Now you can use your Google Sheet document to change the parameters in your project (between runs)

## Classes
- _CSVReader_ - a Simple CSV reader based on [this](https://github.com/tikonen/blog/tree/master/csvreader), we've altered it from reading a _TextAsset_ into parsing a CSV string input
- _GoogleSheetsLoader_ - The class allows you to load a Google Sheet from Google Drive in CSV format, it also parses it and keeps the parsed parameters in-memory
- _CubeMover_ - Moves a 3D Cube (or any 3D object) from side to side on the X-Axis according to the provided configuration from Google Sheets if available.
