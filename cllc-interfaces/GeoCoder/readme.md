# GeoCoder
> see https://aka.ms/autorest

This is the AutoRest configuration file for the APIs.

---

#### Basic Information 
These are the global settings for APIs.

``` yaml
# list all the input OpenAPI files (may be YAML, JSON, or Literate- OpenAPI markdown)
input-file:
  - geocoder.json

# this allows you to programatically tweak the swagger file before it is modeled.
directive:
  from: swagger-document # do it globally 
  where: $.paths.*.* 
  
  # set each operationId to 'GeoCoderAPI<Tag>'
  transform: $.operationId = `GeoCoderAPI_${$.tags[0]}`
 
```