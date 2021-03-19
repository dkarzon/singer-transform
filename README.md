# singer-transform
A data transformation layer for [Singer.io](https://www.singer.io/) taps and targets.


## Usage
Put the command between a tap and a target with simple unix pipes:
```
some-singer-tap | dotnet ./singer-transform.dll -c transform_config.json | some-singer-target
```
It reads incoming messages from STDIN and using config.json to transform incoming [Singer.io SPEC](https://github.com/singer-io/getting-started) messages.


## Config
```json
{
    "transforms": [
        {
            "stream": "ga_pageviews",
            "transformType": "CalculatedField",
            "value": "example.com",
            "field": "ga_site",
            "fieldType": "string",
            "keyProperty": true
        },
        {
            "stream": "teststream",
            "transformType": "CalculatedField",
            "value": "#{id}-new",
            "field": "newid",
            "fieldType": "string"
        },
        {
            "stream": "junkuserstablename",
            "transformType": "RenameStream",
            "value": "users_table"
        },
        {
            "stream": "teststream",
            "transformType": "AddHashId",
            "value": "id",
            "field": "hashid",
            "fieldType": "string",
            "properties": {
                "salt": "my salt",
                "minHashLength": "5",
                "alphabet": "abcdefghijklmnopqrstuvwxyz1234567890",
                "seps": "cfhistu"
            }
        }
    ]
}
```

The transformation configuration contains a list of transforms with the following properties:
- `stream` - The name of the stream to apply the transform to.
- `transformType` - The transformation type to apply (see supported types below).
- `value` -  The value of the transformation.
- `field` - The name of the field to transform.
- `fieldType` - The type of the field if being created to add to the schema.
- `keyProperty` - Sets if the newly created property is to be set as a key property on the schema. (Optional)
- `properties` - An object containing any other non-standard properties for the transform (Optional)


## Supported Transforms
- `CalculatedField` - Add a new field with a calculated value using [Octostache](https://github.com/OctopusDeploy/Octostache) syntax.
- `RenameStream` - Renames a given stream (useful for renaming database tables between tap and target)
- `AddHashId` - Add a new field to the output with the value set to a hash of an existing field value (using [hashids](https://hashids.org/net/) )


## //TODO
How to install it...?