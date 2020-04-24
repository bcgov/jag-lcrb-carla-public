#!/usr/bin/env python

import yaml
import argparse
import os.path
import os
import sys
import datetime
import pytz
import json

# this is for the 2.7 sorting
from collections import OrderedDict


# get name of input file from args

# get a free-form line of text
def get_text(prompt='-->', required=False):
    ret = ''
    while True:
        ret = input(prompt)
        if 0 < len(ret) or not required:
            return ret
    return ret

# get a selection from a list (with default)
def get_option(list, prompt='-->', default=None, required=False, list_style=True):
    iret = -1
    while True:
        items = ''
        for i in range(len(list)):
            if default and default == i+1:
                token = '*'
            else:
                if list_style:
                    token = ' '
                else:
                    token = ''
            if list_style:
                print(token, i+1, list[i])
            else:
                if i > 0:
                    items = items + '/'
                items = items + token + list[i]
        if not list_style:
            prompt = prompt.format(items)
        ret = input(prompt)
        if 0 == len(ret) and default:
            return list[default-1]
        if list_style:
            try:
                iret = int(ret)
            except:
                pass
            if iret < 1 or iret > 1+len(list):
                iret = -1
            else:
                return list[iret-1]
        else:
            if 0 < list.count(ret):
                return ret
    return list[iret-1]

def path_to_name(schema_path):
    return schema_path.replace('/', '')

def now_date():
    tz_aware = timezone.localize(datetime.datetime.now())
    return tz_aware.astimezone(pytz.utc).isoformat()

# ui_name, ui_address, ui_text, ui_date, ui_select, helper_uuid, helper_now_iso, helper_value
def sample_data(attr_name, data_type):
    if data_type == 'ui_name':
        return '$Name'
    elif data_type == 'ui_address':
        return '$Address'
    elif data_type == 'ui_text':
        return '$Text'
    elif data_type == 'ui_date':
        return '$Date' # now_date()
    elif data_type == 'ui_select':
        return '$Select'
    elif data_type == 'helper_uuid':
        return '$UUID'
    elif data_type == 'helper_now_iso':
        return '$Date' # now_date()
    elif data_type == 'helper_value':
        return '$Value'
    else:
        return 'Unknown type'

# return a bare dict
def bare_dict():
    if sys.version_info.major == 2:
        return OrderedDict()
    else:
        return {}

# return a bare array
def bare_array():
    return []

# add a dictionary to our outbound yaml
def add_dict(yaml_struct, entry_name):
    yaml_struct[entry_name] = {}

# add an item to the dict
def add_item_to_dict(yaml_struct, entry_name, item_name, item_value):
    yaml_struct[entry_name][item_name] = item_value

# add an array to our outbound yaml
def add_array(yaml_struct, entry_name):
    yaml_struct[entry_name] = []

# add a item to the array
def add_item_to_array(yaml_struct, entry_name, item_value):
    yaml_struct[entry_name].append(item_value)

# for python 2.7
def ordered_dict_representer(self, value):  # can be a lambda if that's what you prefer
    return self.represent_mapping('tag:yaml.org,2002:map', value.items())
yaml.add_representer(OrderedDict, ordered_dict_representer)

# for python 3+
#Using a custom Dumper class to prevent changing the global state
class CustomDumper(yaml.Dumper):
    #Super neat hack to preserve the mapping key order. See https://stackoverflow.com/a/52621703/1497385
    def represent_dict_preserve_order(self, data):
        return self.represent_dict(data.items())    
CustomDumper.add_representer(dict, CustomDumper.represent_dict_preserve_order)

print("Python version:", sys.version_info.major)
if 2 > len(sys.argv):
    print("usage:", sys.argv[0], "<input file>")
    sys.exit()

# for now, we are in PST time
timezone = pytz.timezone("America/Los_Angeles")

in_file = sys.argv[1]
in_dir = os.path.dirname(in_file)
out_schemas = in_dir + '/gen-schemas.yml'
out_services = in_dir + '/gen-services.yml'
out_routes = in_dir + '/gen-routes.yml'
out_data = in_dir + '/gen-data.json'

with open(in_file, 'r') as stream:
    try:
        schemas = yaml.load(stream) 

        gschemas = bare_array()

        services = bare_dict()
        add_dict(services, 'issuers')
        services['issuers']['myorg'] = bare_dict()
        services['issuers']['myorg']['credential_types'] = bare_array()

        routes = bare_dict()
        routes['forms'] = bare_dict()

        testdata = bare_array()
        for schema in schemas:
            # generate schema-level stuff for schemas.yml
            gschema = bare_dict()
            gschema['name'] = schema['name']
            gschema['version'] = schema['version']
            gschema['description'] = schema['description']
            gschema['attributes'] = bare_dict()

            # generate schema-level stuff for services.yml
            service = bare_dict()
            service['description'] = schema['description']
            service['schema'] = schema['name']
            service['issuer_url'] = schema['endpoint'] + schema['path']
            if 'proof_request' in schema:
                service['depends_on'] = bare_array()
                service['depends_on'].append(schema['proof_request'])
            if 'effective_date' in schema or 'revoked_date' in schema:
                service['credential'] = bare_dict()
                if 'effective_date' in schema:
                    service['credential']['effective_date'] = bare_dict()
                    service['credential']['effective_date']['input'] = schema['effective_date']
                    service['credential']['effective_date']['from'] = 'claim'
                if 'revoked_date' in schema:
                    service['credential']['revoked_date'] = bare_dict()
                    service['credential']['revoked_date']['input'] = schema['revoked_date']
                    service['credential']['revoked_date']['from'] = 'claim'
            service['topic'] = bare_dict()
            service['topic']['source_id'] = bare_dict()
            service['topic']['source_id']['input'] = schema['topic']
            service['topic']['source_id']['from'] = 'claim'
            service['topic']['type'] = bare_dict()
            service['topic']['type']['input'] = 'registration'
            service['topic']['type']['from'] = 'value'
            # for relationship:
            if 'related_topic' in schema:
                service['topic']['related_source_id'] = bare_dict()
                service['topic']['related_source_id']['input'] = schema['related_topic']
                service['topic']['related_source_id']['from'] = 'claim'
                service['topic']['related_type'] = bare_dict()
                service['topic']['related_type']['input'] = 'registration'
                service['topic']['related_type']['from'] = 'value'
            if 'cardinality' in schema:
                service['cardinality_fields'] = bare_array()
                service['cardinality_fields'].append(schema['cardinality'])

            # todo generate attribute-level stuff for services.yml
            service['mapping'] = bare_array()
            has_name = False
            has_address = False
            for attr in schema['attributes'].keys():
                if schema['attributes'][attr]['data_type'] == 'ui_address':
                    addr_fields = ['addressee', 'address_line_1', 'city', 'province', 'postal_code', 'country']
                    for i in range(len(addr_fields)):
                        gmodel = bare_dict()
                        gmodel['description'] = addr_fields[i]
                        gmodel['data_type'] = 'ui_text'
                        gmodel['required'] = schema['attributes'][attr]['required']
                        gschema['attributes'][addr_fields[i]] = gmodel
                else:
                    gmodel = bare_dict()
                    if 'description_en' in schema['attributes'][attr]:
                        gmodel['description_en'] = schema['attributes'][attr]['description_en']
                    if 'label_en' in schema['attributes'][attr]:
                        gmodel['label_en'] = schema['attributes'][attr]['label_en']
                    gmodel['data_type'] = schema['attributes'][attr]['data_type']
                    gmodel['required'] = schema['attributes'][attr]['required']
                    gschema['attributes'][attr] = gmodel

                model = bare_dict()
                if schema['attributes'][attr]['data_type'] == 'ui_name':
                    if not has_name:
                        model['model'] = 'name'
                        model['fields'] = bare_dict()
                        model['fields']['text'] = bare_dict()
                        model['fields']['text']['input'] = attr
                        model['fields']['text']['from'] = 'claim'
                        model['fields']['type'] = bare_dict()
                        model['fields']['type']['input'] = attr
                        model['fields']['type']['from'] = 'value'
                        service['mapping'].append(model)
                        has_name = True
                elif schema['attributes'][attr]['data_type'] == 'ui_address':
                    if not has_address:
                        model['model'] = 'address'
                        model['fields'] = bare_dict()
                        model['fields']['addressee'] = bare_dict()
                        model['fields']['addressee']['input'] = 'addressee'
                        model['fields']['addressee']['from'] = 'claim'
                        model['fields']['civic_address'] = bare_dict()
                        model['fields']['civic_address']['input'] = 'address_line_1'
                        model['fields']['civic_address']['from'] = 'claim'
                        model['fields']['city'] = bare_dict()
                        model['fields']['city']['input'] = 'city'
                        model['fields']['city']['from'] = 'claim'
                        model['fields']['province'] = bare_dict()
                        model['fields']['province']['input'] = 'province'
                        model['fields']['province']['from'] = 'claim'
                        model['fields']['postal_code'] = bare_dict()
                        model['fields']['postal_code']['input'] = 'postal_code'
                        model['fields']['postal_code']['from'] = 'claim'
                        model['fields']['country'] = bare_dict()
                        model['fields']['country']['input'] = 'country'
                        model['fields']['country']['from'] = 'claim'
                        service['mapping'].append(model)
                        has_address = True
                else:
                    model['model'] = 'attribute'
                    model['fields'] = bare_dict()
                    model['fields']['type'] = bare_dict()
                    model['fields']['type']['input'] = attr
                    model['fields']['type']['from'] = 'value'
                    if schema['attributes'][attr]['data_type'] == 'ui_date' or schema['attributes'][attr]['data_type'] == 'helper_now_iso':
                        model['fields']['format'] = bare_dict()
                        model['fields']['format']['input'] = 'datetime'
                        model['fields']['format']['from'] = 'value'
                    model['fields']['value'] = bare_dict()
                    model['fields']['value']['input'] = attr
                    model['fields']['value']['from'] = 'claim'
                    service['mapping'].append(model)

            services['issuers']['myorg']['credential_types'].append(service)

            # generate schema-level stuff for routes.yml
            form_name = path_to_name(schema['path'])
            routes['forms'][form_name] = bare_dict()
            routes['forms'][form_name]['path'] = schema['path']
            routes['forms'][form_name]['type'] = 'issue-credential'
            routes['forms'][form_name]['schema_name'] = schema['name']
            routes['forms'][form_name]['page_title'] = 'Title for ' + schema['name']
            routes['forms'][form_name]['title'] = 'Title for ' + schema['name']
            routes['forms'][form_name]['template'] = 'bcgov.index.html'
            routes['forms'][form_name]['description'] = schema['description']
            routes['forms'][form_name]['explanation'] = 'Use the form below to issue a Credential.'
            if 'proof_request' in schema:
                routes['forms'][form_name]['proof_request'] = bare_dict()
                routes['forms'][form_name]['proof_request']['id'] = schema['proof_request']
                routes['forms'][form_name]['proof_request']['connection_id'] = 'bctob'
            if 'related_credentials' in schema:
                routes['forms'][form_name]['related_credentials'] = bare_dict()
                for related_cred in schema['related_credentials']:
                    routes['forms'][form_name]['related_credentials'][related_cred] = bare_dict()
                    for attr in schema['related_credentials'][related_cred]:
                        routes['forms'][form_name]['related_credentials'][related_cred][attr] = schema['related_credentials'][related_cred][attr]

            # optionally can serve javascript
            #js_includes:
            #  - src: js/bc_registries.js

            # generate attribute-level stuff for routes.yml
            routes['forms'][form_name]['fields'] = bare_array()
            has_address = False
            for attr in schema['attributes'].keys():
                if schema['attributes'][attr]['data_type'].startswith('ui_'):
                    field = bare_dict()
                    field['name'] = attr
                    field['label'] = attr
                    if schema['attributes'][attr]['data_type'] == 'ui_name':
                        field['type'] = 'text'
                    elif schema['attributes'][attr]['data_type'] == 'ui_address':
                        field['label'] = 'Mailing Address'
                        field['type'] = 'address'
                    elif schema['attributes'][attr]['data_type'] == 'ui_text':
                        field['type'] = 'text'
                    elif schema['attributes'][attr]['data_type'] == 'ui_date':
                        field['type'] = 'date'
                    elif schema['attributes'][attr]['data_type'] == 'ui_select':
                        field['type'] = 'select'
                        field['options'] = bare_array()
                        field['options'].append('todo-1')
                        field['options'].append('todo-2')
                        field['options'].append('todo-3')
                    else:
                        field['type'] = schema['attributes'][attr]['data_type']
                    field['required'] = schema['attributes'][attr]['required']
                    if schema['attributes'][attr]['data_type'] == 'ui_address' and not has_address:
                        routes['forms'][form_name]['fields'].append(field)
                        has_address = True
                    elif schema['attributes'][attr]['data_type'] != 'ui_address':
                        routes['forms'][form_name]['fields'].append(field)

            routes['forms'][form_name]['mapping'] = bare_dict()
            routes['forms'][form_name]['mapping']['attributes'] = bare_array()
            for attr in schema['attributes'].keys():
                if schema['attributes'][attr]['data_type'].startswith('helper_'):
                    attribute = bare_dict()
                    attribute['name'] = attr
                    if schema['attributes'][attr]['data_type'] == 'helper_value':
                        attribute['from'] = 'literal'
                        attribute['source'] = 'SomeValue'
                    else:
                        attribute['from'] = 'helper'
                        if schema['attributes'][attr]['data_type'] == 'helper_uuid':
                            attribute['source'] = 'uuid'
                        elif schema['attributes'][attr]['data_type'] == 'helper_now_iso':
                            attribute['source'] = 'now_iso'
                        else:
                            attribute['source'] = schema['attributes'][attr]['data_type']
                    routes['forms'][form_name]['mapping']['attributes'].append(attribute)

            gschemas.append(gschema)

            # no worries about writing json output for test data
            datapacket = {}
            datapacket['schema'] = schema['name']
            datapacket['version'] = schema['version']
            datapacket['attributes'] = {}
            for attr in gschema['attributes'].keys():
                datapacket['attributes'][attr] = sample_data(attr, gschema['attributes'][attr]['data_type'])
            testdata.append(datapacket)

        print('Writing', out_schemas)
        with open(out_schemas, 'w') as outfile:
            yaml.dump(gschemas, outfile, default_flow_style=False, Dumper=CustomDumper)
        
        print('Writing', out_services)
        with open(out_services, 'w') as outfile:
            yaml.dump(services, outfile, default_flow_style=False, Dumper=CustomDumper)
        
        print('Writing', out_routes)
        with open(out_routes, 'w') as outfile:
            yaml.dump(routes, outfile, default_flow_style=False, Dumper=CustomDumper)

        print('Writing', out_data)
        with open(out_data, 'w') as outfile:
            outfile.write(json.dumps(testdata, indent=4, sort_keys=True))
    except yaml.YAMLError as exc:
        print(exc)

