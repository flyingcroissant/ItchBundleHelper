# README

## Setup / Configuration


### appsettings.json
This repo's .gitignore includes appsettings.json as a way to avoid accidentally committing the Notion API Key (this is not the best way to do this -- just the easiest).
Create an appsettings.json in the source directory that contains the other source files like Program.cs. Set the "Copy to Output Directory" property to "Copy Always".

Your appsettings.json should look like this:
```
{
  "Settings": {
    "NotionApiKey": "<secret here>",
    "ParentPageId": "<parent page id guid here",
    "BundleFile": "<path to json file here>"
  }
}
```

## Usage
1) Save the Itch.io bundle JSON for the bundle locally. Update the `BundleFile` value in your appsettings.json.
2) In Notion:
    1) Create the page that will be the parent of the database.
    2) On the parent page, click the share button and copy the link (eg. `https://www.notion.so/<guid with no dashes>`)
    3) Take the GUID from the link and add the dashes again to turn it back into a GUID (eg. `00000000000000000000000000000000` -> `00000000-0000-0000-0000-000000000000`).
    4) Use this GUID as your `ParentPageId` value in your appsettings.
    5) Return to Notion and make sure this parent page is shared with the integration you're using for the API key and that the integration has permission to create things.
3) Paste your Notion API Key in the `NotionApiKey` value in your appsettings.
4) Go!

## References
- https://github.com/notion-dotnet/notion-sdk-net
- https://developers.notion.com/reference
- Itch.io