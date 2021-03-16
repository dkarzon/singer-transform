# singer-transform
A data transformation layer for Singer.io taps and targets.


## Usage
Put the command between a tap and a target with simple unix pipes:
```
some-singer-tap | dotnet ./singer-transform.dll -c transform_config.json | some-singer-target
```
It reads incoming messages from STDIN and using config.json to transform incoming SCHEMA and RECORD messages. (STATE messages are left untouched)


## Config
```json
{
    "transforms": [
        {
            "stream": "ga_pageviews",
            "transformType": "AddStaticField",
            "value": "##SITE##",
            "field": "ga_site",
            "fieldType": "string",
            "keyProperty": true
        },
        {
            "stream": "junkuserstablename",
            "transformType": "RenameStream",
            "value": "users_table"
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
- `keyProperty` - Sets if the newly created property is to be set as a key property on the schema.


## Supported Transforms
- `AddStaticField` - A transform to add a new field to the output with a static value.
- `RenameStream` - Renames a given stream (useful for renaming database tables between tap and target)


## //TODO
How to install it...?