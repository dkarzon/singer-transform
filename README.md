# singer-transform
A data transformation layer for [Singer.io](https://www.singer.io/) taps and targets.


## Usage
Put the command between a tap and a target with simple unix pipes:
```
some-singer-tap | dotnet ./singer-transform.dll -c transform_config.json | some-singer-target
```
It reads incoming messages from STDIN and using config.json to transform incoming [Singer.io SPEC](https://github.com/singer-io/getting-started) messages.


## Supported Transforms
- `CalculatedProperty` - Add a new property with a calculated value using [Octostache](https://github.com/OctopusDeploy/Octostache) syntax.
- `RenameStream` - Renames a given stream (useful for renaming database tables between tap and target)
- `RenameProperty` - Renames a property in a stream
- `AddHashId` - Add a new property to the output with the value set to a hash of an existing property value (using [hashids](https://hashids.org/net/) )
- `FormatDate` - Formats a string date as a regular date (useful for Google Analytics date formats, ie. `"20210101"`)
- `NoTableVersioning` - Hack to bypass table versioning on full table replication (which requires the table to be dropped and recreated)

Check the [Transform Configs doc](transform-configs.md) for more info.


## Installing in a dockerfile
Ensure dotnet is installed [details here](https://github.com/dotnet/dotnet-docker/blob/main/documentation/scenarios/installing-dotnet.md)

```
# Install singer-transform
RUN curl -SL --output singer-transform.zip https://github.com/dkarzon/singer-transform/releases/download/v0.2.1/singer-transform.zip \
    && unzip -d /external/singer-transform/ ./singer-transform.zip
```