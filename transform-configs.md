# Transform Configs

Global transform config properties used for all transform types:
- `stream` - The name of the stream to apply the transform to.
- `transformType` - The transformation type to apply (see supported types below).

## CalculatedProperty
Add a new property with a calculated value using [Octostache](https://github.com/OctopusDeploy/Octostache) syntax.
- `value` - The value of the transformation either static value or can be calculated with Octostache syntax based on other properties in the record.
- `property` - The name of the new property created by this transform.
- `propertyType` - The type of the property being created to add to the schema.
- `keyProperty` - Sets if the newly created property is to be set as a key property on the schema.(Optional)

```json
{
    "transforms": [
        {
            "stream": "ga_pageviews",
            "transformType": "CalculatedProperty",
            "value": "example.com",
            "property": "ga_site",
            "propertyType": "string",
            "keyProperty": true
        },
        {
            "stream": "teststream",
            "transformType": "CalculatedProperty",
            "value": "#{id}-new",
            "property": "newid",
            "propertyType": "string"
        }
    ]
}
```

## RenameStream
Renames a given stream (useful for renaming database tables between tap and target)
- `value` - The new name to rename the stream to.

```json
{
    "transforms": [
        {
            "stream": "junkuserstablename",
            "transformType": "RenameStream",
            "value": "users_table"
        }
    ]
}
```

## RenameProperty
Renames a property in a stream
- `property` - The name of the property to rename.
- `value` - The new name to rename the property to.

```json
{
    "transforms": [
        {
            "stream": "users_table",
            "transformType": "RenameProperty",
            "property": "complexidcolumn",
            "value": "userid"
        }
    ]
}
```

## AddHashId
Add a new property to the output with the value set to a hash of an existing property value (using [hashids](https://hashids.org/net/) )
- `value` - The name of the property on the record to apply the HashId to.
- `property` - The name of the new property created by this transform.
- `propertyType` - The type of the property being created to add to the schema.
- `keyProperty` - Sets if the newly created property is to be set as a key property on the schema. (Optional)
- `settings` - An object containing the specific settings for hashids (Optional)

```json
{
    "transforms": [
        {
            "stream": "teststream",
            "transformType": "AddHashId",
            "value": "id",
            "property": "hashid",
            "propertyType": ["string", "null"],
            "settings": {
                "salt": "my salt",
                "minHashLength": "5",
                "alphabet": "abcdefghijklmnopqrstuvwxyz1234567890",
                "seps": "cfhistu"
            }
        }
    ]
}
```

## FormatDate
Formats a string date as a regular date (useful for Google Analytics date formats, ie. `"20210101"`)
- `property` - The name of the property to format as date.

```json
{
    "transforms": [
        {
            "stream": "users_table",
            "transformType": "FormatDate",
            "property": "datestring"
        }
    ]
}
```

## NoTableVersioning
Hack to convert full table replication to bypass table versioning (which requires the table to be dropped and recreated)

```json
{
    "transforms": [
        {
            "stream": "users_table",
            "transformType": "NoTableVersioning"
        }
    ]
}
```
